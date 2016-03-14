using UnityEngine;

namespace Brainiac
{
	public class Sequence : Composite
	{
		private int m_currentChild;

		public override string Title
		{
			get
			{
				return "Sequence";
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIController ai)
		{
			BehaviourNodeStatus status = BehaviourNodeStatus.Success;
			while(m_currentChild < m_children.Count)
			{
				status = m_children[m_currentChild].Run(ai);
				if (status == BehaviourNodeStatus.Success)
					m_currentChild++;
				else
					break;
			}

			return status;
		}

		protected override void OnStart(AIController ai)
		{
			m_currentChild = 0;
		}
	}
}
