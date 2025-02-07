using TMPro;
using Unity.Netcode;
using Unity.Services.Relay;
using UnityEngine;

public class CodeUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI codeXTxt;

    public override void OnNetworkSpawn()
    {
        //Show relay code

        if(IsHost)
            codeXTxt.text = HostSingleton.Instance.GameManager.JoinCode;
        else if (IsClient)
            codeXTxt.text = ClientSingleton.Instance.GameManager.JoinCode;
    }

}
