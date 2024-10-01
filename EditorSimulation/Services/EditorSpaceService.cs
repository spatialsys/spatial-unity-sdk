using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
#pragma warning disable 67 // Disable unused event warning
    public class EditorSpaceService : ISpaceService
    {
        public int spacePackageVersion => 0;
        public bool isSandbox => true;
        public bool hasLikedSpace => false;

        public event ISpaceService.OnSpaceLikedDelegate onSpaceLiked;

        public void OpenURL(string url)
        {
            Application.OpenURL(url);
        }

        public void EnableAvatarToAvatarCollisions(bool enabled)
        {
        }

        public void TeleportToSpace(string spaceID, bool showPopup = true)
        {
        }
    }
}
