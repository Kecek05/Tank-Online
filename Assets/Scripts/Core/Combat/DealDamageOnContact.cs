using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private Projectile projectile;

    [SerializeField] private int damage = 5;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.attachedRigidbody == null) return;

        if(projectile.TeamIndex != -1) // The game mode isnt FFA
        {
            if (collision.attachedRigidbody.TryGetComponent(out TankPlayer player))
            {
                if (player.TeamIndex.Value == projectile.TeamIndex)
                {
                    //Friendly fire
                    return;
                }
            }
        }

        if (collision.attachedRigidbody.TryGetComponent(out IDamageable damageableObject))
        {
            damageableObject.TakeDamage(damage);
        }
    }
}
