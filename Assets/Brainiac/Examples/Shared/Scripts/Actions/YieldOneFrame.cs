using UnityEngine;

namespace Brainiac
{
	[AddNodeMenu("Action/Yield One Frame")]
	public class YieldOneFrame : Action
	{
		private bool m_canYield;

		public override string Title
		{
			get
			{
				return "Yield One Frame";
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