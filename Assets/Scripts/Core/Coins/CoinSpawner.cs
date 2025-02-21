using QFSW.QC;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{

    [SerializeField] private RespawningCoin coinPrefab;

    [SerializeField] private int maxCoins = 50;
    [SerializeField] private int coinValue = 10;

    [SerializeField] private Vector2 xSpawnRange;
    [SerializeField] private Vector2 ySpawnRange;

    [SerializeField] private LayerMask layerMask;

    private float coinRadius;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        for(int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }

    [Command("coinSpawner-spawnCoin")]
    private void SpawnCoin()
    {
        RespawningCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);

        coinInstance.SetValue(coinValue);

        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.OnCollected += HandleCoinCollected;
    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;

        while(true)
        {
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);
            Vector2 spawnPoint = new Vector2(x, y);

            if(Physics2D.OverlapCircle(spawnPoint, coinRadius, layerMask) == null)
            {
                return spawnPoint;
            }
            
        }
    }
}
