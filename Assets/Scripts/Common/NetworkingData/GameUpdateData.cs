using DarkRift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.NetworkingData
{

    public struct GameUpdateData : IDarkRiftSerializable
    {
        public uint Frame;
        public PlayerSpawnData[] SpawnData;
        public PlayerDespawnData[] DespawnData;
        public PlayerStateData[] UpdateData;

        public GameUpdateData(uint frame, PlayerStateData[] updateData, PlayerSpawnData[] spawnData, PlayerDespawnData[] despawnData)
        {
            Frame = frame;
            UpdateData = updateData;
            DespawnData = despawnData;
            SpawnData = spawnData;
        }

        public void Deserialize(DeserializeEvent e)
        {
            Frame = e.Reader.ReadUInt32();
            SpawnData = e.Reader.ReadSerializables<PlayerSpawnData>();
            DespawnData = e.Reader.ReadSerializables<PlayerDespawnData>();
            UpdateData = e.Reader.ReadSerializables<PlayerStateData>();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Frame);
            e.Writer.Write(SpawnData);
            e.Writer.Write(DespawnData);
            e.Writer.Write(UpdateData);
        }
    }

}
