using DarkRift;
using UnityEngine;

namespace Common.NetworkingData
{

    public struct PlayerSpawnData : IDarkRiftSerializable
    {
        public ushort Id;
        public string Name;
        public Vector3 Position;

        public PlayerSpawnData(ushort id, string name, Vector3 position)
        {
            Id = id;
            Name = name;
            Position = position;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt16();
            Name = e.Reader.ReadString();
            Position = new Vector3(e.Reader.ReadSingle(), e.Reader.ReadSingle(), e.Reader.ReadSingle());
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
            e.Writer.Write(Name);

            e.Writer.Write(Position.x);
            e.Writer.Write(Position.y);
            e.Writer.Write(Position.z);
        }
    }

}
