using UnityEngine;
using UnityEngine.Events;

namespace Brainiac
{
	public class Agent : MonoBehaviour 
	{
		public event UnityAction BeforeUpdate;
		public event UnityAction AfterUpdate;

		[SerializeField]
		private GameObject m_body;
		[SerializeField]
		private Mind m_mind;
		[SerializeField]
		private Memory m_memory;
		[SerializeField]
		private bool m_debugMode;

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

		private void Start()
		{
			if(m_memory == null)
			{
				m_memory = gameObject.AddComponent<Memory>();
			}
		}

		private void Update()
		{
			if(m_mind != null)
			{
				RaiseBeforeUpdateEvent();
				m_mind.OnUpdate(this);
				RaiseAfterUpdateEvent();
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
	}
}
