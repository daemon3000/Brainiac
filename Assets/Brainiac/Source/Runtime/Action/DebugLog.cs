using UnityEngine;

namespace Brainiac
{
	public enum LogLevel
	{
		Info, Warning, Error
	}

	public class DebugLog : Action
	{
		private string m_message;
		private LogLevel m_level;

		public string Message
		{
			get
			{
				return m_message;
			}
			set
			{
				m_message = value;
			}
		}
		
		public LogLevel Level
		{
			get
			{
				return m_level;
			}
			set
			{
				m_level = value;
			}
		}

		public override string Title
		{
			get
			{
				return "Debug";
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIController ai)
		{
			switch (m_level)
			{
				case LogLevel.Info:
					Debug.Log(m_message);
					break;
				case LogLevel.Warning:
					Debug.LogWarning(m_message);
					break;
				case LogLevel.Error:
					Debug.LogError(m_message);
					break;
			}

			return BehaviourNodeStatus.Success;
		}

		public override void OnGUI()
		{
			base.OnGUI();
#if UNITY_EDITOR
			m_level = (LogLevel)UnityEditor.EditorGUILayout.EnumPopup("Level", m_level);
			m_message = UnityEditor.EditorGUILayout.TextField("Message", m_message);
#endif
		}
	}
}
