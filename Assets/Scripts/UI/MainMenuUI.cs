using Sortify;
using System.Threading.Tasks;
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

    [BetterHeader("MatchMaking References")]
    [SerializeField] private Button findMatchBtn;
    [SerializeField] private TextMeshProUGUI findMatchBtnText;
    [SerializeField] private TextMeshProUGUI timeInQueueTxt;
    [SerializeField] private TextMeshProUGUI queueStatusTxt;
    private bool isMatchmaking;
    private bool isCancelling;

    [BetterHeader("PlayerInfo References")]
    [SerializeField] private PlayerInfoUI playerInfoUI;
    [SerializeField] private RenameUI renameUI;


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
            await ClientSingleton.Instance.GameManager.StartRelayClientAsync(lobbyCodeInputField.text);
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

        findMatchBtn.onClick.AddListener(async () =>
        {
            if(isCancelling) return;


            if (isMatchmaking)
            {
                //Cancel matchmaking
                queueStatusTxt.text = "Cancelling matchmaking...";
                isCancelling = true;
                await ClientSingleton.Instance.GameManager.CancelMatchmaking();
                isCancelling = false;
                isMatchmaking = false;
                findMatchBtnText.text = "FIND MATCH";
                queueStatusTxt.text = string.Empty;
                timeInQueueTxt.text = string.Empty;
                UnlocksAllButtons();
                HideBackgroundJoining();
                return;
            }

            ClientSingleton.Instance.GameManager.MatchmakeAsync(OnMatchMade);
            LocksAllButtons();
            findMatchBtnText.text = "CANCEL";
            queueStatusTxt.text = "Searching for a match...";
            isMatchmaking = true;




        });
    }

    private void OnMatchMade(MatchmakerPollingResult result) // as soon as the match has been made, this method will be called
    {
        switch(result)
        {
            case MatchmakerPollingResult.Success:
                queueStatusTxt.text = "Match found!";
                ShowBackgroundJoining();
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                queueStatusTxt.text = "Match assignment error!";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                queueStatusTxt.text = "Ticket cancellation error!";
                break;
            case MatchmakerPollingResult.TicketRetrievalError:
                queueStatusTxt.text = "Ticket retrieval error!";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                queueStatusTxt.text = "Ticket creation error!";
                break;

        }
    }

    private void Start()
    {
        if (ClientSingleton.Instance == null) return; //It's dedicated server

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        lobbyBackgroundUI.SetActive(false);
        HideBackgroundJoining();

        //Clear Matchmaking
        timeInQueueTxt.text = string.Empty;
        queueStatusTxt.text = string.Empty;
    }

    private void LocksAllButtons()
    {
        playerInfoUI.LockPlayerInfoUI();
        renameUI.Hide();

        lobbyCodeInputField.interactable = false;
        clientBtn.interactable = false;
        hostBtn.interactable = false;
        lobbiesBtn.interactable = false;
        lobbyBackgroundUI.SetActive(false);
    }

    private void UnlocksAllButtons()
    {
        playerInfoUI.UnlockPlayerInfoUI();

        lobbyCodeInputField.interactable = true;
        clientBtn.interactable = true;
        hostBtn.interactable = true;
        lobbiesBtn.interactable = true;
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
