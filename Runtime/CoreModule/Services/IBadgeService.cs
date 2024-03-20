namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Service to handle badges
    /// </summary>
    /// <example><code source="Services/BadgeServiceExamples.cs" region="RewardBadge" lang="csharp"/></example>
    [DocumentationCategory("Services/Badge Service")]
    public interface IBadgeService
    {
        /// <summary>
        /// Rewards a badge to the user. This will trigger a badge notification to appear.
        /// </summary>
        /// <param name="badgeID">Badge ID</param>
        /// <example><code source="Services/BadgeServiceExamples.cs" region="RewardBadge" lang="csharp"/></example>
        void RewardBadge(string badgeID);
    }
}