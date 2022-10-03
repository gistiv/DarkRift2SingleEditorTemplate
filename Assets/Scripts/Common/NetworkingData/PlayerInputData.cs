using DarkRift;
using UnityEngine;

namespace Common.NetworkingData
{
	
	public struct PlayerInputData : IDarkRiftSerializable
    {
        public bool[] Keyinputs; // 0 = w, 1 = a, 2 = s, 3 = d, 4 = space, 5 = shift, 6 = mousebuffton left
        public float Yaw;
        public float Pitch;
        public uint Time;

        public PlayerInputData(bool[] keyInputs, float yaw, float pitch, uint time)
        {
            Keyinputs = keyInputs;
            Yaw = yaw;
            Pitch = pitch;
            Time = time;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Keyinputs = e.Reader.ReadBooleans();
            Yaw = e.Reader.ReadSingle();
            Pitch = e.Reader.ReadSingle();

            if (Keyinputs[6])
            {
                Time = e.Reader.ReadUInt32();
            }
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Keyinputs);
            e.Writer.Write(Yaw);
            e.Writer.Write(Pitch);

            if (Keyinputs[6])
            {
                e.Writer.Write(Time);
            }
        }
    }
	
}
