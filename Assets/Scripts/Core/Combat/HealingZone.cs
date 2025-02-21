using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Sortify;
using System.Collections;

public class HealingZone : NetworkBehaviour
{
    [BetterHeader("References")]
    [SerializeField] private Image healPowerBar;

    [BetterHeader("Settings")]

    [SerializeField] private int maxHealPower; //how many times can restore health

    [Unit("s")][Tooltip("In Seconds")]
    [SerializeField] private float healCooldown; // once runs out of healPower, how long to wait to restore to full in seconds
    private float remainingHealCooldown;

    [Unit("s")][Tooltip("In Seconds")]
    [SerializeField] private float healTickRate; //how often to heal in seconds

    [Unit("  coins")][Tooltip("Cost in coins")]
    [SerializeField] private int coinsPerTick; //how many coins to spend per tick

    [Unit("  health")][Tooltip("Health restored each tick")]
    [SerializeField] private int healthPerTick; //how much health to restore per tick

    private List<TankPlayer> playersInZone = new List<TankPlayer>();

    private NetworkVariable<int> healPower = new(); //heal power left

    private Coroutine handleHealCooldownCoroutine;
    private Coroutine handlePlayerHealCoroutine;
    private WaitForSeconds healTickRateSeconds; //cache

    private void Start()
    {
        healTickRateSeconds = new WaitForSeconds(healTickRate);
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            healPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(0, healPower.Value); //Need to call manualy if a client joins after the value has been changed
        }

        if(IsServer)
        {
            healPower.Value = maxHealPower;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            healPower.OnValueChanged -= HandleHealPowerChanged;
        }
    }

    private IEnumerator HandleHealCooldown()
    {
        remainingHealCooldown = healCooldown;

        while(remainingHealCooldown > 0f)
        {
            remainingHealCooldown -= Time.deltaTime;
            yield return null;
        }

        //waited all cooldown, restore heal power
        healPower.Value = maxHealPower;

        if(playersInZone.Count > 0 && handlePlayerHealCoroutine == null) //if players are in zone, start healing
        {
            handlePlayerHealCoroutine = StartCoroutine(HandlePlayerHeal());
        }

        handleHealCooldownCoroutine = null; //clear
    }

    private IEnumerator HandlePlayerHeal()
    {
        while (healPower.Value > 0)
        {
            foreach (TankPlayer player in playersInZone)
            {
                if(player.Health.currentHealth.Value >= player.Health.maxHealth) continue; //max health, dont heal

                if (player.CoinWallet.totalCoins.Value < coinsPerTick) continue; //not enough coins, dont heal

                player.CoinWallet.SpendCoins(coinsPerTick);
                player.Health.Heal(healthPerTick);

                healPower.Value--;
            }
            yield return healTickRateSeconds;
        }

        if (handleHealCooldownCoroutine == null) //start reseting cooldown
        {
            handleHealCooldownCoroutine = StartCoroutine(HandleHealCooldown());
        }

        handlePlayerHealCoroutine = null; //clear
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsServer) return;

        if (collision.attachedRigidbody.TryGetComponent(out TankPlayer tankPlayer))
        {
            playersInZone.Add(tankPlayer);
        }

        if(handlePlayerHealCoroutine == null)
        {
            handlePlayerHealCoroutine = StartCoroutine(HandlePlayerHeal());
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

                if(playersInZone.Count == 0)
                {
                    if(handlePlayerHealCoroutine != null) //if not null, stop the healing
                    {
                        StopCoroutine(handlePlayerHealCoroutine);
                        handlePlayerHealCoroutine = null;
                    }

                    if (handleHealCooldownCoroutine == null && healPower.Value <= 0) //if stoped and should reset the healing
                    {
                        handleHealCooldownCoroutine = StartCoroutine(HandleHealCooldown());
                    }
                }
            }
        }
    }

    private void HandleHealPowerChanged(int oldHealPower, int newHealPower)
    {
        healPowerBar.fillAmount = (float)newHealPower / maxHealPower;
    }
}
