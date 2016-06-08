using Brainiac.Serialization;

namespace Brainiac
{
	[AddNodeMenu("Decorator/Abort")]
	public class Abort : Decorator
	{
		[BTProperty("Trigger")]
		private string m_trigger;

		protected sealed override BehaviourNodeStatus OnExecute(AIAgent agent)
		{
			BehaviourNodeStatus status = BehaviourNodeStatus.Success;
			bool trigger = agent.Memory.GetItem<bool>(m_trigger, false);

			if(trigger)
			{
				status = BehaviourNodeStatus.Failure;
				agent.Memory.SetItem(m_trigger, false);
			}
			else
			{
				if(m_child != null)
				{
					status = m_child.Run(agent);
				}
			}

			return status;
		}
	}
}
