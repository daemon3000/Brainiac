using UnityEngine;
using System;
using System.Text;
using Brainiac.Serialization;

namespace Brainiac
{
	public static class BTUtils
	{
		private const string TYPE_HINT_NAME = "$type";
		private const int MAX_TREE_DEPTH = 1000;

		public static string GenerateUniqueStringID()
		{
			return Guid.NewGuid().ToString("N");
		}

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

		public static Service CreateService(Type serviceType)
		{
			if(serviceType != null && serviceType.IsSubclassOf(typeof(Service)) && !serviceType.IsAbstract)
			{
				return Activator.CreateInstance(serviceType) as Service;
			}

			return null;
		}

		public static Constraint CreateConstraint(Type constraintType)
		{
			if(constraintType != null && constraintType.IsSubclassOf(typeof(Constraint)) && !constraintType.IsAbstract)
			{
				return Activator.CreateInstance(constraintType) as Constraint;
			}

			return null;
		}

		public static string SerializeTree(BehaviourTree behaviourTree)
		{
			try
			{
				if(behaviourTree == null)
					return null;

				StringBuilder builder = new StringBuilder();
				JsonWriterSettings settings = new JsonWriterSettings();
				settings.TypeHintName = TYPE_HINT_NAME;
				settings.TypeHintsOnlyWhenNeeded = true;
				settings.MaxDepth = MAX_TREE_DEPTH;

				using(JsonWriter writer = new JsonWriter(builder, settings))
				{
					writer.Write(behaviourTree);
				}

				return builder.ToString();
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
				return null;
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
				return null;
			}
		}

		public static string SerializeNode(BehaviourNode behaviourNode)
		{
			try
			{
				if(behaviourNode == null)
					return null;

				StringBuilder builder = new StringBuilder();
				JsonWriterSettings settings = new JsonWriterSettings();
				settings.TypeHintName = TYPE_HINT_NAME;
				settings.TypeHintsOnlyWhenNeeded = true;
				settings.MaxDepth = MAX_TREE_DEPTH;

				using(JsonWriter writer = new JsonWriter(builder, settings))
				{
					writer.Write(new object[] { behaviourNode });
				}

				return builder.ToString();
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
				return null;
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

		public static T DeserializeNode<T>(string nodeData) where T : BehaviourNode
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