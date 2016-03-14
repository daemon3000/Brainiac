using UnityEngine;

namespace Brainiac
{
	public class Inverter : Decorator
	{
		public override string Title
		{
			get
			{
				return "Inverter";
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIController ai)
		{
			BehaviourNodeStatus status = m_child.Run(ai);
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