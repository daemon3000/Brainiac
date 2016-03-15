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

		private BehaviourTree m_behaviourTree;

		public BehaviourTree BehaviourTree
		{
			get
			{
				return m_behaviourTree;
			}
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

		public void Serialize()
		{
			m_serializedData = BTUtils.Save(m_behaviourTree);
		}

		public void Deserialize()
		{
			m_behaviourTree = BTUtils.Load(m_serializedData);
		}

		public BehaviourTree DeserializeAsCopy()
		{
			return BTUtils.Load(m_serializedData);
		}

		public void Dispose()
		{
			m_behaviourTree = null;
		}

		private void OnDestroy()
		{
			Dispose();
		}
	}
}