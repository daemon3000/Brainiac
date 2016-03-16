using UnityEngine;

namespace Brainiac
{
	[AddBehaviourNodeMenu("Decorator/Invert")]
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
			if(m_child == null)
				return BehaviourNodeStatus.Success;

			BehaviourNodeStatus status = m_child.Run(agent);
			if(status == BehaviourNodeStatus.Success)
			{
				return BehaviourNodeStatus.Failure;
			}
			else if(status == BehaviourNodeStatus.Failure)
			{
				return BehaviourNodeStatus.Success;
			}

			return BehaviourNodeStatus.Running;
		}
	}
}