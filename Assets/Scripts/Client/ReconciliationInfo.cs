﻿using Common.NetworkingData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{

    public struct ReconciliationInfo
    {
        public ReconciliationInfo(uint frame, PlayerStateData data, PlayerInputData input)
        {
            Frame = frame;
            Data = data;
            Input = input;
        }

        public uint Frame;
        public PlayerStateData Data;
        public PlayerInputData Input;
    }

}
