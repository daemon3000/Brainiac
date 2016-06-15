using UnityEngine;
using Brainiac.Serialization;

namespace Brainiac.Examples
{
	[AddServiceMenu("Increment Value At Interval")]
	public class IncrementValueAtInterval : Service
	{
		[BTProperty("Value")]
		private string m_value;
		[BTProperty("Interval")]
		private MemoryVar m_interval;

		private float m_nextTime;

		public IncrementValueAtInterval()
		{
			m_interval = new MemoryVar();
		}

		public override void OnEnter(AIAgent agent)
		{
			m_nextTime = Time.time + GetInterval(agent);
		}

		public override void OnExecute(AIAgent agent)
		{
			if(Time.time >= m_nextTime)
			{
				IncrementValue(agent);
				m_nextTime = Time.time + GetInterval(agent);
			}
		}

		private float GetInterval(AIAgent agent)
		{
			return m_interval.AsFloat.HasValue ? m_interval.AsFloat.Value : m_interval.Evaluate<float>(agent.Memory, 0.0f);
		}

		private void IncrementValue(AIAgent agent)
		{
			int value = agent.Memory.GetItem<int>(m_value, 0);
			agent.Memory.SetItem(m_value, value + 1);
		}
	}
}