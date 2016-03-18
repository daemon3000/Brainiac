using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using Brainiac;

namespace BrainiacEditor
{
	public class GenericNodeInspector : INodeInspector
	{
		public virtual void OnInspectorGUI(BehaviourNode node)
		{
			node.Name = EditorGUILayout.TextField("Name", node.Name);
			EditorGUILayout.LabelField("Description");
			node.Description = EditorGUILayout.TextArea(node.Description, BTEditorStyle.MultilineTextArea);

			EditorGUILayout.Space();
			DrawProperties(node);
		}

		protected void DrawProperties(BehaviourNode node)
		{
			Type nodeType = node.GetType();
			var fields = from fi in nodeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
						 where fi.FieldType.IsValueType || fi.FieldType == typeof(MemoryVar)
						 select fi;
			var properties = from pi in nodeType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
							 where pi.PropertyType.IsValueType || pi.PropertyType == typeof(MemoryVar)
			select pi;

			foreach(var field in fields)
			{
				object[] attributes = field.GetCustomAttributes(typeof(BTPropertyAttribute), true);
				if(attributes.Length == 0)
					continue;

				BTPropertyAttribute attribute = attributes[0] as BTPropertyAttribute;
				string label = string.IsNullOrEmpty(attribute.PropertyName) ? field.Name : attribute.PropertyName;

				if(field.FieldType.IsValueType)
				{
					object value = null;
					if(TryToDrawValueTypeField(label, field.GetValue(node), field.FieldType, out value))
					{
						field.SetValue(node, value);
					}	
				}
				else
				{
					DrawMemoryVarField(label, (MemoryVar)field.GetValue(node));
				}
			}
			foreach(var property in properties)
			{
				object[] attributes = property.GetCustomAttributes(typeof(BTPropertyAttribute), true);
				if(attributes.Length == 0)
					continue;

				BTPropertyAttribute attribute = attributes[0] as BTPropertyAttribute;
				string label = string.IsNullOrEmpty(attribute.PropertyName) ? property.Name : attribute.PropertyName;

				if(property.PropertyType.IsValueType)
				{
					object value = null;
					if(TryToDrawValueTypeField(label, property.GetValue(node, null), property.PropertyType, out value))
					{
						property.SetValue(node, value, null);
					}
				}
				else
				{
					DrawMemoryVarField(label, (MemoryVar)property.GetValue(node, null));
				}
			}
		}

		private bool TryToDrawValueTypeField(string label, object currentValue, Type type, out object value)
		{
			bool success = true;

			if(type == typeof(bool))
			{
				value = EditorGUILayout.Toggle(label, (bool)currentValue);
			}
			else if(type == typeof(int))
			{
				value = EditorGUILayout.IntField(label, (int)currentValue);
			}
			else if(type == typeof(float))
			{
				value = EditorGUILayout.FloatField(label, (float)currentValue);
			}
			else if(type == typeof(string))
			{
				value = EditorGUILayout.TextField(label, (string)currentValue);
			}
			else if(type == typeof(Vector2))
			{
				value = EditorGUILayout.Vector2Field(label, (Vector2)currentValue);
			}
			else if(type == typeof(Vector3))
			{
				value = EditorGUILayout.Vector3Field(label, (Vector3)currentValue);
			}
			else if(type.IsEnum)
			{
				value = EditorGUILayout.EnumPopup(label, (Enum)currentValue);
			}
			else
			{
				value = null;
				success = false;
			}

			return success;
		}

		private void DrawMemoryVarField(string label, MemoryVar memVar)
		{
			memVar.Content = EditorGUILayout.TextField(label, memVar.Content);
		}
	}
}