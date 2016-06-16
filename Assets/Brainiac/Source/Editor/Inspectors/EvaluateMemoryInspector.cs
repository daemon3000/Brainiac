﻿using UnityEditor;
using Brainiac;

namespace BrainiacEditor
{
	[CustomConditionInspector(typeof(EvaluateMemory))]
	public class EvaluateMemoryInspector : ConditionInspector
	{
		public override void OnInspectorGUI()
		{
			if(Target != null && Target is EvaluateMemory)
			{
				EvaluateMemory target = (EvaluateMemory)Target;
				ConditionValueType oldValueType = target.ValueType;

				target.ValueType = (ConditionValueType)EditorGUILayout.EnumPopup("Value Type", target.ValueType);
				if(target.ValueType != oldValueType)
				{
					target.FirstValue.Value = "";
					target.SecondValue.Value = "";
				}

				switch(target.ValueType)
				{
				case ConditionValueType.Boolean:
					target.FirstValue.Value = EditorGUILayout.TextField("Value", target.FirstValue.Value);
					target.BooleanComparison = (BooleanComparison)EditorGUILayout.EnumPopup("Condition", target.BooleanComparison);
					break;
				case ConditionValueType.Integer:
					target.FirstValue.Value = EditorGUILayout.TextField("Value One", target.FirstValue.Value);
					target.SecondValue.Value = EditorGUILayout.TextField("Value Two", target.SecondValue.Value);
					target.NumericComparison = (NumericComparison)EditorGUILayout.EnumPopup("Condition", target.NumericComparison);
					break;
				case ConditionValueType.Float:
					target.FirstValue.Value = EditorGUILayout.TextField("Value One", target.FirstValue.Value);
					target.SecondValue.Value = EditorGUILayout.TextField("Value Two", target.SecondValue.Value);
					target.NumericComparison = (NumericComparison)EditorGUILayout.EnumPopup("Condition", target.NumericComparison);
					break;
				case ConditionValueType.GameObject:
					target.FirstValue.Value = EditorGUILayout.TextField("Value", target.FirstValue.Value);
					target.ReferenceComparison = (ReferenceComparison)EditorGUILayout.EnumPopup("Condition", target.ReferenceComparison);
					break;
				case ConditionValueType.UnityObject:
					target.FirstValue.Value = EditorGUILayout.TextField("Value", target.FirstValue.Value);
					target.ReferenceComparison = (ReferenceComparison)EditorGUILayout.EnumPopup("Condition", target.ReferenceComparison);
					break;
				}
			}
		}
	}
}