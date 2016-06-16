using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class CustomConstraintInspectorAttribute : Attribute
	{
		public readonly Type ConstraintType;

		public CustomConstraintInspectorAttribute(Type constraintType)
		{
			ConstraintType = constraintType;
		}
	}

	public static class BTConstraintInspectorFactory
	{
		private static Dictionary<Type, Type> m_constraintInspectors;

		static BTConstraintInspectorFactory()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			m_constraintInspectors = new Dictionary<Type, Type>();
			foreach(Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ConstraintInspector))))
			{
				object[] attributes = type.GetCustomAttributes(typeof(CustomConstraintInspectorAttribute), false);
				if(attributes.Length > 0)
				{
					CustomConstraintInspectorAttribute attribute = attributes[0] as CustomConstraintInspectorAttribute;
					if(!m_constraintInspectors.ContainsKey(attribute.ConstraintType))
					{
						m_constraintInspectors.Add(attribute.ConstraintType, type);
					}
				}
			}
		}

		public static Type GetInspectorTypeForConstraint(Type constraintType)
		{
			Type inspectorType = null;

			if(m_constraintInspectors.TryGetValue(constraintType, out inspectorType))
			{
				return inspectorType;
			}
			else
			{
				return typeof(ConstraintInspector);
			}
		}

		public static ConstraintInspector CreateInspectorForConstraint(Constraint constraint)
		{
			if(constraint != null)
			{
				Type inspectorType = GetInspectorTypeForConstraint(constraint.GetType());
				if(inspectorType != null)
				{
					ConstraintInspector inspector = Activator.CreateInstance(inspectorType) as ConstraintInspector;
					inspector.Target = constraint;

					return inspector;
				}
			}

			return null;
		}
	}
}