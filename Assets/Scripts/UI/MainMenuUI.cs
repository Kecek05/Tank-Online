using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button lobbiesBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private TMP_InputField lobbyCodeInputField;

    [Header("Lobby UI")]
    [SerializeField] private GameObject lobbyBackgroundUI;
    [SerializeField] private Button closeLobbyBackgroundBtn;

    private async void Awake()
    {
        hostBtn.onClick.AddListener(async () =>
        {
            hostBtn.interactable = false;
            await HostSingleton.Instance.GameManager.StartHostAsync();
        });

        exitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        clientBtn.onClick.AddListener(async () =>
        {
            await ClientSingleton.Instance.GameManager.StartClientAsync(lobbyCodeInputField.text);
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
    }


}
