using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{

    [Header("References ")]
    [SerializeField] private CinemachineCamera playerCam;

    [Header("Settings")]
    private int ownerPriority = 10;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerCam.Priority.Value = ownerPriority;
        }
    }
}
