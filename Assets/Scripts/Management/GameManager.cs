using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VideoWars.Utility;
using VideoWars.Utility.Attributes;

namespace VideoWars.Management
{
	/** GameState flowchart
	 * 
	 *  +--->TitleScreen
	 *  |        |scene change
	 *  |When    |
	 *  |Game    |
	 *  |Over   \ /
	 *  +---->PlayGame
	 * / \       |scene change
	 *  |        |
	 *  |       \ /
	 *  |    ShowScores
	 *  |        |scene change
	 *  |        |
	 *  +--------+
	 *
	 */
	public enum GameState
	{
		TitleScreen,
		PlayGame,
		ShowScores
	};

	public class GameManager : Singleton<GameManager>
	{
#if WEB_PLAYER
		const bool IS_WEB = true;
#else
		const bool IS_WEB = false;
#endif

		public delegate IEnumerator CoroutineMethod(object[] paramArray);

		#region serialized_fields
		#endregion

		#region other_fields
		private GameState mState;
		#endregion

		#region monobehaviour
		protected new void Awake()
		{
			base.Awake();
			DontDestroyOnLoad(gameObject);
		}
		#endregion

		#region scene_loading
		public void LoadState(GameState state)
		{
			string lvlName = "";
			switch(state)
			{
			case GameState.PlayGame:
				lvlName = "play";
				break;
			case GameState.TitleScreen:
				lvlName = "title";
				break;
			case GameState.ShowScores:
				lvlName = "scores";
				break;
			default:
				lvlName = "title";
				break;
			}
			loadLevel(lvlName);
			mState = state;
		}

		// I think that by using the while loops here I can suspend the update
		// to avoid null reference exceptions and bad timing.  I've never needed
		// a trick like this, so I might be overthinking things, but I want to
		// give it a shot.
		// Originally this was made for considering a web player.  Since I'm
		// building in 4.6.0b17, that doesn't seem necessary.  Even the 
		// built-in variable for web player seems non-existant.
		private void loadLevel(string levelName)
		{
#pragma warning disable 429
			// wait until we can load to try loading
			while(IS_WEB && !Application.CanStreamedLevelBeLoaded(levelName));
#pragma warning restore 429

			//only try loading the once
			if(!Application.isLoadingLevel)
			{
				Application.LoadLevel(levelName);
			}
		}
		#endregion

		#region coroutine_hosting
		IEnumerator RunCoroutine(object[] paramArray)
		{
			return ((CoroutineMethod)paramArray[0])(paramArray);
		}
		
		public void StartCoroutineDelegate(CoroutineMethod coroutineMethod, object[] extraParams)
		{
			object[] paramArray = new object[1+extraParams.Length];
			paramArray[0] = coroutineMethod;
			System.Array.Copy(extraParams, 0, paramArray, 1, extraParams.Length);
			StartCoroutine("RunCoroutine", paramArray);
		}
		#endregion

		#region properties
		public GameState State
		{
			get { return mState; }
		}
		#endregion
	}
}