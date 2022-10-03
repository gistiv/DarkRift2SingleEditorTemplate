using DarkRift;
using UnityEngine;

namespace Common.NetworkingData
{
	
	public struct PlayerStateData : IDarkRiftSerializable
	{
        public PlayerStateData(ushort id, float gravity, Vector3 position, float yaw, float pitch)
        {
            Id = id;
            Position = position;
            Gravity = gravity;
            Yaw = yaw;
            Pitch = pitch;
        }

        public ushort Id;
        public Vector3 Position;
        public float Gravity;
        public float Yaw;
        public float Pitch;

        public void Deserialize(DeserializeEvent e)
        {
            Position = new Vector3(e.Reader.ReadSingle(), e.Reader.ReadSingle(), e.Reader.ReadSingle());
            Id = e.Reader.ReadUInt16();
            Gravity = e.Reader.ReadSingle();
            Yaw = e.Reader.ReadSingle();
            Pitch = e.Reader.ReadSingle();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Position.x);
            e.Writer.Write(Position.y);
            e.Writer.Write(Position.z);

            e.Writer.Write(Yaw);
            e.Writer.Write(Pitch);
            e.Writer.Write(Id);
            e.Writer.Write(Gravity);
        }
    }
	
}
