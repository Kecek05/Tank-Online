using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;

    public ClientGameManager GameManager { get; private set; }

    public static ClientSingleton Instance
    {
        get
        {
            if(instance != null) return instance;

            instance = FindFirstObjectByType<ClientSingleton>();

            if(instance == null)
            {
                Debug.LogError("No ClientSingleton found in the scene.");
                return null;
            }

            return instance;
        }
    }


    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateClient()
    {
        GameManager = new ClientGameManager();
    }

    public async Task<bool> AuthClient()
    {
        return await GameManager.InitAsync();
    }

    public async Task<bool> AuthClientAnonymously()
    {
        return await GameManager.InitAsyncAnonymously();
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
