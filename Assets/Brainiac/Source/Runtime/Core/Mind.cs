using UnityEngine;

namespace Brainiac
{
	public class Mind : MonoBehaviour
	{
		[SerializeField]
		private BTAsset m_behaviourTree;
		[SerializeField]
		private TickMode m_tickMode = TickMode.EveryFrame;
		[SerializeField]
		private float m_tickInterval = 0.0f;

		private float m_lastTickTime;
		private long m_tickCount;
		private BehaviourTree m_btInstance;

		public long TickCount
		{
			get { return m_tickCount; }
		}

		public BTAsset BehaviourTree
		{
			get { return m_behaviourTree; }
		}

		public BehaviourTree BehaviourTreeInstance
		{
			get { return m_btInstance; }
		}

		private void Awake()
		{
			m_tickCount = 0L;
			m_lastTickTime = 0.0f;
			m_btInstance = m_behaviourTree.CreateRuntimeTree();
		}

		public void Tick(Agent agent)
		{
			if(m_btInstance != null)
			{
				if(m_tickMode == TickMode.EveryFrame || Time.time >= m_lastTickTime + m_tickInterval)
				{
					m_btInstance.Root.Run(agent);
					m_lastTickTime = Time.time;
					m_tickCount++;
				}
			}
		}
	}
}
