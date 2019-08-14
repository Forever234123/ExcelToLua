/* ============================================================================== 
* 功能描述：GenerateBytes 
* 创 建 者：q
* 联系方式：null
* 创建日期：2019/7/5 20:17:27 
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToLua.Res.Scharp
{
    public class GenerateBytes
    {
        MyParams param;
        string filestr;
        public void ToByte(MyParams par)
        {
            param = par;
            Console.WriteLine(par);


            string errorStr;
            DataSet dataSet = ReadExcel.ExcelToDataSet(param.filePath, out errorStr);

            if (!string.IsNullOrEmpty(errorStr))
            {
                param.errorH.Invoke(errorStr);
                return;
            }

            DataTable table = dataSet.Tables[0];
            if (table.Rows.Count <= 4)
            {
                errorStr = param.filePath + "表格格式不正确，至少4行";
                param.errorH.Invoke(errorStr);
                return;
            }
            DataRow RowDes = table.Rows[0];
            DataRow RowName = table.Rows[1];
            DataRow RowType = table.Rows[2];
            DataRow RowKey = table.Rows[3];
            //检查字段是否重复
            if (CheckError.CheckRepeatDataRow(param.filePath, RowName, table.Columns, 2, out errorStr))
            {
                param.errorH.Invoke(errorStr);
                return;
            }

            DataColumn ColumnKey = table.Columns[0];
            //检查Key是否重复
            if (CheckError.CheckRepeatDataColumn(param.filePath, ColumnKey, table.Rows, 1, out errorStr))
            {
                //errorStr = 
                param.errorH.Invoke(errorStr);
                return;
            }
            //获取关联表的key 列表
            Dictionary<string, List<string>> dic = FileTool.GetL_tableKey(RowName, RowKey, table.Columns, param);

            filestr = param.filePath.Replace(MyConfig.excelDir, "").Replace(MyConfig.xlsx, "").Replace(MyConfig.xls, "");

            
            MemoryStream ms = new MemoryStream();
            ByteBase byteb = new ByteBase();
            byteb.SetStream(ms);
            int count = FileTool.GetTableDataNum(table, RowKey, 4);
            byteb.WriteInt(count);
            int startNum = 4;
            int allnum = startNum + count;
            for (int i = startNum; i < allnum; i++)
            {
                DataRow mRow = table.Rows[i];
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    DataColumn mDc = table.Columns[j];

                    if (RowKey[mDc].ToString().Trim().Equals(MyConfig.Invalid))//设置为无效字段
                    {
                        continue;
                    }

                    string property = RowName[mDc].ToString();
                    string typestr = RowType[mDc].ToString().ToLower().Trim();
                    if (string.IsNullOrEmpty(property) || string.IsNullOrEmpty(typestr))
                    {
                        continue;
                    }
                    //--检查关联表是否有这个字段------------------------------------
                    if (RowKey[mDc].ToString().StartsWith(MyConfig.linkTable))
                    {
                        string file = RowKey[mDc].ToString().Substring(MyConfig.linkTable.Length);
                        if (dic.ContainsKey(file))
                        {
                            if (!dic[file].Contains(mRow[mDc].ToString()))
                            {
                                int rowIndex = i + 1;
                                int columnIndex = j + 1;
                                errorStr = param.filePath + "表," + rowIndex + "行" + MyConfig.ColumnToA_Z(columnIndex) + "列;" + "关联表" + file + "的Key值没有该值：" + mRow[mDc].ToString();
                                param.errorH.Invoke(errorStr);
                            }
                        }
                    }
                    //------------------------------------------------------------

                    ByteBase.TYPE type = ByteBase.GetTypeByName(typestr);

                    string str = mRow[mDc].ToString();
                    try
                    {
                        switch (type)
                        {
                            case ByteBase.TYPE.ARRAY_FLOAT:
                                string[] strlist = GetSplit(str).ToArray(); //str.Split(';');
                                float[] list = new float[strlist.Length];
                                for (int r = 0; r < strlist.Length; r++)
                                {
                                    list[r] = float.Parse(strlist[r]);
                                }
                                byteb.WriteArrayFloat(list);
                                break;
                            case ByteBase.TYPE.ARRAY_INT:
                                string[] intstrlist =  GetSplit(str).ToArray(); // str.Split(';');
                                int[] intlist = new int[intstrlist.Length];
                                for (int r = 0; r < intstrlist.Length; r++)
                                {
                                    intlist[r] = int.Parse(intstrlist[r]);
                                }
                                byteb.WriteArrayInt(intlist);
                                break;
                            case ByteBase.TYPE.ARRAY_STRING:
                                string[] strlist1 = GetSplit(str).ToArray(); //str.Split(';');
                                byteb.WriteArrayExcelStr(strlist1);
                                break;
                            case ByteBase.TYPE.BOOL:
                                bool b = bool.Parse(str);
                                byteb.WriteBool(b);
                                break;
                            case ByteBase.TYPE.ENUM:
                                int e = int.Parse(str);
                                byteb.WriteInt(e);
                                break;
                            case ByteBase.TYPE.FLOAT:
                                float f = float.Parse(str);
                                byteb.WriteFloat(f);
                                break;
                            case ByteBase.TYPE.INT:
                                int int1 = int.Parse(str);
                                byteb.WriteInt(int1);
                                break;
                            case ByteBase.TYPE.SHORT:
                                short s = short.Parse(str);
                                byteb.WriteShort(s);
                                break;
                            case ByteBase.TYPE.STRING:

                                byteb.WriteExcelString(str);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        errorStr = string.Format("{0}表;类型错误:{1}, {2}行{3}列", filestr, RowType[mDc].ToString(), i + 1, MyConfig.ColumnToA_Z(j + 1));
                        param.errorH.Invoke(errorStr);
                    }
                }
            }
            byte[] bt = byteb.GetByte();
            string pathfile = MyConfig.dataDir + filestr + ".bytes";
            File.WriteAllBytes(pathfile, bt);

            string MsgStr = pathfile + "已完成加载";
            param.completeH.Invoke(MsgStr);
        }
        
        List<string> GetSplit(string str)
        {
            List<string> liststr = new List<string>();
            string[] strlist1 = str.Split(';');
            for (int i = 0; i < strlist1.Length; i++)
            {
                string [] tmp = strlist1[i].Split('|');
                for (int j = 0; j < tmp.Length; j++)
                {
                    liststr.Add(tmp[j]);
                }
            }
            return liststr;
        }
    }
}
