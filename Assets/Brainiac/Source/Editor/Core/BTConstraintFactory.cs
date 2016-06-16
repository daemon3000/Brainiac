using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	public static class BTConstraintFactory
	{
		private static List<Tuple<Type, string>> m_constraintMenuPaths;

		static BTConstraintFactory()
		{
			Type constraintType = typeof(Constraint);
			Assembly assembly = constraintType.Assembly;

			m_constraintMenuPaths = new List<Tuple<Type, string>>();
			foreach(Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(constraintType)))
			{
				object[] attributes = type.GetCustomAttributes(typeof(AddConstraintMenuAttribute), false);
				if(attributes.Length > 0)
				{
					AddConstraintMenuAttribute attribute = attributes[0] as AddConstraintMenuAttribute;
					m_constraintMenuPaths.Add(new Tuple<Type, string>(type, attribute.MenuPath));
				}
			}
		}

		public static void AddConstraint(GenericMenu menu, BehaviourNode targetNode)
		{
			GenericMenu.MenuFunction2 onCreateConstraint = t =>
			{
				Constraint constraint = BTUtils.CreateConstraint(t as Type);
				if(constraint != null)
				{
					targetNode.Constraints.Add(constraint);
				}
			};

			foreach(var item in m_constraintMenuPaths)
			{
				menu.AddItem(new GUIContent("Add Constraint/" + item.Item2), false, onCreateConstraint, item.Item1);
			}
		}
	}
}
