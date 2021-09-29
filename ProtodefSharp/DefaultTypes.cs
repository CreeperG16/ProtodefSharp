using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace ProtodefSharp.DefaultTypes
{
	public class ProtoVoid : ProtodefType
	{
		public new string Id = "void";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			return JToken.FromObject(null);
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			return;
		}
	}

	public class ProtoBool : ProtodefType
	{
		public new string Id = "bool";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			return JToken.FromObject(stream.ReadByte() == 1);
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			bool bo = data.ToObject<bool>();
			byte by = bo ? (byte) 0x01 : (byte) 0x00;
			stream.WriteByte(by);
		}
	}

	#region Numeric

	public class ProtoByte : ProtodefType
	{
		public new string Id = "i8";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			return JToken.FromObject(stream.ReadByte()); // TODO: fix, -128 in read, +128 in write
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			stream.WriteByte(data.ToObject<byte>());
		}
	}

	public class ProtoUByte : ProtodefType
	{
		public new string Id = "u8";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			return JToken.FromObject(stream.ReadByte());
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			stream.WriteByte(data.ToObject<byte>());
		}
	}

	public class ProtoShort : ProtodefType
	{
		public new string Id = "i16";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			byte[] data = new byte[2];
			stream.Read(data, 0, 2);
			return JToken.FromObject(BitConverter.ToInt16(data));
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			byte[] buf = BitConverter.GetBytes(data.ToObject<short>());
			stream.Write(buf, 0, 2);
		}
	}

	public class ProtoUShort : ProtodefType
	{
		public new string Id = "u16";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			byte[] data = new byte[2];
			stream.Read(data, 0, 2);
			return JToken.FromObject(BitConverter.ToUInt16(data));
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			byte[] buf = BitConverter.GetBytes(data.ToObject<ushort>());
			stream.Write(buf, 0, 2);
		}
	}

	public class ProtoInt : ProtodefType
	{
		public new string Id = "i32";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			byte[] data = new byte[4];
			stream.Read(data, 0, 4);
			return JToken.FromObject(BitConverter.ToInt32(data));
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			byte[] buf = BitConverter.GetBytes(data.ToObject<int>());
			stream.Write(buf, 0, 4);
		}
	}

	public class ProtoUInt : ProtodefType
	{
		public new string Id = "u32";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			byte[] data = new byte[4];
			stream.Read(data, 0, 4);
			return JToken.FromObject(BitConverter.ToUInt32(data));
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			byte[] buf = BitConverter.GetBytes(data.ToObject<uint>());
			stream.Write(buf, 0, 4);
		}
	}

	public class ProtoFloat : ProtodefType
	{
		public new string Id = "f32";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			byte[] data = new byte[4];
			stream.Read(data, 0, 4);
			return JToken.FromObject(BitConverter.ToSingle(data));
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			byte[] buf = BitConverter.GetBytes(data.ToObject<float>());
			stream.Write(buf, 0, 4);
		}
	}

	public class ProtoDouble : ProtodefType
	{
		public new string Id = "f64";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			byte[] data = new byte[8];
			stream.Read(data, 0, 8);
			return JToken.FromObject(BitConverter.ToDouble(data));
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			byte[] buf = BitConverter.GetBytes(data.ToObject<double>());
			stream.Write(buf, 0, 8);
		}
	}

	public class ProtoLong : ProtodefType
	{
		public new string Id = "i64";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			byte[] data = new byte[8];
			stream.Read(data, 0, 8);
			return JToken.FromObject(BitConverter.ToInt64(data));
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			byte[] buf = BitConverter.GetBytes(data.ToObject<long>());
			stream.Write(buf, 0, 8);
		}
	}

	public class ProtoULong : ProtodefType
	{
		public new string Id = "u64";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			byte[] data = new byte[8];
			stream.Read(data, 0, 8);
			return JToken.FromObject(BitConverter.ToUInt64(data));
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			byte[] buf = BitConverter.GetBytes(data.ToObject<ulong>());
			stream.Write(buf, 0, 8);
		}
	}

	public class ProtoVarInt : ProtodefType
	{
		public new string Id = "varint";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			int numRead = 0;
			int result = 0;
			byte read;
			do
			{
				read = (byte) stream.ReadByte();
				int value = (read & 0x7f);
				result |= (value << (7 * numRead));

				numRead++;
				if (numRead > 5)
				{
					throw new Exception("VarInt is too big");
				}
			} while ((read & 0x80) != 0);
			return JToken.FromObject(result);
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			uint value = data.ToObject<uint>();
			while ((value & -128) != 0)
			{
				stream.WriteByte((byte)((value & 0x7F) | 0x80));
				value >>= 7;
			}

			stream.WriteByte((byte)value);
		}
	}

	#endregion
	
	public class ProtoCString : ProtodefType // TODO!!
	{
		public new string Id = "cstring";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			return JToken.FromObject("");
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			//
		}
	}

	public class ProtoBuffer : ProtodefType // todo: add support for option bool 'rest'
	{
		public new string Id = "buffer";
		public new JToken Read(Stream stream, JObject opts = null)
		{
			if (opts == null) throw new Exception("Options cannot be null for ProtodefType 'buffer'!");
			if (opts.Properties().Any(p => p.Name == "countType")) {
				string countType = opts.Properties().First(k => k.Name == "countType").Value.ToObject<string>();
				// TODO!!
			} else
			{
				if (!opts.Properties().Any(p => p.Name == "count")) throw new Exception("Either 'countType' or 'count' must be an option for ProtodefType 'buffer'");
				int count = opts.Properties().First(k => k.Name == "count").Value.ToObject<int>();
				byte[] buf = new byte[count];
				for (int i = 0; i < count; i++)
				{
					buf += (byte)stream.ReadByte();
				}

			}
		}
		public new void Write(Stream stream, JToken data, JObject opts = null)
		{
			//
		}
	}
}