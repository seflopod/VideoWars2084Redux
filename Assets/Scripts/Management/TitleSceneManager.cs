using UnityEngine;
using System.Collections;
using VideoWars.Utility.Attributes;
using VideoWars.Utility;

namespace VideoWars.Management
{
	public class TitleSceneManager : Singleton<TitleSceneManager>
	{
		#region serialized_fields
		[SerializeField]
		[PrivateSerialized("Title Background")]
		private Texture mTitleBG;
		#endregion

		#region other_fields
		private Rect mTextDisplayRect;
		private float mBlinkAccum = 0f;
		private float mBlinkTime = 0.5f;
		private bool bIsTextVisible = true;
		#endregion

		#region monobehaviour
		private void Start()
		{
			mTextDisplayRect = new Rect(Screen.width / 3f, 2*Screen.height/3f, Screen.width / 3f, Screen.height / 9f);
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				GameManager.Instance.LoadState(GameState.PlayGame);
			}
			else
			{
				mBlinkAccum += Time.deltaTime;
				if(mBlinkAccum >= mBlinkTime)
				{
					mBlinkAccum = 0f;
					bIsTextVisible = !bIsTextVisible;
				}
			}
		}

		private void OnGUI()
		{
			if(mTitleBG != null)
			{
				GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), mTitleBG, ScaleMode.ScaleToFit, true, 2f);
			}
			if(bIsTextVisible)
			{
				GUI.Box(mTextDisplayRect, "Press [Enter] to start.");
			}
		}
		#endregion
	}
}
