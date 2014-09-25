using UnityEngine;

namespace VideoWars.OtherBehaviours
{
	public class BulletBehaviour : MonoBehaviour
	{
		private float mLifeAccum = 0f;
		private bool bIsEnabled = false;

		private void Update()
		{
			if(bIsEnabled)
			{
				mLifeAccum += Time.deltaTime;
				if(mLifeAccum >= Life)
				{
					Disable();
					gameObject.SetActive(false);
				}
			}
		}

		public void Enable()
		{
			renderer.enabled = true;
			collider2D.enabled = true;
			mLifeAccum = 0f;
			bIsEnabled = true;
		}

		public void Disable()
		{
			renderer.enabled = false;
			collider2D.enabled = false;
			rigidbody2D.velocity = Vector2.zero;
			bIsEnabled = false;
		}

		public int Damage { get; set; }
		public int OwnerId { get; set; }
		public float Life { get; set; }
	}
}