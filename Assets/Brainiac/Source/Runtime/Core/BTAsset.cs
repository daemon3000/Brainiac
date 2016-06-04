using UnityEngine;
using System.Collections.Generic;

namespace Brainiac
{
	[CreateAssetMenu(menuName = "Brainiac/Behaviour Tree")]
	public class BTAsset : ScriptableObject
	{
		[System.Serializable]
		private class AssetIDPair
		{
			public BTAsset asset;
			public string assetID;
		}

		[SerializeField]
		[HideInInspector]
		private string m_serializedData;
		[SerializeField]
		private Vector2 m_canvasPosition;
		[SerializeField]
		private Rect m_canvasArea;
		[SerializeField]
		private List<AssetIDPair> m_subtrees;

#if UNITY_EDITOR
		private BehaviourTree m_editModeTree;
#endif

		public static Vector2 DEFAULT_CANVAS_SIZE
		{
			get { return new Vector2(1000, 1000); }
		}

		public Vector2 CanvasPosition
		{
			get
			{
				return m_canvasPosition;
			}
			set
			{
				m_canvasPosition = value;
			}
		}

		public Rect CanvasArea
		{
			get
			{
				return m_canvasArea;
			}
			set
			{
				m_canvasArea = value;
			}
		}

#if UNITY_EDITOR
		private void OnEnable()
		{
			if(Mathf.Approximately(m_canvasArea.width, 0) || Mathf.Approximately(m_canvasArea.height, 0))
			{
				m_canvasArea = new Rect(-DEFAULT_CANVAS_SIZE.x / 2, -DEFAULT_CANVAS_SIZE.y / 2, DEFAULT_CANVAS_SIZE.x, DEFAULT_CANVAS_SIZE.y);
				m_canvasPosition = m_canvasArea.center;
			}
			if(m_subtrees == null)
			{
				m_subtrees = new List<AssetIDPair>();
			}
		}

		public BehaviourTree GetEditModeTree()
		{
			if(m_editModeTree == null)
			{
				m_editModeTree = BTUtils.DeserializeTree(m_serializedData);
				m_editModeTree.Root.OnAfterDeserialize(this);
				m_editModeTree.ReadOnly = false;
			}

			return m_editModeTree;
		}

		public void Serialize()
		{
			if(m_editModeTree != null)
			{
				m_editModeTree.Root.OnBeforeSerialize(this);

				string serializedData = BTUtils.SerializeTree(m_editModeTree);
				if(serializedData != null)
				{
					m_serializedData = serializedData;
				}
			}
		}

		public void Dispose()
		{
			m_editModeTree = null;
		}
#endif

		public BehaviourTree CreateRuntimeTree()
		{
			BehaviourTree tree = BTUtils.DeserializeTree(m_serializedData);
			tree.Root.OnAfterDeserialize(this);
			tree.ReadOnly = true;

			return tree;
		}

		public void SetSubtreeAsset(string subtreeID, BTAsset subtreeAsset)
		{
			if(!string.IsNullOrEmpty(subtreeID))
			{
				AssetIDPair subtree = m_subtrees.Find(obj => obj.assetID == subtreeID);
				if(subtree != null)
				{
					subtree.asset = subtreeAsset;
				}
				else
				{
					subtree = new AssetIDPair();
					subtree.asset = subtreeAsset;
					subtree.assetID = subtreeID;
					m_subtrees.Add(subtree);
				}
			}
		}

		public BTAsset GetSubtreeAsset(string subtreeID)
		{
			AssetIDPair subtree = m_subtrees.Find(obj => obj.assetID == subtreeID);
			return subtree != null ? subtree.asset : null;
		}
	}
}