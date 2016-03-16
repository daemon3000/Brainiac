using UnityEngine;

namespace Brainiac
{
	[AddNodeMenu("Decorator/Until Failure")]
	public class UntilFailure : Decorator
	{
		public override string Title
		{
			get
			{
				return "Until Failure";
			}
		}

		protected override BehaviourNodeStatus OnExecute(Agent agent)
		{
			if(m_child == null)
				return BehaviourNodeStatus.Failure;

			BehaviourNodeStatus status = m_child.Run(agent);
			if(status != BehaviourNodeStatus.Failure)
			{
				status = BehaviourNodeStatus.Running;
			}

			return status;
		}
	}
}