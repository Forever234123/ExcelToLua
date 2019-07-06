using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public partial class Test2: ByteBase {

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

public partial class Test2Mgr{
	public Dictionary<int,Test2> itemData = new Dictionary<int, Test2>();

	public int size;

	public Test2Mgr(){
		//ReadConfig ();
	}

	public void ReadConfig(object state){
        string assetPath = ConfigMgr.Path + "Test2.bytes";
        byte[] bytes = ConfigMgr.LoadBytes(assetPath);

		MemoryStream stream = new MemoryStream (bytes);
		stream.Position = 0;
		
		Test2 byteObj = new Test2 ();
		byteObj.SetStream (stream);
		size = byteObj.ReadInt ();
		
		for (int i=0; i<size; i++) {
			Test2 item = new Test2();
			item.Deserialization(stream);
			if(itemData.ContainsKey(item.ID))
                Debug.LogError("Test2 is Repeat KEY = "+item.ID);
	        itemData[item.ID]=item;
		}
		Debug.Log("Test2 Config load Complete, size:"+size);
	}

	public Test2 GetData(int  ID){
		if (itemData.ContainsKey (ID)) {
			return itemData[ID];
		}
		Debug.LogError("Test2 not find >> ID:"+ID);
		return null;
	}

	public bool HasData(int ID){
		return itemData.ContainsKey (ID);
	}
}
