using UnityEngine;

namespace Brainiac.Examples
{
	public enum LogLevel
	{
		Info, Warning, Error
	}

	[AddNodeMenu("Action/Debug")]
	public class DebugLog : Action
	{
		private LogLevel m_level;
		private string m_message;
		
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

		public override string Title
		{
			get
			{
				return "Debug";
			}
		}

		protected override BehaviourNodeStatus OnExecute(AIAgent agent)
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
	}
}
