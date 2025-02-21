using System;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;

    [SerializeField] private BountyCoin bountyCoinPrefab;

    [Header("Settings")]
    [SerializeField] private float coinSpread = 3f;
    [SerializeField] private int bountyCoinCount = 20;
    [SerializeField] private int minBountyCoinValue = 5;
    [SerializeField] private LayerMask layerMask;

    private Collider2D[] coinBuffer = new Collider2D[1];

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
        throw new NotImplementedException();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (!collision.TryGetComponent(out Coin coin)) return;

        int collectedCoin = coin.Collect();
        
        AddCoins(collectedCoin);

    }

    public void AddCoins(int amount)
    {
        if (!IsServer) return;
        totalCoins.Value += amount;
    }

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
