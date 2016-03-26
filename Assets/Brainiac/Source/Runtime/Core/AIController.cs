using UnityEngine;
using UnityEngine.Events;

namespace Brainiac
{
	public class AIController : MonoBehaviour 
	{
		public event UnityAction BeforeUpdate;
		public event UnityAction AfterUpdate;

		[SerializeField]
		private BTAsset m_behaviourTree;
		[SerializeField]
		private Memory m_memory;
		[SerializeField]
		private GameObject m_body;
		[SerializeField]
		private UpdateMode m_updateMode;
		[SerializeField]
		private float m_updateInterval;
		[SerializeField]
		private bool m_debugMode;

		private float m_lastUpdateTime;
		private BehaviourTree m_btInstance;

		public GameObject Body
		{
			get
			{
				return m_body != null ? m_body : gameObject;
			}
		}

		public Memory Memory
		{
			get
			{
				return m_memory;
			}
		}
		
		public bool DebugMode
		{
			get
			{
				return m_debugMode;
			}
			set
			{
				m_debugMode = value;
			}
		}

		private void Awake()
		{
			if(m_memory == null)
			{
				m_memory = gameObject.GetComponent<Memory>();
				if(m_memory == null)
				{
					m_memory = gameObject.AddComponent<Memory>();
				}
			}

			if(m_behaviourTree != null)
			{
				m_btInstance = m_behaviourTree.CreateRuntimeTree();
			}

			m_lastUpdateTime = 0.0f;
		}

		private void Start()
		{
			if(m_btInstance != null)
			{
				m_btInstance.Root.OnStart(this);
			}
		}

		private void Update()
		{
			if(m_btInstance != null)
			{
				if(m_updateMode == UpdateMode.EveryFrame || Time.time >= m_lastUpdateTime + m_updateInterval)
				{
					RaiseBeforeUpdateEvent();
					if(m_btInstance.Root.Status != BehaviourNodeStatus.Running)
					{
						m_btInstance.Root.OnReset();
					}
					m_btInstance.Root.Run(this);
					m_lastUpdateTime = Time.time;
					RaiseAfterUpdateEvent();
				}
			}
		}

		private void RaiseBeforeUpdateEvent()
		{
			if(BeforeUpdate != null)
			{
				BeforeUpdate();
			}
		}

		private void RaiseAfterUpdateEvent()
		{
			if(AfterUpdate != null)
			{
				AfterUpdate();
			}
		}

#if UNITY_EDITOR
		public BehaviourTree GetBehaviourTree()
		{
			return m_btInstance;
		}
#endif
	}
}
