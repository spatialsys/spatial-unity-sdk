using System;
using UnityEngine;
using TMPro;

namespace SpatialSys.UnitySDK.Internal
{
    [Serializable]
    public class FontOverride
    {
        public bool overrideFont;
        public TMP_FontAsset font;
        public Material material;
    }
}