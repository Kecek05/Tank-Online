using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> totalCoins { get; private set; } = new();

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
