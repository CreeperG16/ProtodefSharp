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

		public Dictionary<string, ProtodefType> Types = Protodef.NativeTypes;

		public Protodef()
		{
				
		}
	}

	public class ProtodefType
	{
		public string Id;
		public JToken Read(Stream stream, JObject opts = null) {
			return JToken.FromObject(null);
		}
		public void Write(Stream stream, JToken data, JObject opts = null) { }
		// do we even need this
		//public void SizeOf(Stream stream);
	}
}
