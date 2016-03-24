#region License
/*---------------------------------------------------------------------------------*\

	Distributed under the terms of an MIT-style license:

	The MIT License

	Copyright (c) 2006-2010 Stephen M. McKamey

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.

\*---------------------------------------------------------------------------------*/
#endregion License

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
#if !UNITY3D
using System.Xml;
#endif

#if WINDOWS_STORE
using TP = System.Reflection.TypeInfo;
#else
using TP = System.Type;
#endif

using TCU = Brainiac.Serialization.TypeCoercionUtility;

namespace Brainiac.Serialization
{
	/// <summary>
	/// Writer for producing JSON data
	/// </summary>
	public class JsonWriter : IDisposable
	{
		#region Constants

		public const string JsonMimeType = "application/json";
		public const string JsonFileExtension = ".json";

		internal const string AnonymousTypePrefix = "<>f__AnonymousType";
		private const string ErrorMaxDepth = "The maxiumum depth of {0} was exceeded. Check for cycles in object graph.";
		private const string ErrorIDictionaryEnumerator = "Types which implement Generic IDictionary<TKey, TValue> must have an IEnumerator which implements IDictionaryEnumerator. ({0})";

		private const BindingFlags defaultBinding = BindingFlags.Default;
		private const BindingFlags allBinding = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		#endregion Constants

		#region Fields

		private readonly TextWriter Writer;
		private JsonWriterSettings settings;
		private int depth;
		private Dictionary<object,int> previouslySerializedObjects;

		public ReferenceHandlerWriter referenceHandler;

		#endregion Fields

