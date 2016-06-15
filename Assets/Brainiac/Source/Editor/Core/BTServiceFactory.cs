using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	public static class BTServiceFactory
	{
		private static List<Tuple<Type, string>> m_serviceMenuPaths;

		static BTServiceFactory()
		{
			Type serviceType = typeof(Service);
			Assembly assembly = serviceType.Assembly;

			m_serviceMenuPaths = new List<Tuple<Type, string>>();
			foreach(Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(serviceType)))
			{
				object[] attributes = type.GetCustomAttributes(typeof(AddServiceMenuAttribute), false);
				if(attributes.Length > 0)
				{
					AddServiceMenuAttribute attribute = attributes[0] as AddServiceMenuAttribute;
					m_serviceMenuPaths.Add(new Tuple<Type, string>(type, attribute.MenuPath));
				}
			}
		}

		public static void AddService(GenericMenu menu, BehaviourNode targetNode)
		{
			GenericMenu.MenuFunction2 onCreateService = t =>
			{
				Service service = BTUtils.CreateService(t as Type);
				if(service != null)
				{
					targetNode.Services.Add(service);
				}
			};

			foreach(var item in m_serviceMenuPaths)
			{
				menu.AddItem(new GUIContent("Add Service/" + item.Item2), false, onCreateService, item.Item1);
			}
		}
	}
}