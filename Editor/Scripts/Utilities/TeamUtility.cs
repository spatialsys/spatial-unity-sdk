using UnityEngine;
using UnityEditor;
using RSG;
using System.Linq;

namespace SpatialSys.UnitySDK.Editor
{
    public static class TeamUtility
    {
        public static SpatialAPI.Team[] teams { get; private set; } = new SpatialAPI.Team[0];
        public static bool isFetchingTeams { get; private set; }
        public static bool initialFetchComplete { get; private set; }

        private static bool _clearedWhileFetching = false;

        /// <summary>
        /// Returns whether the default team for this project is a private team (not an enterprise team). If there is no default team, returns true.
        /// </summary>
        public static bool isDefaultTeamPrivate
        {
            get
            {
                if (string.IsNullOrEmpty(ProjectConfig.defaultTeamID))
                    return true;

                return teams.FirstOrDefault(t => t.id == ProjectConfig.defaultTeamID).isPrivateTeam;
            }
        }

        [InitializeOnLoadMethod]
        private static void OnScriptsReloaded()
        {
            // Only fetch teams if a team id exists on this project
            if (AuthUtility.isAuthenticated && string.IsNullOrEmpty(ProjectConfig.defaultTeamID))
            {
                Debug.Log("Fetching teams");
                FetchTeams();
            }
        }

        public static IPromise FetchTeams()
        {
            Debug.Log("Fetching teams1");
            if (!AuthUtility.isAuthenticated)
            {
                ClearTeams();
                return Promise.Resolved();
            }

            Debug.Log("Fetching teams2");
            initialFetchComplete = true;
            isFetchingTeams = true;
            _clearedWhileFetching = false;
            return SpatialAPI.GetTeams()
                .Then(resp => {
                    Debug.Log("Fetched teams3");
                    if (!_clearedWhileFetching)
                        Debug.Log("Fetched teams: " + resp.Length);
                    teams = resp;
                })
                .Catch(err => {
                    Debug.Log("Error fetching teams: " + err.Message);
                })
                .Finally(() => {
                    Debug.Log("Fetched teams4");
                    isFetchingTeams = false;
                    _clearedWhileFetching = false;
                });
        }

        public static void ClearTeams()
        {
            teams = new SpatialAPI.Team[0];
            _clearedWhileFetching = true;
        }
    }
}