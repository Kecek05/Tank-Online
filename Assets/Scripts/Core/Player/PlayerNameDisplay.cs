using System;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{
    [SerializeField] private TankPlayer tankPlayer;
    [SerializeField] private TMP_Text playerNameText;

    private void Start()
    {
        OnPlayerNameChanged(string.Empty, tankPlayer.playerName.Value); //force check, in case the value is already set

        tankPlayer.playerName.OnValueChanged += OnPlayerNameChanged;

    }

    private void OnPlayerNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        playerNameText.text = newValue.ToString();
    }

    private void OnDestroy()
    {
        tankPlayer.playerName.OnValueChanged -= OnPlayerNameChanged;
    }
}
