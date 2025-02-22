using Sortify;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [BetterHeader("References")]
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button lobbiesBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private TMP_InputField lobbyCodeInputField;
    [SerializeField] private GameObject backgroundJoining;

    [BetterHeader("Lobby UI")]
    [SerializeField] private GameObject lobbyBackgroundUI;
    [SerializeField] private Button closeLobbyBackgroundBtn;

    private async void Awake()
    {
        hostBtn.onClick.AddListener(async () =>
        {
            hostBtn.interactable = false;
            ShowBackgroundJoining();
            await HostSingleton.Instance.GameManager.StartHostAsync();
            hostBtn.interactable = true;
            HideBackgroundJoining();
        });

        exitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        clientBtn.onClick.AddListener(async () =>
        {
            clientBtn.interactable = false;
            ShowBackgroundJoining();
            await ClientSingleton.Instance.GameManager.StartClientAsync(lobbyCodeInputField.text);
            clientBtn.interactable = true;
            HideBackgroundJoining();
        });


        lobbiesBtn.onClick.AddListener(() =>
        {
            lobbyBackgroundUI.SetActive(true);
        });

        closeLobbyBackgroundBtn.onClick.AddListener(() =>
        {
            lobbyBackgroundUI.SetActive(false);
        });
    }

    private void Start()
    {
        lobbyBackgroundUI.SetActive(false);
        HideBackgroundJoining();
    }

    public void ShowBackgroundJoining()
    {
        backgroundJoining.SetActive(true);
    }

    public void HideBackgroundJoining()
    {
        backgroundJoining.SetActive(false);
    }
}
