using UnityEngine;

namespace Brainiac
{
	public class Root : Decorator
	{
		public override string Title
		{
			get
			{
				return "Root";
			}
		}

		protected override BehaviourNodeStatus OnExecute(Agent agent)
		{
			if(m_child != null)
			{
				return m_child.Run(agent);
			}
			else
			{
				return BehaviourNodeStatus.Success;
			}
		}
	}
}