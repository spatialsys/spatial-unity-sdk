---
title: Home
---

# Spatial C# API Reference

> [!NOTE]
> This documentation is still under development.

The Spatial C# API contains all runtime types that are allowed to be referenced by external developers through C#.

The [Spatial Bridge](/api/SpatialSys.UnitySDK.SpatialBridge.html) is the main class for using the Spatial services.

It is not necessary to provide your own implementations of the Spatial interfaces, such as [IActorService](/api/SpatialSys.UnitySDK.IActorService), [ICameraService](/api/SpatialSys.UnitySDK.ICameraService), and others; the implementations are injected at runtime and accessed using the respective [Spatial Bridge](/api/SpatialSys.UnitySDK.SpatialBridge.html) properties.
