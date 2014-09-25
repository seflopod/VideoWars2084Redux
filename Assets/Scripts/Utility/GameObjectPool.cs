using UnityEngine;
using System.Collections;

namespace VideoWars.Utility
{
	public class GameObjectPool
	{
		/// <summary>
		/// Factory for creating new pools.
		/// </summary>
		/// <returns>The new pool.</returns>
		/// <param name="prefab">Prefab to use for the pool.</param>
		/// <param name="size">The pool size.</param>
		public static GameObjectPool CreateNewPool(GameObject prefab, int size)
		{
			GameObjectPool ret = new GameObjectPool(size);
			GameObject poolObj = new GameObject("Pool - " + prefab.name);
			poolObj.transform.position = Vector3.zero;

			//I don't have a good workaround for this yet
			//I'm thinking some sort of "CoroutineHost" class,
			//but I'm not sure how to go about doing that.
			VideoWars.Management.GameManager.Instance.StartCoroutineDelegate(initPool, new object[3] { ret, prefab, poolObj.transform });
			return ret;
		}

		/// <summary>
		/// Initializes the pool in a coroutine.
		/// </summary>
		/// <returns>An IEnumerator indicating if there's more to init.</returns>
		/// <param name="paramArray">An array of objects used to pass parameters.  The second element is the GameObjectPool
		/// to use.  The third element should be the prefab to spawn.</param>
		private static IEnumerator initPool(object[] paramArray)
		{
			GameObjectPool pool = (GameObjectPool)paramArray[1];
			GameObject prefab = (GameObject)paramArray[2];
			Transform parent = (Transform)paramArray[3];

			for(int i=0;i<pool.mSize;++i)
			{
				GameObject go = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
				go.transform.parent = parent;

				//turn off the usual things that would make the object meaningful in the scene.  These are the renderer
				//(visibility) and the colliders for either physics system (affect objects in scene).  It might be better
				//to just disable the GameObject, but I seem to recall that causing problems...
				if(go.renderer != null)
				{
					go.renderer.enabled = false;
				}
				if(go.collider != null)
				{
					go.collider.enabled = false;
				}
				if(go.collider2D != null)
				{
					go.collider2D.enabled = false;
				}
				go.SetActive(false);
				//add object to the pool
				pool.aPool[i] = go;
				yield return null;
			}
		}

		#region private_fields
		private int mSize;
		private GameObject[] aPool;
		private int mCurIdx = 0;
		#endregion

		private GameObjectPool(int size)
		{
			mSize = size;
			aPool = new GameObject[mSize];
		}

		public GameObject GetGameObject()
		{
			GameObject ret = aPool[mCurIdx++];
			if(mCurIdx >= mSize)
			{
				mCurIdx = 0;
			}
			return ret;
		}

		public int PoolSize
		{
			get { return mSize; }
		}
	}
}