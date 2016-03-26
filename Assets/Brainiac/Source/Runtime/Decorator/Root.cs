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

		protected override BehaviourNodeStatus OnExecute(AIController aiController)
		{
			if(m_child != null)
			{
				return m_child.Run(aiController);
			}

			return BehaviourNodeStatus.Success;
		}
	}
}