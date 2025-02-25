using UnityEngine;

[CreateAssetMenu(fileName = "NewTeamColorLookup", menuName = "ScriptableObjects/TeamColorLookup")]
public class TeamColorLookup : ScriptableObject
{
    [SerializeField] private Color[] teamColors;

    public Color GetTeamColor(int teamIndex)
    {
        if(teamIndex < 0 || teamIndex >= teamColors.Length) // Dont have a team or its FFA
        {
            return Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
        } else
        {
            return teamColors[teamIndex];
        }
    }
}
