using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    public class SpatialEntrancePoint : SpatialComponentBase
    {
        public override string prettyName => "Entrance Point";
        public override string tooltip => "Specify the area in which users will be placed when entering this space. Multiple entrance points can be used in a single scene";
        public override string documentationURL => "https://www.notion.so/spatialxr/Entrance-Point-fa1f8c4776214b6c872b043e14baf0f1";

        public float radius = 1f;
    }
}
