using Sortify;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [BetterHeader("Input")]
    [SerializeField] private InputReader inputReader;

    [BetterHeader("Components")]
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ParticleSystem dustCloud;
    private ParticleSystem.EmissionModule dustCloudEmissionModule;

    [BetterHeader("Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turningSpeed = 30f;
    [SerializeField] private float particleEmissionRate = 10f;

    private const float PARTICLE_STOP_THRESHOLD = 0.005f;

    private Vector2 previousMovementInput;
    private Vector3 previousPosition;

    private void Awake()
    {
        dustCloudEmissionModule = dustCloud.emission;
    }

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
        if((transform.position - previousPosition).sqrMagnitude > PARTICLE_STOP_THRESHOLD) // if we have moved since last frame
        {
            dustCloudEmissionModule.rateOverTime = particleEmissionRate;
        } else
        {
            dustCloudEmissionModule.rateOverTime = 0;
        }

        previousPosition = transform.position;

        if (!IsOwner) return;

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
