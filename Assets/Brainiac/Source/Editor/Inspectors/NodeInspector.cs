using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Brainiac;
using Brainiac.Serialization;

namespace BrainiacEditor
{
	public class NodeInspector
	{
		private BehaviourNode m_target;
		private BTEditorGraphNode m_graphNode;
		private Dictionary<Type, ConstraintInspector> m_constraintInspectors;
		private Dictionary<Type, ServiceInspector> m_serviceInspectors;

		public BehaviourNode Target
		{
			get { return m_target; }
			set { m_target = value; }
		}

		public BTEditorGraphNode GraphNode
		{
			get { return m_graphNode; }
			set { m_graphNode = value; }
		}

		public NodeInspector()
		{
			m_constraintInspectors = new Dictionary<Type, ConstraintInspector>();
			m_constraintInspectors.Add(typeof(ConstraintInspector), new ConstraintInspector());

			m_serviceInspectors = new Dictionary<Type, ServiceInspector>();
			m_serviceInspectors.Add(typeof(ServiceInspector), new ServiceInspector());
		}

		public void OnInspectorGUI()
		{
			if(m_target != null)
			{
				DrawHeader();
				DrawProperties();
				DrawConstraintsAndServices();
				RepaintCanvas();
			}
		}

		private void DrawHeader()
		{
			Rect titlePosition = GUILayoutUtility.GetRect(GUIContent.none, BTEditorStyle.RegionBackground, GUILayout.ExpandWidth(true), GUILayout.Height(15.0f));
			titlePosition.x -= 19;
			titlePosition.y -= 2;
			titlePosition.width += 28;

			Rect optionsButtonPosition = new Rect(titlePosition.xMax - 28.0f, titlePosition.y, 20.0f, 20.0f);

			DrawCategoryHeader(titlePosition, m_target.Title);
			if(GUI.Button(optionsButtonPosition, BTEditorStyle.OptionsIcon, EditorStyles.label))
			{
				GenericMenu menu = BTContextMenuFactory.CreateNodeInspectorContextMenu(m_target);
				menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
			}
			
			GUILayout.Space(2.5f);
			DrawSeparator();

			m_target.Name = EditorGUILayout.TextField("Name", m_target.Name);
			EditorGUILayout.LabelField("Comment");
			m_target.Comment = EditorGUILayout.TextArea(m_target.Comment, BTEditorStyle.MultilineTextArea);

			if(m_graphNode.Parent != null && m_graphNode.Parent.Node is WeightedRandom)
			{
				EditorGUILayout.Space();
				m_target.Weight = EditorGUILayout.Slider("Weight", m_target.Weight, 0.0f, 1.0f);
			}

			EditorGUILayout.Space();
		}

		protected virtual void DrawProperties()
		{
			DrawDefaultProperties();
		}

		protected void DrawDefaultProperties()
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
				string label = BTEditorUtils.MakePrettyName(field.Name);

				if(ignoreAttribute != null || hideAttribute != null || (propertyAttribute == null && field.IsPrivate))
					continue;
				
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
				string label = BTEditorUtils.MakePrettyName(property.Name);

				if(setterMethod == null || ignoreAttribute != null || hideAttribute != null || (propertyAttribute == null && setterMethod.IsPrivate))
					continue;

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

