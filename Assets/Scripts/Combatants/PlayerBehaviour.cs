using UnityEngine;
using VideoWars.Management;

namespace VideoWars.Combatants
{
	public class PlayerBehaviour : CombatantBehaviour
	{
		public float timeToRespawn = 2f;

		private float mRespawnAccum = 0f;

		private void Update()
		{
			if(bIsDead)
			{
				mRespawnAccum += Time.deltaTime;
				if(mRespawnAccum >= timeToRespawn)
				{
					bIsDead = false;
					PlaySceneManager.Instance.SpawnPlayer();
					mRespawnAccum = 0f;
				}
			}
		}

		public void DoMovement(float horizAxis, float vertAxis, bool doSlow, bool doThrust)
		{
			if(doSlow)
			{
				mMovement.Slow();
			}

			if(horizAxis != 0f || vertAxis != 0f)
			{
				mMovement.Move(horizAxis, vertAxis);
			}
			
			if(doThrust)
			{
				mMovement.DoThrust();
			}
			else if(mMovement.IsThrusting)
			{
				mMovement.EndThrust();
			}
		}

		public override void RecordKill(int otherMaxHealth)
		{
			base.RecordKill(otherMaxHealth);
			PlaySceneManager.Instance.EnemiesLeft--;
		}
	}
}
