using Newtonsoft.Json.Linq;
using ProtodefSharp.DefaultTypes;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProtodefSharp
{
	public class Protodef
	{
		public static Dictionary<string, ProtodefType> NativeTypes = new Dictionary<string, ProtodefType>()
		{
			{ "void", new ProtoVoid() },
			{ "bool", new ProtoBool() },

			{ "i8", new ProtoByte() },
			{ "u8", new ProtoUByte() },
			{ "i16", new ProtoShort() },
			{ "u16", new ProtoUShort() },
			{ "i32", new ProtoInt() },
			{ "u32", new ProtoUInt() },
			{ "f32", new ProtoFloat() },
			{ "f64", new ProtoDouble() },
			{ "i64", new ProtoLong() },
			{ "u64", new ProtoULong() },
			{ "varint", new ProtoVarInt() },
		};

		public Dictionary<string, ProtodefType> Types = new();
		public ProtodefNamespace RootNamespace = new();

		public Protodef()
		{
			foreach (KeyValuePair<string, ProtodefType> type in Protodef.NativeTypes)
			{
				AddType(type.Key, type.Value);
			}
		}

		public void AddType(string name, ProtodefType type)
		{
			Types[name] = type;
		}

		public void Parse(string data) => Parse(JToken.Parse(data));
		public void Parse(JToken data)
		{
			if (data.Type != JTokenType.Object) throw new Exception("Root JToken is not an object!");
			JObject obj = (JObject)data;

		}



		public static JObject ParseOptions(JToken data)
		{
			if (data.Type == JTokenType.Undefined)
			{
				return new JObject();
			} else if(data.Type == JTokenType.String)
			{
				JObject obj = new JObject();
				obj.Add("type", data);
				return obj;
			} else if (data.Type == JTokenType.Array)
			{
				JArray arr = (JArray)data;
				JObject obj = new JObject();
				obj.Add("type", arr[0]);
				obj.Add("typeArgs", arr[1]);
				return obj;
			} else if (data.Type == JTokenType.Object)
			{
				return (JObject)data;
			} else
			{
				throw new Exception($"Cannot parse type options for {data.Type.ToString()}");
			}
		}
	}

	public class ProtodefNamespace
	{
		public string Name;
		public Dictionary<string, ProtodefNamespace> SubNamespaces = new();
		public ProtodefNamespace(string name = "root")
		{
			Name = name;
		}
	}

	public class ProtodefContext
	{
		public Stream Stream = null;
		public JObject Options = new JObject();
		public Protodef Protodef;
		public ProtodefNamespace Namespace;

		// Only for Write()
		public JToken Input = JToken.FromObject(null);
	}

	public class ProtodefType
	{
		public string Id;
		public JToken Read(ProtodefContext ctx) {
			return JToken.FromObject(null);
		}
		public void Write(ProtodefContext ctx) { }
		// do we even need this
		//public void SizeOf(Stream stream);
	}
}
