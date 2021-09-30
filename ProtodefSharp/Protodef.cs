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
		public Dictionary<string, ProtodefNamespace> Namespaces = new();

		public Protodef()
		{
				
		}

		public void Parse(string data) => Parse(JToken.Parse(data));
		public void Parse(JToken data)
		{
			if (data.Type != JTokenType.Object) throw new Exception("Root JToken is not an object!");
			JObject obj = (JObject)data;

		}
	}

	public class ProtodefNamespace
	{
		public ProtodefNamespace(string name = "root")
		{

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
