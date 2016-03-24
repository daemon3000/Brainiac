using UnityEngine;
using Brainiac.Serialization;

namespace Brainiac
{
	public class MemoryVar : IJsonSerializable
	{
		private string m_content;
		private bool? m_valueAsBool;
		private int? m_valueAsInt;
		private float? m_valueAsFloat;

		public string Content
		{
			get
			{
				return m_content;
			}
			set
			{
				m_content = value;
			}
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
			get { return m_content; }
		}

		public MemoryVar()
		{
			m_content = "";
		}

		public T Evaluate<T>(Memory memory, T defValue)
		{
			if(!string.IsNullOrEmpty(m_content))
			{
				return memory.GetItem<T>(Content, defValue);
			}

			return defValue;
		}

		private void ParseContent()
		{
			bool valueAsBool = false;
			int valueAsInt = 0;
			float valueAsFloat = 0.0f;

			m_valueAsBool = null;
			m_valueAsInt = null;
			m_valueAsFloat = null;

			if(bool.TryParse(m_content, out valueAsBool))
			{
				m_valueAsBool = valueAsBool;
			}
			if(int.TryParse(m_content, out valueAsInt))
			{
				m_valueAsInt = valueAsInt;
			}
			if(float.TryParse(m_content, out valueAsFloat))
			{
				m_valueAsFloat = valueAsFloat;
			}
		}

		public void ReadJson(JsonReader reader)
		{
			Content = (string)reader.Read(typeof(string), false, false);
			ParseContent();
		}

		public void WriteJson(JsonWriter writer)
		{
			writer.Write(Content);
		}
	}
}