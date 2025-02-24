using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameTxt;
    [SerializeField] private TMP_Text lobbyPlayersTxt;
    [SerializeField] private Button joinBtn;

    private LobbiesList lobbiesList;
    private Lobby lobby;


    public void Initialise(LobbiesList lobbiesList ,Lobby lobby)
    {
        this.lobbiesList = lobbiesList;
        this.lobby = lobby;

        lobbyNameTxt.text = lobby.Name;
        lobbyPlayersTxt.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";


        joinBtn.onClick.AddListener(() =>
        {
            // call to join the lobby
            lobbiesList.JoinAsync(lobby);
        });
    }



}
