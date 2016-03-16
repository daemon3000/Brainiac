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

		protected override BehaviourNodeStatus OnExecute(AIController ai)
		{
			BehaviourNodeStatus status = m_child.Run(ai);
			if(status == BehaviourNodeStatus.Running)
			{
				return BehaviourNodeStatus.Running;
			}

			return BehaviourNodeStatus.Failure;
		}
	}
}