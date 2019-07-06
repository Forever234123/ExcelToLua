using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public partial class Test: ByteBase {

		/// <summary>
	/// 唯一ID
	///</summary>
	public int  ID;
	/// <summary>
	/// 名字
	///</summary>
	public string  Name;
	/// <summary>
	/// 性别
	///</summary>
	public int  Sex;
	/// <summary>
	/// 列表
	///</summary>
	public string []  List;
	/// <summary>
	/// 测试
	///</summary>
	public int  cheshi;

	
	public override void Read(){
	 	           ID=ReadInt();
           Name=ReadString();
           Sex=ReadInt();
           List=ReadArrayStr();
           cheshi=ReadInt();

	}
	
	public override void Write (){
	 	         WriteInt(ID);
         WriteString(Name);
         WriteInt(Sex);
         WriteArrayStr(List);
         WriteInt(cheshi);

	}

	public System.Object Clone()  
	{
		return this.MemberwiseClone();
	}
}

public partial class TestMgr{
	public Dictionary<int,Test> itemData = new Dictionary<int, Test>();

	public int size;

	public TestMgr(){
		//ReadConfig ();
	}

	public void ReadConfig(object state){
        string assetPath = ConfigMgr.Path + "Test.bytes";
        byte[] bytes = ConfigMgr.LoadBytes(assetPath);

		MemoryStream stream = new MemoryStream (bytes);
		stream.Position = 0;
		
		Test byteObj = new Test ();
		byteObj.SetStream (stream);
		size = byteObj.ReadInt ();
		
		for (int i=0; i<size; i++) {
			Test item = new Test();
			item.Deserialization(stream);
			if(itemData.ContainsKey(item.ID))
                Debug.LogError("Test is Repeat KEY = "+item.ID);
	        itemData[item.ID]=item;
		}
		Debug.Log("Test Config load Complete, size:"+size);
	}

	public Test GetData(int  ID){
		if (itemData.ContainsKey (ID)) {
			return itemData[ID];
		}
		Debug.LogError("Test not find >> ID:"+ID);
		return null;
	}

	public bool HasData(int ID){
		return itemData.ContainsKey (ID);
	}
}
