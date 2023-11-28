using SpatialSys.UnitySDK.Internal;
using UnityEngine;

namespace SpatialSys.UnitySDK
{
    // Multiple components can cause the asset to have more dependencies than it needs.
    [DisallowMultipleComponent]
    [InternalType]
    public abstract class SpatialPackageAsset : SpatialComponentBase
    {
        [HideInInspector] public SavedProjectSettings savedProjectSettings;
    }
}
