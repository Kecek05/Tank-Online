using Sortify;
using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{

    [BetterHeader("References ")]
    [SerializeField] private CinemachineCamera playerCam;
    [SerializeField] private CinemachineConfiner2D confiner2D;
    [SerializeField] private Health health;
    public Health Health => health;
    [SerializeField] private CoinWallet coinWallet;
    public CoinWallet CoinWallet => coinWallet;


    [BetterHeader("Settings")]
    private int ownerPriority = 10;

    public NetworkVariable<FixedString32Bytes> PlayerName { get; private set; } = new NetworkVariable<FixedString32Bytes>(new FixedString32Bytes()); // similar to string

    public static event Action<TankPlayer> OnPlayerSpawned;

    public static event Action<TankPlayer> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            UserData userdata = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            PlayerName.Value = userdata.userName;

            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            playerCam.Priority.Value = ownerPriority;
            StartCoroutine(DelayCamerabounds());
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }

    private IEnumerator DelayCamerabounds()
    {
        //Delay to set the camera bounds

        while (confiner2D.BoundingShape2D == null)
        {
            GameObject cameraBounds = GameObject.FindGameObjectWithTag("CameraBounds");
            if (cameraBounds != null)
                confiner2D.BoundingShape2D = cameraBounds.GetComponent<PolygonCollider2D>();
                
            yield return null;
        }
    }
}
