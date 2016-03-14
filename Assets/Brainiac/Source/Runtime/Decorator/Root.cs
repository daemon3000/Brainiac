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

		protected override BehaviourNodeStatus OnExecute(AIController ai)
		{
			return m_child.Run(ai);
		}
	}
}