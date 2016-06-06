using UnityEngine;
using Brainiac.Serialization;

namespace Brainiac
{
	public class MemoryVar : IJsonSerializable
	{
		private string m_value;

		private bool? m_valueAsBool;
		private int? m_valueAsInt;
		private float? m_valueAsFloat;
		private string m_valueAsVariableName;
		private string m_valueAsString;

		public string Value
		{
			get { return m_value; }
			set { m_value = value; }
		}

		public bool? AsBool
		{
			get { return m_valueAsBool; }
		}

		public int? AsInt
		{
			get { return m_valueAsInt; }
		}

		public float? AsFloat
		{
			get { return m_valueAsFloat; }
		}

		public string AsString
		{
			get { return m_valueAsString; }
		}

		public string AsVariableName
		{
			get { return m_valueAsVariableName; }
		}

		public MemoryVar()
		{
			m_value = "";
		}

		public T Evaluate<T>(Memory memory, T defValue)
		{
			if(!string.IsNullOrEmpty(m_valueAsVariableName))
			{
				return memory.GetItem<T>(m_valueAsVariableName, defValue);
			}

			return defValue;
		}

		public bool HasValue<T>(Memory memory)
		{
			return memory.HasItem<T>(m_valueAsVariableName);
		}

		private void ParseContent()
		{
			bool valueAsBool = false;
			int valueAsInt = 0;
			float valueAsFloat = 0.0f;

			m_valueAsBool = null;
			m_valueAsInt = null;
			m_valueAsFloat = null;
			m_valueAsString = null;
			m_valueAsVariableName = null;

			if(string.IsNullOrEmpty(m_value))
				return;

			if(m_value[0] == '"' && m_value[m_value.Length - 1] == '"')
			{
				m_valueAsString = m_value.Substring(1, m_value.Length - 2);
			}
			else
			{
				m_valueAsVariableName = m_value;

				if(bool.TryParse(m_value, out valueAsBool))
				{
					m_valueAsBool = valueAsBool;
				}
				if(int.TryParse(m_value, out valueAsInt))
				{
					m_valueAsInt = valueAsInt;
				}
				if(float.TryParse(m_value, out valueAsFloat))
				{
					m_valueAsFloat = valueAsFloat;
				}
			}
		}

		public void ReadJson(JsonReader reader)
		{
			m_value = (string)reader.Read(typeof(string), false, false);
			ParseContent();
		}

		public void WriteJson(JsonWriter writer)
		{
			writer.Write(m_value);
		}
	}
}