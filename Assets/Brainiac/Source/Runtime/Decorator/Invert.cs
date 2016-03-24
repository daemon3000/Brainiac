using UnityEngine;

namespace Brainiac
{
	[AddNodeMenu("Decorator/Invert")]
	public class Invert : Decorator
	{
		public override string Title
		{
			get
			{
				return "Invert";
			}
		}

		protected override BehaviourNodeStatus OnExecute(Agent agent)
		{
			BehaviourNodeStatus status = BehaviourNodeStatus.Success;

			if(m_child != null)
			{
				status = m_child.Run(agent);
				if(status == BehaviourNodeStatus.Success)
				{
					status = BehaviourNodeStatus.Failure;
				}
				else if(status == BehaviourNodeStatus.Failure)
				{
					status = BehaviourNodeStatus.Success;
				}
			}

			return status;
		}
	}
}