using Brainiac.Serialization;

namespace Brainiac
{
	[AddNodeMenu("Decorator/Repeat")]
	public class Repeat : Decorator
	{
		[JsonMember("RepeatCount")]
		[BTProperty("Repeat Count")]
		private int m_repeatCount;

		private int m_currentIteration;

		protected override void OnEnter(Agent agent)
		{
			m_currentIteration = 0;
		}

		protected override BehaviourNodeStatus OnExecute(Agent agent)
		{
			BehaviourNodeStatus status = BehaviourNodeStatus.Success;

			if(m_child != null && m_repeatCount >= 0)
			{
				if(m_currentIteration <= m_repeatCount)
				{
					if(m_child.Status != BehaviourNodeStatus.None && m_child.Status != BehaviourNodeStatus.Running)
					{
						m_child.OnReset();
					}

					status = m_child.Run(agent);
					if(status == BehaviourNodeStatus.Success)
					{
						m_currentIteration++;
						if(m_currentIteration <= m_repeatCount)
						{
							status = BehaviourNodeStatus.Running;
						}
					}
				}
			}

			return status;
		}
	}
}