using UnityEngine;

namespace Brainiac
{
	[AddBehaviourNodeMenu("Decorator/Until Success")]
	public class UntilSuccess : Decorator
	{
		public override string Title
		{
			get
			{
				return "Until Success";
			}
		}

		protected override BehaviourNodeStatus OnExecute(Agent agent)
		{
			if(m_child == null)
				return BehaviourNodeStatus.Success;

			BehaviourNodeStatus status = m_child.Run(agent);
			if(status != BehaviourNodeStatus.Success)
			{
				status = BehaviourNodeStatus.Running;
			}

			return status;
		}
	}
}