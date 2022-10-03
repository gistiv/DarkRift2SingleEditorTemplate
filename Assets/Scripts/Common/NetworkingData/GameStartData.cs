using DarkRift;

namespace Common.NetworkingData
{

    public struct GameStartData : IDarkRiftSerializable
    {
        public uint OnJoinServerTick;
        public PlayerSpawnData[] Players;

        public GameStartData(PlayerSpawnData[] players, uint serverTick)
        {
            Players = players;
            OnJoinServerTick = serverTick;
        }

        public void Deserialize(DeserializeEvent e)
        {
            OnJoinServerTick = e.Reader.ReadUInt32();
            Players = e.Reader.ReadSerializables<PlayerSpawnData>();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(OnJoinServerTick);
            e.Writer.Write(Players);
        }
    }

}
