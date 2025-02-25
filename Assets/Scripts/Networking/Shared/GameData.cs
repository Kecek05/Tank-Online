using System;


public enum Map
{
    Default,
}

public enum GameMode
{
    Default,
}

public enum GameQueue
{
    Solo,
    Team,
}

[Serializable]
public class UserData
{
    public string userName;
    public string userAuthId;
    public int teamIndex = -1; // If not playing in a team, its -1 else its a team Index valid from 0 to 3

    public GameInfo userGamePreferences = new GameInfo();
}

[Serializable]
public class GameInfo
{
    public Map map;
    public GameMode gameMode;
    public GameQueue gameQueue;

    public string ToMultiplayQueue()
    {
        return gameQueue switch
        {
            GameQueue.Solo => "solo-queue",
            GameQueue.Team => "team-queue",
            _ => "solo-queue" //_ = default
        };
            
    }
}
