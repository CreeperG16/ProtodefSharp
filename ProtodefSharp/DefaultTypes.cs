using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace ProtodefSharp.DefaultTypes
{
	public class ProtoVoid : ProtodefType
	{
		public new string Id = "void";
		public new JToken Read(ProtodefContext ctx)
		{
			return JToken.FromObject(null);
		}
		public new void Write(ProtodefContext ctx)
		{
			return;
		}
	}

	public class ProtoBool : ProtodefType
	{
		public new string Id = "bool";
		public new JToken Read(ProtodefContext ctx)
		{
			return JToken.FromObject(ctx.Stream.ReadByte() == 1);
		}
		public new void Write(ProtodefContext ctx)
		{
			bool bo = ctx.Input.ToObject<bool>();
			byte by = bo ? (byte) 0x01 : (byte) 0x00;
			ctx.Stream.WriteByte(by);
		}
	}

	#region Numeric

	public class ProtoByte : ProtodefType
	{
		public new string Id = "i8";
		public new JToken Read(ProtodefContext ctx)
		{
			return JToken.FromObject(ctx.Stream.ReadByte()); // TODO: fix, -128 in read, +128 in write
		}
		public new void Write(ProtodefContext ctx)
		{
			ctx.Stream.WriteByte(ctx.Input.ToObject<byte>());
		}
	}

	public class ProtoUByte : ProtodefType
	{
		public new string Id = "u8";
		public new JToken Read(ProtodefContext ctx)
		{
			return JToken.FromObject(ctx.Stream.ReadByte());
		}
		public new void Write(ProtodefContext ctx)
		{
			ctx.Stream.WriteByte(ctx.Input.ToObject<byte>());
		}
	}

	public class ProtoShort : ProtodefType
	{
		public new string Id = "i16";
		public new JToken Read(ProtodefContext ctx)
		{
			byte[] data = new byte[2];
			ctx.Stream.Read(data, 0, 2);
			return JToken.FromObject(BitConverter.ToInt16(data));
		}
		public new void Write(ProtodefContext ctx)
		{
			byte[] buf = BitConverter.GetBytes(ctx.Input.ToObject<short>());
			ctx.Stream.Write(buf, 0, 2);
		}
	}

	public class ProtoUShort : ProtodefType
	{
		public new string Id = "u16";
		public new JToken Read(ProtodefContext ctx)
		{
			byte[] data = new byte[2];
			ctx.Stream.Read(data, 0, 2);
			return JToken.FromObject(BitConverter.ToUInt16(data));
		}
		public new void Write(ProtodefContext ctx)
		{
			byte[] buf = BitConverter.GetBytes(ctx.Input.ToObject<ushort>());
			ctx.Stream.Write(buf, 0, 2);
		}
	}

	public class ProtoInt : ProtodefType
	{
		public new string Id = "i32";
		public new JToken Read(ProtodefContext ctx)
		{
			byte[] data = new byte[4];
			ctx.Stream.Read(data, 0, 4);
			return JToken.FromObject(BitConverter.ToInt32(data));
		}
		public new void Write(ProtodefContext ctx)
		{
			byte[] buf = BitConverter.GetBytes(ctx.Input.ToObject<int>());
			ctx.Stream.Write(buf, 0, 4);
		}
	}

	public class ProtoUInt : ProtodefType
	{
		public new string Id = "u32";
		public new JToken Read(ProtodefContext ctx)
		{
			byte[] data = new byte[4];
			ctx.Stream.Read(data, 0, 4);
			return JToken.FromObject(BitConverter.ToUInt32(data));
		}
		public new void Write(ProtodefContext ctx)
		{
			byte[] buf = BitConverter.GetBytes(ctx.Input.ToObject<uint>());
			ctx.Stream.Write(buf, 0, 4);
		}
	}

	public class ProtoFloat : ProtodefType
	{
		public new string Id = "f32";
		public new JToken Read(ProtodefContext ctx)
		{
			byte[] data = new byte[4];
			ctx.Stream.Read(data, 0, 4);
			return JToken.FromObject(BitConverter.ToSingle(data));
		}
		public new void Write(ProtodefContext ctx)
		{
			byte[] buf = BitConverter.GetBytes(ctx.Input.ToObject<float>());
			ctx.Stream.Write(buf, 0, 4);
		}
	}

	public class ProtoDouble : ProtodefType
	{
		public new string Id = "f64";
		public new JToken Read(ProtodefContext ctx)
		{
			byte[] data = new byte[8];
			ctx.Stream.Read(data, 0, 8);
			return JToken.FromObject(BitConverter.ToDouble(data));
		}
		public new void Write(ProtodefContext ctx)
		{
			byte[] buf = BitConverter.GetBytes(ctx.Input.ToObject<double>());
			ctx.Stream.Write(buf, 0, 8);
		}
	}

	public class ProtoLong : ProtodefType
	{
		public new string Id = "i64";
		public new JToken Read(ProtodefContext ctx)
		{
			byte[] data = new byte[8];
			ctx.Stream.Read(data, 0, 8);
			return JToken.FromObject(BitConverter.ToInt64(data));
		}
		public new void Write(ProtodefContext ctx)
		{
			byte[] buf = BitConverter.GetBytes(ctx.Input.ToObject<long>());
			ctx.Stream.Write(buf, 0, 8);
		}
	}

	public class ProtoULong : ProtodefType
	{
		public new string Id = "u64";
		public new JToken Read(ProtodefContext ctx)
		{
			byte[] data = new byte[8];
			ctx.Stream.Read(data, 0, 8);
			return JToken.FromObject(BitConverter.ToUInt64(data));
		}
		public new void Write(ProtodefContext ctx)
		{
			byte[] buf = BitConverter.GetBytes(ctx.Input.ToObject<ulong>());
			ctx.Stream.Write(buf, 0, 8);
		}
	}

	public class ProtoVarInt : ProtodefType
	{
		public new string Id = "varint";
		public new JToken Read(ProtodefContext ctx)
		{
			int numRead = 0;
			int result = 0;
			byte read;
			do
			{
				read = (byte) ctx.Stream.ReadByte();
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
		public new void Write(ProtodefContext ctx)
		{
			uint value = ctx.Input.ToObject<uint>();
			while ((value & -128) != 0)
			{
				ctx.Stream.WriteByte((byte)((value & 0x7F) | 0x80));
				value >>= 7;
			}

			ctx.Stream.WriteByte((byte)value);
		}
	}

	#endregion
	
	public class ProtoCString : ProtodefType // TODO!!
	{
		public new string Id = "cstring";
		public new JToken Read(ProtodefContext ctx)
		{
			return JToken.FromObject("");
		}
		public new void Write(ProtodefContext ctx)
		{
			//
		}
	}

	public class ProtoBuffer : ProtodefType // todo: add support for option bool 'rest'
	{
		public new string Id = "buffer";
		public new JToken Read(ProtodefContext ctx)
		{
			if (ctx.Options == null) throw new Exception("Options cannot be null for ProtodefType 'buffer'!");
			if (ctx.Options.Type != JTokenType.Object) throw new Exception("Options must be an object for ProtodefType 'buffer'!");
			JObject opts = (JObject)ctx.Options;
			if (opts.ContainsKey("countType")) {
				string countType = opts.GetValue("countType").ToObject<string>();
				// TODO!!
			} else
			{
				if (!opts.ContainsKey("count")) throw new Exception("Either 'countType' or 'count' must be an option for ProtodefType 'buffer'");
				int count = opts.GetValue("count").ToObject<int>();
				byte[] buf = new byte[count];
				for (int i = 0; i < count; i++)
				{
					//buf += (byte)ctx.Stream.ReadByte();
				}

			}
		}
		public new void Write(ProtodefContext ctx)
		{
			//
		}
	}
}