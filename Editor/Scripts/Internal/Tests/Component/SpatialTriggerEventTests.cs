using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public static class SpatialTriggerEventTests
    {
        [ComponentTest(typeof(SpatialTriggerEvent))]
        public static void CheckForTrigger(Component target)
        {
            SpatialTriggerEvent trigger = target as SpatialTriggerEvent;

            Collider collider;
            SpatialTestResponse resp;

            //todo check for multiple colliders etc.

            if (!trigger.TryGetComponent(out collider))
            {
                //maybe this should be a warning? 
                resp = new SpatialTestResponse(target, TestResponseType.Warning, "SpatialTriggerEvent is missing a collider.");
                resp.SetAutoFix(isSafe: false, "Adds a box collider to the gameobject and sets is trigger to true.",
                    (target) => {
                        Component c = target as Component;
                        c.gameObject.AddComponent<BoxCollider>().isTrigger = true;
                    }
                );
                SpatialValidator.AddResponse(resp);
                return;
            }

            if (collider.isTrigger == false)
            {
                resp = new SpatialTestResponse(target, TestResponseType.Warning, "SpatialTriggerEvent collider has isTrigger set to false.",
                    "The SpatialTriggerEvent requires isTrigger to be set for collision to be detected.");

                resp.SetAutoFix(false, "Enables isTrigger on the first collider found on the offending object",
                    (target) => {
                        Component c = target as Component;
                        if (c.gameObject.TryGetComponent(out Collider collider))
                        {
                            collider.isTrigger = true;
                        }
                    }
                );
                SpatialValidator.AddResponse(resp);
                return;
            }
        }
    }
}
