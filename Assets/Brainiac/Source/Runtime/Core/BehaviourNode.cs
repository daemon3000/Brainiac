using UnityEngine;
using Newtonsoft.Json;

namespace Brainiac
{
	public abstract class BehaviourNode
	{
		private Vector2 m_position;
		private string m_description;
		private string m_name;
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

		[JsonIgnore]
		public BehaviourNodeStatus Status
		{
			get { return m_status; }
		}

		[JsonIgnore]
		public abstract Vector2 Size { get; }

		[JsonIgnore]
		public abstract string Title { get; }

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
			}

			m_status = OnExecute(agent);

			if(m_status != BehaviourNodeStatus.Running)
			{
				OnStop(agent);
			}

			return m_status;
		}
	}
}
