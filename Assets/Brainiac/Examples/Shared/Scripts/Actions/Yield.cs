using UnityEngine;

namespace Brainiac
{
	[AddNodeMenu("Action/Yield")]
	public class Yield : Action
	{
		private bool m_canYield;

		public override string Title
		{
			get
			{
				return "Yield";
			}
		}

		protected override void OnEnter(AIController aiController)
		{
			m_canYield = true;
		}

		protected override BehaviourNodeStatus OnExecute(AIController aiController)
		{
			if(m_canYield)
			{
				m_canYield = false;
				return BehaviourNodeStatus.Running;
			}

			return BehaviourNodeStatus.Success;
		}
	}
}