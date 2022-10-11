using DarkRift;
using UnityEngine;

namespace Common.NetworkingData
{
	
	public struct BulletHitData : IDarkRiftSerializable
    {
        public BulletHitData(ushort playerId, Vector3 hitPoint, Vector3 hitNormal)
        {
            PlayerId = playerId;
            HitPoint = hitPoint;
            HitNormal = hitNormal;
        }

        // you may wanna add here how much damage it took
        public ushort PlayerId;
        public Vector3 HitNormal;
        public Vector3 HitPoint;

        public void Deserialize(DeserializeEvent e)
        {
            PlayerId = e.Reader.ReadUInt16();
            HitPoint = new Vector3(e.Reader.ReadSingle(), e.Reader.ReadSingle(), e.Reader.ReadSingle());
            HitNormal = new Vector3(e.Reader.ReadSingle(), e.Reader.ReadSingle(), e.Reader.ReadSingle());
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(PlayerId);

            e.Writer.Write(HitPoint.x);
            e.Writer.Write(HitPoint.y);
            e.Writer.Write(HitPoint.z);

            e.Writer.Write(HitNormal.x);
            e.Writer.Write(HitNormal.y);
            e.Writer.Write(HitNormal.z);
        }
    }
	
}
