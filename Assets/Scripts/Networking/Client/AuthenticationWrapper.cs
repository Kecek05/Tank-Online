using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (AuthState == AuthState.Authenticated) return AuthState;

        if(AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already authenticating.");
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxTries);

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

            } catch (AuthenticationException ex)
            {
                Debug.LogError(ex);
                AuthState = AuthState.Error;
            } catch (RequestFailedException exeption)
            {
                Debug.LogError(exeption);
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
