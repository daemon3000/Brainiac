using UnityEngine;
using Newtonsoft.Json;

namespace Brainiac
{
	public abstract class BehaviourNode
	{
		private RepeatMode m_repeatMode;
		private Vector2 m_position;
		private string m_description;
		private BehaviourNodeStatus m_executionStatus;
		private bool m_resetRepeat;
		private bool m_hasStarted;
		
		public RepeatMode RepeatMode
		{
			get
			{
				return m_repeatMode;
			}
			set
			{
				m_repeatMode = value;
			}
		}
		
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

		public abstract string Title { get; }

		[JsonIgnore]
		public abstract Vector2 Size { get; }

		public virtual void OnInitialize()
		{
			m_executionStatus = BehaviourNodeStatus.Failure;
			m_hasStarted = false;
			m_resetRepeat = true;
		}

		public virtual void OnReset() { }
		protected virtual void OnDestroy() { }
		protected virtual void OnStart(AIController ai) { }
		protected virtual void OnStop(AIController ai) { }
		protected abstract BehaviourNodeStatus OnExecute(AIController ai);

		public BehaviourNodeStatus Run(AIController ai)
		{
			if (m_resetRepeat)
			{
				OnReset();
				m_resetRepeat = false;
				m_hasStarted = false;
			}

			if (!m_hasStarted)
			{
				OnStart(ai);
				m_hasStarted = true;
			}

			m_executionStatus = OnExecute(ai);

			if (m_executionStatus != BehaviourNodeStatus.Running)
			{
				OnStop(ai);

				m_resetRepeat = true;
				switch (m_repeatMode)
				{
					case RepeatMode.Forever:
						m_executionStatus = BehaviourNodeStatus.Running;
						break;
					case RepeatMode.UntilFailure:
						if (m_executionStatus != BehaviourNodeStatus.Failure)
							m_executionStatus = BehaviourNodeStatus.Running;
						break;
					case RepeatMode.UntilSuccess:
						if (m_executionStatus != BehaviourNodeStatus.Success)
							m_executionStatus = BehaviourNodeStatus.Running;
						break;
					default:
						break;
				}
			}

			return m_executionStatus;
		}

		public virtual void OnGUI()
		{
#if UNITY_EDITOR
			UnityEditor.EditorStyles.textField.wordWrap = true;

			UnityEditor.EditorGUILayout.LabelField("Description");
			m_description = UnityEditor.EditorGUILayout.TextArea(m_description);
			UnityEditor.EditorGUILayout.Space();

			//UnityEditor.EditorStyles.textField.wordWrap = false;
#endif
		}
	}
}
