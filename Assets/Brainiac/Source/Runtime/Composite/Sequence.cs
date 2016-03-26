using UnityEngine;

namespace Brainiac
{
	[AddNodeMenu("Composite/Sequence")]
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
				if (status == BehaviourNodeStatus.Success)
					m_currentChild++;
				else
					break;
			}

			return status;
		}
	}
}
