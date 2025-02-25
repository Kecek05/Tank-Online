using Sortify;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProjectileLauncher : NetworkBehaviour
{
    public event Action OnPrimaryFire;

    [BetterHeader("Input")]
    [SerializeField] private InputReader inputReader;

    [BetterHeader("References")]
    [SerializeField] private TankPlayer player;
    [SerializeField] private Transform projectileSpawnPoint;

    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;

    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private CoinWallet coinWallet;

    [BetterHeader("Settings")]
    [SerializeField] private float projectileSpeed = 10f;
    [Tooltip("0.5 = 1 bullet each 0.5 secconds")]
    [SerializeField] private float cooldownToFire = 0.5f;

    [SerializeField] private int costToFire;

    [Space]

    [SerializeField] private float muzzleFlashDuration;

    private Color myColor;
    public Color MyColor => myColor;


    private bool isPointerOverUI;
    private bool shouldFire;
    private float cooldownToFireTimer;
    private float muzzleFlashTimer;

    public float GetCooldownToFire() => cooldownToFire;
    public float GetCooldownToFireTimer() => cooldownToFireTimer;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        cooldownToFireTimer = cooldownToFire;

        inputReader.OnPrimaryFireEvent += HandlePrimaryFire_OnPrimaryFireEvent;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        inputReader.OnPrimaryFireEvent -= HandlePrimaryFire_OnPrimaryFireEvent;
    }

    private void Update()
    {

        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }


        if (!IsOwner) return;

        isPointerOverUI = EventSystem.current.IsPointerOverGameObject();

        if (cooldownToFireTimer < cooldownToFire)
            cooldownToFireTimer += Time.deltaTime;

        if (!shouldFire) return;

        if(cooldownToFireTimer < cooldownToFire) return;

        if (!coinWallet.CanSpendCoins(costToFire)) return;

        PrimaryFireRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);

        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up, player.TeamIndex.Value);

        cooldownToFireTimer = 0f;

        OnPrimaryFire?.Invoke();
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction, int teamIndex)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);

        projectileInstance.transform.GetComponentInChildren<SpriteRenderer>().color = myColor; //Change the color to the player color


        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if(projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }

        if (projectileInstance.TryGetComponent<Projectile>(out Projectile projectile))
        {
            projectile.Initialize(teamIndex);
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnDummyProjectileRpc(Vector3 spawnPos, Vector3 direction, int teamIndex)
    {
        if(IsOwner) return;

        SpawnDummyProjectile(spawnPos, direction, teamIndex);
    }



    [Rpc(SendTo.Server)]
    private void PrimaryFireRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (!coinWallet.CanSpendCoins(costToFire)) return;

        coinWallet.SpendCoins(costToFire);

        GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);

        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        //if(projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage)) // now its going to get the dealDamage script from the projectile, because its teams
        //{
        //    dealDamage.SetOwner(OwnerClientId); // who owns this script also owns this projectile
        //}

        if (projectileInstance.TryGetComponent<Projectile>(out Projectile projectile))
        {
            projectile.Initialize(player.TeamIndex.Value);
        }

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }

        SpawnDummyProjectileRpc(spawnPos, direction, player.TeamIndex.Value);
    }


    private void HandlePrimaryFire_OnPrimaryFireEvent(bool shouldFire)
    {
        if(shouldFire && isPointerOverUI) return;

        this.shouldFire = shouldFire;
    }


    public void SetMyColor(Color color)
    {
        myColor = color;
    }
}
