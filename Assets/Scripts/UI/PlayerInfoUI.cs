using Sortify;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    [BetterHeader("References")]

    [SerializeField] private TextMeshProUGUI playerNameXTxt;
    [SerializeField] private Button logoutBtn;
    [SerializeField] private Button renameBtn;
    [SerializeField] private RenameUI renameUI;

    private void Awake()
    {
        logoutBtn.onClick.AddListener(() =>
        {
            AuthenticationWrapper.SignOutAuth();
        });

        renameBtn.onClick.AddListener(() =>
        {
            renameUI.Show();
        });
    }


    private void Start()
    {

        renameUI.OnNameChanged += RenameUI_OnNameChanged;

        playerNameXTxt.text = AuthenticationWrapper.PlayerName.ToString();
    }

    private void RenameUI_OnNameChanged(string newName)
    {
        playerNameXTxt.text = newName;
    }

    public void LockPlayerInfoUI()
    {
        logoutBtn.interactable = false;
        renameBtn.interactable = false;
    }

    public void UnlockPlayerInfoUI()
    {
        logoutBtn.interactable = true;
        renameBtn.interactable = true;
    }
}
