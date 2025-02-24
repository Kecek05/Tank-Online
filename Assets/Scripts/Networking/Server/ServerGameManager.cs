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

public class ServerGameManager : IDisposable
{

    private const string GAME_SCENE = "Game";

    private string serverIP;
    private int serverPort; //serverPort = gameData 
    private int queryPort; // queryPort = analytics and more

    private NetworkServer networkServer; //handle all stuff that server does
    private MultiplayAllocationService multiplayAllocationService;

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

        if(!networkServer.OpenConnection(serverIP, serverPort)) //open the server
        {
            Debug.LogWarning("NetworkServer did not start as expected.");
        }

        NetworkManager.Singleton.SceneManager.LoadScene(GAME_SCENE, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void Dispose()
    {
        multiplayAllocationService?.Dispose();
        networkServer?.Dispose();
    }
}
