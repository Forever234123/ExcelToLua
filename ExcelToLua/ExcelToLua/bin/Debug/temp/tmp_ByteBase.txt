//using UnityEngine;
using System.IO;
using System;
using System.Text;

public class ByteBase
{
	private MemoryStream buff;
	
	public enum TYPE
	{
		NONE,
		BOOL,
		ENUM,
		SHORT,
		INT,
		FLOAT,
		STRING,
		//VECTOR2,
		//VECTOR3,
		
		ARRAY_INT,
		ARRAY_FLOAT,
		ARRAY_STRING,
	}
	
	public static ByteBase.TYPE GetTypeByName(string type)
	{
		switch (type)
		{
		case "bool":
			return ByteBase.TYPE.BOOL;
		case "enum":
			return ByteBase.TYPE.ENUM;
		case "short":
			return ByteBase.TYPE.SHORT;
		case "int":
			return ByteBase.TYPE.INT;
		case "float":
			return ByteBase.TYPE.FLOAT;
		case "string":
			return ByteBase.TYPE.STRING;
		case "int[]":
			return ByteBase.TYPE.ARRAY_INT;
		case "float[]":
			return ByteBase.TYPE.ARRAY_FLOAT;
		case "string[]":
			return ByteBase.TYPE.ARRAY_STRING;
		//case "vector2":
		//	return ByteBase.TYPE.VECTOR2;
		//case "vector3":
		//	return ByteBase.TYPE.VECTOR3;
		}
		return ByteBase.TYPE.NONE;
	}
	
	public virtual void Read()
	{
	}
	
	public virtual void Write()
	{
	}
	
	public byte[] Serialization()
	{
		Write();
		return GetByte();
	}

    public void Serialization(MemoryStream stream) {
        SetStream(stream);
        Write();
    }
	
	public void Deserialization(MemoryStream stream)
	{
		SetStream(stream);
		Read();
	}
	
	public void Deserialization(byte[] data, int position = 0)
	{
		SetByte(data, position);
		Read();
	}
	
	public void SetStream(MemoryStream stream)
	{
		buff = stream;
	}

    public MemoryStream GetStream()
    {
        return buff;
    }
	
	public void SetByte(Byte[] buf, int position = 0)
	{
		buff = new MemoryStream(buf);
		buff.Position = position;
	}
	
	public byte[] GetByte()
	{
		return buff.ToArray();
	}
	
	public void WriteBool(bool value) {
		buff.WriteByte(value ? (byte)1 : (byte)0);
	}
	
	public bool ReadBool() {
		return buff.ReadByte() == 1;
	}
	
	public void WriteShort(short value)
	{
		byte[] tmp = BitConverter.GetBytes(value);
		buff.Write(tmp, 0, tmp.Length);
	}
	
	public void WriteInt(int value)
	{
		byte[] tmp = BitConverter.GetBytes(value);
		buff.Write(tmp, 0, tmp.Length);
	}
	
	public void WriteFloat(float value)
	{
		byte[] tmp = BitConverter.GetBytes(value);
		buff.Write(tmp, 0, tmp.Length);
	}
	
	public void WriteString(string value)
	{
		Byte[] tmp = System.Text.Encoding.Default.GetBytes(value);
		WriteInt(tmp.Length);
		buff.Write(tmp, 0, tmp.Length);
	}
	
	public void WriteExcelString(string value)
	{
		Encoding def = Encoding.Default;
		Encoding utf = Encoding.UTF8;
		
		byte[] tmp = def.GetBytes(value);
		tmp = Encoding.Convert(def, utf, tmp);
		WriteInt(tmp.Length);
		buff.Write(tmp, 0, tmp.Length);
	}
	
	//public void WriteVector2(string value)
	//{
	//	string[] v = value.Split(',');
	//	float v1 = 0, v2 = 0;
	//	if (v.Length != 2 || !float.TryParse(v[0], out v1) || !float.TryParse(v[1], out v2))
	//	{
	//		//Loger.Error("VECTOR2 内容出错： value = " + value);
	//	}
	//	WriteFloat(v1);
	//	WriteFloat(v2);
	//}
	
