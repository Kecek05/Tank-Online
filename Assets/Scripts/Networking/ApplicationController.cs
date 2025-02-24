using Sortify;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    private const string GAME_SCENE = "Game";

    [BetterHeader("Singletons")]
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ServerSingleton serverPrefab;

    [BetterHeader("PlayerPrefab", 11)]
    [SerializeField] private NetworkObject playerPrefab;

    private ApplicationData applicationData;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null); // if dont have a render its the server
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if(isDedicatedServer)
        {
            Application.targetFrameRate = 60; //server could use too mutch processing power and crash if not limited

            applicationData = new ApplicationData();

            ServerSingleton serverSingleton = Instantiate(serverPrefab);

            StartCoroutine(LoadGameSceneAsync(serverSingleton));
        }
        else
        {

            HostSingleton hostSingleton = Instantiate(hostPrefab);

            hostSingleton.CreateHost(playerPrefab);

            ClientSingleton clientSingleton = Instantiate(clientPrefab);

            clientSingleton.CreateClient();

            clientSingleton.GameManager.GoToAuth();
        }
    }

    private IEnumerator LoadGameSceneAsync(ServerSingleton serverSingleton)
    {

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GAME_SCENE);

        while(!asyncOperation.isDone)
        {
            yield return null;
        }
        //Server already loaded the scene before its available to connect

        Task createServerTask = serverSingleton.CreateServer(playerPrefab);
        yield return new WaitUntil(() => createServerTask.IsCompleted);

        Task startServerTask =  serverSingleton.GameManager.StartGameServerAsync();
        yield return new WaitUntil(() => startServerTask.IsCompleted);
    }
}
