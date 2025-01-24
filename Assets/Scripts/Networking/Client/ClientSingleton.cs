using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;

    private ClientGameManager gameManager;

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

    public async Task CreateClient()
    {
        gameManager = new ClientGameManager();

        await gameManager.InitAsync();
    }
}
