using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardUI : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityHolder;
    [SerializeField] private GameObject leaderboardEntityPrefab;

    private NetworkList<LeaderboardEntityState> leaderboardEntities;

    private List<LeaderboardEntity> entityDisplays = new List<LeaderboardEntity>();

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            leaderboardEntities.OnListChanged += LeaderboardEntities_OnListChanged;

            foreach(LeaderboardEntityState entity in leaderboardEntities)
            {
                LeaderboardEntities_OnListChanged(new NetworkListEvent<LeaderboardEntityState>
                {
                    Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                    Value = entity
                });
            }
        }

        if(!IsServer) return;

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
    }

    private void LeaderboardEntities_OnListChanged(NetworkListEvent<LeaderboardEntityState> changeEvent)
    {
        switch(changeEvent.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                if(!entityDisplays.Any(x => x.ClientID == changeEvent.Value.ClientId)) // see if there isn't any item on the list that have the same clientID | same as foreach blablabla
                {
                    LeaderboardEntity leaderboardEntity = Instantiate(leaderboardEntityPrefab, leaderboardEntityHolder).GetComponent<LeaderboardEntity>();

                    leaderboardEntity.Setup(changeEvent.Value.ClientId, changeEvent.Value.PlayerName, changeEvent.Value.Coins);

                    entityDisplays.Add(leaderboardEntity);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                LeaderboardEntity displayToRemove = entityDisplays.FirstOrDefault(x => x.ClientID == changeEvent.Value.ClientId); // get the first element that matches, if not found return null (default = null)
                if(displayToRemove != null)
                {
                    displayToRemove.transform.SetParent(null); // first remove from parent to prevent bugs
                    Destroy(displayToRemove.gameObject); 
                    entityDisplays.Remove(displayToRemove); 
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value: // when the value of the entity changes
                LeaderboardEntity displayToUpdate = entityDisplays.FirstOrDefault(x => x.ClientID == changeEvent.Value.ClientId); // get the first element that matches, if not found return null (default = null)
                if (displayToUpdate != null)
                {
                    displayToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= LeaderboardEntities_OnListChanged;
        }

        if (!IsServer) return;

        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        leaderboardEntities.Add(new LeaderboardEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.PlayerName.Value,
            Coins = 0
        });
        
        player.CoinWallet.totalCoins.OnValueChanged += (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins); //int previousValue, int newValue sintax of OnValueChanged
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        if(leaderboardEntities == null) return; //safety check

        foreach (LeaderboardEntityState entity in leaderboardEntities)
        {
            if(entity.ClientId != player.OwnerClientId) continue;

            leaderboardEntities.Remove(entity);

            break;
        }

        player.CoinWallet.totalCoins.OnValueChanged -= (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
    }

    private void HandleCoinsChanged(ulong clientId, int newCoins)
    {
        for(int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].ClientId != clientId) continue;

            leaderboardEntities[i] = new LeaderboardEntityState
            {
                ClientId = clientId,
                PlayerName = leaderboardEntities[i].PlayerName,
                Coins = newCoins
            };
            return;
        }
    }
}
