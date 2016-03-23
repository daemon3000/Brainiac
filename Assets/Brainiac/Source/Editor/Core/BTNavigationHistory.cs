using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Collections.Generic;
using Brainiac;

namespace BrainiacEditor
{
	[Serializable]
	public class BTNavigationHistory : ISerializationCallbackReceiver
	{
		[SerializeField]
		private string m_serializedHistory;

		private List<Tuple<BTAsset, BehaviourTree>> m_history;

		public int Size
		{
			get { return m_history.Count; }
		}

		public BTNavigationHistory()
		{
			m_history = new List<Tuple<BTAsset, BehaviourTree>>();
		}
		
		public void Push(BTAsset asset, BehaviourTree instance)
		{
			m_history.Add(new Tuple<BTAsset, BehaviourTree>(asset, instance));
		}

		public void Pop(out BTAsset asset, out BehaviourTree instance)
		{
			if(m_history.Count > 0)
			{
				var historyItem = m_history[m_history.Count - 1];
				asset = historyItem.Item1;
				instance = historyItem.Item2;

				m_history.RemoveAt(m_history.Count - 1);
			}
			else
			{
				asset = null;
				instance = null;
			}
		}

		public void Clear()
		{
			m_history.Clear();
		}

		public void Trim(int startIndex)
		{
			for(int i = m_history.Count - 1; i >= startIndex; i--)
			{
				m_history.RemoveAt(i);
			}
		}

		public void DiscardInstances()
		{
			for(int i = 0; i < m_history.Count; i++)
			{
				m_history[i] = new Tuple<BTAsset, BehaviourTree>(m_history[i].Item1, null);
			}
		}

		public BTAsset GetAssetAt(int index)
		{
			if(index >= 0 && index < m_history.Count)
				return m_history[index].Item1;

			return null;
		}

		public void GetAt(int index, out BTAsset asset, out BehaviourTree instance)
		{
			if(index >= 0 && index < m_history.Count)
			{
				asset = m_history[index].Item1;
				instance = m_history[index].Item2;
			}
			else
			{
				asset = null;
				instance = null;
			}
		}

		public void OnBeforeSerialize()
		{
			StringBuilder builder = new StringBuilder();
			foreach(var item in m_history)
			{
				builder.Append(AssetDatabase.GetAssetPath(item.Item1));
				builder.Append(';');
			}

			m_serializedHistory = builder.ToString();
		}

		public void OnAfterDeserialize()
		{
			m_history.Clear();

			if(m_serializedHistory != null)
			{
				string[] paths = m_serializedHistory.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				foreach(var path in paths)
				{
					BTAsset asset = AssetDatabase.LoadAssetAtPath<BTAsset>(path);
					if(asset != null)
					{
						m_history.Add(new Tuple<BTAsset, BehaviourTree>(asset, null));
					}
					else
					{
						m_history.Clear();
						break;
					}
				}
			}
		}
	}
}
