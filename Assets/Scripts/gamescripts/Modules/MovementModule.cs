using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
/// <summary>
/// Module that will handle all movement related.
/// <para>This includes horizontal and vertical</para>
/// </summary>
public class MovementModule : MainModule
{
    [Header("MOVEMENT MODULE")]
	[Header("Gravity")]
	[HideInInspector] public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
	[HideInInspector] public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
												 //Also the value the player's rigidbody2D.gravityScale is set to.
	[Space(5)]
	public float fallGravityMult; //Multiplier to the player's gravityScale when falling.
	public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.
	[Space(5)]
	public float fastFallGravityMult; //Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed.
									  //Seen in games such as Celeste, lets the player fall extra fast if they wish.
	public float maxFastFallSpeed; //Maximum fall speed(terminal velocity) of the player when performing a faster fall.

	[Space(20)]

	[Header("Run")]
	public float runMaxSpeed; //Target speed we want the player to reach.
	public float runAcceleration; //The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
	[HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
	public float runDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
	[HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
	[Space(5)]
	[Range(0f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
	[Range(0f, 1)] public float deccelInAir;
	[Space(5)]
	public bool doConserveMomentum = true;

	[Space(20)]

	[Header("Jump")]
	public float jumpHeight; //Height of the player's jump
	public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
	[HideInInspector] public float jumpForce; //The actual force applied (upwards) to the player when they jump.

	[Header("Both Jumps")]
	[Tooltip("Multiplier to increase gravity if the player releases thje jump button while still jumping")]
	public float jumpCutGravityMult;
	[Tooltip("Reduces gravity while close to the apex (desired max height) of the jump")]
	[Range(0f, 1)] public float jumpHangGravityMult;
	[Tooltip("Speeds (close to 0) where the player will experience extra 'jump hang'. The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)")]
	public float jumpHangTimeThreshold;
	[Space(0.5f)]
	public float jumpHangAccelerationMult;
	public float jumpHangMaxSpeedMult;

	[Space(20)]

	[Header("Assists")]
	[Tooltip("Grace period after falling off a platform, where you can still jump")]
	[Range(0.01f, 0.5f)] public float coyoteTime; 
	[Tooltip("Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.")]
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; 

	#region VARIABLES

	//Components
	public Rigidbody2D RB { get; private set; }

	// Variables control the various actions the player can perform at any time.
	// These are fields which can are public allowing for other sctipts to read them
	// but can only be privately written to.
	public bool IsFacingRight { get; private set; }
	bool _isJmp;
	public bool IsJumping
	{
		get => _isJmp;
		private set
		{
			if (Unit != null)
				Unit.IsJumping = value;

			_isJmp = value;
		}
	}

	// Timers (also all fields, could be private and a method returning a bool could be used)
	public float LastOnGroundTime { get; private set; }
	public bool IsGrounded => LastOnGroundTime > 0;
	public bool IsFalling
	{
		get
        {
			bool falling = RB.velocity.y < 0;

			if (Unit != null)
				Unit.IsFalling = falling;

			return falling;
		}
	}

	// Jump
	private bool _isJumpCut;
	private bool _isJumpFalling;

	// Inputs
	private float _moveInput;
	private bool _holdingDuck;

	public float LastPressedJumpTime { get; private set; }

	//Set all of these up in the inspector
	[Header("Checks")]
	[SerializeField] private Transform _groundCheckPoint;
	//Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
	[SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);

	[Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;
	#endregion

	private void Awake()
	{
		RB = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		SetGravityScale(gravityScale);
		IsFacingRight = true;
	}

    public override void UpdateModule()
    {
		_holdingDuck = Unit.IsDucking;


		// if we're in air, then we don't need to check others stuff
		bool canMove = !IsGrounded || (!Unit.IsHurting && !_holdingDuck && !Unit.IsHiding && !Unit.IsAttacking && !Unit.IsReloading);
		_moveInput = canMove ? Unit.Horizontal : 0;

		if (_moveInput != 0)
			CheckDirectionToFace(_moveInput > 0);

		if (Unit.TryJump)
		{
			OnJumpDownInput();
        }
        else
        {
			OnJumpUpInput();
        }
		
	}

    private void Update()
	{
		// TIMERS
		LastOnGroundTime -= Time.deltaTime;

		LastPressedJumpTime -= Time.deltaTime;



		// COLLISION CHECKS
		if (!IsJumping)
		{
			//Ground Check
			if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping) //checks if set box overlaps with ground
			{
				LastOnGroundTime = coyoteTime; //if so sets the lastGrounded to coyoteTime
			}
		}

		// JUMP CHECKS
		if (IsJumping && RB.velocity.y < 0)
		{
			IsJumping = false;
		}

		if (LastOnGroundTime > 0 && !IsJumping)
		{
			_isJumpCut = false;

			if (!IsJumping)
				_isJumpFalling = false;
		}

		//Jump
		if (!Unit.IsAttacking && CanJump() && LastPressedJumpTime > 0)
		{
			IsJumping = true;
			_isJumpCut = false;
			_isJumpFalling = false;

			Jump();
		}



		#region GRAVITY
		//Higher gravity if we've released the jump input or are falling
		if (RB.velocity.y < 0 && _holdingDuck)
		{
			//Much higher gravity if holding down
			SetGravityScale(gravityScale * fastFallGravityMult);
			//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
			RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFastFallSpeed));
		}
		else if (_isJumpCut)
		{
			//Higher gravity if jump button released
			SetGravityScale(gravityScale * jumpCutGravityMult);
			RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFallSpeed));
		}
		else if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < jumpHangTimeThreshold)
		{
			SetGravityScale(gravityScale * jumpHangGravityMult);
		}
		else if (RB.velocity.y < 0)
		{
			//Higher gravity if falling
			SetGravityScale(gravityScale * fallGravityMult);
			//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
			RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFallSpeed));
		}
		else
		{
			//Default gravity if standing on a platform or moving upwards
			SetGravityScale(gravityScale);
		}
		#endregion
	}

	private void FixedUpdate()
	{
		//Handle Run
		Run(1);
	}


	#region INPUT CALLBACKS

	//Methods which whandle input detected in Update()
	public void OnJumpDownInput()
	{
		LastPressedJumpTime = jumpInputBufferTime;
	}

	public void OnJumpUpInput()
	{
		if (CanJumpCut())
			_isJumpCut = true;
	}

	#endregion


	#region GENERAL METHODS

	public void SetGravityScale(float scale)
	{
		RB.gravityScale = scale;
	}

	#endregion


	#region RUN

	private void Run(float lerpAmount)
	{
		//Calculate the direction we want to move in and our desired velocity
		float targetSpeed = _moveInput * runMaxSpeed;
		//We can reduce are control using Lerp() this smooths changes to are direction and speed
		targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

		// Calculate Accelration
		float accelRate;

		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount : runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount * accelInAir : runDeccelAmount * deccelInAir;

		// Add Bonus Jump Apex Acceleration
		//Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
		if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < jumpHangTimeThreshold)
		{
			accelRate *= jumpHangAccelerationMult;
			targetSpeed *= jumpHangMaxSpeedMult;
		}

		// Conserve Momentum
		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if (doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			//Prevent any deceleration from happening, or in other words conserve are current momentum
			//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
			accelRate = 0;
		}

		//Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - RB.velocity.x;
		//Calculate force along x-axis to apply to thr player

		float movement = speedDif * accelRate;

		//Convert this to a vector and apply to rigidbody
		RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

		/*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
	}

	public void Turn()
	{
		//stores scale and flips the player along the x axis, 
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
	}
	#endregion


	#region JUMP
	private void Jump()
	{
		//Ensures we can't call Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		// Perform Jump
		//We increase the force applied if we are falling
		//This means we'll always feel like we jump the same amount 
		//(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)

		float force = jumpForce;
		if (RB.velocity.y < 0)
			force -= RB.velocity.y;

		RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);

	}
	#endregion


	#region CHECK

	public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
			Turn();
	}

	private bool CanJump()
	{
		return LastOnGroundTime > 0 && !IsJumping;
	}

	private bool CanJumpCut()
	{
		return IsJumping && RB.velocity.y > 0;
	}

	#endregion


	#region EDITOR METHODS

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = IsGrounded ? Color.green : Color.red;

		if(_groundCheckPoint != null)
			Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
			
	}

	private void OnValidate()
	{

		//Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
		gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

		//Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
		gravityScale = gravityStrength / Physics2D.gravity.y;

		//Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
		runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
		runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

		//Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
		jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

		// Variable Ranges
		runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
		runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);

	}

	#endregion

}


