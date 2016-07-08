using UnityEngine;
using Brainiac.Serialization;

namespace Brainiac
{
	[AddConstraintMenu("Blackboard")]
	public class BlackboardConstraint : Constraint
	{
		[BTProperty("FirstValue")]
		private MemoryVar m_firstValue;
		[BTProperty("SecondValue")]
		private MemoryVar m_secondValue;
		[BTProperty("ConditionType")]
		private ConditionValueType m_valueType;
		[BTProperty("BooleanComparison")]
		private BooleanComparison m_booleanComparison;
		[BTProperty("NumericComparison")]
		private NumericComparison m_numericComparison;
		[BTProperty("ReferenceComparison")]
		private ReferenceComparison m_referenceComparison;

		public override string Title
		{
			get
			{
				return "Blackboard";
			}
		}

		[BTIgnore]
		public MemoryVar FirstValue
		{
			get { return m_firstValue; }
			set { m_firstValue = value; }
		}

		[BTIgnore]
		public MemoryVar SecondValue
		{
			get { return m_secondValue; }
			set { m_secondValue = value; }
		}

		[BTIgnore]
		public ConditionValueType ValueType
		{
			get { return m_valueType; }
			set { m_valueType = value; }
		}

		[BTIgnore]
		public BooleanComparison BooleanComparison
		{
			get { return m_booleanComparison; }
			set { m_booleanComparison = value; }
		}

		[BTIgnore]
		public NumericComparison NumericComparison
		{
			get { return m_numericComparison; }
			set { m_numericComparison = value; }
		}

		[BTIgnore]
		public ReferenceComparison ReferenceComparison
		{
			get { return m_referenceComparison; }
			set { m_referenceComparison = value; }
		}

		public BlackboardConstraint()
		{
			m_firstValue = new MemoryVar();
			m_secondValue = new MemoryVar();
			m_valueType = ConditionValueType.Boolean;
			m_booleanComparison = BooleanComparison.IsTrue;
			m_numericComparison = NumericComparison.Equal;
			m_referenceComparison = ReferenceComparison.IsNotNull;
		}

		protected override bool Evaluate(AIAgent agent)
		{
			switch(m_valueType)
			{
			case ConditionValueType.Boolean:
				return CompareBool(agent);
			case ConditionValueType.Integer:
				return CompareInteger(agent);
			case ConditionValueType.Float:
				return CompareFloat(agent);
			case ConditionValueType.GameObject:
				return CompareGameObject(agent);
			case ConditionValueType.UnityObject:
				return CompareUnityObject(agent);
			}

			return false;
		}

		private bool CompareBool(AIAgent agent)
		{
			bool value = false;

			if(m_firstValue.AsBool.HasValue)
				value = m_firstValue.AsBool.Value;
			else
				value = m_firstValue.Evaluate<bool>(agent.Blackboard, false);

			return (m_booleanComparison == BooleanComparison.IsTrue) ? value : !value;
		}

		private bool CompareInteger(AIAgent agent)
		{
			int? firstValue = null;
			int? secondValue = null;

			if(m_firstValue.AsInt.HasValue)
				firstValue = m_firstValue.AsInt.Value;
			else if(m_firstValue.HasValue<int>(agent.Blackboard))
				firstValue = m_firstValue.Evaluate<int>(agent.Blackboard, 0);
			if(m_secondValue.AsInt.HasValue)
				secondValue = m_secondValue.AsInt.Value;
			else if(m_firstValue.HasValue<int>(agent.Blackboard))
				secondValue = m_secondValue.Evaluate<int>(agent.Blackboard, 0);

			if(firstValue.HasValue && secondValue.HasValue)
			{
				switch(m_numericComparison)
				{
				case NumericComparison.Equal:
					return firstValue.Value == secondValue.Value;
				case NumericComparison.Less:
					return firstValue.Value < secondValue.Value;
				case NumericComparison.LessThanOrEqual:
					return firstValue.Value <= secondValue.Value;
				case NumericComparison.Greater:
					return firstValue.Value > secondValue.Value;
				case NumericComparison.GreaterThanOrEqual:
					return firstValue.Value >= secondValue.Value;
				}
			}

			return false;
		}

		private bool CompareFloat(AIAgent agent)
		{
			float? firstValue = null;
			float? secondValue = null;

			if(m_firstValue.AsFloat.HasValue)
				firstValue = m_firstValue.AsFloat.Value;
			else if(m_firstValue.HasValue<float>(agent.Blackboard))
				firstValue = m_firstValue.Evaluate<float>(agent.Blackboard, 0);
			if(m_secondValue.AsFloat.HasValue)
				secondValue = m_secondValue.AsFloat.Value;
			else if(m_firstValue.HasValue<float>(agent.Blackboard))
				secondValue = m_secondValue.Evaluate<float>(agent.Blackboard, 0);

			if(firstValue.HasValue && secondValue.HasValue)
			{
				switch(m_numericComparison)
				{
				case NumericComparison.Equal:
					return firstValue.Value == secondValue.Value;
				case NumericComparison.Less:
					return firstValue.Value < secondValue.Value;
				case NumericComparison.LessThanOrEqual:
					return firstValue.Value <= secondValue.Value;
				case NumericComparison.Greater:
					return firstValue.Value > secondValue.Value;
				case NumericComparison.GreaterThanOrEqual:
					return firstValue.Value >= secondValue.Value;
				}
			}

			return false;
		}

		private bool CompareGameObject(AIAgent agent)
		{
			GameObject value = m_firstValue.Evaluate<GameObject>(agent.Blackboard, null);
			return (m_referenceComparison == ReferenceComparison.IsNull) ? value == null : value != null;
		}

		private bool CompareUnityObject(AIAgent agent)
		{
			UnityEngine.Object value = m_firstValue.Evaluate<UnityEngine.Object>(agent.Blackboard, null);
			return (m_referenceComparison == ReferenceComparison.IsNull) ? value == null : value != null;
		}
	}
}
