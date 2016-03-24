using UnityEngine;
using Brainiac.Serialization;

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
			m_status = BehaviourNodeStatus.None;
		}

		public virtual void OnAwake() { }
		protected virtual void OnEnter(Agent agent) { }
		protected virtual void OnExit(Agent agent) { }
		protected abstract BehaviourNodeStatus OnExecute(Agent agent);

		public virtual void OnReset()
		{
			m_status = BehaviourNodeStatus.None;
		}

		public BehaviourNodeStatus Run(Agent agent)
		{
			if(m_status != BehaviourNodeStatus.Running)
			{
				OnEnter(agent);
#if UNITY_EDITOR
				if(agent.DebugMode)
				{
					if(m_debugOptions.Has(DebugOptions.BreakOnEnter))
					{
						Debug.Break();
					}
				}
#endif
			}

			m_status = OnExecute(agent);

			if(m_status != BehaviourNodeStatus.Running)
			{
				OnExit(agent);
#if UNITY_EDITOR
				if(agent.DebugMode)
				{
					if((m_status == BehaviourNodeStatus.Success && m_debugOptions.Has(DebugOptions.BreakOnSuccess)) ||
						(m_status == BehaviourNodeStatus.Failure && m_debugOptions.Has(DebugOptions.BreakOnFailure)) ||
						m_debugOptions.Has(DebugOptions.BreakOnExit))
					{
						Debug.Break();
					}
				}
#endif
			}

			return m_status;
		}
	}
}
