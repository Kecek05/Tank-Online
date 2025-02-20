using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{

    [SerializeField] private NetworkObject playePrefab;


    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        TankPlayer[] players = FindObjectsOfType<TankPlayer>();

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
        Destroy(player.gameObject);

        StartCoroutine(DelayRespawnPlayer(player.OwnerClientId));
    }

    private IEnumerator DelayRespawnPlayer(ulong OwnerClientId)
    {
        yield return null;

        NetworkObject playerInstance = Instantiate(playePrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        playerInstance.SpawnAsPlayerObject(OwnerClientId);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        TankPlayer.OnPlayerSpawned -= TankPlayer_OnPlayerSpawned;
        TankPlayer.OnPlayerDespawned -= TankPlayer_OnPlayerDespawned;
    }
}
