using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace VideoWars.OtherBehaviours
{
	[CustomEditor(typeof(PlatformBehaviour))]
	public class PlatformBehaviourInspector : Editor
	{
		private List<GameObject> mMiddlePieces;
		private Transform mRightTrans;
		private int mDesired;
		
		private void OnEnable()
		{
			GameObject baseGO = null;
			mMiddlePieces = new List<GameObject>();
			baseGO = ((PlatformBehaviour)serializedObject.targetObject).gameObject;
			Transform bodyParent = baseGO.transform.Find("Body");
			mRightTrans = ((PlatformBehaviour)serializedObject.targetObject).gameObject.transform.Find("Body/RIGHT_END");
			for(int i=0; i<bodyParent.childCount; ++i)
			{
				if(bodyParent.GetChild(i).gameObject.name == "MIDDLE_PIECE")
				{
					mMiddlePieces.Add(bodyParent.GetChild(i).gameObject);
				}
			}
			
			mDesired = mMiddlePieces.Count;
		}
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.LabelField("Number of Middle Sections");
			mDesired = EditorGUILayout.IntSlider(mDesired, 1, 30);
			while(mDesired > mMiddlePieces.Count)
			{
				Transform lastTrans = mMiddlePieces[mMiddlePieces.Count - 1].transform;
				GameObject newGO = (GameObject)Instantiate(mMiddlePieces[0], Vector3.zero, Quaternion.identity);
				newGO.transform.parent = mMiddlePieces[0].transform.parent;
				newGO.transform.localPosition = lastTrans.localPosition + Vector3.right;
				newGO.transform.localRotation = lastTrans.localRotation;
				newGO.name = "MIDDLE_PIECE";
				mMiddlePieces.Add(newGO);
			}
			while(mDesired < mMiddlePieces.Count)
			{
				GameObject oldGO = mMiddlePieces[mMiddlePieces.Count - 1];
				mMiddlePieces.RemoveAt(mMiddlePieces.Count - 1);
				DestroyImmediate(oldGO);
			}
			mRightTrans.localPosition = mMiddlePieces[mMiddlePieces.Count - 1].transform.localPosition + Vector3.right;
			
			serializedObject.ApplyModifiedProperties();
			
		}
	}
}
