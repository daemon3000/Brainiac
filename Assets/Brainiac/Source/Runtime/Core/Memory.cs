using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Brainiac
{
	public class Memory : MonoBehaviour 
	{
		[SerializeField]
		private MemoryItem[] m_startMemory;

		private Dictionary<string, MemoryItem> m_memoryLookup;

		private void Awake()
		{
			m_memoryLookup = new Dictionary<string, MemoryItem>();
			for(int i = 0; i < m_startMemory.Length; i++)
			{
				if(!m_memoryLookup.ContainsKey(m_startMemory[i].Name))
					m_memoryLookup.Add(m_startMemory[i].Name, m_startMemory[i]);
			}
		}

		public void SetItem(string name, object item)
		{
			MemoryItem.ItemType? itemType = GetValueType(item);
			if(!itemType.HasValue)
				return;

			MemoryItem memItem = null;
			if(m_memoryLookup.TryGetValue(name, out memItem))
			{
				memItem.SetValue(item, itemType.Value, false);
			}
			else
			{
				memItem = new MemoryItem();
				memItem.SetValue(item, itemType.Value, true);
				m_memoryLookup.Add(name, memItem);
			}
		}

		public object GetItem(string name)
		{
			MemoryItem memItem = null;
			if(m_memoryLookup.TryGetValue(name, out memItem))
				return memItem.GetValue();
			
			return null;
		}

		private object GetItem(string name, MemoryItem.ItemType type)
		{
			MemoryItem memItem = null;
			if(m_memoryLookup.TryGetValue(name, out memItem))
			{
				if(memItem.Type == type)
					return memItem.GetValue();
			}

			return null;
		}

		public bool GetBool(string name, bool defaultValue = false)
		{
			object value = GetItem(name, MemoryItem.ItemType.Boolean);
			return (value != null) ? (bool)value : defaultValue;
		}

		public int GetInt(string name, int defaultValue = 0)
		{
			object value = GetItem(name, MemoryItem.ItemType.Integer);
			return (value != null) ? (int)value : defaultValue;
		}

		public float GetFloat(string name, float defaultValue = 0.0f)
		{
			object value = GetItem(name, MemoryItem.ItemType.Float);
			return (value != null) ? (float)value : defaultValue;
		}

		public string GetString(string name, string defaultValue = null)
		{
			object value = GetItem(name, MemoryItem.ItemType.String);
			return (value != null) ? (string)value : defaultValue;
		}

		public Vector3 GetVector3(string name, Vector3 defaultValue)
		{
			object value = GetItem(name, MemoryItem.ItemType.Vector3);
			return (value != null) ? (Vector3)value : defaultValue;
		}

		public UnityEngine.Object GetUnityObject(string name, UnityEngine.Object defaultValue = null)
		{
			object value = GetItem(name, MemoryItem.ItemType.UnityObject);
			return (value != null) ? (UnityEngine.Object)value : defaultValue;
		}

		public T GetUnityObject<T>(string name, T defaultValue = null) where T : UnityEngine.Object
		{
			object value = GetItem(name, MemoryItem.ItemType.UnityObject);
			return (value != null && value is T) ? (T)value : defaultValue;
		}

		private MemoryItem.ItemType? GetValueType(object value)
		{
			if(value is bool)
				return MemoryItem.ItemType.Boolean;
			else if(value is int)
				return MemoryItem.ItemType.Integer;
			else if(value is float)
				return MemoryItem.ItemType.Float;
			else if(value is string)
				return MemoryItem.ItemType.String;
			else if(value is Vector3)
				return MemoryItem.ItemType.Vector3;
			else if(value is UnityEngine.Object)
				return MemoryItem.ItemType.UnityObject;
			else
				return null;
		}
	}
}
