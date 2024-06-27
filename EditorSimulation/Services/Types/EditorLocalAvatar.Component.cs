using System;
using UnityEngine;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    // Class name is changed from file name so it can be added as a component to a game object even if
    // its technically an editor script.
    public class EditorLocalAvatar : MonoBehaviour, IAvatar, IReadOnlyAvatar
    {
        private GameObject _body;
        private CharacterController _controller;
        private bool _wasGroundedLastFrame;
        private bool _sprint = false;
        private Vector2 _moveInput = Vector2.zero;
        private Vector3 _externalVelocity = Vector3.zero;
        private Vector3 _directMovementVelocity = Vector3.zero;
        private float _gravitationalVelocity = 0f;
        private float _lastGroundedTime;

        private float fallTimeout = 0.15f;

        private Collider _groundedCollider;
        private SpatialMovementMaterial _movementMaterial;

        // Jump
        private float jumpTimeout = 0.50f;
        private float jumpBufferTime = 0.1f;
        private float coyoteTime = 0.1f;

        private bool _forceJump;
        private int _jumpCount = 0;

#pragma warning disable 0414 // Disable "is assigned but its value is never used" warning
        private bool _isJumping;
#pragma warning restore 0414

        private bool _triggerJump; // jump event triggered externally
        private bool _lastJumpWasTriggered;
        private bool _jumpButtonPressed; // jump button is pressed
        private float _jumpButtonPressedTime; // time jump button was pressed
        private bool _jumpButtonReleasedWhileJumping = false; // jump button is released while jumping

        // Player
        private float moveSpeed = 3.0f;
        private float sprintSpeed = 6.875f;
        private float speedChangeRate = 20.0f;
        private float rotationSmoothTime = 0.12f;
        private bool defaultFrictionIsInfinite = true;

        private float _speed;
        private float _targetRotation;
        private float _rotationVelocity;


        // Timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _lastJumpTime;

        private bool _canJump => _jumpCount < maxJumpCount && jumpHeight > Mathf.Epsilon;

        /// <summary>
        /// The sum of all velocity vectors that contribute to movement, EXCEPT for gravity/jump. This is used for syncing velocity to remote users.
        /// </summary>
        private Vector3 _movementVelocity => _directMovementVelocity + _externalVelocity;
        /// <summary>
        /// The sum of all velocity vectors that were applied to the character controller during this frame.
        /// </summary>
        private Vector3 _appliedVelocity => _movementVelocity + new Vector3(0f, _gravitationalVelocity, 0f);

        private const float TERMINAL_VELOCITY = 53f;
        private const float SHORT_JUMP_EXTRA_GRAVITY_DELAY = 0.15f;
        private const float GROUNDED_VERTICAL_VELOCITY = -2f; // constant force to keep avatar grounded while moving across slopes.

        private void Start()
        {
            gameObject.name = "[Spatial SDK] Editor Local Avatar";
            gameObject.layer = LayerMask.NameToLayer("AvatarLocal");

            _controller = gameObject.AddComponent<CharacterController>();
            _controller.height = 1.8f;
            _controller.center = new Vector3(0, 0.92f, 0);
            _controller.radius = 0.28f;
            _controller.slopeLimit = 45;
            _controller.stepOffset = 0.4f;
            _controller.skinWidth = 0.02f;

            LoadBody();
        }

        private void LoadBody()
        {
            if (isBodyLoaded)
            {
                onAvatarBeforeUnload?.Invoke();
                UnloadBody();
            }

            // TODO: Load humanoid avatar
            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.transform.SetParent(transform, false);
            capsule.GetComponent<Collider>().enabled = false;
            capsule.transform.localPosition = new Vector3(0, 0.92f, 0);
            _body = capsule;

            isBodyLoaded = true;
            onAvatarLoadComplete?.Invoke();
        }

        private void UnloadBody()
        {
            isBodyLoaded = false;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.normal.y > 0.7f)
            {
                _groundedCollider = hit.collider;
            }

            onColliderHit?.Invoke(hit, _controller.velocity);
        }

        private void LateUpdate()
        {
            UpdateGroundedCheck();
            UpdateJumpAndGravity();
            UpdateMovementFromInput();
        }

        private void UpdateGroundedCheck()
        {
            if (_wasGroundedLastFrame != _controller.isGrounded)
            {
                onIsGroundedChanged?.Invoke(_controller.isGrounded);

                if (_controller.isGrounded)
                {
                    onLanded?.Invoke();
                }
            }
            _wasGroundedLastFrame = _controller.isGrounded;
        }

        private void UpdateJumpAndGravity()
        {
            if (isGrounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = fallTimeout;

                _isJumping = false;
                _jumpCount = 0;
                _lastGroundedTime = 0f;

                // stop our velocity dropping infinitely when grounded
                if (_gravitationalVelocity < 0.0f)
                {
                    _gravitationalVelocity = GROUNDED_VERTICAL_VELOCITY;
                }

                // Jump
                bool jumpPressedRecently = Time.time - _jumpButtonPressedTime < jumpBufferTime;
                if (_canJump && (_triggerJump || jumpPressedRecently) && (_jumpTimeoutDelta <= 0.0f || _forceJump))
                {
                    Jump(false);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }

                _forceJump = false;
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = jumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                if (_lastGroundedTime < coyoteTime)
                {
                    _lastGroundedTime += Time.deltaTime;
                }

                // Allow jump if we are in the air for less than coyote time
                if (_lastGroundedTime > coyoteTime)
                {
                    // But if you're in the air for more than coyote time, set jump count 1.
                    _jumpCount = Mathf.Max(_jumpCount, 1); // jump count can be 0 in air because of external velocity
                }
                // inAir jump
                if (_canJump && (_triggerJump || (_jumpButtonPressed && (_jumpButtonReleasedWhileJumping || _lastJumpWasTriggered))))
                {
                    if (_gravitationalVelocity + _externalVelocity.y < 0)
                    {
                        _externalVelocity.y = 0;
                    }
                    else if (_gravitationalVelocity < 0)
                    {
                        _externalVelocity.y = _externalVelocity.y + _gravitationalVelocity;
                    }

                    Jump(true);
                }
            }
        }

        private void Jump(bool isInAirJump)
        {
            if (!_canJump)
                return;

            // set velocity needed to reach the desired jump height
            _gravitationalVelocity = Mathf.Sqrt(2f * Mathf.Abs(jumpHeight * Physics.gravity.y * gravityMultiplier));

            _jumpCount++;
            _lastJumpWasTriggered = _triggerJump;
            _triggerJump = false;
            _jumpButtonReleasedWhileJumping = false;
            _jumpButtonPressedTime = 0f; // avoid jump again without pressing jump button again
            _isJumping = true;
            onJump?.Invoke(_controller.isGrounded);
            _lastJumpTime = Time.time;
        }

        private void SetVelocity(Vector3 veclocity)
        {
            _directMovementVelocity = Vector3.zero;
            _externalVelocity = velocity;
            _gravitationalVelocity = 0f;
        }

        public Vector3 Flatten(Vector3 source, float y = 0f, bool normalized = false)
        {
            return normalized ? new Vector3(source.x, y, source.z).normalized : new Vector3(source.x, y, source.z);
        }

        private void UpdateMovementFromInput()
        {
            float previousSpeed = _speed;

            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _sprint ? sprintSpeed : moveSpeed;

            float inputMagnitude = _moveInput.magnitude;

            // if there is no input, set the target speed to 0
            if (inputMagnitude < 0.001f)
            {
                targetSpeed = 0.0f;
            }

            targetSpeed *= inputMagnitude;

            // a reference to the players current movement velocity
            float currentSpeed = Flatten(_directMovementVelocity).magnitude;

            // speed does not need to be lerped because we lerp _directMovementVelocity below
            _speed = targetSpeed;

            if (_moveInput != Vector2.zero)
            {
                // normalise input direction
                Vector3 inputDirection = new Vector3(_moveInput.x, 0.0f, _moveInput.y).normalized;
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + SpatialBridge.cameraService.rotation.eulerAngles.y;
            }

            // smoothly update avatar rotation to target, if not using hmd
            float rotation = Mathf.SmoothDampAngle(_controller.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);

            // rotate to face input direction relative to camera position
            _controller.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            if (isGrounded)
            {
                float movementFriction = GetMovementFriction();

                if (defaultFrictionIsInfinite && movementFriction >= 1f)
                {
                    // no acceleration time when friction is high and grounded 
                    _directMovementVelocity = targetDirection.normalized * _speed;
                }
                else
                {
                    // smoothly update velocity to target velocity using friction
                    _directMovementVelocity = Vector3.Lerp(_directMovementVelocity, targetDirection.normalized * _speed, Time.deltaTime * speedChangeRate * movementFriction);
                }
            }
            else
            {
                // always have some acceleration while in air. Also multiply by air control.
                _directMovementVelocity = Vector3.Lerp(_directMovementVelocity, targetDirection.normalized * _speed, Time.deltaTime * speedChangeRate * airControl);
            }

            ApplyMoveVelocity(_directMovementVelocity);
        }

        private void ApplyMoveVelocity(Vector3 velocity)
        {
            // Simplified Velocity Verlet integration instead of Euler (https://youtu.be/hG9SzQxaCm8?t=1312)
            // To keep the same trajectory between different framerates.
            // p += v * dt + 0.5 * a * dt * dt
            // v += a + dt
            float globalGravity = Mathf.Abs(Physics.gravity.y * gravityMultiplier);

            if (_controller.enabled)
            {
                var prevCollider = _groundedCollider;
                _groundedCollider = null;
                _controller.Move((velocity + new Vector3(0.0f, _gravitationalVelocity + 0.5f * -globalGravity * Time.deltaTime, 0.0f)) * Time.deltaTime);

                if (prevCollider != _groundedCollider)
                {
                    _movementMaterial = _groundedCollider?.GetComponent<SpatialMovementMaterialSurface>()?.movementMaterial;
                }
            }

            // apply gravity acceleration if below terminal velocity.
            if (_appliedVelocity.y > -TERMINAL_VELOCITY)
            {
                // If jump is released early, increase gravity to make jump shorter.
                if (useVariableHeightJump && _jumpButtonReleasedWhileJumping && _gravitationalVelocity > 0f && Time.time - _lastJumpTime > SHORT_JUMP_EXTRA_GRAVITY_DELAY)
                {
                    const float SHORT_JUMP_EXTRA_GRAVITY_MULTIPLIER = 1.0f;
                    _gravitationalVelocity += -globalGravity * SHORT_JUMP_EXTRA_GRAVITY_MULTIPLIER * Time.deltaTime;
                }

                bool isFalling = _appliedVelocity.y < 0.0f;
                _gravitationalVelocity += -globalGravity * Time.deltaTime * (isFalling ? fallingGravityMultiplier : 1f);
                _gravitationalVelocity = Mathf.Max(-TERMINAL_VELOCITY - _movementVelocity.y, _gravitationalVelocity);
            }
        }

        /// <summary>
        /// Get the friction taking into consideration avatar friction and MovementMaterials / PhysicsMaterials
        /// </summary>
        private float GetMovementFriction()
        {
            float movementFriction = groundFriction;
            float materialFriction = 1f;
            bool lookForPhysicsMaterial = lookForPhysicsMaterial = _movementMaterial?.usePhysicsMaterial ?? true;
            PhysicMaterialCombine combineType = PhysicMaterialCombine.Minimum;//default minimum so that avatar friction is used entirely if no material is found

            if (lookForPhysicsMaterial && _groundedCollider != null && _groundedCollider.material != null && !string.IsNullOrEmpty(_groundedCollider.material.name))
            {
                // Select the friction value to use based on the movement speed
                materialFriction = (_speed > _directMovementVelocity.magnitude) ?
                    _groundedCollider.material.staticFriction :
                    _groundedCollider.material.dynamicFriction;
                combineType = _groundedCollider.material.frictionCombine;
            }
            else if (_movementMaterial != null)
            {
                // Use the friction value from the Spatial Movement Material
                materialFriction = (_speed > _directMovementVelocity.magnitude) ?
                    _movementMaterial.staticFriction :
                    _movementMaterial.dynamicFriction;
                combineType = _movementMaterial.frictionCombine;
            }

            // Combine the friction values based on the material's friction combine mode
            switch (combineType)
            {
                default:
                case PhysicMaterialCombine.Average:
                    return movementFriction = (groundFriction + materialFriction) * 0.5f;
                case PhysicMaterialCombine.Multiply:
                    return movementFriction = groundFriction * materialFriction;
                case PhysicMaterialCombine.Minimum:
                    return movementFriction = Mathf.Min(groundFriction, materialFriction);
                case PhysicMaterialCombine.Maximum:
                    return movementFriction = Mathf.Max(groundFriction, materialFriction);
            }
        }

        //-------------------------------------------------------------------------
        // IAvatar implementation
        //-------------------------------------------------------------------------
        public bool isDisposed => false;
        public int spaceObjectID => 0;
        IReadOnlySpaceObject IReadOnlySpaceObjectComponent.spaceObject => null;
        public ISpaceObject spaceObject => null;

        public bool isNPC => false;
        public bool isBodyLoaded { get; private set; }
        public bool visibleLocally { get => _body?.activeSelf ?? false; set => _body?.SetActive(value); }
        public bool visibleRemotely { get => _body?.activeSelf ?? false; set => _body?.SetActive(value); }

        public string displayName { get; } = "Editor Local Avatar";
        public bool nametagVisible { get; set; }
        public string nametagSubtext { get; set; }
        public float nametagBarValue { get; set; }
        public bool nametagBarVisible { get; set; }

        public Vector3 position { get => transform.position; set => transform.position = value; }
        public Quaternion rotation { get => transform.rotation; set => transform.rotation = value; }
        public Vector3 velocity { get => _controller.velocity; set => SetVelocity(value); }

        public bool isGrounded { get => _controller.isGrounded; }
        public float groundFriction { get; set; } = 1f;
        public float walkSpeed { get; set; } = 3f;
        public float runSpeed { get; set; } = 6.875f;
        public float jumpHeight { get; set; } = 1.5f;
        public int maxJumpCount { get; set; } = 2;
        public float airControl { get; set; } = 1;
        public bool useVariableHeightJump { get; set; } = true;
        public float gravityMultiplier { get; set; } = 1.5f;
        public float fallingGravityMultiplier { get; set; } = 1f;
        public bool ragdollPhysicsActive => false;
        public Vector3 ragdollVelocity { get; set; } = Vector3.zero;
        public Material[] bodyMaterials => null;

        public event Action onAvatarLoadComplete;
        public event Action onAvatarBeforeUnload;

        public event IAvatar.OnColliderHitDelegate onColliderHit;
        public event IAvatar.OnIsGroundedChangedDelegate onIsGroundedChanged;
        public event Action onLanded;
        public event IAvatar.OnJumpDelegate onJump;
