using UnityEngine;
using VideoWars.Utility;
using VideoWars.Utility.Attributes;
using VideoWars.OtherBehaviours;

namespace VideoWars.Components
{
	[RequireComponent(typeof(AudioSource))]
	public class WeaponComponent : MonoBehaviour
	{
		#region serialized_fields
		[SerializeField]
		[PrivateSerialized("Bullet Speed")]
		private float mBulletSpeed = 20f;

		[SerializeField]
		[PrivateSerialized("Max Ammo")]
		private int mMaxAmmo = 5000;

		[SerializeField]
		[PrivateSerialized("Damage Multiplier")]
		private int mDamageMultiplier = 1;

		[SerializeField]
		[PrivateSerialized("Base Damage")]
		private int mBaseDamage = 200;

		[SerializeField]
		[PrivateSerialized("Fire Cooldown")]
		private float mFireCooldown = 0.2f;

		[SerializeField]
		[PrivateSerialized("Seconds to Reload")]
		private float mSecondToReload = 1f;

		[SerializeField]
		[PrivateSerialized("Firing Sound")]
		private AudioClip mSound = null;

		[SerializeField]
		[PrivateSerialized("Bullet Prefab")]
		private GameObject mBulletPrefab;

		[SerializeField]
		[PrivateSerialized("Bullet Time to Live")]
		private float mBulletTTL = 2.5f;
		#endregion
		
		#region other_fields
		private float mFireCDAccum = 0f;
		private float mReloadAccum = 0f;
		private bool bIsFireOnCD = false;
		private bool bIsReloading = false;
		private bool bCanShoot = true;
		private int mAmmoRemaining;
		private GameObjectPool mBulletPool;
		#endregion
		
		#region monobehaviour
		private void Start()
		{
			mAmmoRemaining = mMaxAmmo;

			//purely a guess on the size of the pool.  This should be adjusted here if it isn't sufficient
			//rather than adjust the size of the maxAmmo.
			mBulletPool = GameObjectPool.CreateNewPool(mBulletPrefab, Mathf.RoundToInt(mMaxAmmo * 0.01f));
		}
		
		private void Update()
		{
			float dt = Time.deltaTime;
			if(bIsFireOnCD)
			{
				mFireCDAccum += dt;
				bCanShoot = mFireCDAccum >= mFireCooldown;
				bIsFireOnCD = !bCanShoot;
			}
			else if(bIsReloading)
			{
				mReloadAccum += dt;
				bCanShoot = mReloadAccum >= mSecondToReload;
				bIsReloading = !bCanShoot;
			}
		}
		#endregion
		
		public void Fire(bool shouldShootBackward)
		{
			if(!bCanShoot)
			{
				return;
			}
			
			Vector2 dir = ((shouldShootBackward) ? -1 : 1) * rigidbody2D.velocity.normalized;
			
			//this assumes the Collider2D is centered
			float offset = Mathf.Sqrt(collider2D.bounds.size.x * collider2D.bounds.size.x + collider2D.bounds.size.y * collider2D.bounds.size.y);
			Vector3 spawnPos = transform.position + new Vector3(Mathf.Sign(dir.x) * offset, Mathf.Sign(dir.y) * offset, 0f);
			GameObject bulletGO = mBulletPool.GetGameObject();
			bulletGO.SetActive(true);
			bulletGO.transform.position = spawnPos;
			bulletGO.rigidbody2D.velocity = (Mathf.Abs(rigidbody2D.velocity.magnitude) + mBulletSpeed) * dir;
			bulletGO.renderer.material.SetColor("_Color", BulletColor);

			//adjust bullet damage and other info.
			BulletBehaviour bb = bulletGO.GetComponent<BulletBehaviour>();
			bb.Damage = mBaseDamage * mDamageMultiplier;
			bb.OwnerId = gameObject.GetInstanceID();
			bb.Life = mBulletTTL;
			bb.Enable();

			// spawn audio
			if(mSound != null)
			{
				audio.loop = false;
				audio.clip = mSound;
				audio.Play();
			}
			
			// setup timers for fire rate OR reload
			bCanShoot = false;
			mAmmoRemaining -= 1;
			
			if(mAmmoRemaining == 0)
			{
				bIsReloading = true;
				mReloadAccum = 0f;
			}
			else
			{
				bIsFireOnCD = true;
				mFireCDAccum = 0f;
			}
		}
		
		#region properties		
		public int DamageMultiplier
		{
			get { return mDamageMultiplier; }
			set { mDamageMultiplier = value; }
		}

		public bool CanShoot
		{
			get { return bCanShoot; }
			set { bCanShoot = value; }
		}

		public Color BulletColor { get; set; }
		#endregion
	}
}