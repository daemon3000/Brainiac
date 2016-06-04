using Brainiac.Serialization;

namespace Brainiac
{
	[AddNodeMenu("Utility/Node Group")]
	public class NodeGroup : Decorator
	{
		public override string Title
		{
			get
			{
				return "NodeGroup";
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIAgent agent)
		{
			if(m_child != null)
			{
				return m_child.Run(agent);
			}

			return BehaviourNodeStatus.Success;
		}
	}
}