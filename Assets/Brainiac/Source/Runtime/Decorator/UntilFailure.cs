using UnityEngine;

namespace Brainiac
{
	[AddBehaviourNodeMenu("Decorator/Until Failure")]
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
			BehaviourNodeStatus status = m_child.Run(agent);
			if(status != BehaviourNodeStatus.Failure)
			{
				status = BehaviourNodeStatus.Running;
			}

			return status;
		}
	}
}