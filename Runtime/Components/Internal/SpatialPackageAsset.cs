using UnityEngine;

namespace SpatialSys.UnitySDK
{
    // Multiple components can cause the asset to have more dependencies than it needs.
    [DisallowMultipleComponent]
    public abstract class SpatialPackageAsset : SpatialComponentBase
    {
    }
}