#pragma warning disable 0067 // Disable "event is never used" warning
        public event IAvatar.OnRespawnedDelegate onRespawned;
        public event Action onEmote;
        public event IAvatar.OnAttachmentEquippedChangedDelegate onAttachmentEquippedChanged;
#pragma warning restore 0067

        public void Respawn() { }

        public void SetPositionRotation(Vector3 position, Quaternion rotation)
        {
            transform.position = position;

            _targetRotation = transform.rotation.eulerAngles.y;

            transform.rotation = Quaternion.Euler(0, _targetRotation, 0);
        }

        public void Move(Vector2 moveInput, bool sprint = false)
        {
            _moveInput = moveInput;
            _sprint = sprint;
        }

        public void SetDestination(Vector3 destination, bool sprint = false) { }

        public void AddForce(Vector3 force)
        {
            _externalVelocity += velocity;
        }

        public void Jump()
        {
            _triggerJump = true;
            _forceJump = true;
        }

        public void Sit(Transform target) { }
        public void Stand() { }

        public void SetRagdollPhysicsActive(bool active, Vector3 initialVelocity) { }
        public void AddRagdollForce(Vector3 force, bool ignoreMass = false) { }

        public void PlayEmote(AssetType assetType, string assetID, bool immediately = false, bool loop = false) { }
        public void StopEmote() { }

        public void SetAvatarBody(AssetType assetType, string assetID) { }
        public void ResetAvatarBody() { }
        public Transform GetAvatarBoneTransform(HumanBodyBones humanBone) => null;

        public EquipAttachmentRequest EquipAttachment(AssetType assetType, string assetID, bool equip = true, bool clearOccupiedPrimarySlot = true, string optionalTag = null)
        {
            EquipAttachmentRequest request = new()
            {
                succeeded = false,
            };
            request.InvokeCompletionEvent();
            return request;
        }
        public void ClearAttachments() { }
        public void ClearAttachmentSlot(SpatialAvatarAttachment.Slot slot) { }
        public void ClearAttachmentsByTag(string tag) { }
        public bool IsAttachmentEquipped(string assetID) => false;

        public bool Equals(IAvatar other)
        {
            return other != null && other.spaceObjectID == spaceObjectID;
        }

        public bool Equals(IReadOnlyAvatar other)
        {
            return other != null && other.spaceObjectID == spaceObjectID;
        }
    }
}