using UnityEngine;
using System;
using Newtonsoft.Json;

namespace Brainiac
{
	public class Vector2Converter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if(value == null)
			{
				writer.WriteNull();
				return;
			}

			Vector2 vec = (Vector2)value;
			writer.WriteValue(string.Format("({0}, {1})", vec.x, vec.y));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if(reader.TokenType == JsonToken.Null)
			{
				if(!BTUtils.IsNullableType(objectType))
				{
					throw new JsonSerializationException(string.Format("Cannot convert null value to {0}.", objectType));
				}

				return null;
			}

			try
			{
				if(reader.TokenType == JsonToken.String)
				{
					string rawValue = reader.Value.ToString();
					string[] split = rawValue.Split(new char[] { '(', ')', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if(split.Length == 2)
					{
						Vector2 vec = new Vector2();
						vec.x = float.Parse(split[0]);
						vec.y = float.Parse(split[1]);

						return vec;
					}
				}
			}
			catch(System.Exception ex)
			{
				throw new JsonSerializationException(string.Format("Error converting value {0} to type '{1}'. {2}", reader.Value.GetType().FullName, typeof(int).FullName, ex.Message));
			}

			throw new JsonSerializationException(string.Format("Unexpected token {0} when parsing integer.", reader.TokenType));
		}

		public override bool CanConvert(Type objectType)
		{
			Type t = (BTUtils.IsNullableType(objectType)) ? Nullable.GetUnderlyingType(objectType) : objectType;
			return t == typeof(Vector2);
		}
	}
}