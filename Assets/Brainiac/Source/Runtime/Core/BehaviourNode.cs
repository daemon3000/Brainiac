using UnityEngine;
using Newtonsoft.Json;

namespace Brainiac
{
	public abstract class BehaviourNode
	{
		private Vector2 m_position;
		private string m_description;
		private string m_name;
		private DebugOptions m_debugOptions;
		private BehaviourNodeStatus m_status;

		public Vector2 Position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
			}
		}

		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public string Description
		{
			get
			{
				return m_description;
			}
			set
			{
				m_description = value;
			}
		}

		public DebugOptions DebugOptions
		{
			get
			{
				return m_debugOptions;
			}
			set
			{
				m_debugOptions = value;
			}
		}

		[JsonIgnore]
		public virtual string Title
		{
			get { return GetType().Name; }
		}

		[JsonIgnore]
		public BehaviourNodeStatus Status
		{
			get { return m_status; }
		}

		public BehaviourNode()
		{
			m_debugOptions = DebugOptions.None;
		}

		protected abstract BehaviourNodeStatus OnExecute(Agent agent);
		protected virtual void OnStop(Agent agent) { }

		protected virtual void OnStart(Agent agent)
		{
			m_status = BehaviourNodeStatus.Failure;	
		}

		public BehaviourNodeStatus Run(Agent agent)
		{
			if(m_status != BehaviourNodeStatus.Running)
			{
				OnStart(agent);

				if(agent.DebugMode)
				{
					if(m_debugOptions.Has(DebugOptions.BreakOnEnter))
					{
						Debug.Break();
					}
				}
			}

			m_status = OnExecute(agent);

			if(m_status != BehaviourNodeStatus.Running)
			{
				OnStop(agent);

				if(agent.DebugMode)
				{
					if((m_status == BehaviourNodeStatus.Success && m_debugOptions.Has(DebugOptions.BreakOnSuccess)) ||
						(m_status == BehaviourNodeStatus.Failure && m_debugOptions.Has(DebugOptions.BreakOnFailure)) ||
						m_debugOptions.Has(DebugOptions.BreakOnExit))
					{
						Debug.Break();
					}
				}
			}

			return m_status;
		}
	}
}
