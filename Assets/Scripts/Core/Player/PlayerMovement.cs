using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Input")]
    [SerializeField] private InputReader inputReader;

    [Header("Components")]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turningSpeed = 30f;

    private Vector2 previousMovementInput;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputReader.OnMoveEvent += HandleMovement_OnMoveEvent;
    }


    private void Update()
    {
        if (!IsOwner) return;

        float zRotation = previousMovementInput.x * -turningSpeed * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, zRotation);
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;

        rb.linearVelocity = (Vector2)bodyTransform.up * previousMovementInput.y * moveSpeed;
    }

    private void HandleMovement_OnMoveEvent(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        inputReader.OnMoveEvent -= HandleMovement_OnMoveEvent;
    }
}
