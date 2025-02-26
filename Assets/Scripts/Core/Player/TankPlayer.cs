using QFSW.QC;
using Sortify;
using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{

    [BetterHeader("References ")]
    [SerializeField] private CinemachineCamera playerCam;
    [SerializeField] private CinemachineConfiner2D confiner2D;
    [SerializeField] private Health health;
    public Health Health => health;
    [SerializeField] private CoinWallet coinWallet;
    public CoinWallet CoinWallet => coinWallet;

    [SerializeField] private SpriteRenderer minimapIconspriteRenderer;

    [SerializeField] private Texture2D crosshair;


    [BetterHeader("Settings")]
    private int ownerPriority = 10;

    [SerializeField] private Color ownerMinimapColor;
    public Color OwnerMinimapColor => ownerMinimapColor;


    public NetworkVariable<FixedString32Bytes> PlayerName { get; private set; } = new NetworkVariable<FixedString32Bytes>(); // similar to string
    public NetworkVariable<int> TeamIndex { get; private set; } = new NetworkVariable<int>();

    public static event Action<TankPlayer> OnPlayerSpawned;

    public static event Action<TankPlayer> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            UserData userData = null;

            if(IsHost)
            {
                userData = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
                

            } else
            {
                userData = ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }

            PlayerName.Value = userData.userName;
            TeamIndex.Value = userData.teamIndex;


            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            playerCam.Priority.Value = ownerPriority;
            StartCoroutine(DelayCamerabounds());

            minimapIconspriteRenderer.color = ownerMinimapColor;

            Cursor.SetCursor(crosshair, new Vector2(crosshair.width / 2, crosshair.height / 2), CursorMode.Auto);
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }

    private IEnumerator DelayCamerabounds()
    {
        //Delay to set the camera bounds

        while (confiner2D.BoundingShape2D == null)
        {
            GameObject cameraBounds = GameObject.FindGameObjectWithTag("CameraBounds");
            if (cameraBounds != null)
                confiner2D.BoundingShape2D = cameraBounds.GetComponent<PolygonCollider2D>();
                
            yield return null;
        }
    }

    [Command("player-setTeam")]
    public void DebugSetPlayerTeam(int teamIndex)
    {
        DebugSetPlayerTeamRpc(teamIndex);
        Debug.Log($"Player {PlayerName.Value} set to team {teamIndex}");
    }

    [Rpc(SendTo.Server)]
    private void DebugSetPlayerTeamRpc(int teamIndex)
    {
        TeamIndex.Value = teamIndex;
    }
}
