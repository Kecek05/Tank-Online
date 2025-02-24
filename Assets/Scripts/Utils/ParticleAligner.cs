using UnityEngine;

public class ParticleAligner : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    private ParticleSystem.MainModule particleSystemMainModule;

    private void Start()
    {
        particleSystemMainModule = particleSystem.main;
    }

    private void Update()
    {
        if (particleSystem == null) return;

        
        particleSystemMainModule.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad; // need to multiply because startRotation it's in radians and euler angles are in degrees
    }
}
