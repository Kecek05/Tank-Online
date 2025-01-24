using Unity.Netcode;
using UnityEngine;

public abstract class Coin : NetworkBehaviour
{
    [SerializeField] private GameObject coinGFX;

    protected int coinValue;
    protected bool alreadyCollected;

    public abstract int Collect();

    public void SetValue(int value)
    {
        coinValue = value;
    }

    protected void Show(bool show)
    {
        coinGFX.SetActive(show);
    }
}
