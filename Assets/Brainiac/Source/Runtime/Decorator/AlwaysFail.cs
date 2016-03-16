using UnityEngine;

namespace Brainiac
{
	[AddBehaviourNodeMenu("Decorator/Always Fail")]
	public class AlwaysFail : Decorator
	{
		public override string Title
		{
			get
			{
				return "Always Fail";
			}
		}

		protected override BehaviourNodeStatus OnExecute(Agent agent)
		{
			BehaviourNodeStatus status = m_child.Run(agent);
			if(status == BehaviourNodeStatus.Running)
			{
				return BehaviourNodeStatus.Running;
			}

			return BehaviourNodeStatus.Failure;
		}
	}
}