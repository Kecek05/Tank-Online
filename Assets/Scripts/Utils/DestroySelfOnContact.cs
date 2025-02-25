using UnityEngine;

public class DestroySelfOnContact : MonoBehaviour
{
    [SerializeField] private Projectile projectile;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(projectile.TeamIndex != -1) // The game mode isnt FFA
        {
            if (collision.attachedRigidbody.TryGetComponent(out TankPlayer tankPlayer))
            {
                if (tankPlayer.TeamIndex.Value == projectile.TeamIndex)
                {
                    //Friendly fire
                    return;
                }
            }
        }


        Destroy(gameObject);
    }
}
