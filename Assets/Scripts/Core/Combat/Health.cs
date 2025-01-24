using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour, IDamageable, IHealable
{
    public Action<Health> OnDie;

    [field: SerializeField] public int maxHealth { get; private set; } = 100;

    public NetworkVariable<int> currentHealth = new();

    private bool isDead;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        currentHealth.Value = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if(isDead) return;

        ModifyHealth(Mathf.RoundToInt(-damage));

    }

    public void Die()
    {
        Debug.Log("Dead!");
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        ModifyHealth(Mathf.RoundToInt(amount));

    }

    private void ModifyHealth(int value)
    {
        if (isDead) return;

        int newHealth = currentHealth.Value + value;

        currentHealth.Value = Mathf.Clamp(newHealth, 0, maxHealth);

        if (currentHealth.Value == 0)
        {
            isDead = true;
            OnDie?.Invoke(this);
        }
    }
}
