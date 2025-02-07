using System.Threading.Tasks;
using UnityEngine;

public class AuthenticationController : MonoBehaviour
{

    private async void Start()
    {
        await Task.Delay(1000);

        await ClientSingleton.Instance.AuthClient();
    }

}
