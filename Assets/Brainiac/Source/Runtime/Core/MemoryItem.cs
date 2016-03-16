using UnityEngine;
using System;

namespace Brainiac
{
	[Serializable]
	public class MemoryItem
	{
		//	The enum values should start at zero and have incremental values so they can be easily retreived
		//	from a SerializedProperty in editor scripts.
		public enum ItemType
		{
			Boolean = 0, Integer, Float, String, Vector2, Vector3, GameObject, Asset
		}

		[SerializeField]
		private string m_name;
		[SerializeField]
		private ItemType m_type;
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

		public ItemType Type
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
			case ItemType.Boolean:
				return m_valueBool;
			case ItemType.Integer:
				return m_valueInteger;
			case ItemType.Float:
				return m_valueFloat;
			case ItemType.String:
				return m_valueString;
			case ItemType.Vector2:
				return m_valueVector2;
			case ItemType.Vector3:
				return m_valueVector3;
			case ItemType.GameObject:
				return m_valueGameObject;
			case ItemType.Asset:
				return m_valueAsset;
			}

			return null;
		}

		public void SetValue(object value, ItemType type, bool canOverrideType)
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

		private void SetValueInternal(object value, ItemType type)
		{
			switch(type)
			{
			case ItemType.Boolean:
				m_valueBool = (bool)value;
				break;
			case ItemType.Integer:
				m_valueInteger = (int)value;
				break;
			case ItemType.Float:
				m_valueFloat = (float)value;
				break;
			case ItemType.String:
				m_valueString = (string)value;
				break;
			case ItemType.Vector2:
				m_valueVector2 = (Vector2)value;
				break;
			case ItemType.Vector3:
				m_valueVector3 = (Vector3)value;
				break;
			case ItemType.GameObject:
				m_valueGameObject = (GameObject)value;
				break;
			case ItemType.Asset:
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
