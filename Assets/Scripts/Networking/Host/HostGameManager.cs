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

public class HostGameManager : IDisposable
{
    private const int MAX_CONNECTIONS = 30;
    private const string GAME_SCENE = "Game";

    private NetworkServer networkServer;
    public NetworkServer NetworkServer => networkServer;

    private Allocation allocation;
    public string joinCode;
    public string JoinCode => joinCode;

    private string lobbyId;
    private NetworkObject playerPrefab;

    public HostGameManager(NetworkObject playerPrefab)
    {
        this.playerPrefab = playerPrefab;
    }

    public async Task StartHostAsync()
    {
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(MAX_CONNECTIONS);

           

        } catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        try
        {
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);

        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");

        transport.SetRelayServerData(relayServerData);

        //Create the lobby, before .StartHost an after get joinCode
        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: joinCode
                    )
                }
            };

            Lobby lobby =  await LobbyService.Instance.CreateLobbyAsync($"{AuthenticationWrapper.PlayerName}'s Lobby", MAX_CONNECTIONS, lobbyOptions);

            lobbyId = lobby.Id;


            HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15f));

        } catch (LobbyServiceException lobbyEx)
        {
            Debug.Log(lobbyEx);
            return;
        }

        networkServer = new NetworkServer(NetworkManager.Singleton, playerPrefab);

        UserData userData = new UserData
        {
            userName = AuthenticationWrapper.PlayerName,
            userAuthId = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(userData); //serialize the payload to json
        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload); //serialize the payload to bytes

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;


        NetworkManager.Singleton.StartHost();

        NetworkServer.OnClientLeft += HandleClientLeft;

        NetworkManager.Singleton.SceneManager.LoadScene(GAME_SCENE, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private IEnumerator HeartbeatLobby(float delayHeartbeatSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(delayHeartbeatSeconds); //optimization

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);

            yield return delay;
        }
    }

    public void Dispose() // Similar to OnDestroy but dont need to be attached to a GameObject
    {
        Shutdown();
    }

    public async void Shutdown()
    {

        if (string.IsNullOrEmpty(lobbyId)) return;

        HostSingleton.Instance.StopCoroutine(nameof(HeartbeatLobby));

        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        lobbyId = string.Empty;

        NetworkServer.OnClientLeft -= HandleClientLeft;

        networkServer?.Dispose();
    }

    private async void HandleClientLeft(string authId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(lobbyId, authId); //Owner of the lobby is allowed to kick players
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
