using UnityEngine;
using System;
using System.Text;
using Brainiac.Serialization;

namespace Brainiac
{
	public static class BTUtils
	{
		private const string TYPE_HINT_NAME = "$type";

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

				StringBuilder builder = new StringBuilder();
				JsonWriterSettings settings = new JsonWriterSettings();
				settings.TypeHintName = TYPE_HINT_NAME;
				settings.TypeHintsOnlyWhenNeeded = true;

				using(JsonWriter writer = new JsonWriter(builder, settings))
				{
					writer.Write(behaviourTree);
				}

				return builder.ToString();
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

				JsonReaderSettings settings = new JsonReaderSettings();
				settings.TypeHintName = TYPE_HINT_NAME;

				JsonReader reader = new JsonReader(btData, settings);
				return reader.Deserialize(typeof(BehaviourTree)) as BehaviourTree;
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

				StringBuilder builder = new StringBuilder();
				JsonWriterSettings settings = new JsonWriterSettings();
				settings.TypeHintName = TYPE_HINT_NAME;
				settings.TypeHintsOnlyWhenNeeded = true;

				using(JsonWriter writer = new JsonWriter(builder, settings))
				{
					writer.Write(new object[] { behaviourNode });
				}

				return builder.ToString();
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

				JsonReaderSettings settings = new JsonReaderSettings();
				settings.TypeHintName = TYPE_HINT_NAME;

				JsonReader reader = new JsonReader(nodeData, settings);
				object[] obj = reader.Deserialize() as object[];
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

				JsonReaderSettings settings = new JsonReaderSettings();
				settings.TypeHintName = TYPE_HINT_NAME;

				JsonReader reader = new JsonReader(nodeData, settings);
				object[] obj = reader.Deserialize() as object[];
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