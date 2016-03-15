using UnityEngine;
using System;
using Newtonsoft.Json;

namespace Brainiac
{
	public static class BTUtils
	{
		public static bool IsSameOrSubclass(this Type potentialSubclass, Type potentialBase)
		{
			return potentialSubclass.IsSubclassOf(potentialBase) || (potentialSubclass == potentialBase);
		}

		public static bool IsNullableType(Type t)
		{
			return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		public static BehaviourNode CreateNode(Type nodeType)
		{
			if(nodeType.IsSubclassOf(typeof(BehaviourNode)) && !nodeType.IsAbstract)
			{
				return Activator.CreateInstance(nodeType) as BehaviourNode;
			}

			return null;
		}

		public static string Save(BehaviourTree behaviourTree)
		{
			try
			{
				if(behaviourTree == null)
					return "";

				JsonSerializerSettings settings = new JsonSerializerSettings();
				settings.TypeNameHandling = TypeNameHandling.Auto;
				settings.Converters.Add(new Vector2Converter());
				settings.Converters.Add(new Vector3Converter());

				string serializedData = JsonConvert.SerializeObject(behaviourTree, settings);
				return serializedData;
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
				return "";
			}
		}

		public static BehaviourTree Load(string btData)
		{
			try
			{
				if(string.IsNullOrEmpty(btData))
					return new BehaviourTree();

				JsonSerializerSettings settings = new JsonSerializerSettings();
				settings.TypeNameHandling = TypeNameHandling.Auto;
				settings.Converters.Add(new Vector2Converter());
				settings.Converters.Add(new Vector3Converter());

				return JsonConvert.DeserializeObject<BehaviourTree>(btData, settings);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
				return new BehaviourTree();
			}
		}
	}
}