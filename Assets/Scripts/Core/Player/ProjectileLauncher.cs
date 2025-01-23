using System;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("Input")]
    [SerializeField] private InputReader inputReader;

    [Header("Position")]
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Projectiles")]
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed = 10f;


    private bool shouldFire;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputReader.OnPrimaryFireEvent += HandlePrimaryFire_OnPrimaryFireEvent;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        inputReader.OnPrimaryFireEvent -= HandlePrimaryFire_OnPrimaryFireEvent;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (!shouldFire) return;

        PrimaryFireRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);

        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);

        projectileInstance.transform.up = direction;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnDummyProjectileRpc(Vector3 spawnPos, Vector3 direction)
    {
        if(IsOwner) return;

        SpawnDummyProjectile(spawnPos, direction);
    }



    [Rpc(SendTo.Server)]
    private void PrimaryFireRpc(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);

        projectileInstance.transform.up = direction;

        SpawnDummyProjectileRpc(spawnPos, direction);
    }


    private void HandlePrimaryFire_OnPrimaryFireEvent(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    
}