		#region Init

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="output">TextWriter for writing</param>
		public JsonWriter(TextWriter output)
			: this(output, new JsonWriterSettings())
		{
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="output">TextWriter for writing</param>
		/// <param name="settings">JsonWriterSettings</param>
		public JsonWriter (TextWriter output, JsonWriterSettings settings)
		{
			if (output == null) {
				throw new ArgumentNullException ("output");
			}
			if (settings == null) {
				throw new ArgumentNullException ("settings");
			}

			this.Writer = output;
			this.settings = settings;
			this.Writer.NewLine = this.settings.NewLine;
			
			if (settings.HandleCyclicReferences)
			{
				this.previouslySerializedObjects = new Dictionary<object, int> ();
			}
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="output">Stream for writing</param>
		public JsonWriter(Stream output)
			: this(output, new JsonWriterSettings())
		{
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="output">Stream for writing</param>
		/// <param name="settings">JsonWriterSettings</param>
		public JsonWriter(Stream output, JsonWriterSettings settings)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}

			this.Writer = new StreamWriter(output, Encoding.UTF8);
			this.settings = settings;
			this.Writer.NewLine = this.settings.NewLine;
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="output">file name for writing</param>
		public JsonWriter(string outputFileName)
			: this(outputFileName, new JsonWriterSettings())
		{
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="output">file name for writing</param>
		/// <param name="settings">JsonWriterSettings</param>
		public JsonWriter(string outputFileName, JsonWriterSettings settings)
		{
#if WINDOWS_STORE && !DEBUG
			throw new System.NotSupportedException ("Not supported on this platform");
#else
			if (outputFileName == null)
			{
				throw new ArgumentNullException("outputFileName");
			}
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}

			Stream stream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
			this.Writer = new StreamWriter(stream, Encoding.UTF8);
			this.settings = settings;
			this.Writer.NewLine = this.settings.NewLine;
#endif
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="output">StringBuilder for appending</param>
		public JsonWriter(StringBuilder output)
			: this(output, new JsonWriterSettings())
		{
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="output">StringBuilder for appending</param>
		/// <param name="settings">JsonWriterSettings</param>
		public JsonWriter(StringBuilder output, JsonWriterSettings settings)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}

			this.Writer = new StringWriter(output, System.Globalization.CultureInfo.InvariantCulture);
			this.settings = settings;
			this.Writer.NewLine = this.settings.NewLine;
		}

		#endregion Init

		#region Properties

		/// <summary>
		/// Gets the underlying TextWriter
		/// </summary>
		public TextWriter TextWriter
		{
			get { return this.Writer; }
		}

		/// <summary>
		/// Gets and sets the JsonWriterSettings
		/// </summary>
		public JsonWriterSettings Settings
		{
			get { return this.settings; }
			set
			{
				if (value == null)
				{
					value = new JsonWriterSettings();
				}
				this.settings = value;
			}
		}

		#endregion Properties

		#region Static Methods

		/// <summary>
		/// A helper method for serializing an object to JSON
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string Serialize(object value)
		{
			StringBuilder output = new StringBuilder();

			using (JsonWriter writer = new JsonWriter(output))
			{
				writer.Write(value);
			}

			return output.ToString();
		}

		#endregion Static Methods

		#region Public Methods

		public void Write(object value)
		{
			this.Write(value, false);
		}

		public void WriteSkipConverters (object value)
		{
			this.Write(value, false, null, true);
		}

		protected virtual void Write(object value, bool isProperty, Type fieldType = null, bool skipConverters = false)
		{
			if (isProperty && this.settings.PrettyPrint)
			{
				this.Writer.Write(' ');
			}

			if (value == null)
			{
				this.Writer.Write(JsonReader.LiteralNull);
				return;
			}

			if (value is IJsonSerializable && !skipConverters)
			{
				try
				{
					if (isProperty)
					{
						this.depth++;
						if (this.depth > this.settings.MaxDepth)
						{
							throw new JsonSerializationException(String.Format(JsonWriter.ErrorMaxDepth, new System.Object[] {this.settings.MaxDepth}));
						}
						this.WriteLine();
					}
					((IJsonSerializable)value).WriteJson(this);
				}
				finally
				{
					if (isProperty)
					{
						this.depth--;
					}
				}
				return;
			}
			
			// must test enumerations before value types
			if (value is Enum)
			{
				this.Write((Enum)value);
				return;
			}

			// Type.GetTypeCode() allows us to more efficiently switch type
			// plus cannot use 'is' for ValueTypes
			Type type = value.GetType();
				
#if WINDOWS_STORE
			if (Type.Equals (type, typeof(bool)))
				{
					this.Write((Boolean)value);
					return;
				}
			else if (Type.Equals (type, typeof(System.Int32)))
			{
				this.Write((Int32)value);
				return;
			}
			else if (Type.Equals (type, typeof(System.Single)))
			{
				this.Write((Single)value);
				return;
			}
			else if (Type.Equals (type, typeof(System.String)))
			{
				this.Write((String)value);
				return;
			}
			else if (Type.Equals (type, typeof(byte)))
				{
					this.Write((Byte)value);
					return;
				}
			else if (Type.Equals (type, typeof(char)))
				{
					this.Write((Char)value);
					return;
				}
			else if (Type.Equals (type, typeof(DateTime)))
				{
					this.Write((DateTime)value);
					return;
				}
			else if (Type.Equals (type, typeof(DBNull)) || Type.Equals (type, null))
				{
					this.Writer.Write(JsonReader.LiteralNull);
					return;
				}
			else if (Type.Equals (type, typeof(Decimal)))
				{
					// From MSDN:
					// Conversions from Char, SByte, Int16, Int32, Int64, Byte, UInt16, UInt32, and UInt64
					// to Decimal are widening conversions that never lose information or throw exceptions.
					// Conversions from Single or Double to Decimal throw an OverflowException
					// if the result of the conversion is not representable as a Decimal.
					this.Write((Decimal)value);
					return;
				}
			else if (Type.Equals (type, typeof(double)))
				{
					this.Write((Double)value);
					return;
				}
			else if (Type.Equals (type, typeof(System.Int16)))
				{
					this.Write((Int16)value);
					return;
				}
			else if (Type.Equals (type, typeof(System.Int64)))
				{
					this.Write((Int64)value);
					return;
				}
			else if (Type.Equals (type, typeof(System.SByte)))
				{
					this.Write((SByte)value);
					return;
				}
			else if (Type.Equals (type, typeof(System.UInt16)))
				{
					this.Write((UInt16)value);
					return;
				}
			else if (Type.Equals (type, typeof(System.UInt32)))
				{
					this.Write((UInt32)value);
					return;
				}
			else if (Type.Equals (type, typeof(System.UInt64)))
				{
					this.Write((UInt64)value);
					return;
				}
			else
				{
					// all others must be explicitly tested
			}
#else
			// Faster to switch on typecode, but Windows Store does not support it
			switch (TP.GetTypeCode(type))
			{
				case TypeCode.Boolean:
				{
					this.Write((Boolean)value);
					return;
				}
				case TypeCode.Byte:
				{
					this.Write((Byte)value);
					return;
				}
				case TypeCode.Char:
				{
					this.Write((Char)value);
					return;
				}
				case TypeCode.DateTime:
				{
					this.Write((DateTime)value);
					return;
				}
				case TypeCode.DBNull:
				case TypeCode.Empty:
				{
					this.Writer.Write(JsonReader.LiteralNull);
					return;
				}
				case TypeCode.Decimal:
				{
					// From MSDN:
					// Conversions from Char, SByte, Int16, Int32, Int64, Byte, UInt16, UInt32, and UInt64
					// to Decimal are widening conversions that never lose information or throw exceptions.
					// Conversions from Single or Double to Decimal throw an OverflowException
					// if the result of the conversion is not representable as a Decimal.
					this.Write((Decimal)value);
					return;
				}
				case TypeCode.Double:
				{
					this.Write((Double)value);
					return;
				}
				case TypeCode.Int16:
				{
					this.Write((Int16)value);
					return;
				}
				case TypeCode.Int32:
				{
					this.Write((Int32)value);
					return;
				}
				case TypeCode.Int64:
				{
					this.Write((Int64)value);
					return;
				}
				case TypeCode.SByte:
				{
					this.Write((SByte)value);
					return;
				}
				case TypeCode.Single:
				{
					this.Write((Single)value);
					return;
				}
				case TypeCode.String:
				{
					this.Write((String)value);
					return;
				}
				case TypeCode.UInt16:
				{
					this.Write((UInt16)value);
					return;
				}
				case TypeCode.UInt32:
				{
					this.Write((UInt32)value);
					return;
				}
				case TypeCode.UInt64:
				{
					this.Write((UInt64)value);
					return;
				}
				default:
				case TypeCode.Object:
				{
					// all others must be explicitly tested
					break;
				}
			}
#endif

			if (!skipConverters) {
				JsonConverter converter = this.Settings.GetConverter (type);
				if (converter != null) {
					// Return if converter serialized the object
					if (converter.Write (this, depth, type, value)) {
						return;
					}
				}
			}

			if (value is Guid)
			{
				this.Write((Guid)value);
				return;
			}

			if (value is Uri)
			{
				this.Write((Uri)value);
				return;
			}

			if (value is TimeSpan)
			{
				this.Write((TimeSpan)value);
				return;
			}

			if (value is Version)
			{
				this.Write((Version)value);
				return;
			}

			// IDictionary test must happen BEFORE IEnumerable test
			// since IDictionary implements IEnumerable
			if (value is IDictionary)
			{
				try
				{
					if (isProperty)
					{
						this.depth++;
						if (this.depth > this.settings.MaxDepth)
						{
							throw new JsonSerializationException(String.Format(JsonWriter.ErrorMaxDepth, new System.Object[] {this.settings.MaxDepth}));
						}
						this.WriteLine();
					}
					this.WriteObject((IDictionary)value);
				}
				finally
				{
					if (isProperty)
					{
						this.depth--;
					}
				}
				return;
			}
				
			//if (!Type.Equals (TCU.GetTypeInfo(type).GetInterface (JsonReader.TypeGenericIDictionary), null))
			if ( TCU.GetTypeInfo(typeof(IDictionary)).IsAssignableFrom (TCU.GetTypeInfo(value.GetType())) )
			{
				try {
					if (isProperty) {
						this.depth++;
						if (this.depth > this.settings.MaxDepth) {
							throw new JsonSerializationException (String.Format (JsonWriter.ErrorMaxDepth, new System.Object[] { this.settings.MaxDepth }));
						}
						this.WriteLine ();
					}

					this.WriteDictionary ((IEnumerable)value);
				} finally {
					if (isProperty) {
						this.depth--;
					}
				}
				return;
			}

			if (value is IList && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
				// Is List<T>
				var memberType = type.GetGenericArguments () [0];

				try
				{
					if (isProperty)
					{
						this.depth++;
						if (this.depth > this.settings.MaxDepth)
						{
							throw new JsonSerializationException(String.Format(JsonWriter.ErrorMaxDepth, new System.Object[] {this.settings.MaxDepth}));
						}
						this.WriteLine();
					}

					this.WriteArray((IEnumerable)value, memberType);
				}
				finally
				{
					if (isProperty)
					{
						this.depth--;
					}
				}
				return;
			}

			// IDictionary test must happen BEFORE IEnumerable test
			// since IDictionary implements IEnumerable
			if (value is IEnumerable)
			{
#if !UNITY3D
				if (value is XmlNode)
				{
					this.Write((System.Xml.XmlNode)value);
					return;
				}
#endif
				try
				{
					if (isProperty)
					{
						this.depth++;
						if (this.depth > this.settings.MaxDepth)
						{
							throw new JsonSerializationException(String.Format(JsonWriter.ErrorMaxDepth, new System.Object[] {this.settings.MaxDepth}));
						}
						this.WriteLine();
					}
						
					this.WriteArray((IEnumerable)value, type.GetElementType());
				}
				finally
				{
					if (isProperty)
					{
						this.depth--;
					}
				}
				return;
			}

			// structs and classes
			try
			{
				if (isProperty)
				{
					this.depth++;
					if (this.depth > this.settings.MaxDepth)
					{
						throw new JsonSerializationException(String.Format(JsonWriter.ErrorMaxDepth, new System.Object[] {this.settings.MaxDepth}));
					}
					this.WriteLine();
				}
				this.WriteObject(value, type,true, fieldType);
			}
			finally
			{
				if (isProperty)
				{
					this.depth--;
				}
			}
		}

		public virtual void WriteBase64(byte[] value)
		{
			this.Write(Convert.ToBase64String(value));
		}

		public virtual void WriteHexString(byte[] value)
		{
			if (value == null || value.Length == 0)
			{
				this.Write(String.Empty);
				return;
			}

			StringBuilder builder = new StringBuilder();

			// Loop through each byte of the binary data 
			// and format each one as a hexadecimal string
			for (int i=0; i<value.Length; i++)
			{
				builder.Append(value[i].ToString("x2"));
			}

			// the hexadecimal string
			this.Write(builder.ToString());
		}

		public virtual void Write(DateTime value)
		{
			if (this.settings.DateTimeSerializer != null)
			{
				this.settings.DateTimeSerializer(this, value);
				return;
			}

			switch (value.Kind)
			{
				case DateTimeKind.Local:
				{
					value = value.ToUniversalTime();
					goto case DateTimeKind.Utc;
				}
				case DateTimeKind.Utc:
				{
					// UTC DateTime in ISO-8601
					this.Write(String.Format("{0:s}Z", new System.Object[] {value}));
					break;
				}
				default:
				{
					// DateTime in ISO-8601
					this.Write(String.Format("{0:s}", new System.Object[] {value}));
					break;
				}
			}
		}

		public virtual void Write(Guid value)
		{
			this.Write(value.ToString("D"));
		}

		public virtual void Write(Enum value)
		{
			string enumName = null;

			Type type = value.GetType();

			if (type.IsDefined(typeof(FlagsAttribute), true) && !Enum.IsDefined(type, value))
			{
				Enum[] flags = JsonWriter.GetFlagList(type, value);
				string[] flagNames = new string[flags.Length];
				for (int i=0; i<flags.Length; i++)
				{
					flagNames[i] = JsonNameAttribute.GetJsonName(flags[i]);
					if (String.IsNullOrEmpty(flagNames[i]))
					{
						flagNames[i] = flags[i].ToString("f");
					}
				}
				enumName = String.Join(", ", flagNames);
			}
			else
			{
				enumName = JsonNameAttribute.GetJsonName(value);
				if (String.IsNullOrEmpty(enumName))
				{
					enumName = value.ToString("f");
				}
			}

			this.Write(enumName);
		}

		public virtual void Write(string value)
		{
			if (value == null)
			{
				this.Writer.Write(JsonReader.LiteralNull);
				return;
			}

			int start = 0,
				length = value.Length;

			this.Writer.Write(JsonReader.OperatorStringDelim);

			for (int i=start; i<length; i++)
			{
				char ch = value[i];

				if (ch <= '\u001F' ||
					ch >= '\u007F' ||
					ch == '<' || // improves compatibility within script blocks
					ch == JsonReader.OperatorStringDelim ||
					ch == JsonReader.OperatorCharEscape)
				{
					if (i > start)
					{
						this.Writer.Write(value.Substring(start, i-start));
					}
					start = i+1;

					switch (ch)
					{
						case JsonReader.OperatorStringDelim:
						case JsonReader.OperatorCharEscape:
						{
							this.Writer.Write(JsonReader.OperatorCharEscape);
							this.Writer.Write(ch);
							continue;
						}
						case '\b':
						{
							this.Writer.Write("\\b");
							continue;
						}
						case '\f':
						{
							this.Writer.Write("\\f");
							continue;
						}
						case '\n':
						{
							this.Writer.Write("\\n");
							continue;
						}
						case '\r':
						{
							this.Writer.Write("\\r");
							continue;
						}
						case '\t':
						{
							this.Writer.Write("\\t");
							continue;
						}
						default:
						{
							this.Writer.Write("\\u");
							this.Writer.Write(Char.ConvertToUtf32(value, i).ToString("X4"));
							continue;
						}
					}
				}
			}

			if (length > start)
			{
				this.Writer.Write(value.Substring(start, length-start));
			}

			this.Writer.Write(JsonReader.OperatorStringDelim);
		}

		#endregion Public Methods

		#region Primative Writer Methods

		public virtual void Write(bool value)
		{
			this.Writer.Write(value ? JsonReader.LiteralTrue : JsonReader.LiteralFalse);
		}

		public virtual void Write(byte value)
		{
			this.Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(sbyte value)
		{
			this.Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(short value)
		{
			this.Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(ushort value)
		{
			this.Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(int value)
		{
			this.Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(uint value)
		{
			if (this.InvalidIeee754(value))
			{
				// emit as string since Number cannot represent
				this.Write(value.ToString("g", CultureInfo.InvariantCulture));
				return;
			}

			this.Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(long value)
		{
			if (this.InvalidIeee754(value))
			{
				// emit as string since Number cannot represent
				this.Write(value.ToString("g", CultureInfo.InvariantCulture));
				return;
			}

			this.Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(ulong value)
		{
			if (this.InvalidIeee754(value))
			{
				// emit as string since Number cannot represent
				this.Write(value.ToString("g", CultureInfo.InvariantCulture));
				return;
			}

			this.Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(float value)
		{
			if (Single.IsNaN(value) || Single.IsInfinity(value))
			{
				this.Writer.Write(JsonReader.LiteralNull);
			}
			else
			{
				this.Writer.Write(value.ToString("r", CultureInfo.InvariantCulture));
			}
		}

		public virtual void Write(double value)
		{
			if (Double.IsNaN(value) || Double.IsInfinity(value))
			{
				this.Writer.Write(JsonReader.LiteralNull);
			}
			else
			{
				this.Writer.Write(value.ToString("r", CultureInfo.InvariantCulture));
			}
		}

		public virtual void Write(decimal value)
		{
			if (this.InvalidIeee754(value))
			{
				// emit as string since Number cannot represent
				this.Write(value.ToString("g", CultureInfo.InvariantCulture));
				return;
			}

			this.Writer.Write(value.ToString("g", CultureInfo.InvariantCulture));
		}

		public virtual void Write(char value)
		{
			this.Write(new String(value, 1));
		}

		public virtual void Write(TimeSpan value)
		{
			this.Write(value.Ticks);
		}

		public virtual void Write(Uri value)
		{
			this.Write(value.ToString());
		}

		public virtual void Write(Version value)
		{
			this.Write(value.ToString());
		}

#if !UNITY3D
		public virtual void Write(XmlNode value)
		{
			// TODO: auto-translate XML to JsonML
			this.Write(value.OuterXml);
		}
#endif

		#endregion Primative Writer Methods

		#region Writer Methods

		protected internal virtual void WriteArray(IEnumerable value, Type elementType )
		{
			bool appendDelim = false;

			this.Writer.Write(JsonReader.OperatorArrayStart);

			// Are the references to the elements of this array tracked
			bool handledRef = referenceHandler != null && !Type.Equals (elementType, null) && referenceHandler.IsHandled (elementType);

			this.depth++;
			if (this.depth > this.settings.MaxDepth)
			{
				throw new JsonSerializationException(String.Format(JsonWriter.ErrorMaxDepth, new System.Object[] {this.settings.MaxDepth}));
			}
			try
			{
				foreach (object item in value)
				{
					if (appendDelim)
					{
						this.WriteArrayItemDelim();
					}
					else
					{
						appendDelim = true;
					}
						
					this.WriteLine();

					if (!handledRef || item == null) {
						// Reference is not tracked, serialize it normally
						this.WriteArrayItem(item, elementType);
					} else {
						// Reference is not owned by this field, write a reference to it
						// Arrays/Lists cannot own references
						this.WriteArrayItem ("@" + referenceHandler.GetReferenceID (item), typeof(string));
					}
				}
			}
			finally
			{
				this.depth--;
			}

			if (appendDelim)
			{
				this.WriteLine();
			}
			this.Writer.Write(JsonReader.OperatorArrayEnd);
		}

		protected virtual void WriteArrayItem(object item, Type elementType)
		{
			this.Write(item, false, elementType);
		}

		protected virtual void WriteObject(IDictionary value)
		{
			this.WriteDictionary((IEnumerable)value);
		}

		protected virtual void WriteDictionary(IEnumerable value)
		{
			IDictionaryEnumerator enumerator = value.GetEnumerator() as IDictionaryEnumerator;
			if (enumerator == null)
			{
				throw new JsonSerializationException(String.Format(JsonWriter.ErrorIDictionaryEnumerator, new System.Object[] {value.GetType()}));
			}

			bool appendDelim = false;

			if (settings.HandleCyclicReferences)
			{
				int prevIndex = 0;
				if (this.previouslySerializedObjects.TryGetValue (value, out prevIndex)) {
					this.Writer.Write(JsonReader.OperatorObjectStart);
					this.WriteObjectProperty("@ref", prevIndex);
					this.WriteLine();
					this.Writer.Write(JsonReader.OperatorObjectEnd);
					return;
				} else {
					this.previouslySerializedObjects.Add (value, this.previouslySerializedObjects.Count);
				}
			}

			this.Writer.Write(JsonReader.OperatorObjectStart);

			this.depth++;
			if (this.depth > this.settings.MaxDepth)
			{
				throw new JsonSerializationException(String.Format(JsonWriter.ErrorMaxDepth, new System.Object[] {this.settings.MaxDepth}));
			}

			try
			{
				while (enumerator.MoveNext())
				{
					if (appendDelim)
					{
						this.WriteObjectPropertyDelim();
					}
					else
					{
						appendDelim = true;
					}

					bool handledRef = referenceHandler != null && enumerator.Entry.Value != null && referenceHandler.IsHandled (enumerator.Entry.Value.GetType());

					if (!handledRef) {
						// Reference is not tracked, serialize it normally
						this.WriteObjectProperty(Convert.ToString(enumerator.Entry.Key), enumerator.Entry.Value);
					} else {
						// Reference is not owned by this field, write a reference to it
						// Dictionaries cannot own references
						this.WriteObjectProperty (Convert.ToString(enumerator.Entry.Key), "@" + referenceHandler.GetReferenceID (enumerator.Entry.Value), typeof(string));
					}

				}
			}
			finally
			{
				this.depth--;
			}

			if (appendDelim)
			{
				this.WriteLine();
			}
			this.Writer.Write(JsonReader.OperatorObjectEnd);
		}

		private void WriteObjectProperty(string key, object value, Type fieldType = null)
		{
			this.WriteLine();
			this.WriteObjectPropertyName(key);
			this.Writer.Write(JsonReader.OperatorNameDelim);
			this.WriteObjectPropertyValue(value, fieldType);
		}

		protected virtual void WriteObjectPropertyName(string name)
		{
			this.Write(name);
		}

		protected virtual void WriteObjectPropertyValue(object value, Type fieldType = null)
		{
			this.Write(value, true, fieldType);
		}

		protected virtual void WriteObject(object value, Type type, bool serializePrivate, Type fieldType = null )
		{
			bool appendDelim = false;

			if (settings.HandleCyclicReferences && !TCU.GetTypeInfo(type).IsValueType)
			{
				int prevIndex = 0;
				if (this.previouslySerializedObjects.TryGetValue (value, out prevIndex)) {
					this.Writer.Write(JsonReader.OperatorObjectStart);
					this.WriteObjectProperty("@ref", prevIndex);
					this.WriteLine();
					this.Writer.Write(JsonReader.OperatorObjectEnd);
					return;
				} else {
					this.previouslySerializedObjects.Add (value, this.previouslySerializedObjects.Count);
				}
			}

			this.Writer.Write(JsonReader.OperatorObjectStart);

			this.depth++;
			if (this.depth > this.settings.MaxDepth)
			{
				throw new JsonSerializationException(String.Format(JsonWriter.ErrorMaxDepth, new System.Object[] {this.settings.MaxDepth}));
			}
			try
			{
				if (!String.IsNullOrEmpty(this.settings.TypeHintName) && (!settings.TypeHintsOnlyWhenNeeded || depth <= 1 || (!Type.Equals(fieldType, type) && !Type.Equals (fieldType, null)) ))
				{

					if (appendDelim)
						this.WriteObjectPropertyDelim();
					else
						appendDelim = true;

					this.WriteObjectProperty(this.settings.TypeHintName, type.FullName+", "+type.Assembly.GetName().Name);
				}


				// Set tag so that other objects can find it when deserializing
				// Note that this must be set after the type hint is serialized
				// To make sure the fields are read correctly when deserializing

				if (referenceHandler != null && referenceHandler.IsHandled (type)) {

					if (appendDelim)
						this.WriteObjectPropertyDelim();
					else
						appendDelim = true;

					this.WriteObjectProperty("@tag", referenceHandler.GetReferenceID (value));

					// Notify the reference handler that this value has now been serialized
					referenceHandler.MarkAsSerialized (value);
				}

				//Console.WriteLine ("Anon " + anonymousType);

				//Console.WriteLine (type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Length  + " " + type.GetProperties().Length);

				// serialize public properties

				KeyValuePair<string,FieldInfo>[] fields;
				KeyValuePair<string,PropertyInfo>[] properties;
				this.settings.Coercion.GetMemberWritingMap (type, settings, out fields, out properties);

				for (int j = 0; j < properties.Length; j++ )
				{
					PropertyInfo property  = properties[j].Value;

					object propertyValue = property.GetValue(value, null);
					if (this.IsDefaultValue(property, propertyValue)) {
						if (Settings.DebugMode)
							Console.WriteLine ("Cannot serialize "+property.Name+" : is default value");
						continue;
					}
					
					
					if (appendDelim)
						this.WriteObjectPropertyDelim();
					else
						appendDelim = true;

					string name = properties[j].Key;

					bool ownedRef = referenceHandler == null || !referenceHandler.IsHandled (property.PropertyType) || referenceHandler.IsOwnedRef (property);

					if (ownedRef || propertyValue == null) {
						// Reference is owned by this property, serialize it as normal
						this.WriteObjectProperty(name, propertyValue, property.PropertyType);
					} else {
						// Reference is not owned by this property, write a reference to it
						this.WriteObjectProperty (name, "@" + referenceHandler.GetReferenceID (propertyValue), typeof(string));
					}
				}
					
				for (int j = 0; j < fields.Length; j++ ) {
					FieldInfo field = fields[j].Value;
					
					object fieldValue = field.GetValue(value);
					if (this.IsDefaultValue(field, fieldValue)) {
						if (Settings.DebugMode)
							Console.WriteLine ("Cannot serialize "+field.Name+" : is default value");
						continue;
					}
				
					if (appendDelim)
						this.WriteObjectPropertyDelim();
					else
						appendDelim = true;

					string name = fields[j].Key;

					bool ownedRef = referenceHandler == null || !referenceHandler.IsHandled (field.FieldType) || referenceHandler.IsOwnedRef (field);

					if (ownedRef || fieldValue == null) {
						// Reference is owned by this field, serialize it as normal
						this.WriteObjectProperty(name, fieldValue, field.FieldType);
					} else {
						// Reference is not owned by this field, write a reference to it
						this.WriteObjectProperty (name, "@" + referenceHandler.GetReferenceID (fieldValue), typeof(string));
					}
				}
			}
			finally
			{
				this.depth--;
			}

			if (appendDelim)
				this.WriteLine();
			
			this.Writer.Write(JsonReader.OperatorObjectEnd);
		}

		protected virtual void WriteArrayItemDelim()
		{
			this.Writer.Write(JsonReader.OperatorValueDelim);
		}

		protected virtual void WriteObjectPropertyDelim()
		{
			this.Writer.Write(JsonReader.OperatorValueDelim);
		}

		protected virtual void WriteLine()
		{
			if (!this.settings.PrettyPrint)
			{
				return;
			}

			this.Writer.WriteLine();
			for (int i=0; i<this.depth; i++)
			{
				this.Writer.Write(this.settings.Tab);
			}
		}

		#endregion Writer Methods

		#region Private Methods

		/// <summary>
		/// Determines if the member value matches the DefaultValue attribute
		/// </summary>
		/// <returns>if has a value equivalent to the DefaultValueAttribute</returns>
		private bool IsDefaultValue(MemberInfo member, object value)
		{
			#if WINDOWS_STORE
			var attribute = member.GetCustomAttribute<DefaultValueAttribute> (true);
			#else
			var attribute = Attribute.GetCustomAttribute(member, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
			#endif

			if (attribute == null)
			{
				return false;
			}

			if (attribute.Value == null)
			{
				return (value == null);
			}

			return (attribute.Value.Equals(value));
		}

		#endregion Private Methods

		#region Utility Methods

		/// <summary>
		/// Splits a bitwise-OR'd set of enums into a list.
		/// </summary>
		/// <param name="enumType">the enum type</param>
		/// <param name="value">the combined value</param>
		/// <returns>list of flag enums</returns>
		/// <remarks>
		/// from PseudoCode.EnumHelper
		/// </remarks>
		private static Enum[] GetFlagList(Type enumType, object value)
		{
			ulong longVal = Convert.ToUInt64(value);
			Array enumValues = Enum.GetValues(enumType);

			List<Enum> enums = new List<Enum>(enumValues.Length);

			// check for empty
			if (longVal == 0L)
			{
				// Return the value of empty, or zero if none exists
				enums.Add((Enum)Convert.ChangeType(value, enumType));
				return enums.ToArray();
			}

			for (int i = enumValues.Length-1; i >= 0; i--)
			{
				ulong enumValue = Convert.ToUInt64(enumValues.GetValue(new int[]{i}));

				if ((i == 0) && (enumValue == 0L))
				{
					continue;
				}

				// matches a value in enumeration
				if ((longVal & enumValue) == enumValue)
				{
					// remove from val
					longVal -= enumValue;

					// add enum to list
					enums.Add(enumValues.GetValue(new int[] {i}) as Enum);
				}
			}

			if (longVal != 0x0L)
			{
				enums.Add(Enum.ToObject(enumType, value) as Enum);
			}

			return enums.ToArray();
		}

		/// <summary>
		/// Determines if a numberic value cannot be represented as IEEE-754.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual bool InvalidIeee754(decimal value)
		{
			// http://stackoverflow.com/questions/1601646

			try
			{
				return (decimal)((double)value) != value;
			}
			catch
			{
				return true;
			}
		}

		#endregion Utility Methods

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			if (this.Writer != null)
			{
				this.Writer.Dispose();
			}
		}

		#endregion IDisposable Members
	}
}
