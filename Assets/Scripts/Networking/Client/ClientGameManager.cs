using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private const string MENU_SCENE = "Menu";
    private const string AUTH_SCENE = "AuthBootstrap";

    private JoinAllocation joinAllocation;
    private NetworkClient networkClient;


    public string joinCode;
    public string JoinCode => joinCode;

    public async Task<bool> InitAsync()
    {
        //Authenticate player
        await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);

        AuthState authState = await AuthenticationWrapper.DoAuth();

        if(authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
       
    }

    public async Task<bool> InitAsyncAnonymously()
    {
        //Authenticate player
        await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);

        AuthState authState = await AuthenticationWrapper.DoAuthAnonymously();

        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;

    }


    public void GoToMenu()
    {
        SceneManager.LoadScene(MENU_SCENE);
    }

    public void GoToAuth()
    {
        SceneManager.LoadScene(AUTH_SCENE);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");

        transport.SetRelayServerData(relayServerData);

       
        this.joinCode = joinCode;
        Debug.Log("Code Relay:" + this.joinCode);

        UserData userData = new UserData
        {
            userName = AuthenticationWrapper.PlayerName,
            userAuthId = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(userData); //serialize the payload to json
        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload); //serialize the payload to bytes

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();

        Debug.Log("Started Client!");
    }

    public void Disconnect()
    {
        networkClient.Disconnect();
    }

    public void Dispose() // Similar to OnDestroy but dont need to be attached to a GameObject
    {
        networkClient?.Dispose();
    }

    
}
