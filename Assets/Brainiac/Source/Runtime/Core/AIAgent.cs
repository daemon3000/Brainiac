using UnityEngine;
using UnityEngine.Events;

namespace Brainiac
{
	public class AIAgent : MonoBehaviour 
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

		private BehaviourTree m_btInstance;
		private float m_timeElapsedSinceLastUpdate;
		private bool m_isRunning;

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

			m_timeElapsedSinceLastUpdate = 0.0f;
			m_isRunning = true;
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
			if(m_updateMode != UpdateMode.Manual && m_isRunning)
			{
				if(m_updateMode == UpdateMode.EveryFrame || m_timeElapsedSinceLastUpdate >= m_updateInterval)
				{
					UpdateInternal();
					m_timeElapsedSinceLastUpdate = 0.0f;
				}

				m_timeElapsedSinceLastUpdate += Time.deltaTime;
			}
		}

		private void UpdateInternal()
		{
			RaiseBeforeUpdateEvent();

			if(m_btInstance != null)
			{
				if(m_btInstance.Root.Status != BehaviourNodeStatus.Running)
				{
					m_btInstance.Root.OnReset();
				}
				m_btInstance.Root.Run(this);
			}
			
			RaiseAfterUpdateEvent();
		}

		public void Stop()
		{
			if(m_updateMode != UpdateMode.Manual)
			{
				m_timeElapsedSinceLastUpdate = 0.0f;
				m_isRunning = false;
				if(m_btInstance != null)
				{
					m_btInstance.Root.OnReset();
				}
			}
#if UNITY_EDITOR
			else
			{
				if(m_btInstance != null)
				{
					m_btInstance.Root.OnReset();
				}
			}
#endif
		}

		public void Pause()
		{
			if(m_updateMode != UpdateMode.Manual)
			{
				m_isRunning = false;
			}
#if UNITY_EDITOR
			else
			{
				Debug.LogWarning("Can't pause AIAgent! Update mode is set to 'Manual'.", this);
			}
#endif
		}

		public void Resume()
		{
			if(m_updateMode != UpdateMode.Manual)
			{
				m_isRunning = true;
			}
#if UNITY_EDITOR
			else
			{
				Debug.LogWarning("Can't resume AIAgent! Update mode is set to 'Manual'.", this);
			}
#endif
		}

		public void Tick()
		{
			if(m_updateMode == UpdateMode.Manual)
			{
				UpdateInternal();
			}
#if UNITY_EDITOR
			else
			{
				Debug.LogWarning("Can't tick AIAgent! Update mode needs to be set to 'Manual'.", this);
			}
#endif
		}

#if UNITY_EDITOR
		public BehaviourTree GetBehaviourTree()
		{
			return m_btInstance;
		}
#endif
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
	}
}
