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
				if(status != BehaviourNodeStatus.Failure)
				{
					status = BehaviourNodeStatus.Running;
				}
			}

			return status;
		}
	}
}