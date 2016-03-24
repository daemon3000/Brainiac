using UnityEngine;
using Brainiac.Serialization;

namespace Brainiac
{
	[AddNodeMenu("Action/Timer")]
	public class Timer : Action
	{
		[JsonMember][JsonName("Duration")]
		[BTProperty(PropertyName = "Duration")]
		private MemoryVar m_duration;

		private float m_startTime;

		[JsonIgnore]
		public MemoryVar Duration
		{
			get
			{
				return m_duration;
			}
		}

		public override string Title
		{
			get
			{
				return "Timer";
			}
		}

		public Timer()
		{
			m_duration = new MemoryVar();
		}

		protected override BehaviourNodeStatus OnExecute(Agent agent)
		{
			float duration = m_duration.AsFloat.HasValue ? m_duration.AsFloat.Value : m_duration.Evaluate<float>(agent.Memory, 0.0f);

			if(Time.time < m_startTime + duration)
				return BehaviourNodeStatus.Running;

			return BehaviourNodeStatus.Success;
		}

		protected override void OnStart(Agent agent)
		{
			base.OnStart(agent);
			m_startTime = Time.time;
		}
	}
}