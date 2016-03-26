using UnityEngine;

namespace Brainiac
{
	[CreateAssetMenu(menuName = "Brainiac/Behaviour Tree")]
	public class BTAsset : ScriptableObject
	{
		[SerializeField]
		[HideInInspector]
		private string m_serializedData;
		[SerializeField]
		private Vector2 m_canvasPosition;
		[SerializeField]
		private Vector2 m_canvasSize;

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

		public Vector3 CanvasSize
		{
			get
			{
				return m_canvasSize;
			}
			set
			{
				m_canvasSize = value;
			}
		}

#if UNITY_EDITOR
		private void OnEnable()
		{
			if(m_canvasSize == Vector2.zero)
			{
				m_canvasSize = DEFAULT_CANVAS_SIZE;
			}
		}

		public BehaviourTree GetEditModeTree()
		{
			if(m_editModeTree == null)
			{
				m_editModeTree = BTUtils.DeserializeTree(m_serializedData);
				m_editModeTree.ReadOnly = false;
			}

			return m_editModeTree;
		}

		public void Serialize()
		{
			if(m_editModeTree != null)
			{
				m_serializedData = BTUtils.SerializeTree(m_editModeTree);
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
			tree.ReadOnly = true;

			return tree;
		}
	}
}