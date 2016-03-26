using UnityEngine;

namespace Brainiac
{
	[AddNodeMenu("Decorator/Until Success")]
	public class UntilSuccess : Decorator
	{
		public override string Title
		{
			get
			{
				return "Until Success";
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIController aiController)
		{
			BehaviourNodeStatus status = BehaviourNodeStatus.Success;

			if(m_child != null)
			{
				if(m_child.Status != BehaviourNodeStatus.None && m_child.Status != BehaviourNodeStatus.Running)
				{
					m_child.OnReset();
				}

				status = m_child.Run(aiController);
				if(status != BehaviourNodeStatus.Success)
				{
					status = BehaviourNodeStatus.Running;
				}
			}

			return status;
		}
	}
}