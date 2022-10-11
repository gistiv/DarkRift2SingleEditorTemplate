using DarkRift;

namespace Common.NetworkingData
{

    public struct GameUpdateData : IDarkRiftSerializable
    {
        public uint Frame;
        public PlayerSpawnData[] SpawnData;
        public PlayerDespawnData[] DespawnData;
        public PlayerStateData[] UpdateData;
        public PlayerFireWeaponData[] FireWeaponData;
        public BulletHitData[] HitData;

        public GameUpdateData(uint frame, PlayerStateData[] updateData, PlayerSpawnData[] spawnData, PlayerDespawnData[] despawnData, PlayerFireWeaponData[] fireWeaponData, BulletHitData[] bulletHitData)
        {
            Frame = frame;
            UpdateData = updateData;
            DespawnData = despawnData;
            SpawnData = spawnData;
            FireWeaponData = fireWeaponData;
            HitData = bulletHitData;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Frame = e.Reader.ReadUInt32();
            SpawnData = e.Reader.ReadSerializables<PlayerSpawnData>();
            DespawnData = e.Reader.ReadSerializables<PlayerDespawnData>();
            UpdateData = e.Reader.ReadSerializables<PlayerStateData>();
            FireWeaponData = e.Reader.ReadSerializables<PlayerFireWeaponData>();
            HitData = e.Reader.ReadSerializables<BulletHitData>();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Frame);
            e.Writer.Write(SpawnData);
            e.Writer.Write(DespawnData);
            e.Writer.Write(UpdateData);
            e.Writer.Write(FireWeaponData);
            e.Writer.Write(HitData);
        }
    }

}
