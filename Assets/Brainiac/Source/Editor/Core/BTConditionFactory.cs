using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	public static class BTConditionFactory
	{
		private static List<Tuple<Type, string>> m_conditionMenuPaths;

		static BTConditionFactory()
		{
			Type conditionType = typeof(Condition);
			Assembly assembly = conditionType.Assembly;

			m_conditionMenuPaths = new List<Tuple<Type, string>>();
			foreach(Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(conditionType)))
			{
				object[] attributes = type.GetCustomAttributes(typeof(AddConditionMenuAttribute), false);
				if(attributes.Length > 0)
				{
					AddConditionMenuAttribute attribute = attributes[0] as AddConditionMenuAttribute;
					m_conditionMenuPaths.Add(new Tuple<Type, string>(type, attribute.MenuPath));
				}
			}
		}

		public static void AddCondition(GenericMenu menu, BehaviourNode targetNode)
		{
			GenericMenu.MenuFunction2 onCreateCondition = t =>
			{
				Condition condition = BTUtils.CreateCondition(t as Type);
				if(condition != null)
				{
					targetNode.Conditions.Add(condition);
				}
			};

			foreach(var item in m_conditionMenuPaths)
			{
				menu.AddItem(new GUIContent("Add Condition/" + item.Item2), false, onCreateCondition, item.Item1);
			}
		}
	}
}
