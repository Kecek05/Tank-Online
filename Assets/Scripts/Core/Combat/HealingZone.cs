using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Image healPowerBar;

    [Header("Settings")]

    [SerializeField] private int maxHealPower; //how many times can restore health

    [Tooltip("In Seconds")][SerializeField] private float healCooldown; // once runs out of healPower, how long to wait to restore to full in seconds
    [Tooltip("In Seconds")][SerializeField] private float healTickRate; //how often to heal in seconds

    [Tooltip("Cost in coins")][SerializeField] private int coinsPerTick; //how many coins to spend per tick
    [Tooltip("Health restored each tick")][SerializeField] private int healthPerTick; //how much health to restore per tick

    private List<TankPlayer> playersInZone = new List<TankPlayer>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsServer) return;

        if (collision.attachedRigidbody.TryGetComponent(out TankPlayer tankPlayer))
        {
            playersInZone.Add(tankPlayer);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.attachedRigidbody.TryGetComponent(out TankPlayer tankPlayer))
        {
            if(playersInZone.Contains(tankPlayer)) //null check
            {
                playersInZone.Remove(tankPlayer);
            }
        }
    }
}
