namespace SpatialSys.UnitySDK
{
    /// <summary>
    /// Service to handle badges
    /// </summary>
    public interface IBadgeService
    {
        /// <summary>
        /// Rewards a badge to the user. This will trigger a badge notification to appear.
        /// </summary>
        /// <param name="badgeID">Badge ID</param>
        void RewardBadge(string badgeID);
    }
}