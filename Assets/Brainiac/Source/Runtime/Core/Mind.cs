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

		private float m_lastProccesTime;
		private long m_tickCount;
		private BehaviourTree m_btInstance;

		public long TickCount { get { return m_tickCount; } }

		private void Awake()
		{
			m_tickCount = 0L;
			m_lastProccesTime = 0.0f;
			m_btInstance = m_behaviourTree.DeserializeAsCopy();
			m_btInstance.Root.OnInitialize();
		}

		public void Tick(AIController ai)
		{
			if(m_btInstance != null)
			{
				if (m_tickMode == TickMode.EveryFrame || Time.time >= m_lastProccesTime + m_tickInterval)
				{
					m_btInstance.Root.Run(ai);
					m_lastProccesTime = Time.time;
					m_tickCount++;
				}
			}
		}
	}
}
