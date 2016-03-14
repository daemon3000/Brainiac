using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using Brainiac;

namespace BrainiacEditor
{
	[CustomEditor(typeof(BTAsset))]
	public class BTAssetEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if(GUILayout.Button("Open In Editor", GUILayout.Height(24.0f)))
			{
				BehaviourTreeEditor.Open(target as BTAsset);
			}
		}
	}
}