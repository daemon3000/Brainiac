using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Brainiac
{
	public class MemoryVar
	{
		[JsonProperty]
		private string m_content;

		private bool? m_valueAsBool;
		private int? m_valueAsInt;
		private float? m_valueAsFloat;

		[JsonIgnore]
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

		[JsonIgnore]
		public bool? AsBool
		{
			get { return m_valueAsBool; }
		}

		[JsonIgnore]
		public int? AsInt
		{
			get { return m_valueAsInt; }
		}

		[JsonIgnore]
		public float? AsFloat
		{
			get { return m_valueAsFloat; }
		}

		[JsonIgnore]
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

		[OnDeserialized]
		private void OnContentDeserialized(StreamingContext steamingContext)
		{
			ParseContent();
		}
	}
}