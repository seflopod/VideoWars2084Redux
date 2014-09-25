using UnityEngine;
using System.Collections;
using VideoWars.Components;
using VideoWars.Management;
using VideoWars.OtherBehaviours;
using VideoWars.Utility.Attributes;

namespace VideoWars.Combatants
{
	[RequireComponent(typeof(MovementComponent), typeof(WeaponComponent))]
	public class CombatantBehaviour : MonoBehaviour
	{
		#region serialized_fields
		[SerializeField]
		[PrivateSerialized("Color")]
		protected Color mColor;

		[SerializeField]
		[PrivateSerialized("Max Health")]
		protected int mMaxHealth = 1000;
		#endregion

		#region other_fields
		protected MovementComponent mMovement;
		protected WeaponComponent mWeapon;

		protected bool bIsDead = false;
		protected int mHealth;
		protected int mKills = 0;
		protected int mDeaths = 0;
		protected int mScore = 0;
		#endregion

		#region monobehaviour
		protected virtual void Start()
		{
			mMovement = GetComponent<MovementComponent>();
			mWeapon = GetComponent<WeaponComponent>();
			mWeapon.BulletColor = mColor;
			renderer.material.SetColor("_Color", mColor);
			mHealth = mMaxHealth;
			PlaySceneManager.Instance.RegisterCombatant(gameObject.GetInstanceID(), this);
		}

		protected virtual void OnCollisionEnter2D(Collision2D col)
		{
			if(col.gameObject.CompareTag("Platform") && !mMovement.CanJump)
			{
				//if the normal's y component is greater than 0, then the top-ish of
				//the platform has been hit
				if(col.contacts[0].normal.y > 0)
				{
					mMovement.CanJump = true;
					mMovement.IsApplyingGravity = false;
				}
			}
			else if(col.gameObject.CompareTag("Bullet"))
			{
				BulletBehaviour bb = col.gameObject.GetComponent<BulletBehaviour>();
				if(bb.OwnerId != gameObject.GetInstanceID())
				{
					mHealth -= bb.Damage;
					bb.Disable();
					bb.gameObject.SetActive(false);
					CombatantBehaviour other = PlaySceneManager.Instance.GetCombatantById(bb.OwnerId);
					if(mHealth <= 0)
					{
						other.RecordKill(mMaxHealth);
						Die();
					}
					else
					{
						other.RecordHit(bb.Damage);
					}
				}
			}
		}

		protected virtual void OnCollisionExit2D(Collision2D col)
		{
			if(col.gameObject.CompareTag("Platform"))
			{
				mMovement.IsApplyingGravity = true;
			}
		}

		#endregion

		#region death_and_spawn
		public virtual void Die()
		{
			mDeaths++;
			bIsDead = true;
			rigidbody2D.velocity = Vector2.zero;
			rigidbody2D.isKinematic = true;
			renderer.enabled = false;
			collider2D.enabled = false;
		}
		
		public virtual void Spawn(Vector3 position)
		{
			transform.position = position;
			bIsDead = false;
			renderer.enabled = true;
			collider2D.enabled = true;
			rigidbody2D.isKinematic = false;
		}
		#endregion

		#region scorekeeping
		public virtual void RecordKill(int otherMaxHealth)
		{
			mKills++;
			mScore += otherMaxHealth;
		}
		
		public virtual void RecordHit(int damageDone)
		{
			mScore += damageDone;
		}
		#endregion

		#region properites
		public MovementComponent Movement
		{
			get { return mMovement; }
			set { mMovement = value; }
		}

		public WeaponComponent Weapon
		{
			get { return mWeapon; }
			set { mWeapon = value; }
		}

		public bool IsDead
		{
			get { return bIsDead; }
			set { bIsDead = value; }
		}

		public int Health { get { return mHealth; } }
		public int Kills { get { return mKills; } }
		public int Deaths { get { return mDeaths; } }
		public int Score { get { return mScore; } }
		#endregion

	}
}
