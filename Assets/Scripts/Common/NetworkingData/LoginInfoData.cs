using DarkRift;

namespace Common.NetworkingData
{
	
	public struct LoginInfoData : IDarkRiftSerializable
    {
        public ushort Id;

        public LoginInfoData(ushort id)
        {
            Id = id;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Id = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Id);
        }
    }
	
}
