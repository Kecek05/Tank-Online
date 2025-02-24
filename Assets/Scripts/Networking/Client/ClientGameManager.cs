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
    private MatchplayMatchmaker matchmaker;

    private UserData userData;

    public string joinCode;
    public string JoinCode => joinCode;

    public async Task<bool> InitAsync(bool isAnonymously = false)
    {
        //Authenticate player

        //Debugging code
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());
        await UnityServices.InitializeAsync(initializationOptions);
        //
        //await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);
        matchmaker = new MatchplayMatchmaker();

        AuthState authState = isAnonymously ? await AuthenticationWrapper.DoAuthAnonymously() : await AuthenticationWrapper.DoAuth();

        if(authState == AuthState.Authenticated)
        {
            userData = new UserData
            {
                userName = AuthenticationWrapper.PlayerName,
                userAuthId = AuthenticationService.Instance.PlayerId
            };

            return true;
        }

        return false;
       
    }

    public void UpdateUserDataName()
    {
        userData.userName = AuthenticationWrapper.PlayerName;
    }


    public void GoToMenu()
    {
        SceneManager.LoadScene(MENU_SCENE);
    }

    public void GoToAuth()
    {
        SceneManager.LoadScene(AUTH_SCENE);
    }

    public async Task StartRelayClientAsync(string joinCode)
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

        ConnectClient();
    }

    private void StartServerClient(string ip, int port) //dedicated server
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, (ushort)port);


        ConnectClient();
    }

    private void ConnectClient()
    {
        string payload = JsonUtility.ToJson(userData); //serialize the payload to json
        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload); //serialize the payload to bytes

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();

        Debug.Log("Started Client!");
    }

    public async void MatchmakeAsync(Action<MatchmakerPollingResult> onMatchmakeResponse) // not void because we are not waiting for the method to finish but still need to call await methods
    {
        //Action its for the callback
        if (matchmaker.IsMatchmaking) return;

        MatchmakerPollingResult matchResult = await GetMatchAsync();
        
        onMatchmakeResponse?.Invoke(matchResult); //send the error to the UI

    }

    private async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        MatchmakingResult matchmakingResult = await matchmaker.Matchmake(userData);

        if(matchmakingResult.result == MatchmakerPollingResult.Success)
        {
            //Connect to server
            StartServerClient(matchmakingResult.ip, matchmakingResult.port);
        }
        return matchmakingResult.result;
    }

    public async Task CancelMatchmaking() // Task = can be awaited itself
    {
        await matchmaker.CancelMatchmaking();
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
