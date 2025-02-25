using UnityEngine;

public class Projectile : MonoBehaviour
{

    private int teamIndex;
    public int TeamIndex => teamIndex;

    public void Initialize(int teamIndex)
    {
        this.teamIndex = teamIndex;
    }

}
