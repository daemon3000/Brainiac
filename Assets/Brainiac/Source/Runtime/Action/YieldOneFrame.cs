
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

		protected override void OnEnter(AIAgent agent)
		{
			m_canYield = true;
		}

		protected override BehaviourNodeStatus OnExecute(AIAgent agent)
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