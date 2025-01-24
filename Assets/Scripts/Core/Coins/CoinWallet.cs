using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> totalCoins = new();


    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (!collision.TryGetComponent(out Coin coin)) return;

        int collectedCoin = coin.Collect();
        
        if(!IsServer) return;

        totalCoins.Value += collectedCoin;


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
}
