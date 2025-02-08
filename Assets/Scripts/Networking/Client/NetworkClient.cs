using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient
{
    private const string MENU_SCENE = "Menu";

    private NetworkManager networkManager;

    public NetworkClient(NetworkManager networkManager) // our constructor
    {
        this.networkManager = networkManager;

        networkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        //check if a client has diconnected.
        //For the client: Only call when they disconects
        //For the host: Cuz its the server too, will be called when any client disconects and there is never a scenario where the host disconects cuz he is the host too


        // clientId != 0 && clientId : each time a client disconects for the host, the host will trigger this event with his clientId. We need to check if isnt 0 cuz 0 is for the server
        // clientId != networkManager.LocalClientId : safety check
        if (clientId != 0 && clientId != networkManager.LocalClientId) return;

        //SceneManager.GetActiveScene().name != MENU_SCENE : could disconnect by timeout, need to check
        if (SceneManager.GetActiveScene().name != MENU_SCENE)
        {
            SceneManager.LoadScene(MENU_SCENE);
        }

        if(networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }
    }
}
