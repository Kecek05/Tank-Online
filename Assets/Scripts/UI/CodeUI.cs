using Sortify;
using TMPro;
using Unity.Netcode;
using Unity.Services.Relay;
using UnityEngine;

public class CodeUI : NetworkBehaviour
{
    [BetterHeader("References")]
    [SerializeField] private GameObject codeTitle;
    [SerializeField] private TextMeshProUGUI codeXTxt;


    public override void OnNetworkSpawn()
    {
        //Show relay code

        if(IsHost)
            codeXTxt.text = HostSingleton.Instance.GameManager.JoinCode;
        else if (IsClient)
            codeXTxt.text = ClientSingleton.Instance.GameManager.JoinCode;

        if(codeXTxt.text == null || codeXTxt.text == string.Empty)
        {
            //It's a dedicated server
            codeTitle.SetActive(false);
            codeXTxt.gameObject.SetActive(false);
        }
    }

}
