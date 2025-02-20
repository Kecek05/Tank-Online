using TMPro;
using Unity.Collections;
using UnityEngine;

public class LeaderboardEntity : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTxt;

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

        UpdateCoins(_coins);
    }

    private void UpdateText()
    {
        playerNameTxt.text = $"1. {playerName} - ({coins})";
    }

    public void UpdateCoins(int newCoins)
    {
        coins = newCoins;
        UpdateText();
    }
}
