using UnityEngine;

namespace Brainiac
{
	public abstract class Condition : Action
	{
		protected sealed override BehaviourNodeStatus OnExecute(AIAgent agent)
		{
			bool result = EvaluateCondition(agent);
			return result ? BehaviourNodeStatus.Success : BehaviourNodeStatus.Failure;
		}

		protected abstract bool EvaluateCondition(AIAgent agent);
	}
}
