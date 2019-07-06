/* ============================================================================== 
* 功能描述：GenerateLua 
* 创 建 者：全国祥
* 联系方式：13826503059
* 创建日期：2018/12/21 11:26:38 
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExcelToLua.Res
{
    public class GenerateLua
    {
        MyParams param;
        public void ToLua(MyParams par)
        {
            param = par;

            string errorStr;
            DataSet dataSet = ReadExcel.ExcelToDataSet(param.filePath, out errorStr);

            if (!string.IsNullOrEmpty(errorStr))
            {
                param.errorH.Invoke(errorStr);
                return;
            }

            DataTable table = dataSet.Tables[0];
            if (table.Rows.Count <= 3)
            {
                errorStr = param.filePath +"表格格式不正确，至少3行";
                param.errorH.Invoke(errorStr);
                return;
            }
            DataRow RowDes = table.Rows[0];
            DataRow RowName = table.Rows[1];
            DataRow RowKey = table.Rows[2];
            //检查字段是否重复
            if (CheckError.CheckRepeatDataRow(param.filePath, RowName, table.Columns,2, out errorStr))
            {
                param.errorH.Invoke(errorStr);
                return;
            }

            DataColumn ColumnKey = table.Columns[0];
            //检查Key是否重复
            if (CheckError.CheckRepeatDataColumn(param.filePath,ColumnKey, table.Rows,1, out errorStr))
            {
                param.errorH.Invoke(errorStr);
                return;
            }
            //获取关联表的key 列表
            Dictionary<string, List<string>> dic = GetL_tableKey(RowName, RowKey, table.Columns);

            string filestr = param.filePath.Replace(MyConfig.excelDir, "").Replace(MyConfig.xlsx, "").Replace(MyConfig.xls, "");

            string luaStr = "local " + filestr + "={\n";
            for (int i = 3; i < table.Rows.Count; i++)
            {
                DataRow mDr = table.Rows[i];

                string key = "";
                string str = "{";
                for(int j = 0;j < table.Columns.Count;j++)
                {
                    DataColumn mDc = table.Columns[j];
                    if (RowKey[mDc].ToString().ToLower() == "key" && string.IsNullOrEmpty(mDr[mDc].ToString()))
                    {
                        break;
                    }
                    if (string.IsNullOrEmpty(RowName[mDc].ToString()))
                    {
                        continue;
                    }
                    if (RowKey[mDc].ToString().ToLower() == "key")
                    {
                        key += "[" + mDr[mDc] + "]=";
                        str += RowName[mDc].ToString() + "=" + mDr[mDc].ToString() + ", ";
                    }
                    else {
                        bool islink = false;
                        if (RowKey[mDc].ToString().StartsWith(MyConfig.linkTable))
                        {
                            islink = true;
                            //--检查关联表是否有这个字段------------------------------------
                            string file = RowKey[mDc].ToString().Substring(MyConfig.linkTable.Length);
                            if (dic.ContainsKey(file))
                            {
                                if (!dic[file].Contains(mDr[mDc].ToString()))
                                {
                                    int rowIndex = i + 1;
                                    int columnIndex = j + 1;
                                    errorStr = param.filePath + "表," + rowIndex + "行" + MyConfig.ColumnToA_Z(columnIndex) + "列;" + "关联表" + file + "的Key值没有该值：" + mDr[mDc].ToString();
                                    param.errorH.Invoke(errorStr);
                                }
                            }
                        }
                    
                        string value = mDr[mDc].ToString();
                        if (value.Contains('|') || value.Contains(';'))
                        {
                            string[] list1 = value.Split(';');
                            if (list1.Length > 1)
                            {
                                str += RowName[mDc].ToString() + "={";
                                for (int r = 0; r < list1.Length; r++)
                                {
                                    int index = r + 1;
                                    string str1 = GetLuaList(list1[r], '|');
                                    str += "[" + index + "]=" + str1;
                                }
                                str += "}, ";
                            }
                            else {

                                if (islink)
                                {
                                    str += RowName[mDc].ToString() + "=" + list1[0] + ", ";
                                }
                                else
                                {
                                    str += RowName[mDc].ToString() + "=\"" + list1[0] + "\", ";
                                }
                                
                            }
                        }
                        else {
                            if (islink)
                            {
                                str += RowName[mDc].ToString() + "=" + value + ", ";
                            }
                            else
                            {
                                str += RowName[mDc].ToString() + "=\"" + value + "\", ";
                            }
                        }
                    }
                }
                if (!str.Equals("{"))
                {
                    str += "},\n";

                    str = "\t" + key + str;

                    luaStr += str; 
                }
            }

            luaStr += "}\nreturn " + filestr;

            string luaFile = MyConfig.luaDir + filestr + ".lua";
            File.WriteAllText(luaFile, luaStr);

            string MsgStr = luaFile + "已完成加载";
            param.completeH.Invoke(MsgStr);
        }

        string GetLuaList(string srcStr,char sp)
        {
            string lualist = "";
            string[] list = srcStr.Split(sp);
            if (list.Length > 1)
            {
                lualist += "{";
                for (int i = 0; i < list.Length; i++)
                {
                    int index = i + 1;
                    lualist += "\"" + list[i] + "\", ";
                }
                lualist += "},";
            }
            else
            {
                lualist ="\""+ list[0]+"\", ";
            }
            return lualist;
        }

        /// <summary>
        ///获取关联表的Key
        /// </summary>
        /// <param name="RowName"></param>
        /// <param name="RowKey"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        Dictionary<string, List<string>> GetL_tableKey(DataRow RowName, DataRow RowKey, DataColumnCollection columns)
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

            for (int i = 0; i< columns.Count;i++)
            {
                DataColumn column = columns[i];
                if (!string.IsNullOrEmpty(RowName[column].ToString()))
                {
                    string rowkey = RowKey[column].ToString();
                    Console.WriteLine(rowkey);
                    if (rowkey.StartsWith(MyConfig.linkTable))
                    {
                        string file = rowkey.Substring(MyConfig.linkTable.Length);
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

        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }
    }
}
