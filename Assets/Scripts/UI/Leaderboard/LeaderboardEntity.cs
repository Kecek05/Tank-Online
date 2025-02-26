using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntity : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    //[SerializeField] private Color myColor;

    private FixedString32Bytes displayName;
    public FixedString32Bytes DisplayName => displayName;

    private int teamIndex;
    public int TeamIndex => teamIndex;

    private ulong clientID;
    public ulong ClientID => clientID;

    private int coins;
    public int Coins => coins;

    public void Setup(ulong _clientID, FixedString32Bytes _displayName, int _coins)
    {
        displayName = _displayName;
        clientID = _clientID;

        //if (clientID == NetworkManager.Singleton.LocalClientId)
        //    playerNameTxt.color = myColor;

        UpdateCoins(_coins);
    }

    public void Setup(int _teamIndex, FixedString32Bytes _displayName, int _coins)
    {
        displayName = _displayName;
        teamIndex = _teamIndex;

        UpdateCoins(_coins);
    }

    public void SetColor(Color color)
    {
        playerNameTxt.color = color;
    }

    public void UpdateText()
    {
        playerNameTxt.text = $"{transform.GetSiblingIndex()+ 1}. {displayName} - ({coins})";
    }

    public void UpdateCoins(int newCoins)
    {
        coins = newCoins;
        UpdateText();
    }
}
