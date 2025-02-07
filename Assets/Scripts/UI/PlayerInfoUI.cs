using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameXTxt;
    [SerializeField] private Button logoutBtn;


    private void Awake()
    {
        logoutBtn.onClick.AddListener(() =>
        {
            AuthenticationWrapper.SignOutAuth();
        });
    }


    private void Start()
    {
        playerNameXTxt.text = AuthenticationWrapper.PlayerName.ToString();
    }
}
