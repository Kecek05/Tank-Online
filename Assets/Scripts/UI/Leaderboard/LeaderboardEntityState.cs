using System;
using Unity.Collections;
using Unity.Netcode;

public struct LeaderboardEntityState : INetworkSerializable, IEquatable<LeaderboardEntityState>
{
    public ulong ClientId;
    public int TeamIndex;
    public FixedString32Bytes PlayerName;

    public int Coins;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref TeamIndex);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Coins);
    }

    public bool Equals(LeaderboardEntityState other)
    {
        // used to determine if two instances of LeaderboardEntityState are equal
        return ClientId == other.ClientId && PlayerName.Equals(other.PlayerName) && Coins == other.Coins && TeamIndex == other.TeamIndex;
    }
}
