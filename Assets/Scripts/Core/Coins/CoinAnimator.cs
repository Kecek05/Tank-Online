using System;
using UnityEngine;

public class CoinAnimator : MonoBehaviour
{
    private readonly int COIN_SPAWN = Animator.StringToHash("CoinSpawn");

    [SerializeField] private Animator animator;
    [SerializeField] private RespawningCoin coinSpawner;

    private void Start()
    {
        coinSpawner.OnRespawned += HandleCoinRespawned;
    }

    private void HandleCoinRespawned()
    {
        animator.Play(COIN_SPAWN);
    }


    private void OnDestroy()
    {
        coinSpawner.OnRespawned -= HandleCoinRespawned;
    }
}
