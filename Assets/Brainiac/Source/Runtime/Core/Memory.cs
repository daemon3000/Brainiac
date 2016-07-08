using UnityEngine;
using System;

namespace Brainiac
{
	//	The enum values should start at zero and have incremental values so they can be easily retreived
	//	from a SerializedProperty in editor scripts.
	public enum MemoryType
	{
		Boolean = 0, Integer, Float, String, Vector2, Vector3, GameObject, Asset
	}

	[Serializable]
	public class Memory
	{
		[SerializeField]
		private string m_name;
		[SerializeField]
		private MemoryType m_type;
		[SerializeField]
		private bool m_valueBool;
		[SerializeField]
		private int m_valueInteger;
		[SerializeField]
		private float m_valueFloat;
		[SerializeField]
		private string m_valueString;
		[SerializeField]
		private Vector2 m_valueVector2;
		[SerializeField]
		private Vector3 m_valueVector3;
		[SerializeField]
		private GameObject m_valueGameObject;
		[SerializeField]
		private UnityEngine.Object m_valueAsset;

		public string Name
		{
			get
			{
				return m_name;
			}
		}

		public MemoryType Type
		{
			get
			{
				return m_type;
			}
		}

		public object GetValue()
		{
			switch(m_type)
			{
			case MemoryType.Boolean:
				return m_valueBool;
			case MemoryType.Integer:
				return m_valueInteger;
			case MemoryType.Float:
				return m_valueFloat;
			case MemoryType.String:
				return m_valueString;
			case MemoryType.Vector2:
				return m_valueVector2;
			case MemoryType.Vector3:
				return m_valueVector3;
			case MemoryType.GameObject:
				return m_valueGameObject;
			case MemoryType.Asset:
				return m_valueAsset;
			}

			return null;
		}

		public void SetValue(object value, MemoryType type, bool canOverrideType)
		{
			if(canOverrideType)
			{
				m_type = type;
				ResetValues();
				SetValueInternal(value, type);
			}
			else
			{
				if(m_type == type)
					SetValueInternal(value, type);
			}
		}

		private void SetValueInternal(object value, MemoryType type)
		{
			switch(type)
			{
			case MemoryType.Boolean:
				m_valueBool = (bool)value;
				break;
			case MemoryType.Integer:
				m_valueInteger = (int)value;
				break;
			case MemoryType.Float:
				m_valueFloat = (float)value;
				break;
			case MemoryType.String:
				m_valueString = (string)value;
				break;
			case MemoryType.Vector2:
				m_valueVector2 = (Vector2)value;
				break;
			case MemoryType.Vector3:
				m_valueVector3 = (Vector3)value;
				break;
			case MemoryType.GameObject:
				m_valueGameObject = (GameObject)value;
				break;
			case MemoryType.Asset:
				m_valueAsset = (UnityEngine.Object)value;
				break;
			}
		}

		private void ResetValues()
		{
			m_valueBool = false;
			m_valueInteger = 0;
			m_valueFloat = 0.0f;
			m_valueString = null;
			m_valueVector2.Set(0, 0);
			m_valueVector3.Set(0, 0, 0);
			m_valueGameObject = null;
			m_valueAsset = null;
		}
	}
}
