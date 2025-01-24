using System;
using UnityEngine;

public class RespawningCoin : Coin
{
    public event Action<RespawningCoin> OnCollected;

    private Vector3 previousPosition;

    public override int Collect()
    {

        if(!IsServer)
        {
            Show(false);
            return 0;
        }

        if (alreadyCollected) return 0;

        alreadyCollected = true;

        OnCollected?.Invoke(this);

        return coinValue;

    }

    private void Update()
    {
        if (!IsClient) return; // only non-server players

        if(transform.position != previousPosition)
        {
            Show(true);
        }

        previousPosition = transform.position;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}
