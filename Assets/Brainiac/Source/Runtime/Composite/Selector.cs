using UnityEngine;

namespace Brainiac
{
	[AddNodeMenu("Composite/Selector")]
	public class Selector : Composite
	{
		private int m_currentChild;

		public override string Title
		{
			get
			{
				return "Selector";
			}
		}

		protected override void OnEnter(AIController aiController)
		{
			m_currentChild = 0;
		}

		protected override BehaviourNodeStatus OnExecute(AIController aiController)
		{
			BehaviourNodeStatus status = BehaviourNodeStatus.Success;
			while(m_currentChild < m_children.Count)
			{
				status = m_children[m_currentChild].Run(aiController);
				if (status == BehaviourNodeStatus.Failure)
					m_currentChild++;
				else
					break;
			}

			return status;
		}
	}
}
