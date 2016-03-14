using UnityEngine;
using System.Collections;

namespace Brainiac
{
	public class AIController : MonoBehaviour 
	{
		[SerializeField]
		private GameObject m_avatar;
		[SerializeField]
		private Mind m_mind;
		[SerializeField]
		private Memory m_memory;
		[SerializeField]
		private Navigator m_navigator;


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

		public Navigator Navigator
		{
			get
			{
				return m_navigator;
			}
		}

		private void Update()
		{
			if (m_mind != null)
				m_mind.Tick(this);
		}
	}
}
