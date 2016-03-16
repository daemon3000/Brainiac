using UnityEngine;
using System.Collections.Generic;

namespace Brainiac
{
	public class Memory : MonoBehaviour 
	{
		[SerializeField]
		private MemoryItem[] m_startMemory;

		private Dictionary<string, object> m_globalMemory;

		private void Awake()
		{
			m_globalMemory = new Dictionary<string, object>();
			for(int i = 0; i < m_startMemory.Length; i++)
			{
				SetItem(m_startMemory[i].Name, m_startMemory[i].GetValue());
			}
		}

		public void SetItem(string name, object item)
		{
			if(m_globalMemory.ContainsKey(name))
			{
				m_globalMemory[name] = item;
			}
			else
			{
				m_globalMemory.Add(name, item);
			}
		}

		public object GetItem(string name)
		{
			object value = null;
			if(m_globalMemory.TryGetValue(name, out value))
			{
				return value;
			}
			
			return null;
		}

		public T GetItem<T>(string name, T defaultValue)
		{
			object value = GetItem(name);
			return (value != null && value is T) ? (T)value : defaultValue;
		}
	}
}
