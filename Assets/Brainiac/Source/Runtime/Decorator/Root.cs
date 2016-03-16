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
			return m_child.Run(agent);
		}
	}
}