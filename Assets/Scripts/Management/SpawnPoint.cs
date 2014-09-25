using UnityEngine;
using System.Collections;
using VideoWars.Utility.Attributes;

namespace VideoWars.Management
{
	[System.Flags]
	internal enum SpawnPointType
	{
		Player = 1,
		Enemy = 2
	};

	[ExecuteInEditMode]
	public class SpawnPoint : MonoBehaviour
	{
		[SerializeField]
		[FlagsSelect("Spawn Type")]
		private SpawnPointType mSpawnType = (SpawnPointType)3;

		private void Awake()
		{
			gameObject.tag = "SpawnPoint";
		}

		private void OnDrawGizmos()
		{
			Color color = Color.black;
			if((mSpawnType & SpawnPointType.Player) != 0)
			{
				color.r += 1f;
			}

			if((mSpawnType & SpawnPointType.Enemy) != 0)
			{
				color.b += 1f;
			}
			Gizmos.color = color;
			Gizmos.DrawWireSphere(transform.position, 0.25f);
		}

		internal SpawnPointType SpawnType
		{
			get { return mSpawnType; }
		}
	}
}
