using UnityEngine;

namespace Brainiac
{
	[AddBehaviourNodeMenu("Action/Yield")]
	public class Yield : Action
	{
		private bool m_yield;

		public override string Title
		{
			get
			{
				return "Yield";
			}
		}

		protected override BehaviourNodeStatus OnExecute(Agent agent)
		{
			if(m_yield)
			{
				m_yield = false;
				return BehaviourNodeStatus.Running;
			}

			return BehaviourNodeStatus.Success;
		}

		protected override void OnStart(Agent agent)
		{
			base.OnStart(agent);
			m_yield = true;
		}
	}
}