		protected bool TryToDrawField(string label, object currentValue, Type type, out object value)
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
			else if(type == typeof(LayerMask))
			{
				LayerMask lm = (LayerMask)currentValue;
				lm.value = EditorGUILayout.MaskField(label, lm.value, UnityEditorInternal.InternalEditorUtility.layers);

				value = lm;
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

		protected void DrawMemoryVarField(string label, MemoryVar memVar)
		{
			if(memVar != null)
			{
				memVar.Value = EditorGUILayout.TextField(label, memVar.Value);
			}
		}

		private void DrawConstraintsAndServices()
		{
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			if(m_target.Constraints.Count > 0)
			{
				EditorGUILayout.Space();
				DrawCategoryHeader("Constraints");
				DrawConstraints();
			}

			if(m_target.Services.Count > 0)
			{
				EditorGUILayout.Space();
				DrawCategoryHeader("Services");
				DrawServices();
			}
		}

		private void DrawConstraints()
		{
			EditorGUILayout.Space();
			DrawSeparator();

			for(int i = 0; i < m_target.Constraints.Count; i++)
			{
				Constraint constraint = m_target.Constraints[i];
				Rect headerPos = GUILayoutUtility.GetRect(0, 18.0f, GUILayout.ExpandWidth(true));
				Rect foldoutPos = new Rect(headerPos.x, headerPos.y, 20.0f, headerPos.height);
				Rect labelPos = new Rect(foldoutPos.xMax, headerPos.y, headerPos.width - 56.0f, headerPos.height);
				Rect togglePos = new Rect(labelPos.xMax, headerPos.y, 18.0f, headerPos.height);
				Rect optionsButtonPos = new Rect(togglePos.xMax, headerPos.y, 18.0f, headerPos.height);

				constraint.IsExpanded = EditorGUI.Foldout(foldoutPos, constraint.IsExpanded, GUIContent.none);
				EditorGUI.LabelField(labelPos, constraint.Title, BTEditorStyle.BoldLabel);
				constraint.InvertResult = EditorGUI.Toggle(togglePos, GUIContent.none, constraint.InvertResult);
				if(GUI.Button(optionsButtonPos, BTEditorStyle.OptionsIcon, EditorStyles.label))
				{
					GenericMenu menu = BTContextMenuFactory.CreateConstraintContextMenu(m_target, i);
					menu.DropDown(new Rect(BTEditorCanvas.Current.Event.mousePosition, Vector2.zero));
				}

				if(constraint.IsExpanded)
				{
					var constraintInspector = GetConstraintInspector(constraint);
					constraintInspector.OnInspectorGUI();
				}

				DrawSeparator();
			}
		}

		private void DrawServices()
		{
			EditorGUILayout.Space();
			DrawSeparator();

			for(int i = 0; i < m_target.Services.Count; i++)
			{
				Service service = m_target.Services[i];
				Rect headerPos = GUILayoutUtility.GetRect(0, 18.0f, GUILayout.ExpandWidth(true));
				Rect foldoutPos = new Rect(headerPos.x, headerPos.y, 20.0f, headerPos.height);
				Rect labelPos = new Rect(foldoutPos.xMax, headerPos.y, headerPos.width - 38.0f, headerPos.height);
				Rect optionsButtonPos = new Rect(labelPos.xMax, headerPos.y, 18.0f, headerPos.height);

				service.IsExpanded = EditorGUI.Foldout(foldoutPos, service.IsExpanded, GUIContent.none);
				EditorGUI.LabelField(labelPos, service.Title, BTEditorStyle.BoldLabel);
				if(GUI.Button(optionsButtonPos, BTEditorStyle.OptionsIcon, EditorStyles.label))
				{
					GenericMenu menu = BTContextMenuFactory.CreateServiceContextMenu(m_target, i);
					menu.DropDown(new Rect(BTEditorCanvas.Current.Event.mousePosition, Vector2.zero));
				}

				if(service.IsExpanded)
				{
					var serviceInspector = GetServiceInspector(service);
					serviceInspector.OnInspectorGUI();
				}

				DrawSeparator();
			}
		}

		private void DrawCategoryHeader(string label)
		{
			Rect position = GUILayoutUtility.GetRect(GUIContent.none, BTEditorStyle.RegionBackground, GUILayout.ExpandWidth(true), GUILayout.Height(15.0f));
			position.x -= 19;
			position.width += 28;

			EditorGUI.LabelField(position, label, BTEditorStyle.RegionBackground);
		}

		private void DrawCategoryHeader(Rect position, string label)
		{
			EditorGUI.LabelField(position, label, BTEditorStyle.RegionBackground);
		}

		private void DrawSeparator()
		{
			Rect position = GUILayoutUtility.GetRect(GUIContent.none, BTEditorStyle.SeparatorStyle, GUILayout.Height(5.0f));
			position.x -= 16;
			position.width += 18;

			EditorGUI.LabelField(position, "", BTEditorStyle.SeparatorStyle);
		}

		private ConstraintInspector GetConstraintInspector(Constraint constraint)
		{
			ConstraintInspector inspector = null;
			if(!m_constraintInspectors.TryGetValue(BTConstraintInspectorFactory.GetInspectorTypeForConstraint(constraint.GetType()), out inspector))
			{
				inspector = BTConstraintInspectorFactory.CreateInspectorForConstraint(constraint);
				m_constraintInspectors.Add(inspector.GetType(), inspector);
			}

			inspector.Target = constraint;
			return inspector;
		}

		private ServiceInspector GetServiceInspector(Service service)
		{
			ServiceInspector inspector = null;
			if(!m_serviceInspectors.TryGetValue(BTServiceInspectorFactory.GetInspectorTypeForService(service.GetType()), out inspector))
			{
				inspector = BTServiceInspectorFactory.CreateInspectorForService(service);
				m_serviceInspectors.Add(inspector.GetType(), inspector);
			}

			inspector.Target = service;
			return inspector;
		}

		private void RepaintCanvas()
		{
			if(BTEditorCanvas.Current != null)
			{
				BTEditorCanvas.Current.Repaint();
			}
		}
	}
}