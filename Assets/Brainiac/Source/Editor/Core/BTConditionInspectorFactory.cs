using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class CustomConditionInspectorAttribute : Attribute
	{
		public readonly Type ConditionType;

		public CustomConditionInspectorAttribute(Type conditionType)
		{
			ConditionType = conditionType;
		}
	}

	public static class BTConditionInspectorFactory
	{
		private static Dictionary<Type, Type> m_conditionInspector;

		static BTConditionInspectorFactory()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			m_conditionInspector = new Dictionary<Type, Type>();
			foreach(Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ConditionInspector))))
			{
				object[] attributes = type.GetCustomAttributes(typeof(CustomConditionInspectorAttribute), false);
				if(attributes.Length > 0)
				{
					CustomConditionInspectorAttribute attribute = attributes[0] as CustomConditionInspectorAttribute;
					if(!m_conditionInspector.ContainsKey(attribute.ConditionType))
					{
						m_conditionInspector.Add(attribute.ConditionType, type);
					}
				}
			}
		}

		public static Type GetInspectorTypeForCondition(Type conditionType)
		{
			Type inspectorType = null;

			if(m_conditionInspector.TryGetValue(conditionType, out inspectorType))
			{
				return inspectorType;
			}
			else
			{
				return typeof(ConditionInspector);
			}
		}

		public static ConditionInspector CreateInspectorForCondition(Condition condition)
		{
			if(condition != null)
			{
				Type inspectorType = GetInspectorTypeForCondition(condition.GetType());
				if(inspectorType != null)
				{
					ConditionInspector inspector = Activator.CreateInstance(inspectorType) as ConditionInspector;
					inspector.Target = condition;

					return inspector;
				}
			}

			return null;
		}
	}
}