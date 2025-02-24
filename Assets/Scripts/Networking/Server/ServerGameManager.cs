using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using System.Collections;
using Unity.Services.Authentication;
using Unity.Services.Matchmaker.Models;

public class ServerGameManager : IDisposable
{

    private const string GAME_SCENE = "Game";

    private string serverIP;
    private int serverPort; //serverPort = gameData 
    private int queryPort; // queryPort = analytics and more

    private NetworkServer networkServer; //handle all stuff that server does
    public NetworkServer NetworkServer => networkServer;

    private MultiplayAllocationService multiplayAllocationService;

    private MatchplayBackfiller matchplayBackfiller;

    public ServerGameManager(string serverIP, int serverPort, int queryPort, NetworkManager manager) //Constructor, called when class is built 
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.queryPort = queryPort;
        networkServer = new NetworkServer(manager); //create a new network server
        multiplayAllocationService = new MultiplayAllocationService(); 
    }

    public async Task StartGameServerAsync()
    {
        //Successfully connected to UGS
        await multiplayAllocationService.BeginServerCheck();

        try
        {
            MatchmakingResults matchmakerPayload =  await GetMatchmakerPayload();

            if(matchmakerPayload != null)
            {
                await StartBackfill(matchmakerPayload);

                networkServer.OnUserJoined += NetworkServer_OnUserJoined;
                networkServer.OnUserLeft += NetworkServer_OnUserLeft;

            } else
            {
                // Timed out
                Debug.LogWarning("Matchmaker Payload timed out.");
            }
        } catch (Exception ex)
        {
            Debug.LogWarning($"Error starting game server.\n{ex}");
        }

        if (!networkServer.OpenConnection(serverIP, serverPort)) //open the server
        {
            Debug.LogWarning("NetworkServer did not start as expected.");
        }

        NetworkManager.Singleton.SceneManager.LoadScene(GAME_SCENE, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void NetworkServer_OnUserLeft(UserData userData)
    {
        
    }

    private void NetworkServer_OnUserJoined(UserData userData)
    {
        
    }

    private async Task StartBackfill(MatchmakingResults matchmakerPayload)
    {
        matchplayBackfiller = new MatchplayBackfiller($"{serverIP}:{serverPort}", matchmakerPayload.QueueName, matchmakerPayload.MatchProperties, 30);

        if(matchplayBackfiller.NeedsPlayers()) 
        {
            await matchplayBackfiller.BeginBackfilling();

        }

    }

    private void UserJoined(UserData userData)
    {
        matchplayBackfiller.AddPlayerToMatch(userData);
        
        multiplayAllocationService.AddPlayer(); //Analytics

        if (!matchplayBackfiller.NeedsPlayers() && matchplayBackfiller.IsBackfilling) // Just got the max users, stop backfilling
        {
            _ = matchplayBackfiller.StopBackfill(); // _ = ignores the await
        }
    }

    private void UserLeft(UserData userData)
    {
        int playerCount = matchplayBackfiller.RemovePlayerFromMatch(userData.userAuthId); // returns a int of how many players are left in match

        
        multiplayAllocationService.RemovePlayer(); //Analytics

        if(playerCount <= 0)
        {
            //Empty Server, Close it
            CloseServer();
            return;
        }

        if(matchplayBackfiller.NeedsPlayers() && !matchplayBackfiller.IsBackfilling) //not at max and isnt backfilling, start it
        {
            _ = matchplayBackfiller.BeginBackfilling();
        }
    }

    private async void CloseServer() // call async but dont await this method
    {
        await matchplayBackfiller.StopBackfill();
        Dispose();
        Application.Quit();

    }

    private async Task<MatchmakingResults> GetMatchmakerPayload()
    {
        Task<MatchmakingResults> matchmakerPayloadTask = multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();


        if(await Task.WhenAny(matchmakerPayloadTask, Task.Delay(20000)) == matchmakerPayloadTask) // if our task finished or the delay finished, move on
        {
            return matchmakerPayloadTask.Result;
            
        }

        //The delay finished first, delay its not equal to matchmakerPayloadTask, so execute below
        return null;
    }

    public void Dispose()
    {
        networkServer.OnUserJoined -= NetworkServer_OnUserJoined;
        networkServer.OnUserLeft -= NetworkServer_OnUserLeft;

        matchplayBackfiller?.Dispose();
        multiplayAllocationService?.Dispose();
        networkServer?.Dispose();
    }
}
