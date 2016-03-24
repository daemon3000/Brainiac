using UnityEngine;

namespace Brainiac
{
	public class Mind : MonoBehaviour
	{
		[SerializeField]
		private BTAsset m_behaviourTree;
		[SerializeField]
		private UpdateMode m_updateMode;
		[SerializeField]
		private float m_updateInterval;

		private float m_lastUpdateTime;
		private BehaviourTree m_btInstance;

		private void Awake()
		{
			m_lastUpdateTime = 0.0f;
			m_btInstance = m_behaviourTree.CreateRuntimeTree();
		}

		public void OnUpdate(Agent agent)
		{
			if(m_btInstance != null)
			{
				if(m_updateMode == UpdateMode.EveryFrame || Time.time >= m_lastUpdateTime + m_updateInterval)
				{
					if(m_btInstance.Root.Status != BehaviourNodeStatus.Running)
					{
						m_btInstance.Root.OnReset();
					}
					m_btInstance.Root.Run(agent);
					m_lastUpdateTime = Time.time;
				}
			}
		}

#if UNITY_EDITOR
		public BTAsset GetBehaviourTree()
		{
			return m_behaviourTree;
		}

		public BehaviourTree GetBehaviourTreeInstance()
		{
			return m_btInstance;
		}
#endif
	}
}
