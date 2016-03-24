using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace Brainiac.Serialization {

	public class JsonOwnedReferenceAttribute : System.Attribute {
	}

	public class JsonHandledReferenceAttribute : System.Attribute {
	}

	public class ReferenceHandlerWriter {

		Dictionary<System.Object, int> mapping = new Dictionary<object, int>();
		Dictionary<System.Object, bool> hasBeenSerialized = new Dictionary<System.Object, bool>();
		Dictionary<System.Type, bool> isHandledCache = new Dictionary<Type, bool>();
		Dictionary<MemberInfo, bool> isOwnedRefCache = new Dictionary<MemberInfo, bool>();

		public int GetReferenceID ( System.Object val ) {
			int id;
			if (!mapping.TryGetValue (val, out id)) {
				id = mapping.Count + 1;
				mapping.Add (val, id);
				hasBeenSerialized[val] = false;
			}
			return id;
		}

		/** If an object is serialized elsewhere, you can set a custom id in the writer and the reader to be able to serialize references to it.
		 * Use negative IDs to avoid collisions with the default ones.
		 */
		public void SetCustomID (System.Object val, int id) {
			if (mapping.ContainsKey (val))
				throw new System.Exception ("This object has already an ID set. Make sure the custom ID is set before any serialization starts");

			mapping[val] = id;
			MarkAsSerialized(val);
		}

		public bool IsHandled ( System.Type type ) {
			bool result;
			if (!isHandledCache.TryGetValue (type, out result)) {
				result = isHandledCache[type] = type.GetCustomAttributes (typeof(JsonHandledReferenceAttribute), true).Length != 0;
			}
			return result;
		}

		public bool IsOwnedRef ( MemberInfo info ) {
			bool result;
			if (!isOwnedRefCache.TryGetValue (info, out result)) {
				result = isOwnedRefCache[info] = info.GetCustomAttributes (typeof(JsonOwnedReferenceAttribute), true).Length != 0;
			}
			return result;
		}

		/** Marks this value as having been serialized somewhere with the correct @tag field set.
		 * After serialization we can then know what references were not serialized anywhere, which helps with debugging
		 */
		public void MarkAsSerialized ( System.Object val ) {
			hasBeenSerialized[val] = true;
		}

		public List<System.Object> GetNonSerializedReferences () {
			var ls = new List<System.Object> ();
			foreach (var pair in hasBeenSerialized) {
				if (!pair.Value) {
					ls.Add (pair.Key);
				}
			}
			return ls;
		}

		public void TransferNonSerializedReferencesToReader (ReferenceHandlerReader reader) {
			foreach (var pair in hasBeenSerialized) {
				if (!pair.Value) {
					reader.Set (mapping [pair.Key], pair.Key);
				}
			}
		}
	}

	public class ReferenceHandlerReader {

		Dictionary<int, System.Object> mapping = new Dictionary<int, System.Object>();
		Dictionary<int, List<KeyValuePair<MemberInfo, System.Object>>> delayedSetters = new Dictionary<int, List<KeyValuePair<MemberInfo, System.Object>>>();
		Dictionary<int, List<KeyValuePair<string, IDictionary>>> delayedDictSetters = new Dictionary<int, List<KeyValuePair<string, IDictionary>>>();
		Dictionary<int, List<KeyValuePair<int, IList>>> delayedListSetters = new Dictionary<int, List<KeyValuePair<int, IList>>>();
		Dictionary<int, List<DeepLazySetter>> deepDelayedSetters = new Dictionary<int, List<DeepLazySetter>>();

		struct DeepLazySetter {
			public System.Object parentClass;
			public MemberInfo child;
			public MemberInfo member;
		}

		public bool TryGetValueFromID ( int id, out System.Object result ) {
			return mapping.TryGetValue (id, out result);
		}

		/** If an object is serialized elsewhere, you can set a custom id in the writer and the reader to be able to serialize references to it.
		 * Use negative IDs to avoid collisions with the default ones.
		 */
		public void SetCustomID (System.Object val, int id) {
			if (mapping.ContainsKey (id))
				throw new System.Exception ("That ID is already used. Set custom IDs before deserialization is started.");

			mapping[id] = val;
		}

		public void AddDelayedSetter ( int id, MemberInfo memberInfo, System.Object val ) {
			if (val == null) {
				throw new System.ArgumentNullException ("val");
			}

			if (!delayedSetters.ContainsKey (id)) {
				delayedSetters[id] = new List<KeyValuePair<MemberInfo, object>> ();
			}

			//System.Console.WriteLine ("Adding delayed setter for " + id);
			delayedSetters[id].Add (new KeyValuePair<MemberInfo, object> (memberInfo, val));
		}

		public void AddDelayedSetter ( int id, System.Object parentClass, MemberInfo child, MemberInfo memberInfo) {
			if (parentClass == null) {
				throw new System.ArgumentNullException ("val");
			}

			if (!deepDelayedSetters.ContainsKey (id)) {
				deepDelayedSetters[id] = new List<DeepLazySetter> ();
			}

			//System.Console.WriteLine ("Adding delayed setter for " + id);
			deepDelayedSetters[id].Add (new DeepLazySetter { parentClass = parentClass, child = child, member = memberInfo });
		}

		public void AddDelayedDictionarySetter ( int id, IDictionary dict, string key ) {

			if (key == null) {
				throw new System.ArgumentNullException ("key");
			}

			if (dict == null) {
				throw new System.ArgumentNullException ("dict");
			}

			if (!delayedDictSetters.ContainsKey (id)) {
				delayedDictSetters[id] = new List<KeyValuePair<string, IDictionary>> ();
			}

			//System.Console.WriteLine ("Adding delayed setter for " + id);
			delayedDictSetters[id].Add (new KeyValuePair<string, IDictionary> (key, dict));
		}

		public void AddDelayedListSetter ( int id, IList list, int index ) {

			if (list == null) {
				throw new System.ArgumentNullException ("list");
			}

			if (!delayedListSetters.ContainsKey (id)) {
				delayedListSetters[id] = new List<KeyValuePair<int, IList>> ();
			}

			//System.Console.WriteLine ("Adding delayed setter for " + id);
			delayedListSetters[id].Add (new KeyValuePair<int, IList> (index, list));
		}

		public void Set ( int id, System.Object val ) {
			mapping.Add (id, val);

			if (delayedSetters.ContainsKey (id)) {
				var setters = delayedSetters[id];

				for (int i = 0; i < setters.Count; i++) {

					var memberInfo = setters[i].Key;
					var target = setters[i].Value;

					if (memberInfo is PropertyInfo) {
						// set value of public property
						((PropertyInfo)memberInfo).SetValue(target, val, null);
					}
					else if (memberInfo is FieldInfo) {
						// set value of public field
						((FieldInfo)memberInfo).SetValue(target, val);
					}
				}

				setters.Clear ();
				delayedSetters.Remove (id);
			}

			if (delayedDictSetters.ContainsKey (id)) {
				var setters = delayedDictSetters[id];

				for (int i = 0; i < setters.Count; i++) {

					var key = setters[i].Key;
					var target = setters[i].Value;

					target[key] = val;
				}

				setters.Clear ();
				delayedDictSetters.Remove (id);
			}

			if (delayedListSetters.ContainsKey (id)) {
				var setters = delayedListSetters[id];

				for (int i = 0; i < setters.Count; i++) {

					var index = setters[i].Key;
					var target = setters[i].Value;

					target[index] = val;
				}

				setters.Clear ();
				delayedListSetters.Remove (id);
			}

			if (deepDelayedSetters.ContainsKey (id)) {
				var setters = deepDelayedSetters[id];

				for (int i = 0; i < setters.Count; i++) {

					var child = setters[i].child;
					var parent = setters [i].parentClass;

					System.Object childObject = null;

					if (child is PropertyInfo) {
						// set value of public property
						childObject = ((PropertyInfo)child).GetValue (parent, null);
					} else if (child is FieldInfo) {
						// set value of public field
						childObject = ((FieldInfo)child).GetValue (parent);
					} else {
						throw new System.Exception();
					}

					// Child object should be a struct so it cannot be null
					var member = setters[i].member;

					if (member is PropertyInfo) {
						// set value of public property
						((PropertyInfo)member).SetValue(childObject, val, null);
					} else if (member is FieldInfo) {
						// set value of public field
						((FieldInfo)member).SetValue(childObject, val);
					} else {
						throw new System.Exception();
					}

					if (child is PropertyInfo) {
						// set value of public property
						((PropertyInfo)child).SetValue (parent, childObject, null);
					} else if (child is FieldInfo) {
						// set value of public field
						((FieldInfo)child).SetValue (parent, childObject);
					}
				}

				setters.Clear ();
				delayedSetters.Remove (id);
			}
		}

		public List<int> GetMissingReferences () {

			var dict = new Dictionary<int,bool> ();

			foreach (var pair in delayedSetters) {
				dict[pair.Key] = true;
			}

			foreach (var pair in delayedDictSetters) {
				dict[pair.Key] = true;
			}

			foreach (var pair in delayedListSetters) {
				dict[pair.Key] = true;
			}

			var ls = new List<int> ();
			foreach (var pair in dict) {
				ls.Add (pair.Key);
			}

			return ls;
		}
	}
}

