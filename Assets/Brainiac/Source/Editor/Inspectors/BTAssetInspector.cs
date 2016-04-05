using UnityEngine;
using UnityEditor;
using Brainiac;

namespace BrainiacEditor
{
	[CustomEditor(typeof(BTAsset))]
	public class BTAssetInspector : Editor
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