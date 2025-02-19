using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{

    [Header("References ")]
    [SerializeField] private CinemachineCamera playerCam;
    [SerializeField] private CinemachineConfiner2D confiner2D;

    [Header("Settings")]
    private int ownerPriority = 10;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerCam.Priority.Value = ownerPriority;
            StartCoroutine(DelayCamerabounds());
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
