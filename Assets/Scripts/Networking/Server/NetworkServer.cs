using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;

    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>(); // save client IDs to their authentication IDs
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>(); // save authentication IDs to user data

    public NetworkServer(NetworkManager networkManager) // our constructor
    {
        this.networkManager = networkManager;

        networkManager.ConnectionApprovalCallback += ApprovalCheck;

        networkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload); //Deserialize the payload to jason

        UserData userData = JsonUtility.FromJson<UserData>(payload); //Deserialize the payload to UserData

        clientIdToAuth[request.ClientNetworkId] = userData.userAuthId; //if dont exist, add to dictionary
        authIdToUserData[userData.userAuthId] = userData; 

        response.Approved = true; // connection is approved
        response.Position = SpawnPoint.GetRandomSpawnPos(); // set the spawn position
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true; // create a player object

        //after, check if there is anyone with this username, if have, dont aprove the connection (idk if nessesary)
    }

    private void NetworkManager_OnServerStarted()
    {
        networkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientIdToAuth.TryGetValue(clientId, out string authId)) //Handle disconnections
        {
            clientIdToAuth.Remove(clientId);
            authIdToUserData.Remove(authId);
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
