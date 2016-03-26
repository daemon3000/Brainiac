using UnityEngine;

namespace Brainiac
{
	[AddNodeMenu("Decorator/Always Fail")]
	public class AlwaysFail : Decorator
	{
		public override string Title
		{
			get
			{
				return "Always Fail";
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIController aiController)
		{
			BehaviourNodeStatus status = BehaviourNodeStatus.Failure;
			if(m_child != null)
			{
				status = m_child.Run(aiController);
				if(status != BehaviourNodeStatus.Running)
				{
					status = BehaviourNodeStatus.Failure;
				}
			}

			return status;
		}
	}
}