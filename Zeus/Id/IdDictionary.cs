using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Zeus.Id
{
    public class IdDictionary
	{
		private readonly Dictionary<string, int> _nameToId = new Dictionary<string, int>();
		private Dictionary<int, string> _idToName;
		public readonly int Count;

		public IEnumerable<string> Names => _nameToId.Keys;

		private IdDictionary(int count)
		{
			Count = count;
		}

		public bool TryGetName(int id, out string name) => _idToName.TryGetValue(id, out name);
		public bool TryGetId(string name, out int id) => _nameToId.TryGetValue(name, out id);
		public bool ContainsName(string name) => _nameToId.ContainsKey(name);
		public bool ContainsId(int id) => _idToName.ContainsKey(id);
		public string GetName(int id) => _idToName[id];
		public int GetId(string name) => _nameToId[name];

		public void Add(string name, int id)
		{
			_idToName.Add(id, name);
			_nameToId.Add(name, id);
		}

		public void Remove(string name)
		{
			_idToName.Remove(_nameToId[name]);
			_nameToId.Remove(name);
		}

		public void Remove(int id)
		{
			_nameToId.Remove(_idToName[id]);
			_idToName.Remove(id);
		}

		public static IdDictionary Create(Type idClass, Type idType)
		{
			int count = int.MaxValue;
			FieldInfo fieldInfo = idClass.GetFields().FirstOrDefault((FieldInfo field) => field.Name == "Count");
			if (fieldInfo != null)
				count = Convert.ToInt32(fieldInfo.GetValue(null));

			IdDictionary dictionary = new IdDictionary(count);
			(from f in idClass.GetFields(BindingFlags.Static | BindingFlags.Public)
			 where f.FieldType == idType
			 select f).ToList().ForEach(delegate (FieldInfo field) {
				 int num = Convert.ToInt32(field.GetValue(null));
				 if (num < dictionary.Count)
					 dictionary._nameToId.Add(field.Name, num);
			 });

			dictionary._idToName = dictionary._nameToId.ToDictionary((KeyValuePair<string, int> kp) => kp.Value, (KeyValuePair<string, int> kp) => kp.Key);
			return dictionary;
		}

		public static IdDictionary Create<IdClass, IdType>() => Create(typeof(IdClass), typeof(IdType));
	}

}
