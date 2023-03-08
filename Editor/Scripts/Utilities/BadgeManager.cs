using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RSG;
using Proyecto26;

namespace SpatialSys.UnitySDK.Editor
{
    public static class BadgeManager
    {
        // Maximum size is 1024 x 1024
        private const int BADGE_SIZE = 1024;
        // Maximum file size is 1024 kb
        private const int BADGE_FILE_SIZE = 1024 * 1024;

        private static Dictionary<string, Texture2D> _badgeIconCache = new Dictionary<string, Texture2D>();
        private static List<SpatialAPI.Badge> _badges;
        private static Texture2D _badgeIconToUpload;

        [InitializeOnLoadMethod]
        private static void OnScriptsReloaded()
        {
            // Only fetch badges if a world id exists on this project
            if (!string.IsNullOrEmpty(ProjectConfig.worldID))
            {
                FetchBadges();
            }
        }

        public static List<SpatialAPI.Badge> GetCachedBadges()
        {
            return _badges;
        }

        public static IPromise<List<SpatialAPI.Badge>> FetchBadges()
        {
            return WorldUtility.ValidateWorldExists().Then(() => {
                return SpatialAPI.GetBadges(ProjectConfig.worldID)
                    .Then(response => {
                        _badges = response.badges.ToList();
                        return _badges;
                    });
            });
        }

        public static IPromise<Texture2D> GetBadgeIcon(string badgeID)
        {
            if (_badgeIconCache.ContainsKey(badgeID))
            {
                return Promise<Texture2D>.Resolved(_badgeIconCache[badgeID]);
            }
            else
            {
                int index = _badges.FindIndex(b => b.id == badgeID);

                if (index == -1)
                {
                    return Promise<Texture2D>.Rejected(new ArgumentException("Invalid badge id"));
                }
                else if (_badges[index].badgeIconURL == null)
                {
                    _badgeIconCache[badgeID] = null;
                    return Promise<Texture2D>.Resolved(null);
                }
                else
                {
                    return DownloadBadgeIcon(_badges[index].badgeIconURL)
                        .Then(texture => {
                            _badgeIconCache[badgeID] = texture;
                            return texture;
                        });
                }
            }
        }

        public static IPromise<SpatialAPI.Badge> CreateBadge(string name, string description, byte[] icon)
        {
            string badgeID = null;
            int index = -1;
            return SpatialAPI.CreateBadge(ProjectConfig.worldID, name, description)
                .Then(response => {
                    badgeID = response.id;
                    index = _badges.Count;
                    _badges.Add(new SpatialAPI.Badge() {
                        id = response.id,
                        name = response.name,
                        description = response.description,
                        badgeIconURL = response.badgeIconURL,
                        worldID = response.worldID,
                        worldName = response.worldName,
                        updatedAt = response.updatedAt,
                        createdAt = response.createdAt,
                    });
                    return UpdateBadgeIcon(response.id, icon);
                })
                .Then(response => {
                    return _badges[index];
                });
        }

        public static IPromise<Texture2D> UpdateBadgeIcon(string badgeID, byte[] fileContent)
        {
            // Load content into texture
            if (fileContent.Length > BADGE_FILE_SIZE)
            {
                return Promise<Texture2D>.Rejected(new ArgumentException("Image size must be less than 1024 kb"));
            }
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(fileContent);

            if (texture.width != BADGE_SIZE && texture.height != BADGE_SIZE)
                return Promise<Texture2D>.Rejected(new ArgumentException($"Image dimensions are not {BADGE_SIZE}x{BADGE_SIZE}."));

            // Upload if badge exists, otherwise just return texture
            if (badgeID != null)
            {
                return SpatialAPI.UploadBadgeIcon(badgeID, fileContent)
                    .Then(response => {
                        int index = _badges.FindIndex(b => b.id == badgeID);
                        // Update cached badge
                        SpatialAPI.Badge badge = _badges[index];
                        badge.badgeIconURL = response.badgeIconURL;
                        _badges[index] = badge;

                        // Cache icon
                        _badgeIconCache[badgeID] = texture;
                        return texture;
                    });
            }
            else
            {
                return Promise<Texture2D>.Resolved(texture);
            }
        }

        public static IPromise DeleteBadge(string badgeID)
        {
            return SpatialAPI.DeleteBadge(badgeID)
                .Then(() => {
                    int index = _badges.FindIndex(b => b.id == badgeID);
                    _badges.RemoveAt(index);
                });
        }

        public static IPromise UpdateBadge(string badgeID, string name, string description)
        {
            return SpatialAPI.UpdateBadge(badgeID, name, description)
                .Then(response => {
                    int index = _badges.FindIndex(b => b.id == badgeID);

                    // Update cached badge
                    SpatialAPI.Badge badge = _badges[index];
                    badge.name = name;
                    badge.description = description;
                    _badges[index] = badge;
                });
        }

        private static IPromise<byte[]> Download(string url)
        {
            RequestHelper request = new RequestHelper();
            request.Uri = url;
            return RestClient.Get(request).Then(response => {
                return response.Data;
            });
        }

        private static IPromise<Texture2D> DownloadBadgeIcon(string url)
        {
            return Download(url).Then(fileContent => {
                // Load into texture
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(fileContent);
                texture.Apply();
                return texture;
            });
        }

    }
}