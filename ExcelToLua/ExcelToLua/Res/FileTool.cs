/* ============================================================================== 
* 功能描述：FileTool 
* 创 建 者：q
* 联系方式：null
* 创建日期：2019/7/5 16:56:33 
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToLua.Res
{
    public class TMP {
        public const string classTmp = "#Class#";
        public const string propertyTmp = "#Property#";
        public const string descTmp = "#Desc#";
        public const string typeTmp = "#Type#";
        public const string propertyListTmp = "#PropertyList#";
        public const string readListTmp = "#ReadList#";
        public const string writeListTmp = "#WriteList#";
        public const string keyPropertyTmp = "#KeyProperty#";
        public const string keyTypeTmp = "#KeyType#";

        public const string configPropertyItemTmp = "#ConfigPropertyItem#";
        public const string configReadItemTmp = "#ConfigReadItem#";

        public const string keystr = "key";

    }
  public  class FileTool
  {
        const string classFile = "temp/tmp_class.txt";
        const string propertyFile = "temp/tmp_property.txt";
        const string readFile = "temp/tmp_read.txt";
        const string writeFile = "temp/tmp_write.txt";

        const string configMgrFile = "temp/tmp_configmgr.txt";
        const string configPropertyFile = "temp/tmp_configpropertyItem.txt";
        const string configReadFile = "temp/tmp_configreadItem.txt";

        public  const string byteBaseFile = "temp/tmp_ByteBase.txt";

        string _classStr;
        string _propertyStr;
        string _readStr;
        string _writeStr;
        string _configMgrStr;
        string _configPropertyStr;
        string _configReadStr;
        public FileTool()
        {
            _classStr = File.ReadAllText(classFile);
            _propertyStr = File.ReadAllText(propertyFile);
            _readStr = File.ReadAllText(readFile);
            _writeStr = File.ReadAllText(writeFile);
            _configMgrStr = File.ReadAllText(configMgrFile);
            _configPropertyStr = File.ReadAllText(configPropertyFile);
            _configReadStr = File.ReadAllText(configReadFile);
        }
        public static FileTool _instance;
        public static FileTool Instance
        {
            get {
                if (_instance == null)
                {
                    _instance = new FileTool();
                }
                return _instance;
            }
        }

        public string ClassStr
        {
            get {
                return _classStr;
            }
        }

        public string PropertyStr
        {
            get
            {
                return _propertyStr;
            }
        }
        public string ReadStr
        {
            get
            {
                return _readStr;
            }
        }
        public string WriteStr
        {
            get
            {
                return _writeStr;
            }
        }
        public string ConfigMgrStr
        {
            get
            {
                return _configMgrStr;
            }
        }
        public string ConfigPropertyStr
        {
            get {
                return _configPropertyStr;
            }
        }
        public string ConfigReadStr
        {
            get
            {
                return _configReadStr;
            }
        }
        public static string GetTypeStr(ByteBase.TYPE type)
        {
            string str = null;
            switch (type)
            {
                case ByteBase.TYPE.ARRAY_FLOAT:
                    str = "float []";
                    break;
                case ByteBase.TYPE.ARRAY_INT:
                    str = "int []";
                    break;
                case ByteBase.TYPE.ARRAY_STRING:
                    str = "string []";
                    break;
                case ByteBase.TYPE.BOOL:
                    str = "bool";
                    break;
                case ByteBase.TYPE.ENUM:
                    str = "int";
                    break;
                case ByteBase.TYPE.FLOAT:
                    str = "float";
                    break;
                case ByteBase.TYPE.INT:
                    str = "int";
                    break;
                case ByteBase.TYPE.SHORT:
                    str = "short";
                    break;
                case ByteBase.TYPE.STRING:
                    str = "string";
                    break;
            }
            return str;
        }

        public static string GetMethodStr(ByteBase.TYPE type)
        {
            string str = null;
            switch(type)
            {
                case ByteBase.TYPE.ARRAY_FLOAT:
                    str = "ArrayFloat";
                    break;
                case ByteBase.TYPE.ARRAY_INT:
                    str = "ArrayInt";
                    break;
                case ByteBase.TYPE.ARRAY_STRING:
                    str = "ArrayStr";
                    break;
                case ByteBase.TYPE.BOOL:
                    str = "Bool";
                    break;
                case ByteBase.TYPE.ENUM:
                    str = "Int";
                    break;
                case ByteBase.TYPE.FLOAT:
                    str = "Float";
                    break;
                case ByteBase.TYPE.INT:
                    str = "Int";
                    break;
                case ByteBase.TYPE.SHORT:
                    str = "Short";
                    break;
                case ByteBase.TYPE.STRING:
                    str = "String";
                    break;
            }
            return str;
        }
        /// <summary>
        /// 获取有效数据的个数
        /// </summary>
        /// <param name="table"></param>
        /// <param name="RowKey"></param>
        /// <param name="startRow"></param>
        /// <returns></returns>
        public static int GetTableDataNum(DataTable table, DataRow RowKey,int startRow)
        {
            int count = 0;
            for (int i = 0; i < table.Columns.Count; i++)
            {
                DataColumn mDc = table.Columns[i];
                if (RowKey[mDc].ToString().ToLower().Trim() == TMP.keystr)
                {
                    count = 0;
                    for (int j = startRow; j < table.Rows.Count; j++)
                    {
                        DataRow row = table.Rows[j];
                        if (string.IsNullOrEmpty(row[mDc].ToString()))
                        {
                            return count;
                        }
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        ///获取关联表的Key
        /// </summary>
        /// <param name="RowName"></param>
        /// <param name="RowKey"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
         public static   Dictionary<string, List<string>> GetL_tableKey(DataRow RowName, DataRow RowKey, DataColumnCollection columns, MyParams param)
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

            for (int i = 0; i < columns.Count; i++)
            {
                DataColumn column = columns[i];
                if (!string.IsNullOrEmpty(RowName[column].ToString()))
                {
                    string rowkey = RowKey[column].ToString();
                    Console.WriteLine(rowkey);
                    if (rowkey.StartsWith(MyConfig.linkTable))
                    {
                        string file = rowkey.Substring(MyConfig.linkTable.Length);
                        if (dic.ContainsKey(file)) continue;
                        string filefullpath = MyConfig.GetFullExcelFile(file);
                        string errorStr;
                        DataSet dataSet = ReadExcel.ExcelToDataSet(filefullpath, out errorStr);
                        if (!string.IsNullOrEmpty(errorStr))
                        {
                            param.errorH.Invoke(errorStr);

                            int rowIndex = 3;
                            int columnIndex = i + 1;
                            errorStr = param.filePath + "表;" + rowIndex + "行" + MyConfig.ColumnToA_Z(columnIndex) + "列;" + "关联表" + file + "不存在";
                            param.errorH.Invoke(errorStr);
                            continue;
                        }
                        DataTable table = dataSet.Tables[0];
                        if (table.Rows.Count <= 3)
                        {
                            errorStr = filefullpath + "表格格式不正确，至少3行";
                            param.errorH.Invoke(errorStr);
                            continue;
                        }
                        List<string> keyList = new List<string>();
                        DataColumn ColumnKey = table.Columns[0];
                        for (int j = 3; j < table.Rows.Count; j++)
                        {
                            DataRow row = table.Rows[j];
                            keyList.Add(row[ColumnKey].ToString());
                        }
                        dic.Add(file, keyList);
                    }
                }
            }
            return dic;
        }
    }
}
