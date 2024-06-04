using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

namespace SpatialSys.UnitySDK.VisualScripting
{
    [UnitTitle("Spatial Network Object: Find Object")]
    [UnitSurtitle("Spatial Network Object")]
    [UnitShortTitle("Find")]
    [UnitCategory("Spatial\\Network Object")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class NetworkObject_FindObjectNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [DoNotSerialize]
        public ValueInput objectID { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput networkObject { get; private set; }

        protected override void Definition()
        {
            objectID = ValueInput<int>(nameof(objectID), 0);
            networkObject = ValueOutput<SpatialNetworkObject>(nameof(networkObject));

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                int objectIDValue = f.GetValue<int>(objectID);
                if (SpatialBridge.spaceContentService.TryFindNetworkObject(objectIDValue, out SpatialNetworkObject nwo))
                    f.SetValue(networkObject, nwo);

                return outputTrigger;
            });
            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial Network Object: Get Is Spawned")]
    [UnitSurtitle("Spatial Network Object")]
    [UnitShortTitle("Is Spawned")]
    [UnitCategory("Spatial\\Network Object")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class NetworkObject_GetIsSpawnedNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput networkObject { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput isSpawned { get; private set; }

        protected override void Definition()
        {
            networkObject = ValueInput<SpatialNetworkObject>(nameof(networkObject), null).NullMeansSelf();
            isSpawned = ValueOutput<bool>(nameof(isSpawned), (f) => {
                SpatialNetworkObject nwo = f.GetValue<SpatialNetworkObject>(networkObject);
                return !nwo?.spaceObject?.isDisposed ?? false;
            });
        }
    }


    [UnitTitle("Spatial Network Object: Get Object ID")]
    [UnitSurtitle("Spatial Network Object")]
    [UnitShortTitle("Get Object ID")]
    [UnitCategory("Spatial\\Network Object")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class NetworkObject_GetObjectIDNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput networkObject { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput objectID { get; private set; }

        protected override void Definition()
        {
            networkObject = ValueInput<SpatialNetworkObject>(nameof(networkObject), null).NullMeansSelf();
            objectID = ValueOutput<int>(nameof(objectID), (f) => f.GetValue<SpatialNetworkObject>(networkObject)?.objectID ?? 0);
        }
    }

    [UnitTitle("Spatial Network Object: Get Is Mine")]
    [UnitSurtitle("Spatial Network Object")]
    [UnitShortTitle("Is Mine")]
    [UnitCategory("Spatial\\Network Object")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class NetworkObject_GetIsMineNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput networkObject { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput isMine { get; private set; }

        protected override void Definition()
        {
            networkObject = ValueInput<SpatialNetworkObject>(nameof(networkObject), null).NullMeansSelf();
            isMine = ValueOutput<bool>(nameof(isMine), (f) => f.GetValue<SpatialNetworkObject>(networkObject)?.isMine ?? false);
        }
    }

    [UnitTitle("Spatial Network Object: Get Has Control")]
    [UnitSurtitle("Spatial Network Object")]
    [UnitShortTitle("Has Control")]
    [UnitCategory("Spatial\\Network Object")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class NetworkObject_GetHasControlNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput networkObject { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput hasControl { get; private set; }

        protected override void Definition()
        {
            networkObject = ValueInput<SpatialNetworkObject>(nameof(networkObject), null).NullMeansSelf();
            hasControl = ValueOutput<bool>(nameof(hasControl), (f) => f.GetValue<SpatialNetworkObject>(networkObject)?.hasControl ?? false);
        }
    }

    //if has control
    [UnitTitle("Spatial Network Object: If Has Control")]
    [UnitSurtitle("Spatial Network Object")]
    [UnitShortTitle("If Has Control")]
    [UnitCategory("Spatial\\Network Object")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class NetworkObject_IfHasControlNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabel("True")]
        public ControlOutput outputTriggerTrue { get; private set; }
        [DoNotSerialize]
        [PortLabel("False")]
        public ControlOutput outputTriggerFalse { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput networkObject { get; private set; }

        protected override void Definition()
        {
            networkObject = ValueInput<SpatialNetworkObject>(nameof(networkObject), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                if (f.GetValue<SpatialNetworkObject>(networkObject)?.hasControl ?? false)
                    return outputTriggerTrue;
                return outputTriggerFalse;
            });

            outputTriggerTrue = ControlOutput(nameof(outputTriggerTrue));
            outputTriggerFalse = ControlOutput(nameof(outputTriggerFalse));

            Succession(inputTrigger, outputTriggerTrue);
            Succession(inputTrigger, outputTriggerFalse);
        }
    }


    [UnitTitle("Spatial Network Object: Get Owner Actor Number")]
    [UnitSurtitle("Spatial Network Object")]
    [UnitShortTitle("Get Owner Actor Number")]
    [UnitCategory("Spatial\\Network Object")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class NetworkObject_GetOwnerActorNumberNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput networkObject { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput ownerActorNumber { get; private set; }

        protected override void Definition()
        {
            networkObject = ValueInput<SpatialNetworkObject>(nameof(networkObject), null).NullMeansSelf();
            ownerActorNumber = ValueOutput<int>(nameof(ownerActorNumber), (f) => f.GetValue<SpatialNetworkObject>(networkObject)?.ownerActorNumber ?? 0);
        }
    }

    [UnitTitle("Spatial Network Object: Request Ownership")]
    [UnitSurtitle("Spatial Network Object")]
    [UnitShortTitle("Request Ownership")]
    [UnitCategory("Spatial\\Network Object")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class NetworkObject_RequestOwnershipNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput networkObject { get; private set; }

        protected override void Definition()
        {
            networkObject = ValueInput<SpatialNetworkObject>(nameof(networkObject), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                f.GetValue<SpatialNetworkObject>(networkObject)?.RequestOwnership();
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }

    [UnitTitle("Spatial Network Object: Release Ownership")]
    [UnitSurtitle("Spatial Network Object")]
    [UnitShortTitle("Release Ownership")]
    [UnitCategory("Spatial\\Network Object")]
    [TypeIcon(typeof(SpatialComponentBase))]
    public class NetworkObject_ReleaseOwnershipNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput networkObject { get; private set; }

        protected override void Definition()
        {
            networkObject = ValueInput<SpatialNetworkObject>(nameof(networkObject), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) => {
                f.GetValue<SpatialNetworkObject>(networkObject)?.ReleaseOwnership();
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