	//public void WriteVector3(string value)
	//{
	//	string[] v = value.Split(',');
	//	float v1 = 0, v2 = 0, v3 = 0;
	//	if (v.Length != 3 || !float.TryParse(v[0], out v1) || !float.TryParse(v[1], out v2) || !float.TryParse(v[2], out v3))
	//	{
	//		//Loger.Error("VECTOR2 内容出错： value = " + value);
	//	}
	//	WriteFloat(v1);
	//	WriteFloat(v2);
	//	WriteFloat(v3);
	//}

	//public void WriteVector2(Vector2 value){
	//	WriteFloat(value.x);
	//	WriteFloat(value.y);
	//}

	//public void WriteVector3(Vector3 value){
	//	WriteFloat(value.x);
	//	WriteFloat(value.y);
	//	WriteFloat(value.z);
	//}
	
	public void WriteArrayInt(int[] value)
	{
		WriteInt(value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			WriteInt(value[i]);
		}
	}
	
	public void WriteArrayFloat(float[] value)
	{
		WriteInt(value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			WriteFloat(value[i]);
		}
	}

	//public Vector3[] ReadArrayVector3(){
	//	int len = ReadInt ();
	//	Vector3[] result = new Vector3[len];
	//	for (int i=0; i<len; i++) {
	//		result[i] = new Vector3(ReadFloat(),ReadFloat(),ReadFloat());
	//	}
	//	return result;
	//}
	
	//public void WriteArrayVector3(Vector3[] value){
	//	WriteInt (value.Length);
	//	for (int i=0; i<value.Length; i++) {
	//		WriteFloat(value[i].x);
	//		WriteFloat(value[i].y);
	//		WriteFloat(value[i].z);
	//	}
	//}

	//public Vector2[] ReadArrayVector2(){
	//	int len = ReadInt ();
	//	Vector2[] result = new Vector2[len];
	//	for (int i=0; i<len; i++) {
	//		result[i] = new Vector2(ReadFloat(),ReadFloat());
	//	}
	//	return result;
	//}

	//public void WriteArrayVector2(Vector2[] value){
	//	WriteInt (value.Length);
	//	for (int i=0; i<value.Length; i++) {
	//		WriteFloat(value[i].x);
	//		WriteFloat(value[i].y);
	//	}
	//}
	
	public void WriteArrayExcelStr(string[] value)
	{
		WriteInt(value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			WriteExcelString(value[i]);
		}
	}
	
	public void WriteArrayStr(string[] value)
	{
		WriteInt(value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			WriteString(value[i]);
		}
	}
	
	public short ReadShort()
	{
		byte[] tmp = new byte[2];
		buff.Read(tmp, 0, 2);
		return BitConverter.ToInt16(tmp, 0);
	}
	
	public int ReadInt()
	{
		byte[] tmp = new byte[4];
		buff.Read(tmp, 0, 4);
		return BitConverter.ToInt32(tmp, 0);
	}
	
	public float ReadFloat()
	{
		byte[] tmp = new byte[4];
		buff.Read(tmp, 0, 4);
		return BitConverter.ToSingle(tmp, 0);
	}
	
	public string ReadString()
	{
		int len = ReadInt();
		
		byte[] tmp = new byte[len];
		buff.Read(tmp, 0, len);
		string str = Encoding.UTF8.GetString(tmp);
		return str;
	}
	
	//public Vector2 ReadVector2()
	//{
	//	Vector2 result = new Vector2(ReadFloat(), ReadFloat());
	//	return result;
	//}
	
	//public Vector3 ReadVector3()
	//{
	//	Vector3 result = new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
	//	return result;
	//}
	
	public int[] ReadArrayInt()
	{
		int len = ReadInt();
		int[] result = new int[len];
		for (int i = 0; i < len; i++)
		{
			result[i] = ReadInt();
		}
		return result;
	}
	
	public float[] ReadArrayFloat()
	{
		int len = ReadInt();
		float[] result = new float[len];
		for (int i = 0; i < len; i++)
		{
			result[i] = ReadFloat();
		}
		return result;
	}
	
	public string[] ReadArrayStr()
	{
		int len = ReadInt();
		string[] result = new string[len];
		for (int i = 0; i < len; i++)
		{
			result[i] = ReadString();
		}
		return result;
	}
}
