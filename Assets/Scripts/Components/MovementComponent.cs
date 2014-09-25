using UnityEngine;
using VideoWars.Utility.Attributes;

namespace VideoWars.Components
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class MovementComponent : MonoBehaviour
	{
		#region inspector_fields
		[SerializeField]
		[PrivateSerialized("Move Speed")]
		private float mMoveSpeed = 15f;
		
		[SerializeField]
		[PrivateSerialized("Speed Reduction")]
		private float mSpeedReduction = 0.4f;
		
		[SerializeField]
		[PrivateSerialized("Jump Speed")]
		private float mJumpSpeed = 20f;
		
		[SerializeField]
		[PrivateSerialized("Thrust Cost per Tick")]
		private int mThrustCostPerTick = 100;
		
		[SerializeField]
		[PrivateSerialized("Thrust Force")]
		private float mThrustForce = 500f;
		
		[SerializeField]
		[PrivateSerialized("Max Fuel")]
		private int mMaxFuel = 6000;
		
		[SerializeField]
		[PrivateSerialized("Fuel Regen per Tick")]
		private int mFuelRegenPerTick = 25;
		#endregion
		
		#region private_fields
		private bool bCanJump = false;
		private bool bIsThrusting = false;
		private bool bShouldForceFullRegen = false;
		private int mFuelRemaining = 0;
		private float mThrustDelay = 0f;
		private bool bIsApplyingGravity = true;
		#endregion
		
		#region monobehaviour
		private void Start()
		{
			mFuelRemaining = mMaxFuel;
			rigidbody2D.gravityScale = 0f;
		}
		
		private void Update()
		{
			if(!bIsThrusting)
			{
				RegenFuel();
			}
		}
		
		private void FixedUpdate()
		{
			if(bIsApplyingGravity)
			{
				rigidbody2D.velocity += Physics2D.gravity * Time.fixedDeltaTime;
			}
		}
		#endregion
		
		#region basic_movement
		public void Move(Vector2 dir)
		{
			if(dir != Vector2.zero)
			{
				rigidbody2D.velocity = mMoveSpeed * dir;
			}
		}

		public void Move(float horizAxis, float vertAxis)
		{
			Vector2 dir = (new Vector2(horizAxis, vertAxis)).normalized;
			Move(dir);
		}
		
		public void Jump()
		{
			if(bCanJump)
			{
				Vector2 curVel = rigidbody2D.velocity;
				curVel.y = mJumpSpeed;
				rigidbody2D.velocity = curVel;
				bIsApplyingGravity = true;
				bCanJump = false;
			}
		}
		
		public void Slow()
		{
			Vector2 curVel = rigidbody2D.velocity;
			curVel.x *= mSpeedReduction;
			
			//if the player is moving up the reduction is less than if they are moving
			//down
			curVel.y = (curVel.y > 0f) ? curVel.y * 2 * mSpeedReduction : curVel.y * 3 * mSpeedReduction;
			rigidbody2D.velocity = curVel;
		}
		
		public void StopInX()
		{
			Vector2 vel = new Vector2(0f, rigidbody2D.velocity.y);
			rigidbody2D.velocity = vel;
		}
		
		public void StopInY()
		{
			Vector2 vel = new Vector2(rigidbody2D.velocity.x, 0f);
			rigidbody2D.velocity = vel;
		}
		#endregion
		
		#region thrusting
		public void DoThrust()
		{
			if(!bIsThrusting && !bShouldForceFullRegen && mFuelRemaining >= 2 * mThrustCostPerTick)
			{
				//starting thrust gives a bit more of a boost
				Vector2 force = new Vector2(0f, 3 * mThrustForce);
				rigidbody2D.AddForce(force, ForceMode2D.Impulse);
				bIsThrusting = true;
				bIsApplyingGravity = true;
				
				//cost to start thrusting is higher than normal
				mFuelRemaining -= 2 * mThrustCostPerTick;
				bShouldForceFullRegen = (mFuelRemaining < mThrustCostPerTick / 2);
			}
			else if(bIsThrusting && !bShouldForceFullRegen && mFuelRemaining >= mThrustCostPerTick)
			{
				mFuelRemaining -= mThrustCostPerTick;
				Vector2 force = new Vector2(0f, mThrustForce);
				rigidbody2D.AddForce(force, ForceMode2D.Force);
				bShouldForceFullRegen = (mFuelRemaining < mThrustCostPerTick / 2);
			}
			else
			{
				EndThrust();
			}
		}
		
		public void EndThrust()
		{
			bIsThrusting = false;
			mThrustDelay = mMaxFuel * -0.1f;
		}
		
		public void RegenFuel()
		{
			if(mFuelRemaining < mMaxFuel && !bIsThrusting)
			{
				//after thrusting you have to wait some time before regen begins
				if(mThrustDelay < 0)
				{
					mThrustDelay += mFuelRegenPerTick;
				}
				else
				{
					mFuelRemaining += mFuelRegenPerTick;
					if(mFuelRemaining >= mMaxFuel)
					{
						mFuelRemaining = mMaxFuel;
						bShouldForceFullRegen = false;
					}
				}
			}
		}
		#endregion

		#region properites
		public bool CanJump
		{
			get { return bCanJump; }
			set { bCanJump = value; }
		}

		public bool IsApplyingGravity
		{
			get { return bIsApplyingGravity; }
			set { bIsApplyingGravity = value; }
		}

		public bool IsThrusting
		{
			get { return bIsThrusting; }
		}
		#endregion
	}
}
