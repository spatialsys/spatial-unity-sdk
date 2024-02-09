using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorReward : IReward
    {
        public EditorReward(RewardType type, string id, int amount)
        {
            this.type = type;
            this.id = id;
            this.amount = amount;
        }

        //--------------------------------------------------------------------------------------------------------------
        // IReward
        //--------------------------------------------------------------------------------------------------------------
        public RewardType type { get; }

        public string id { get; }

        public int amount { get; }
    }
}