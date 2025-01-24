using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;

    private ulong ownerClientId;

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.attachedRigidbody == null) return;

        if(collision.attachedRigidbody.TryGetComponent(out NetworkObject netObj))
        {
            if (netObj.OwnerClientId == ownerClientId) return;
        }

        if (collision.attachedRigidbody.TryGetComponent(out IDamageable damageableObject))
        {
            damageableObject.TakeDamage(damage);
        }
    }
}
