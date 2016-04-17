using Brainiac.Serialization;

namespace Brainiac
{
	[AddNodeMenu("Decorator/Limiter")]
	public class Limiter : Decorator
	{
		[BTProperty("MaxExecutions")]
		private MemoryVar m_maxExecutions;

		private int m_numberOfExecutions;

		public override void OnStart(AIAgent agent)
		{
			base.OnStart(agent);
			m_numberOfExecutions = 0;
		}

		protected override BehaviourNodeStatus OnExecute(AIAgent agent)
		{
			BehaviourNodeStatus status = BehaviourNodeStatus.Failure;
			int maxExecutions = m_maxExecutions.AsInt.HasValue ? m_maxExecutions.AsInt.Value : m_maxExecutions.Evaluate<int>(agent.Memory, 0);

			if(m_child != null && m_numberOfExecutions < maxExecutions)
			{
				status = m_child.Run(agent);
			}

			return status;
		}
	}
}