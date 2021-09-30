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
		public ProtodefNamespace RootNamespace;

		public Protodef()
		{
			RootNamespace = new ProtodefNamespace(this);
			foreach (KeyValuePair<string, ProtodefType> type in Protodef.NativeTypes)
			{
				AddType(type.Key, type.Value);
			}
		}

		public void AddType(string name, ProtodefType type)
		{
			Types[name] = type;
		}

		public void ParseTypes(string data) => ParseTypes(JToken.Parse(data));
		public void ParseTypes(JToken data)
		{
			if (data.Type != JTokenType.Object) throw new Exception("Root JToken is not an object!");
			JObject obj = (JObject)data;

		}

		public byte[] CreatePacketBuffer(string packedId, JToken packetData)
		{
			return new byte[0];
		}

		public JToken ParsePacketBuffer(byte[] data)
		{
			return JToken.FromObject(null);
		}

		// static methods

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
		public Protodef Protodef;
		public string Name;
		public Dictionary<string, ProtodefNamespace> SubNamespaces = new();
		public Dictionary<string, ProtodefType> Types = new();
		public ProtodefNamespace(Protodef parent, string name = "root")
		{
			Protodef = parent;
			Name = name;
		}

		public ProtodefType ResolveType(string id)
		{
			if (Types.ContainsKey(id)) return Types[id];
			if (Protodef.Types.ContainsKey(id)) return Protodef.Types[id];
			throw new Exception($"Type not found: {id}");
		}

		public JToken ReadPath(string typeName, MemoryStream stream)
		{
			ProtodefType type = ResolveType(typeName);

			ProtodefContext ctx = new ProtodefContext()
			{
				Stream = stream,
				Protodef = Protodef,
				Namespace = this
			};

			return type.Read(ctx);
		}

		public JToken Read(string typeName, ProtodefContext ctx, JObject opts = null)
		{
			ProtodefType type = ResolveType(typeName);

			if (opts != null) ctx.Options = opts;

			return type.Read(ctx);
		}
	}

	public class ProtodefContext
	{
		public ProtodefContext() { }

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
