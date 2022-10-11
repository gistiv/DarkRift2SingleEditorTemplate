using DarkRift;

namespace Common.NetworkingData
{

    public struct PlayerFireWeaponData : IDarkRiftSerializable
    {
        public PlayerFireWeaponData(ushort playerId)
        {
            PlayerId = playerId;
        }

        public ushort PlayerId;

        public void Deserialize(DeserializeEvent e)
        {
            PlayerId = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(PlayerId);
        }

    }
}
