using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntity : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    [SerializeField] private Color myColor;

    private FixedString32Bytes playerName;
    public FixedString32Bytes PlayerName => playerName;

    private ulong clientID;
    public ulong ClientID => clientID;

    private int coins;
    public int Coins => coins;

    public void Setup(ulong _clientID, FixedString32Bytes _playerName, int _coins)
    {
        playerName = _playerName;
        clientID = _clientID;

        if (clientID == NetworkManager.Singleton.LocalClientId)
            playerNameTxt.color = myColor;

        UpdateCoins(_coins);
    }

    public void UpdateText()
    {
        playerNameTxt.text = $"{transform.GetSiblingIndex()+ 1}. {playerName} - ({coins})";
    }

    public void UpdateCoins(int newCoins)
    {
        coins = newCoins;
        UpdateText();
    }
}
