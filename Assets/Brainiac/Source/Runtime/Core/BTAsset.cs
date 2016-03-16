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
		public BehaviourTree GetEditModeTree()
		{
			if(m_editModeTree == null)
			{
				m_editModeTree = BTUtils.LoadTree(m_serializedData);
			}

			return m_editModeTree;
		}

		public void Serialize()
		{
			m_serializedData = BTUtils.SaveTree(m_editModeTree);
		}

		public void Dispose()
		{
			m_editModeTree = null;
		}
#endif

		public BehaviourTree CreateRuntimeTree()
		{
			return BTUtils.LoadTree(m_serializedData);
		}
	}
}