using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Image healthBarImage;

    public override void OnNetworkSpawn()
    {
        if(!IsClient) return;

        health.currentHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0, health.currentHealth.Value); // if changed before subscribing
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;

        health.currentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        healthBarImage.fillAmount = (float)newHealth / health.maxHealth;
    }
}
