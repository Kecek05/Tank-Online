using Sortify;
using Unity.Netcode;
using UnityEngine;

public class PlayerColorDisplay : NetworkBehaviour
{
    [BetterHeader("References")]
    [SerializeField] private TeamColorLookup teamColorLookup;
    [SerializeField] private TankPlayer tankPlayer;
    [SerializeField] private SpriteRenderer[] tankSpriteRenderers;
    [SerializeField] private SpriteRenderer minimapSpriteRenderer;

    private void Start()
    {
        OnTeamIndexChanged(-1, tankPlayer.TeamIndex.Value); //call manually just in case to not subscribe in time

        tankPlayer.TeamIndex.OnValueChanged += OnTeamIndexChanged;

    }

    private void OnTeamIndexChanged(int previousValue, int newValue)
    {
        Color myTeamColor = teamColorLookup.GetTeamColor(newValue);

        foreach (SpriteRenderer spriteRenderer in tankSpriteRenderers)
        {
            spriteRenderer.color = myTeamColor;
        }

        if(IsOwner)
        {
            minimapSpriteRenderer.color = tankPlayer.OwnerMinimapColor;
        }
    }

    private void OnDestroy()
    {
        tankPlayer.TeamIndex.OnValueChanged -= OnTeamIndexChanged;
    }


}
