using UnityEngine;
using System.Collections.Generic;
using VideoWars.Utility.Attributes;

namespace VideoWars.Combatants
{
	public class HumanEnemyBehaviour : CombatantBehaviour
	{
		[SerializeField]
		[PrivateSerialized("Target Mask")]
		private LayerMask mTargetMask;

		private float mChangeDirAccum = 1f;
		private float mMaxTimeToChangeDir = 1f;
		private float mMinTimeToChangeDir = 0.5f;
		private float mCurTimeToChangeDir = 1f;
		private Vector2 mMoveDir = Vector2.zero;
		
		private Transform mCurTargetTrans;
		private Collider2D[] mTargets = new Collider2D[20];
		private bool bHasTarget = false;


		protected override void Start()
		{
			base.Start();
			pickMoveDirection(100f);
		}

		private void Update()
		{
			if(!bHasTarget)
			{
				pickMoveDirection(Time.deltaTime);
			}
			else
			{
				mMoveDir = ((Vector3)mCurTargetTrans.rigidbody2D.velocity + mCurTargetTrans.position - transform.position).normalized;
			}
			mMovement.Move(mMoveDir);
		}
		
		private void FixedUpdate()
		{
			findTarget();
			if(bHasTarget)
			{
				mWeapon.Fire(false);
			}
		}
		
		protected override void OnCollisionEnter2D(Collision2D col)
		{
			base.OnCollisionEnter2D(col);
			if(col.gameObject.CompareTag("Platform"))
			{
				pickMoveDirection(10000f);
			}
		}
		
		private void pickMoveDirection(float dt)
		{
			mChangeDirAccum += dt;
			if(mChangeDirAccum >= mCurTimeToChangeDir)
			{
				mChangeDirAccum = 0.0f;
				int aiDir = Random.Range(0,8);
				mMoveDir = new Vector2(Mathf.Cos(aiDir*Mathf.PI/4),
				                       Mathf.Sin(aiDir*Mathf.PI/4));
				mCurTimeToChangeDir = Random.Range(mMinTimeToChangeDir, mMaxTimeToChangeDir);
			}
		}
		
		private void findTarget()
		{
			Vector2 origin = new Vector2(transform.position.x, transform.position.y);
			float r = 5.0f;

			int numTargets = Physics2D.OverlapCircleNonAlloc(origin, r, mTargets, mTargetMask.value);
			List<Collider2D> targets = new List<Collider2D>(mTargets);
			//first remove self from targets
			for(int i=0;i<numTargets;++i)
			{
				if(mTargets[i].gameObject.GetInstanceID() == gameObject.GetInstanceID())
				{
					targets.RemoveAt(i);
					numTargets--;
					break;
				}
			}

			bHasTarget = false;
			if(numTargets > 0)
			{
				//then look to see if the player is available as a target
				for(int i=0;i<numTargets;++i)
				{
					if(targets[i].gameObject.CompareTag("Player"))
					{
						mCurTargetTrans = targets[i].gameObject.transform;
						bHasTarget = true;
						break;
					}
				}

				//the player wasn't available as a target.  what else is there?
				if(!bHasTarget)
				{	//randomly choose something to kill
					mCurTargetTrans = targets[Random.Range(0, numTargets)].gameObject.transform;
					bHasTarget = true;
				}
			}
		}

	}
}

	