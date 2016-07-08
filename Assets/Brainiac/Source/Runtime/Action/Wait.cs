using UnityEngine;
using Brainiac.Serialization;

namespace Brainiac
{
	[AddNodeMenu("Action/Wait")]
	public class Wait : Action
	{
		[BTProperty("Duration")]
		private MemoryVar m_duration;

		private float m_startTime;

		[BTIgnore]
		public MemoryVar Duration
		{
			get
			{
				return m_duration;
			}
		}

		public Wait()
		{
			m_duration = new MemoryVar();
		}

		protected override void OnEnter(AIAgent agent)
		{
			m_startTime = Time.time;
		}

		protected override BehaviourNodeStatus OnExecute(AIAgent agent)
		{
			float duration = m_duration.AsFloat.HasValue ? m_duration.AsFloat.Value : m_duration.Evaluate<float>(agent.Blackboard, 0.0f);

			if(Time.time < m_startTime + duration)
				return BehaviourNodeStatus.Running;

			return BehaviourNodeStatus.Success;
		}
	}
}