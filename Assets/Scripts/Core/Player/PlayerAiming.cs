using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{

    [SerializeField] private InputReader inputReader;

    [SerializeField] private Transform turretTransform;

    private Vector2 aimWorldPosition;
    private Vector2 aimScreenPosition;

    private void LateUpdate()
    {
        if (!IsOwner) return;

        aimScreenPosition = inputReader.aimPosition;
        aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);

        

        turretTransform.up = new Vector2(
            aimWorldPosition.x - turretTransform.position.x,
            aimWorldPosition.y - turretTransform.position.y
            );

    }
}
