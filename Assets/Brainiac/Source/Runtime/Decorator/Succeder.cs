using UnityEngine;

namespace Brainiac
{
	public class Succeder : Decorator
	{
		public override string Title
		{
			get
			{
				return "Succeder";
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIController ai)
		{
			BehaviourNodeStatus status = m_child.Run(ai);
			if(status == BehaviourNodeStatus.Running)
			{
				return BehaviourNodeStatus.Running;
			}

			return BehaviourNodeStatus.Success;
		}
	}
}