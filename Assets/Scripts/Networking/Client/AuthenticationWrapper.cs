using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    private static string playerName;
    public static string PlayerName => playerName;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (AuthState == AuthState.Authenticated) return AuthState;

        if(AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already authenticating.");
            await Authenticating();
            return AuthState;
        }

        //await SignInAnonymouslyAsync(maxTries);

        await InitSignIn();

        return AuthState;
    }

    private static async Task<AuthState> Authenticating()
    {
        while(AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return AuthState;
    }

    public static void SignOutAuth()
    {
        try
        {
            PlayerAccountService.Instance.SignOut();
            AuthenticationService.Instance.SignOut();

            PlayerPrefs.DeleteKey("AccessToken");
            AuthState = AuthState.NotAuthenticated;
            Debug.Log("SignOut successfull.");

            ClientSingleton.Instance.GameManager.GoToAuth();

        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    private static async Task InitSignIn()
    {
        AuthState = AuthState.Authenticating;



        if (PlayerPrefs.HasKey("AccessToken"))
        {
            Debug.Log("PlayerPrefs");
            string accessTokenPlayerPrefs = PlayerPrefs.GetString("AccessToken");

            await SignInWithUnityAsync(accessTokenPlayerPrefs);

        }
        else
        {

            PlayerAccountService.Instance.SignedIn += SignedIn;

            await PlayerAccountService.Instance.StartSignInAsync();

        }

    }

    private static async void SignedIn()
    {
        try
        {
            var accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(accessToken);

            
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        PlayerAccountService.Instance.SignedIn -= SignedIn;
    }

    private static async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("SignIn successfull.");

            PlayerPrefs.SetString("AccessToken", accessToken);

            AuthState = AuthState.Authenticated;


            playerName = await AuthenticationService.Instance.GetPlayerNameAsync();

            Debug.Log(playerName);

            ClientSingleton.Instance.GameManager.GoToMenu();

        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    public static async Task RenamePlayerName(string newName)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(newName);
            playerName = newName;
            Debug.Log("Player name changed to: " + newName);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }


    private static async Task SignInAnonymouslyAsync(int maxTries = 5)
    {
        AuthState = AuthState.Authenticating;

        int tries = 0;

        while (AuthState == AuthState.Authenticating && tries < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }

            } catch (AuthenticationException authEx)
            {
                Debug.LogError(authEx);
                AuthState = AuthState.Error;
            } catch (RequestFailedException requestEx)
            {
                Debug.LogError(requestEx);
                AuthState = AuthState.Error;
            }


            tries++;

            await Task.Delay(1000);
        }

        if(AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player could not authenticate after {tries} tries.");
            AuthState = AuthState.TimeOut;
        }
    }


}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut,
}
