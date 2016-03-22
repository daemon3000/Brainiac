using UnityEngine;
using System.Collections;

namespace Brainiac
{
	public class Agent : MonoBehaviour 
	{
		[SerializeField]
		private GameObject m_avatar;
		[SerializeField]
		private Mind m_mind;
		[SerializeField]
		private Memory m_memory;
		[SerializeField]
		private bool m_debugMode;

		public GameObject Avatar
		{
			get
			{
				return m_avatar;
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

		private void Update()
		{
			if(m_mind != null)
			{
				m_mind.Tick(this);
			}
		}
	}
}
