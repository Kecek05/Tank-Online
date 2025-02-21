using System;
using Unity.Netcode;
using UnityEngine;
using QFSW.QC;
public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;

    [SerializeField] private BountyCoin bountyCoinPrefab;

    [Header("Settings")]
    [SerializeField] private float coinSpread = 3f; //spread radius of coins
    [Tooltip("Percent to lost | 30 = lost only 30% that have")] [SerializeField] private float bountyPercentage; //percent to drop
    [SerializeField] private int bountyCoinCount; // ammount of coins to drop
    [SerializeField] private int minBountyCoinValue; // min ammount of value a bountyCoin can be
    [SerializeField] private LayerMask layerMask;


    private float coinRadius;
    public NetworkVariable<int> totalCoins { get; private set; } = new();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        coinRadius = bountyCoinPrefab.GetComponent<CircleCollider2D>().radius;

        health.OnDie += HandleDeath;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        health.OnDie -= HandleDeath;
    }

    private void HandleDeath(Health health)
    {
        int bountyValue = Mathf.RoundToInt(totalCoins.Value * (bountyPercentage / 100f));
        int bountyCoinValue = bountyValue / bountyCoinCount; //Value of each bounty coin

        if (bountyCoinValue < minBountyCoinValue) return; //Not enough value to spawn bounty coins

        for (int i = 0; i < bountyCoinCount; i++)
        {
            BountyCoin bountyCoinInstance = Instantiate(bountyCoinPrefab, GetSpawnPoint(), Quaternion.identity); //UnityEngine.Random.rotation
            bountyCoinInstance.SetValue(bountyCoinValue);
            bountyCoinInstance.NetworkObject.Spawn(); // inherits from NetworkBehaviour
        }
    }

    private Vector2 GetSpawnPoint()
    {

        while (true)
        {
            // Random point around the player, "insideUnitCircle" one unity around the player, multiply for a bigger radius
            Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * coinSpread; 

            if (Physics2D.OverlapCircle(spawnPoint, coinRadius, layerMask) == null)
            {
                return spawnPoint;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (!collision.TryGetComponent(out Coin coin)) return;

        int collectedCoin = coin.Collect();
        
        AddCoins(collectedCoin);

    }


    [Command("coins-addCoins")]
    public void AddCoins(int amount)
    {
        if (!IsServer) return;
        totalCoins.Value += amount;
    }

    [Command("coins-spendCoins")]
    public void SpendCoins(int amount)
    {
        if (!IsServer) return;
        totalCoins.Value -= amount;
    }


    public bool CanSpendCoins(int valueToSpend)
    {
        return totalCoins.Value >= valueToSpend;
    }

    public int GetCoins()
    {
        return totalCoins.Value;
    }
}
