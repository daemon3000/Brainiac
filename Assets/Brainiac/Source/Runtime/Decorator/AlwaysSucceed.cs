using UnityEngine;

namespace Brainiac
{
	[AddBehaviourNodeMenu("Decorator/Always Succeed")]
	public class AlwaysSucceed : Decorator
	{
		public override string Title
		{
			get
			{
				return "Always Succeed";
			}
		}

		protected override BehaviourNodeStatus OnExecute(Agent agent)
		{
			BehaviourNodeStatus status = m_child.Run(agent);
			if(status == BehaviourNodeStatus.Running)
			{
				return BehaviourNodeStatus.Running;
			}

			return BehaviourNodeStatus.Success;
		}
	}
}