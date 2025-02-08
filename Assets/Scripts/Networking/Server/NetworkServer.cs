using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer
{
    private NetworkManager networkManager;

    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager) // our constructor
    {
        this.networkManager = networkManager;

        networkManager.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload); //Deserialize the payload to jason

        UserData userData = JsonUtility.FromJson<UserData>(payload); //Deserialize the payload to UserData

        clientIdToAuth[request.ClientNetworkId] = userData.userAuthId; //if dont exist, add to dictionary
        authIdToUserData[userData.userAuthId] = userData; //if dont exist, add to dictionary

        response.Approved = true; // connection is approved

        response.CreatePlayerObject = true; // create a player object

        //after, check if there is anyone with this username, if have, dont aprove the connection (idk if nessesary)
    }
}
