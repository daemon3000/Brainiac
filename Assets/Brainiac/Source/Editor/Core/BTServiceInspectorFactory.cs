using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class CustomServiceInspectorAttribute : Attribute
	{
		public readonly Type ServiceType;

		public CustomServiceInspectorAttribute(Type serviceType)
		{
			ServiceType = serviceType;
		}
	}

	public static class BTServiceInspectorFactory
	{
		private static Dictionary<Type, Type> m_serviceInspectors;

		static BTServiceInspectorFactory()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			m_serviceInspectors = new Dictionary<Type, Type>();
			foreach(Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ServiceInspector))))
			{
				object[] attributes = type.GetCustomAttributes(typeof(CustomServiceInspectorAttribute), false);
				if(attributes.Length > 0)
				{
					CustomServiceInspectorAttribute attribute = attributes[0] as CustomServiceInspectorAttribute;
					if(!m_serviceInspectors.ContainsKey(attribute.ServiceType))
					{
						m_serviceInspectors.Add(attribute.ServiceType, type);
					}
				}
			}
		}

		public static Type GetInspectorTypeForService(Type serviceType)
		{
			Type inspectorType = null;

			if(m_serviceInspectors.TryGetValue(serviceType, out inspectorType))
			{
				return inspectorType;
			}
			else
			{
				return typeof(ServiceInspector);
			}
		}

		public static ServiceInspector CreateInspectorForService(Service service)
		{
			if(service != null)
			{
				Type inspectorType = GetInspectorTypeForService(service.GetType());
				if(inspectorType != null)
				{
					ServiceInspector inspector = Activator.CreateInstance(inspectorType) as ServiceInspector;
					inspector.Target = service;

					return inspector;
				}
			}

			return null;
		}
	}
}