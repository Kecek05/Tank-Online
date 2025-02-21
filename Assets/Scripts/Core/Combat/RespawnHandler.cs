using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{

    [SerializeField] private TankPlayer playePrefab;
    [Tooltip("Percent to keep | 30 = keep only 30% that have")][SerializeField] private float coinKeepPercentage;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

        foreach(TankPlayer player in players)
        {
            TankPlayer_OnPlayerSpawned(player);
        }

        TankPlayer.OnPlayerSpawned += TankPlayer_OnPlayerSpawned;
        TankPlayer.OnPlayerDespawned += TankPlayer_OnPlayerDespawned;
    }

    private void TankPlayer_OnPlayerSpawned(TankPlayer player)
    {
        player.Health.OnDie += (health) => HandlePlayerDie(player);
    }

    private void TankPlayer_OnPlayerDespawned(TankPlayer player)
    {
        player.Health.OnDie -= (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(TankPlayer player)
    {
        int coinsToKeep = Mathf.RoundToInt(player.CoinWallet.GetCoins() * (coinKeepPercentage / 100));

        Destroy(player.gameObject);

        StartCoroutine(DelayRespawnPlayer(player.OwnerClientId, coinsToKeep));
    }

    private IEnumerator DelayRespawnPlayer(ulong OwnerClientId, int coinsToKeep)
    {
        yield return null;

        TankPlayer playerInstance = Instantiate(playePrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        playerInstance.NetworkObject.SpawnAsPlayerObject(OwnerClientId); 


        playerInstance.CoinWallet.AddCoins(coinsToKeep); // must be after spawn as network player object

    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        TankPlayer.OnPlayerSpawned -= TankPlayer_OnPlayerSpawned;
        TankPlayer.OnPlayerDespawned -= TankPlayer_OnPlayerDespawned;
    }
}
