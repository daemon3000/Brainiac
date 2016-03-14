using UnityEngine;
using System.Collections;

namespace Brainiac
{
	public class Navigator : MonoBehaviour 
	{
		[SerializeField]
		private NavMeshAgent m_navMeshAgent;
		[SerializeField]
		private float m_closeEnoughDistance;

		public void SetDestination(Vector3 destination)
		{
			m_navMeshAgent.SetDestination(destination);
		}

		public bool IsAtDestination(Vector3 destination)
		{
			if (!m_navMeshAgent.pathPending)
			{
				float distance = Vector3.Distance(destination, m_navMeshAgent.transform.position);
				if (distance <= m_closeEnoughDistance)
				{
					if (!m_navMeshAgent.hasPath || Mathf.Approximately(m_navMeshAgent.velocity.sqrMagnitude, 0.0f))
						return true;
				}
			}

			return false;
		}
	}
}