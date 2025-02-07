using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null); // if dont have a render its the server
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if(isDedicatedServer)
        {

        } else
        {

            HostSingleton hostSingleton = Instantiate(hostPrefab);

            hostSingleton.CreateHost();

            ClientSingleton clientSingleton = Instantiate(clientPrefab);

            clientSingleton.CreateClient();

            clientSingleton.GameManager.GoToAuth();
        }
    }
}
