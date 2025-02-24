using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;

    public event Action<string> OnClientLeft;

    public event Action<UserData> OnUserJoined;
    public event Action<UserData> OnUserLeft;

    private NetworkObject playerPrefab;

    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>(); // save client IDs to their authentication IDs
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>(); // save authentication IDs to user data

    public NetworkServer(NetworkManager networkManager, NetworkObject playerPrefab) // our constructor
    {
        this.networkManager = networkManager;
        this.playerPrefab = playerPrefab;

        networkManager.ConnectionApprovalCallback += ApprovalCheck;

        networkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    public bool OpenConnection(string ip, int port)
    {
        UnityTransport transport = networkManager.gameObject.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, (ushort)port);
        return networkManager.StartServer(); //returns true if the server started
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload); //Deserialize the payload to jason

        UserData userData = JsonUtility.FromJson<UserData>(payload); //Deserialize the payload to UserData

        clientIdToAuth[request.ClientNetworkId] = userData.userAuthId; //if dont exist, add to dictionary
        authIdToUserData[userData.userAuthId] = userData; 

        OnUserJoined?.Invoke(userData);

        _ = SpawnPlayerDelay(request.ClientNetworkId);

        response.Approved = true; // connection is approved
        response.CreatePlayerObject = false; // create a player object

        //after, check if there is anyone with this username, if have, dont aprove the connection (idk if nessesary)
    }

    private void NetworkManager_OnServerStarted()
    {
        networkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

    }

    private async Task SpawnPlayerDelay(ulong clientId)
    {
        await Task.Delay(1000); // delay to spawn the player
        NetworkObject playerInstance = GameObject.Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity); // spawn the player

        playerInstance.SpawnAsPlayerObject(clientId);
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientIdToAuth.TryGetValue(clientId, out string authId)) //Handle disconnections
        {
            clientIdToAuth.Remove(clientId);
            OnUserLeft?.Invoke(authIdToUserData[authId]);
            authIdToUserData.Remove(authId);
            
            OnClientLeft?.Invoke(authId);
        }
    }

    public void Dispose()
    {
       if(networkManager != null)
       {
            networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            networkManager.OnServerStarted -= NetworkManager_OnServerStarted;
            networkManager.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
       }

       if(networkManager.IsListening)
       {
            networkManager.Shutdown();
       }
    }

    public UserData GetUserDataByClientId(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            //Get Auth by client ID
            if (authIdToUserData.TryGetValue(authId, out UserData userData))
            {
                //Get UserData by Auth
                return userData;
            }
        }
        return null;
    }
}
