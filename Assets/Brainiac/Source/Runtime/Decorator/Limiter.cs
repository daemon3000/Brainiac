using Brainiac.Serialization;

namespace Brainiac
{
	[AddNodeMenu("Decorator/Limiter")]
	public class Limiter : Decorator
	{
		[BTProperty("MaxExecutions")]
		private int m_maxExecutions;

		private int m_numberOfExecutions;

		public override void OnStart(AIAgent agent)
		{
			base.OnStart(agent);
			m_numberOfExecutions = 0;
		}

		protected override BehaviourNodeStatus OnExecute(AIAgent agent)
		{
			BehaviourNodeStatus status = BehaviourNodeStatus.Failure;
			if(m_child != null && m_numberOfExecutions < m_maxExecutions)
			{
				status = m_child.Run(agent);
			}

			return status;
		}
	}
}