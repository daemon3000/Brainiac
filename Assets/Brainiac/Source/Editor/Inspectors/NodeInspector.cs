using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using Brainiac;
using Brainiac.Serialization;

namespace BrainiacEditor
{
	public class NodeInspector
	{
		private BehaviourNode m_target;

		protected BehaviourNode Target
		{
			get { return m_target; }
		}

		public virtual void SetTarget(BehaviourNode target)
		{
			m_target = target;
		}

		public virtual void OnInspectorGUI()
		{
			if(m_target != null)
			{
				DrawHeader();
				DrawProperties();
				RepaintCanvas();
			}
		}

		protected void DrawHeader()
		{
			EditorGUILayout.LabelField(m_target.Title, EditorStyles.boldLabel);
			m_target.Name = EditorGUILayout.TextField("Name", m_target.Name);
			EditorGUILayout.LabelField("Comment");
			m_target.Comment = EditorGUILayout.TextArea(m_target.Comment, BTEditorStyle.MultilineTextArea);

			EditorGUILayout.Space();
			m_target.Weight = EditorGUILayout.Slider("Random Weight", m_target.Weight, 0.0f, 1.0f);

			EditorGUILayout.Space();
		}

		protected void DrawProperties()
		{
			Type nodeType = m_target.GetType();
			var fields = from fi in nodeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
						 select fi;
			var properties = from pi in nodeType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
							 select pi;

			foreach(var field in fields)
			{
				BTPropertyAttribute propertyAttribute = Attribute.GetCustomAttribute(field, typeof(BTPropertyAttribute)) as BTPropertyAttribute;
				BTIgnoreAttribute ignoreAttribute = Attribute.GetCustomAttribute(field, typeof(BTIgnoreAttribute)) as BTIgnoreAttribute;
				BTHideInInspectorAttribute hideAttribute = Attribute.GetCustomAttribute(field, typeof(BTHideInInspectorAttribute)) as BTHideInInspectorAttribute;

				if(ignoreAttribute != null || hideAttribute != null || (propertyAttribute == null && field.IsPrivate))
					continue;

				string label = field.Name;
				if(propertyAttribute != null && !string.IsNullOrEmpty(propertyAttribute.PropertyName))
				{
					label = propertyAttribute.PropertyName;
				}

				if(field.FieldType == typeof(MemoryVar))
				{
					DrawMemoryVarField(label, (MemoryVar)field.GetValue(m_target));
				}
				else
				{
					object value = null;
					if(TryToDrawField(label, field.GetValue(m_target), field.FieldType, out value))
					{
						field.SetValue(m_target, value);
					}	
				}
			}
			foreach(var property in properties)
			{
				BTPropertyAttribute propertyAttribute = Attribute.GetCustomAttribute(property, typeof(BTPropertyAttribute)) as BTPropertyAttribute;
				BTIgnoreAttribute ignoreAttribute = Attribute.GetCustomAttribute(property, typeof(BTIgnoreAttribute)) as BTIgnoreAttribute;
				BTHideInInspectorAttribute hideAttribute = Attribute.GetCustomAttribute(property, typeof(BTHideInInspectorAttribute)) as BTHideInInspectorAttribute;
				var setterMethod = property.GetSetMethod(true);

				if(setterMethod == null || ignoreAttribute != null || hideAttribute != null || (propertyAttribute == null && setterMethod.IsPrivate))
					continue;

				string label = property.Name;
				if(propertyAttribute != null && !string.IsNullOrEmpty(propertyAttribute.PropertyName))
				{
					label = propertyAttribute.PropertyName;
				}

				if(property.PropertyType == typeof(MemoryVar))
				{
					DrawMemoryVarField(label, (MemoryVar)property.GetValue(m_target, null));
				}
				else
				{
					object value = null;
					if(TryToDrawField(label, property.GetValue(m_target, null), property.PropertyType, out value))
					{
						property.SetValue(m_target, value, null);
					}
				}
			}
		}

		private bool TryToDrawField(string label, object currentValue, Type type, out object value)
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
			if(memVar != null)
			{
				memVar.Content = EditorGUILayout.TextField(label, memVar.Content);
			}
		}

		protected void RepaintCanvas()
		{
			if(BTEditorCanvas.Current != null)
			{
				BTEditorCanvas.Current.Repaint();
			}
		}
	}
}