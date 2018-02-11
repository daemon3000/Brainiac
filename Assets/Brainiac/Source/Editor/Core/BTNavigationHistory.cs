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
		private const int MAX_RECENT_FILES = 5;

		[SerializeField]
		private string m_serializedHistory;
		
		[NonSerialized]
		private bool m_needRefresh;

		private List<Tuple<string, BehaviourTree>> m_history;
		private List<string> m_recentFiles;
		private StringBuilder m_stringBuilder;

		public int Size
		{
			get { return m_history.Count; }
		}

		public IList<string> RecentFiles
		{
			get
			{
				LoadRecentFiles();
				return m_recentFiles.AsReadOnly();
			}
		}

		public BTNavigationHistory()
		{
			m_history = new List<Tuple<string, BehaviourTree>>();
			m_recentFiles = new List<string>();
			m_stringBuilder = new StringBuilder();
		}
		
		public void Push(BTAsset asset, BehaviourTree instance)
		{
			m_history.Add(new Tuple<string, BehaviourTree>(AssetDatabase.GetAssetPath(asset), instance));
			AddRecentFile(asset);
			SaveRecentFiles();
		}

		private void AddRecentFile(BTAsset asset)
		{
			string filename = AssetDatabase.GetAssetPath(asset);
			for(int i = 0; i < m_recentFiles.Count; i++)
			{
				if(m_recentFiles[i] == filename)
					return;
			}

			if(m_recentFiles.Count < MAX_RECENT_FILES)
			{
				m_recentFiles.Add(filename);
			}
			else
			{
				for(int i = 0; i < m_recentFiles.Count - 1; i++)
				{
					m_recentFiles[i] = m_recentFiles[i + 1];
				}
				m_recentFiles[m_recentFiles.Count - 1] = filename;
			}
		}

		public void Pop(out BTAsset asset, out BehaviourTree instance)
		{
			if(m_history.Count > 0)
			{
				var historyItem = m_history[m_history.Count - 1];
				asset = AssetDatabase.LoadAssetAtPath<BTAsset>(historyItem.Item1);
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
				m_history[i] = new Tuple<string, BehaviourTree>(m_history[i].Item1, null);
			}
		}

		public BTAsset GetAssetAt(int index)
		{
			if(index >= 0 && index < m_history.Count)
				return AssetDatabase.LoadAssetAtPath<BTAsset>(m_history[index].Item1);

			return null;
		}

		public void GetAt(int index, out BTAsset asset, out BehaviourTree instance)
		{
			if(index >= 0 && index < m_history.Count)
			{
				asset = AssetDatabase.LoadAssetAtPath<BTAsset>(m_history[index].Item1);
				instance = m_history[index].Item2;
			}
			else
			{
				asset = null;
				instance = null;
			}
		}

		public void Refresh()
		{
			if (m_needRefresh)
			{
				m_needRefresh = false;

				m_history.Clear();

				if(m_serializedHistory != null)
				{
					string[] paths = m_serializedHistory.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					foreach(var path in paths)
					{
						BTAsset asset = AssetDatabase.LoadAssetAtPath<BTAsset>(path);
						if(asset != null)
						{
							m_history.Add(new Tuple<string, BehaviourTree>(path, null));
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

		public void OnBeforeSerialize()
		{
			m_stringBuilder.Length = 0;

			foreach(var item in m_history)
			{
				m_stringBuilder.Append(item.Item1);
				m_stringBuilder.Append(';');
			}

			m_serializedHistory = m_stringBuilder.ToString();
		}

		public void OnAfterDeserialize()
		{
			m_needRefresh = true;
		}

		private void SaveRecentFiles()
		{
			m_stringBuilder.Length = 0;
			foreach(var file in m_recentFiles)
			{
				m_stringBuilder.Append(file);
				m_stringBuilder.Append(';');
			}

			EditorPrefs.SetString(PlayerSettings.productName + ".Brainiac.RecentFiles", m_stringBuilder.ToString());
		}

		private void LoadRecentFiles()
		{
			string saveData = EditorPrefs.GetString(PlayerSettings.productName + ".Brainiac.RecentFiles");

			m_recentFiles.Clear();
			if(!string.IsNullOrEmpty(saveData))
			{
				string[] paths = m_serializedHistory.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				foreach(var path in paths)
				{
					if(System.IO.File.Exists(Application.dataPath + path.Substring(6)))
					{
						m_recentFiles.Add(path);
					}
				}
			}
		}
	}
}
