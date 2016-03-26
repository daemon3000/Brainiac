using UnityEngine;
using System;
using System.Collections;

namespace Brainiac
{
	[AddNodeMenu("Composite/Parallel")]
	public class Parallel : Composite
	{
		private bool m_failOnAny;
		private bool m_succeedOnAny;
		private bool m_failOnTie;

		public bool FailOnAny
		{
			get
			{
				return m_failOnAny;
			}
			set
			{
				m_failOnAny = value;
			}
		}

		public bool SucceedOnAny
		{
			get
			{
				return m_succeedOnAny;
			}
			set
			{
				m_succeedOnAny = value;
			}
		}

		public bool FailOnTie
		{
			get
			{
				return m_failOnTie;
			}
			set
			{
				m_failOnTie = value;
			}
		}

		public Parallel()
		{
			m_failOnAny = true;
			m_succeedOnAny = false;
			m_failOnTie = true;
		}

		protected override BehaviourNodeStatus OnExecute(AIController aiController)
		{
			BehaviourNodeStatus status = BehaviourNodeStatus.Success;
			int numberOfFailures = 0;
			int numberOfSuccesses = 0;
			int numberOfRunningChildren = 0;

			if(m_children.Count > 0)
			{
				for(int i = 0; i < m_children.Count; i++)
				{
					BehaviourNodeStatus childStatus = m_children[i].Status;
					if(childStatus == BehaviourNodeStatus.None || childStatus == BehaviourNodeStatus.Running)
					{
						childStatus = m_children[i].Run(aiController);
					}

					if(childStatus == BehaviourNodeStatus.Success)
						numberOfSuccesses++;
					else if(childStatus == BehaviourNodeStatus.Failure)
						numberOfFailures++;
					else if(childStatus == BehaviourNodeStatus.Running)
						numberOfRunningChildren++;
				}

				if((m_failOnAny && numberOfFailures > 0) || (m_succeedOnAny && numberOfSuccesses > 0))
				{
					if(m_failOnTie)
					{
						if(m_failOnAny && numberOfFailures > 0)
							status = BehaviourNodeStatus.Failure;
						else if(m_succeedOnAny && numberOfSuccesses > 0)
							status = BehaviourNodeStatus.Success;
					}
					else
					{
						if(m_succeedOnAny && numberOfSuccesses > 0)
							status = BehaviourNodeStatus.Success;
						else if(m_failOnAny && numberOfFailures > 0)
							status = BehaviourNodeStatus.Failure;
					}
				}
				else
				{
					if(numberOfSuccesses == m_children.Count)
						status = BehaviourNodeStatus.Success;
					else if(numberOfFailures == m_children.Count)
						status = BehaviourNodeStatus.Failure;
					else
						status = BehaviourNodeStatus.Running;
				}
			}

			return status;
		}
	}
}