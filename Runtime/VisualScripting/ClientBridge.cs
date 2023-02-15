using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.UnitySDK.VisualScripting
{
    public class ClientBridge
    {
        public delegate int GetLocalActorDelegate();
        public static GetLocalActorDelegate GetLocalActor;

        public delegate List<int> GetActorsDelegate();
        public static GetActorsDelegate GetActors;

        public delegate bool GetAvatarExistsDelegate(int actorNumber);
        public static GetAvatarExistsDelegate GetAvatarExists;

        public delegate Vector3 GetLocalAvatarPositionDelegate();
        public static GetLocalAvatarPositionDelegate GetLocalAvatarPosition;

        public delegate void SetLocalAvatarPositionDelegate(Vector3 position);
        public static SetLocalAvatarPositionDelegate SetLocalAvatarPosition;

        public delegate Quaternion GetLocalAvatarRotationDelegate();
        public static GetLocalAvatarRotationDelegate GetLocalAvatarRotation;

        public delegate Vector3 GetAvatarPositionWithActorDelegate(int actorNumber);
        public static GetAvatarPositionWithActorDelegate GetAvatarPositionWithActor;

        public delegate Quaternion GetAvatarRotationWithActorDelegate(int actorNumber);
        public static GetAvatarRotationWithActorDelegate GetAvatarRotationWithActor;

        public delegate int GetLocalActorPlatformDelegate();
        public static GetLocalActorPlatformDelegate GetLocalActorPlatform;

        // bool = actorNumber is valid
        public delegate Tuple<bool, int> GetActorPlatformDelegate(int actorNumber);
        public static GetActorPlatformDelegate GetActorPlatform;

        public delegate int GetRandomActorDelegate();
        public static GetRandomActorDelegate GetRandomActor;

        public delegate void AddForceToLocalAvatarDelegate(Vector3 force);
        public static AddForceToLocalAvatarDelegate AddForceToLocalAvatar;

        public delegate bool IsLocalOwnerDelegate();
        public static IsLocalOwnerDelegate IsLocalOwner;

        public delegate bool IsLocalHostDelegate();
        public static IsLocalHostDelegate IsLocalHost;

        public delegate bool HasLocalLovedSpaceDelegate();
        public static HasLocalLovedSpaceDelegate HasLocalLovedSpace;
    }
}
