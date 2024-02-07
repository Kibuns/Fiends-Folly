
using System.Collections;
using UnityEngine;

using TMPro;

namespace Code.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Ground Movement")]
        [SerializeField] float moveSpeed;
        [SerializeField] AnimationCurve groundSpeedCurve; //used to multiply the force added to player the slower they are.
        [SerializeField] float maxSpeedForMultiplier; //the speedcurve will be used to multiply the speed of the player (first few steps) until this speed is reached
        [SerializeField] float baseGroundDrag;
        [SerializeField] float dynamicGroundFriction; //friction applied to the physics material when player is grounded

        [Header("Air Movement")]
        [SerializeField] AnimationCurve airSpeedCurve; //used to multiply the force added to player the slower they are.
        [SerializeField] float airSpeedLimit; //not actual max speed, just the speed where you cant add more speed by holding forward in the air.
        [Range(0f, 1f)]
        [SerializeField] float airPivotInterpolationFactor;
        [SerializeField] float defaultAirMultiplier;
        [SerializeField] float airDrag;

        [Header("Jump")]
        [SerializeField] float jumpForce;
        [SerializeField] float jumpCooldown;
        [SerializeField] float lateJumpGraceTime;
        [SerializeField] float coyoteJumpGraceTime;
        [SerializeField] float perfectJumpGraceTime;

        [Header("Wall Jump")]
        [SerializeField] float wallJumpForce;
        [SerializeField] bool isNearWall;
        [SerializeField] AnimationCurve airControlCurve;
        [SerializeField] int maxWallJumpAmount;

        [Header("Dash")]
        [SerializeField] float dashVelocityMagnitude;
        [SerializeField] float canceledDashVelocityMagnitude; //when the dash get's canceled by a jump, what should the player velocity be in the dashDirection?
        [SerializeField] float dashDuration;
        [SerializeField] float startInvulnerabilityFramesTime;
        [SerializeField] float InvulnerabilityFramesDuration;
        [SerializeField] public int maxDashAmount;
        [SerializeField] float dashCooldown;
        [SerializeField] bool retainHeightWhileDashing;
        [SerializeField] float dashFOVDiff;

        [Header("Pound / Slide")]
        [SerializeField] float poundVelocityMagnitude;
        [SerializeField] float slideSpeed;
        [SerializeField] float slideFOVDiff;
        [SerializeField] float slideCamHeight;
        [SerializeField] float slideMaxTiltAmount;
        [Range(0f, 1f)]
        [SerializeField] float slidePivotFactor;
        [SerializeField] float slideVelChangeSpeed;
        [SerializeField] GameObject playerBody;
        [SerializeField] float slideBodyYpos;
        [SerializeField] float slideBodyYscale;
        [SerializeField] float groundSlamRange;
        [SerializeField] float groundSlamDamage;
        [SerializeField] float slideRaycastCheckDistance;

        [Header("Ground Check")]
        [SerializeField] public bool grounded;
        [SerializeField] public Transform orientation;
        [SerializeField] PhysicMaterial playerPhysicsMat;
        [SerializeField] float maxWalkableSlopeAngle;
        [SerializeField] Transform slopeCheckPoint;
        [SerializeField] GameObject movementDirectionRotater;

        [Header("Camera Tilt")]
        [SerializeField] GameObject playerCamera;
        [SerializeField] bool cameraTilt;
        [SerializeField] float maxTiltAmount;
        [SerializeField] float tiltSpeed;


        [Header("Behavioural Options")]
        [SerializeField] bool easyBunnyHop;

        [Header("Other")]
        [SerializeField] public float defaultFOV;
        [SerializeField] float FOVLerpSpeed;
        [SerializeField] Transform cameraPos;
        [SerializeField] private TMP_Text horizontalSpeedText;
        [SerializeField] private TMP_Text verticalSpeedText;
        public float gravityMultiplier;

        [Header("Displacement Settings")]
        [SerializeField] private float maxForceMultiplier; //maxForceMultiplier is the jumpForce when the groundpillar is horizontal
        private const float ForceAppliedTimer = 1f;
        private bool forceApplied;




        PlayerInputActions input; // Reference to your PlayerInputActions asset


        private string currentGroundType;
        private string previousGroundType;

        public int currentDashAmount;
        public int currentWallJumpAmount;

        public bool readyToJump;
        private bool punishDragActivated;
        public bool isDashing;
        private bool isGroundPounding;
        public bool isSliding;
        private bool coyoteJumpUsed;
        private bool slideIsBuffered;

        private float defaultCamHeight;
        private float horizontalInput;
        private float verticalInput;
        private float jumpTimer;
        private float dynamicFriction;
        private float drag;
        private float groundDrag;
        private float dashTimer;
        private float coyoteTimer;
        private float targetFOV;
        private float airMultiplier;
        private float currentSlideSpeed;
        private float currentMaxTiltAmount;
        private float startingPlayerBodyYpos;
        private float startingPlayerBodyYscale;

        private Vector3 moveDirection;
        private Vector3 dashDirection;
        private Vector3 slideDirection;

        private Coroutine dashCoroutine;
        private Coroutine airControlCoroutine;

        private Rigidbody rb;

        private CameraController camController;

        private LayerMask whatIsGround;


        private void OnValidate()
        {
            startInvulnerabilityFramesTime = Mathf.Clamp(startInvulnerabilityFramesTime, 0f, dashDuration);
            InvulnerabilityFramesDuration = Mathf.Clamp(InvulnerabilityFramesDuration, 0f, dashDuration - startInvulnerabilityFramesTime);
        }
        void Awake()
        {
            
            //defaultCamHeight = cameraPos.localPosition.y;
            startingPlayerBodyYpos = playerBody.transform.localPosition.y;
            startingPlayerBodyYscale = playerBody.transform.localScale.y;
            currentMaxTiltAmount = maxTiltAmount;
            currentSlideSpeed = slideSpeed;
            currentWallJumpAmount = maxWallJumpAmount;
            playerPhysicsMat = GetComponentInChildren<Collider>().material;
            whatIsGround = 0;
            input = new PlayerInputActions();
            rb = GetComponent<Rigidbody>();
            dynamicFriction = dynamicGroundFriction;
            playerPhysicsMat.dynamicFriction = dynamicFriction;
            groundDrag = baseGroundDrag;
            jumpTimer = 100f;
            airMultiplier = defaultAirMultiplier;
            rb.freezeRotation = true;
            readyToJump = true;
            currentDashAmount = maxDashAmount;
            maxForceMultiplier = 1.45f;
            camController = playerCamera.GetComponent<CameraController>();
        }

        private void RotateTowardsMoveDirection()
        {
            if (moveDirection != Vector3.zero)
            {
                movementDirectionRotater.transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            }
            else if (slideDirection != Vector3.zero)
            {
                movementDirectionRotater.transform.rotation = Quaternion.LookRotation(slideDirection, Vector3.up);
            }
        }

        private void Update()
        {
            if (!camController.IsInFPSState()) return;
            RotateTowardsMoveDirection();
            SetDashVelocity();
            JumpInput();
            DashInput();
            CrouchInput();
            RechargeDash();
            MovementInput();
            HandleDrag();
            HandleCoyoteJump();
            HandleSlide();
            //HandleFOV();
        }

        private void FixedUpdate()
        {
            if (!camController.IsInFPSState()) return;
            GroundCheck();

            MovePlayer();
            UpdateSpeedText();

            if (!grounded && !input.Player.Jump.IsPressed() && rb.velocity.y < 0)
            {
                rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
            }
        }

        private void UpdateSpeedText()
        {
            if(horizontalSpeedText != null)
            {
                horizontalSpeedText.text = "hor: " + flatVelocity.magnitude.ToString("F1");
            }

            if(verticalSpeedText != null)
            {
                verticalSpeedText.text = "ver: " + verticalVelocity.magnitude.ToString("F1");
            }  
        }

        //private void HandleFOV()
        //{
        //    if (isDashing) { targetFOV = defaultFOV + dashFOVDiff; }
        //    else if (isSliding) { targetFOV = defaultFOV + slideFOVDiff; }
        //    else { targetFOV = defaultFOV; }
        //    Camera cam = playerCamera.gameObject.GetComponent<Camera>();
        //    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * FOVLerpSpeed);
        //}

        private void GroundCheck()
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, 1.2f);
            if (grounded) { JumpInput(); } // for bunnyhopping
            if (grounded && isGroundPounding)
            {
                isGroundPounding = false;
            }
            if (grounded)
            {
                //transform.SetParent(groundCheck.standingOnObject?.transform); <- illegal code
                currentWallJumpAmount = maxWallJumpAmount;
                JumpInput();
            }
            else
            {
                //transform.SetParent(null);
                PivotVelocity(airPivotInterpolationFactor);
            }
        }


        private void SetDashVelocity()
        {
            if (isDashing)
            {
                Vector3 dashVelocity = dashDirection.normalized * dashVelocityMagnitude;
                rb.velocity = new Vector3(dashVelocity.x, retainHeightWhileDashing ? dashVelocity.y : rb.velocity.y, dashVelocity.z);
            }
        }


        private void JumpInput()
        {

            if (easyBunnyHop)
            {
                if (input.Player.Jump.IsPressed() && readyToJump && grounded) { HandleJump(); }
                return;
            }
            jumpTimer += Time.deltaTime;

            if (input.Player.Jump.triggered) { jumpTimer = 0f; }

            if (jumpTimer <= lateJumpGraceTime && readyToJump && grounded)
            {
                Debug.Log("jump input");
                HandleJump();

                if (jumpTimer > perfectJumpGraceTime)
                {
                    float timeDifference = jumpTimer - perfectJumpGraceTime;
                    StartCoroutine(ActivateDragForSeconds(timeDifference));
                }

            }


            else if (input.Player.Jump.triggered && readyToJump && coyoteTimer < coyoteJumpGraceTime && !coyoteJumpUsed)
            {
                HandleJump();
            }

        }

        private void HandleJump()
        {
            Debug.Log("jump");
            readyToJump = false; //communicates to other methods a jump has begun this frame, makes things like bunny hops possible
            coyoteJumpUsed = true;
            StopSliding();

            Jump();



            Invoke(nameof(ResetJump), jumpCooldown); //makes it so you can hold jump button to jump as soon as you can
        }



        private IEnumerator LerpAirControl()
        {
            airMultiplier = 0f;
            float timer = 0f;
            while (airMultiplier < defaultAirMultiplier)
            {
                timer += Time.deltaTime;
                airMultiplier = airControlCurve.Evaluate(timer) * defaultAirMultiplier;
                yield return null;
            }
            yield return null;
        }

        private void Jump()
        {
            PivotVelocity(1f); //player is in full controll of their direction during their jump, allowing them to take all their velocity into a new direction

            if (isDashing)
            {
                isDashing = false;
                Vector3 dashVelocity = dashDirection.normalized * canceledDashVelocityMagnitude;
                rb.velocity = new Vector3(dashVelocity.x, rb.velocity.y, dashVelocity.z);

            }
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); //reset vertical velocity
            SetDrag(airDrag);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }


        private void DashInput()
        {
            if (input.Player.Dash.triggered)
            {
                if (currentDashAmount > 0) { Dash(); }
                else
                {
                    //play "no dash available" sound
                }
            }
        }


        private void CrouchInput() //monstrosity method
        {
            if (isSliding)
            {
                playerBody.transform.localPosition = new Vector3(0f, slideBodyYpos, 0f);
                playerBody.transform.localScale = new Vector3(1f, slideBodyYscale, 1f);
            }
            else
            {
                playerBody.transform.localPosition = new Vector3(0f, startingPlayerBodyYpos, 0f);
                playerBody.transform.localScale = new Vector3(1f, startingPlayerBodyYscale, 1f);
            }
            if (input.Player.Crouch.triggered && grounded)
            {
                if (!isSliding)
                {
                    if (moveDirection == new Vector3(0, 0, 0)) { moveDirection = orientation.forward; }
                    slideDirection = moveDirection;
                }
                isSliding = true;
            }
            else if (input.Player.Crouch.triggered && !grounded)
            {
                slideIsBuffered = true;
            }
            if (input.Player.Crouch.IsPressed() && grounded && slideIsBuffered)
            {
                if (!isSliding)
                {
                    slideDirection = moveDirection;
                    if (moveDirection == new Vector3(0, 0, 0)) { slideDirection = orientation.forward; }
                }
                isSliding = true;
                slideIsBuffered = false;
            }
            if (!input.Player.Crouch.IsPressed() || (!grounded && !slideIsBuffered))
            {
                StopSliding();
                slideIsBuffered = false;

            }
            if (input.Player.Crouch.triggered && !grounded)
            {
                GroundPound();
            }
        }

        private void MovementInput()
        {
            horizontalInput = input.Player.Move.ReadValue<Vector2>().x;
            verticalInput = input.Player.Move.ReadValue<Vector2>().y;
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            //if (cameraTilt) { TiltCamera(); }
        }

        private void GroundPound()
        {
            isGroundPounding = true;
            rb.velocity = Vector3.down * poundVelocityMagnitude;
        }

        public void GivePillarForce(Vector3 direction, float force, float maxForceMultiplier)
        {
            // Calculate rotation differences from transform.up and transform.down
            float rotationDifferenceUp = Vector3.Angle(transform.up, direction);
            float rotationDifferenceDown = Vector3.Angle(-transform.up, direction);

            // Use rotation differences to adjust force
            float forceMultiplierUp = Mathf.Lerp(1f, maxForceMultiplier, rotationDifferenceUp / 180f);
            float forceMultiplierDown = Mathf.Lerp(1f, maxForceMultiplier, rotationDifferenceDown / 180f);

            // Use the smaller force multiplier to ensure it's 1 when close to up or down
            float forceMultiplier = Mathf.Min(forceMultiplierUp, forceMultiplierDown);
            Debug.Log("horizontal pillar force multiplier: " + forceMultiplier);
            float adjustedForce = force * forceMultiplier;

            GiveForce(direction, adjustedForce);
        }

        public void GiveForce(Vector3 direction, float force)
        {
            readyToJump = false;
            SetDrag(0);
            SetDynamicFriction(0);
            Invoke(nameof(ResetJump), jumpCooldown);

            rb.AddForce(direction * force, ForceMode.Impulse);
        }

        public void ApplyForce(float force, Vector3 direction)
        {
            if (forceApplied) return;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); //reset vertical velocity
            GivePillarForce(direction, force, maxForceMultiplier);
            StopSliding();
            forceApplied = true;
            Invoke(nameof(AllowDisplacement), ForceAppliedTimer);
        }

        private void MovePlayer()
        {
            if (isDashing || isSliding) return;
            if (grounded && readyToJump)
            {
                
                float groundSpeedMultiplier = groundSpeedCurve.Evaluate(flatVelocity.magnitude / maxSpeedForMultiplier);
                rb.AddForce(moveDirection.normalized * (moveSpeed * groundSpeedMultiplier), ForceMode.Force);
            }
            else if (!grounded)
            {
                float airSpeedMultiplier = airSpeedCurve.Evaluate(flatVelocity.magnitude / airSpeedLimit);
                rb.AddForce(moveDirection.normalized * (moveSpeed * airMultiplier * airSpeedMultiplier), ForceMode.Force);
            }
        }

        private void PivotVelocity(float interpolationFactor)
        {
            if (isDashing) return;

            Vector3 desiredVelocity = moveDirection.normalized * flatVelocity.magnitude; //the direction the player is inputting, with the velocity the player currently has

            if (desiredVelocity.magnitude == 0f) { return; } //when player doesnt give any direction inputs, the velocity should not be pivoted.

            var velocity = rb.velocity;
            float currentYVelocity = velocity.y;

            // Smoothly change the x and z components of velocity towards the desired velocity
            Vector3 newVelocity = Vector3.Lerp(
                new Vector3(velocity.x, 0f, velocity.z),
                new Vector3(desiredVelocity.x, 0f, desiredVelocity.z),
                interpolationFactor //when this value is 1, it essentially doesnt lerp at al. and just makes the desired velocity the actual velocity
            );

            newVelocity.y = currentYVelocity; //dont affect the y velocity

            rb.velocity = newVelocity;
        }

        private IEnumerator ActivateDragForSeconds(float seconds)
        {
            punishDragActivated = true;
            yield return new WaitForSeconds(seconds);
            punishDragActivated = false;
        }

        private void HandleSlide()
        {
            if (isSliding) { Slide(); }
            else
            {
                currentSlideSpeed = slideSpeed;
                slideDirection = Vector3.zero;
                currentMaxTiltAmount = maxTiltAmount;
                SetCamHeight(defaultCamHeight);
            }
        }

        private void Slide()
        {
            Vector3 slopeDirection = GetSlopeDirection(slideDirection.normalized);
            //currentSlideSpeed -= (slopeDirection.y * Time.deltaTime * slideVelChangeSpeed);
            //currentSlideSpeed = currentSlideSpeed < 0 ? 0 : currentSlideSpeed; //limit to 0

            currentSlideSpeed = rb.velocity.magnitude > currentSlideSpeed ? rb.velocity.magnitude : currentSlideSpeed;
            Vector3 slideVelocity = slopeDirection * currentSlideSpeed;
            rb.velocity = slideVelocity; //if current flat velocity faster than a slide, keep that velocity during slide
            //PivotVelocity(slidePivotFactor);
            currentMaxTiltAmount = slideMaxTiltAmount;

            //lower camera
            SetCamHeight(slideCamHeight);


        }

        private void Dash()
        {
            TryUseDash(1);

            dashDirection = moveDirection;
            if (moveDirection == new Vector3(0, 0, 0)) { dashDirection = orientation.forward; }

            if (dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
            }

            dashCoroutine = StartCoroutine(DashSequenceCoroutine(dashDuration));
        }

        public bool TryUseDash(int amount)
        {
            int amountCheck = currentDashAmount;
            amountCheck -= amount;

            if (amountCheck < 0)
            {
                return false;
            }
            currentDashAmount -= amount;
            return true;
        }

        private IEnumerator DashSequenceCoroutine(float dashDuration)
        {
            isDashing = true;
            float timer = 0f;
            //yield return new WaitForSeconds(dashDuration);


            while (timer <= dashDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (isDashing) //if dash is not canceled
            {
                Vector3 endingDashVelocity = dashDirection.normalized * airSpeedLimit;
                rb.velocity = new Vector3(endingDashVelocity.x, rb.velocity.y, endingDashVelocity.z);
            }

            isDashing = false;
            dashCoroutine = null;
        }

        private void RechargeDash()
        {
            if (currentDashAmount == maxDashAmount) return;
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashCooldown)
            {
                currentDashAmount += 1;
                dashTimer = 0;
            }
        }

        private void HandleCoyoteJump()
        {
            if (grounded && readyToJump)
            {
                coyoteJumpUsed = false;
                coyoteTimer = 0f;
                return;
            }
            coyoteTimer += Time.deltaTime;
        }

        //private void TiltCamera()
        //{
        //    playerCamera.TiltCamera(currentMaxTiltAmount, horizontalInput, tiltSpeed);
        //}

        private void HandleDrag()
        {
            HandleGroundDrag();
            HandlePunishDrag();
        }

        private void HandleGroundDrag()
        {
            SetDrag(grounded && readyToJump && !isSliding ? groundDrag : airDrag);
            SetDynamicFriction(grounded && readyToJump && !isSliding ? dynamicFriction : 0f);
        }

        private void HandlePunishDrag()
        {
            SetDrag(punishDragActivated ? groundDrag : drag); //punish drag for early bhop attempt (see docs)
        }

        public void SetDynamicFriction(float value)
        {
            if (dynamicFriction == value) return;
            dynamicFriction = value;
            //Debug.Log("set friction to " + dynamicFriction);
            playerPhysicsMat.dynamicFriction = dynamicFriction;
            if (dynamicFriction == 0)
            {
                playerPhysicsMat.frictionCombine = PhysicMaterialCombine.Minimum;
            }
            else
            {
                playerPhysicsMat.frictionCombine = PhysicMaterialCombine.Average;
            }
        }

        public void SetDrag(float value)
        {
            if (drag == value) return;
            drag = value;
            rb.drag = drag;
        }

        private void SetCamHeight(float height)
        {
            cameraPos.localPosition = new Vector3(0, height, 0);
        }

        private Vector3 GetSlopeDirection(Vector3 directionToProject)
        {
            RaycastHit slopeHit1;
            RaycastHit slopeHit2;
            float slopeAngle1 = 0f;
            float slopeAngle2 = 0f;
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit1, 10f, whatIsGround))
            {
                slopeAngle1 = Vector3.Angle(Vector3.up, slopeHit1.normal);
            }
            if (Physics.Raycast(slopeCheckPoint.position, Vector3.down, out slopeHit2, 10f, whatIsGround))
            {
                slopeAngle2 = Vector3.Angle(Vector3.up, slopeHit2.normal);
            }

            if (slopeHit1.collider != null && slopeHit2.collider != null)
            {
                float largestAngle = slopeAngle1 > slopeAngle2 ? slopeAngle1 : slopeAngle2;
                if (largestAngle < maxWalkableSlopeAngle)
                {
                    SetDynamicFriction(dynamicGroundFriction);
                }
                else
                {
                    Debug.Log("slippery cuz too steep!");
                    SetDynamicFriction(0f);
                }
                return Vector3.ProjectOnPlane(directionToProject, largestAngle == slopeAngle1 ? slopeHit1.normal : slopeHit2.normal).normalized;
            }


            return directionToProject;
        }

        public float GetDashProgress() { return (100 * currentDashAmount) + (dashTimer / dashCooldown) * 100; }

        public void ResetYVelocity() { rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); }
        public void StopSliding()
        {
            if (!Physics.Raycast(playerCamera.transform.position, Vector3.up, slideRaycastCheckDistance, whatIsGround))
            {
                isSliding = false;
            }
        }
        public void ResetJump() => readyToJump = true;

        public void AllowDisplacement() => forceApplied = false;
        private void OnEnable() => input.Enable();
        private void OnDisable() => input.Disable();
        public Vector3 flatVelocity => new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        public Vector3 verticalVelocity => new Vector3(0f, rb.velocity.y, 0f);

    }
}
