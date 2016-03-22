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
			if(nodeType != null && nodeType.IsSubclassOf(typeof(BehaviourNode)) && !nodeType.IsAbstract)
			{
				return Activator.CreateInstance(nodeType) as BehaviourNode;
			}

			return null;
		}

		public static string SerializeTree(BehaviourTree behaviourTree)
		{
			try
			{
				if(behaviourTree == null)
					return "";

				JsonSerializerSettings settings = new JsonSerializerSettings();
				settings.TypeNameHandling = TypeNameHandling.Auto;
				settings.Converters.Add(new Vector2Converter());
				settings.Converters.Add(new Vector3Converter());

				return JsonConvert.SerializeObject(behaviourTree, settings);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
				return "";
			}
		}

		public static BehaviourTree DeserializeTree(string btData)
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

		public static string SerializeNode(BehaviourNode behaviourNode)
		{
			try
			{
				if(behaviourNode == null)
					return "";

				JsonSerializerSettings settings = new JsonSerializerSettings();
				settings.TypeNameHandling = TypeNameHandling.Objects;
				settings.Converters.Add(new Vector2Converter());
				settings.Converters.Add(new Vector3Converter());

				return JsonConvert.SerializeObject(new object[] { behaviourNode }, settings);
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
				return "";
			}
		}

		public static BehaviourNode DeserializeNode(string nodeData)
		{
			try
			{
				if(string.IsNullOrEmpty(nodeData))
					return null;

				JsonSerializerSettings settings = new JsonSerializerSettings();
				settings.TypeNameHandling = TypeNameHandling.Objects;
				settings.Converters.Add(new Vector2Converter());
				settings.Converters.Add(new Vector3Converter());

				object[] obj = JsonConvert.DeserializeObject<object[]>(nodeData, settings);
				return (obj != null && obj.Length > 0) ? obj[0] as BehaviourNode : null;
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
				return null;
			}
		}

		public static T LoadNode<T>(string nodeData) where T : BehaviourNode
		{
			try
			{
				if(string.IsNullOrEmpty(nodeData))
					return null;

				JsonSerializerSettings settings = new JsonSerializerSettings();
				settings.TypeNameHandling = TypeNameHandling.Objects;
				settings.Converters.Add(new Vector2Converter());
				settings.Converters.Add(new Vector3Converter());

				object[] obj = JsonConvert.DeserializeObject<object[]>(nodeData, settings);
				return (obj != null && obj.Length > 0) ? obj[0] as T : null;
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
				return null;
			}
		}
	}
}