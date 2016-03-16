using UnityEngine;
using Newtonsoft.Json;

namespace Brainiac
{
	public abstract class BehaviourNode
	{
		private Vector2 m_position;
		private string m_description;
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

		public abstract string Title { get; }

		[JsonIgnore]
		public abstract Vector2 Size { get; }

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

		public virtual void OnGUI()
		{
#if UNITY_EDITOR
			UnityEditor.EditorStyles.textField.wordWrap = true;

			UnityEditor.EditorGUILayout.LabelField("Description");
			m_description = UnityEditor.EditorGUILayout.TextArea(m_description);
			UnityEditor.EditorGUILayout.Space();
#endif
		}
	}
}
