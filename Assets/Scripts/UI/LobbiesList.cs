using Sortify;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbiesList : MonoBehaviour
{
    [BetterHeader("References")]
    [SerializeField] private MainMenuUI mainMenuUI;
    [SerializeField] private LobbyItem lobbyItemPrefab;
    [SerializeField] private Transform lobbyItemParent;
    [SerializeField] private Button refreshBtn;

    private bool isJoining;
    private bool isRefreshing;

    private void Awake()
    {
        refreshBtn.onClick.AddListener(RefreshListAsync);
    }

    private void OnEnable()
    {
        RefreshListAsync();
    }

    public async void RefreshListAsync()
    {
        if (isRefreshing) return;

        isRefreshing = true;

        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions();
            queryLobbiesOptions.Count = 25;

            queryLobbiesOptions.Filters = new List<QueryFilter>()
            {
            new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GT, value: "0"),
            new QueryFilter(field: QueryFilter.FieldOptions.IsLocked, op: QueryFilter.OpOptions.EQ, value: "0") // 0 = false
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            // clear list
            foreach (Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            // add to  list
            foreach(Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                lobbyItem.Initialise(this, lobby);
            }

        } catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        

        isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining) return;

        isJoining = true;
        mainMenuUI.ShowBackgroundJoining();
        // Join the lobby
        try
        {
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joinedLobby.Data["JoinCode"].Value;
           
            await ClientSingleton.Instance.GameManager.StartRelayClientAsync(joinCode);

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        isJoining = false;
        mainMenuUI.HideBackgroundJoining();
    }
}
