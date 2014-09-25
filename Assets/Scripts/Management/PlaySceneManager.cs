using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VideoWars.Combatants;
using VideoWars.Utility;
using VideoWars.Utility.Attributes;

namespace VideoWars.Management
{
	public class PlaySceneManager : Singleton<PlaySceneManager>
	{
		#region serialized_fields
		[SerializeField]
		[PrivateSerialized("Player Prefab")]
		private GameObject mPlayerPrefab = null;

		public Wave[] waves = new Wave[0];
		#endregion

		#region other_fields
		private Dictionary<int, CombatantBehaviour> mCombatantsById = new Dictionary<int, CombatantBehaviour>();
		private PlayerBehaviour mPlayer;
		private Vector3[] mEnemySpawnPoints;
		private Vector3[] mPlayerSpawnPoints;
		private int mCurPlayerSpawnIdx = 0;
		private int mCurEnemySpawnIdx = 0;
		private GameObject[][] mWavesGOArray;
		private int mCurWave = 0;
		private int mWaveIdxLastSpawned = -1;
		private bool bIsSpawning = false;
		private int mEnemiesLeft = 0;
		#endregion

		#region monobehaviour
		private void Start()
		{
			collectSpawnPoints();
			//first time spawning player
			GameObject playerGO = (GameObject)Instantiate(mPlayerPrefab);
			mPlayer = playerGO.GetComponent<PlayerBehaviour>();
			SpawnPlayer();

			mWavesGOArray = new GameObject[waves.Length][];
			StartCoroutine(spawnWaves());
		}

		private void Update()
		{
			//player input
			float horizAxis = Input.GetAxis("Horizontal");
			float vertAxis = Input.GetAxis("Vertical");

			//remove acceleration, when the player inputs a direction
			//just go that way
			if(horizAxis < 0)
			{
				horizAxis = -1;
			}
			else if(horizAxis > 0)
			{
				horizAxis = 1;
			}

			if(vertAxis < 0)
			{
				vertAxis = -1;
			}
			else if(vertAxis > 0)
			{
				vertAxis = 1;
			}
			
			mPlayer.DoMovement(horizAxis, vertAxis, Input.GetButton("Fire3"), Input.GetButton("Jump"));
			
			if(Input.GetButtonDown("Fire1"))
			{
				mPlayer.Weapon.Fire(Input.GetButton("Fire2"));
			}
		}

		private void LateUpdate()
		{
			//wave spawning
			if(mEnemiesLeft <= 0 && mCurWave != waves.Length && !bIsSpawning)
			{
				StartCoroutine(activateWave(mCurWave++));
			}
		}
		#endregion

		#region spawn_point_management
		//Gets all children that have a SpawnPoint script
		//using that list, divide the spawn points into player and enemy spawn points
		//create the respective arrays and shuffle them.
		private void collectSpawnPoints()
		{
			List<Vector3> enemySpawns = new List<Vector3>();
			List<Vector3> playerSpawns = new List<Vector3>();

			SpawnPoint[] allSpawnPoints = GetComponentsInChildren<SpawnPoint>();
			if(allSpawnPoints.Length == 0)
			{
				Debug.LogError("No SpawnPoints defined as children of the PlaySceneManager");
				mPlayerSpawnPoints = new Vector3[0];
				mEnemySpawnPoints = new Vector3[0];
				return;
			}

			for(int i=0;i<allSpawnPoints.Length;++i)
			{
				SpawnPoint tmp = allSpawnPoints[i];
				if((tmp.SpawnType & SpawnPointType.Enemy) != 0)
				{
					enemySpawns.Add(tmp.gameObject.transform.position);
				}

				if((tmp.SpawnType & SpawnPointType.Player) != 0)
				{
					playerSpawns.Add(tmp.gameObject.transform.position);
				}
			}

			mEnemySpawnPoints = enemySpawns.ToArray();
			shuffleSpawnPoints(mEnemySpawnPoints);
			if(mEnemySpawnPoints.Length == 0)
			{
				Debug.LogError("No enemy spawn points found.");
			}

			mPlayerSpawnPoints = playerSpawns.ToArray();
			shuffleSpawnPoints(mPlayerSpawnPoints);
			if(mPlayerSpawnPoints.Length == 0)
			{
				Debug.LogError("No player spawn points found.");
			}
		}

		private void shuffleSpawnPoints(Vector3[] toShuffle)
		{
			for(int i=0;i<toShuffle.Length;++i)
			{
				int rndIdx = Random.Range(i, toShuffle.Length);
				Vector3 tmp = toShuffle[i];
				toShuffle[i] = toShuffle[rndIdx];
				toShuffle[rndIdx] = tmp;
			}
		}
		#endregion

		#region combatant_management
		public CombatantBehaviour GetCombatantById(int id)
		{
			if(mCombatantsById.ContainsKey(id))
			{
				return mCombatantsById[id];
			}
			return null;
		}

		public void RegisterCombatant(int id, CombatantBehaviour behaviour)
		{
			mCombatantsById[id] = behaviour;
		}

		public void SpawnPlayer()
		{
			Vector3 pos = mPlayerSpawnPoints[mCurPlayerSpawnIdx++];
			if(mCurPlayerSpawnIdx > mPlayerSpawnPoints.Length)
			{
				mCurPlayerSpawnIdx = 0;
				shuffleSpawnPoints(mPlayerSpawnPoints);
			}
			mPlayer.Spawn(pos);
		}
		#endregion

		#region wave_spawning
		private IEnumerator spawnWaves()
		{
			for(int i=0;i<waves.Length;++i)
			{
				yield return StartCoroutine(spawnWave(i));
				mWaveIdxLastSpawned++;
			}
		}
		
		private IEnumerator spawnWave(int waveIdx)
		{
			Transform parent = (new GameObject("Wave " + waveIdx)).transform;
			parent.position = Vector3.zero;
			mWavesGOArray[waveIdx] = new GameObject[waves[waveIdx].GetTotalEnemies()];
			mCurEnemySpawnIdx = 0;
			for(int i=0;i<waves[waveIdx].enemies.Length;++i)
			{
				for(int j=0;j<waves[waveIdx].enemies[i].number;++j)
				{
					mWavesGOArray[waveIdx][j+i] = (GameObject)GameObject.Instantiate(waves[waveIdx].enemies[i].enemyPrefab, mEnemySpawnPoints[mCurEnemySpawnIdx++], Quaternion.identity);
					mWavesGOArray[waveIdx][j+i].transform.parent = parent;
					mWavesGOArray[waveIdx][j+i].SetActive(false);
					
					// cannot spawn more enemies than spawn points
					if(mCurEnemySpawnIdx == mEnemySpawnPoints.Length-1)
					{
						i = waves[waveIdx].enemies.Length;
						break;
					}
					yield return null;
				}
				yield return null;
			}

			shuffleSpawnPoints(mEnemySpawnPoints);
			bIsSpawning = false;
		}
		
		private IEnumerator activateWave(int waveIdx)
		{
			while(mWaveIdxLastSpawned < waveIdx)
			{
				yield return null;
			}
			mEnemiesLeft = mWavesGOArray[waveIdx].Length;
			for(int i=0;i<mWavesGOArray[waveIdx].Length;++i)
			{
				mWavesGOArray[waveIdx][i].SetActive(true);
			}

		}
		#endregion

		public int EnemiesLeft
		{
			get { return mEnemiesLeft; }
			set { mEnemiesLeft = value; }
		}
	}
}