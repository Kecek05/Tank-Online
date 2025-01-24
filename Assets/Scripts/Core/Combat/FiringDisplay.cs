using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FiringDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private ProjectileLauncher projectileLauncher;
    [SerializeField] private Image firingBarImage;


    private float cooldownToFire;

    private float cooldownToFireTimer;

    private IEnumerator cooldownToFireCoroutine;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;

        firingBarImage.gameObject.SetActive(true);

        projectileLauncher.OnPrimaryFire += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        projectileLauncher.OnPrimaryFire -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire()
    {
        cooldownToFire = projectileLauncher.GetCooldownToFire();
        cooldownToFireTimer = 0f;
        firingBarImage.fillAmount = 0f;

        if(cooldownToFireCoroutine != null)
        {
            StopCoroutine(cooldownToFireCoroutine);
        }
        cooldownToFireCoroutine = CalculateCooldownToFire();

        StartCoroutine(cooldownToFireCoroutine);
    }

    private IEnumerator CalculateCooldownToFire()
    {

        while (cooldownToFireTimer < cooldownToFire)
        {
            cooldownToFireTimer += Time.deltaTime;
            firingBarImage.fillAmount = cooldownToFireTimer / cooldownToFire;
            yield return null;
        }
        cooldownToFireCoroutine = null;
    }
}
