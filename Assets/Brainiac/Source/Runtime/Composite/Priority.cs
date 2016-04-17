using UnityEngine;

namespace Brainiac
{
	[AddNodeMenu("Composite/Priority")]
	public class Priority : Composite
	{
		private int m_currentChild;

		protected override void OnEnter(AIAgent agent)
		{
			m_currentChild = 0;
		}

		protected override BehaviourNodeStatus OnExecute(AIAgent agent)
		{
			BehaviourNodeStatus status = BehaviourNodeStatus.Success;
			while(m_currentChild < m_children.Count)
			{
				status = m_children[m_currentChild].Run(agent);
				if(status == BehaviourNodeStatus.Failure)
					m_currentChild++;
				else
					break;
			}

			return status;
		}
	}
}
