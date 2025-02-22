using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LeaveUI : MonoBehaviour
{
    [SerializeField] private Button leaveButton;

    private void Awake()
    {
        leaveButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.IsHost) //Server cant click buttons
            {
                HostSingleton.Instance.GameManager.Shutdown();
            }

            ClientSingleton.Instance.GameManager.Disconnect();
        });
    }
}
