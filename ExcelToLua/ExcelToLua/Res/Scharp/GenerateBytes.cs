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
                    string typestr = RowType[mDc].ToString().ToLower().Trim();
                    ByteBase.TYPE type = ByteBase.GetTypeByName(typestr);

                    string str = mRow[mDc].ToString();
                    switch (type)
                    {
                        case ByteBase.TYPE.ARRAY_FLOAT:
                            string[] strlist = str.Split(';');
                            float[] list = new float[strlist.Length];
                            for(int r = 0; r< strlist.Length; r++)
                            {
                                list[r] = int.Parse(strlist[r]);
                            }
                            byteb.WriteArrayFloat(list);
                            break;
                        case ByteBase.TYPE.ARRAY_INT:
                            string[] intstrlist = str.Split(';');
                            int[] intlist = new int[intstrlist.Length];
                            for (int r = 0; r < intstrlist.Length; r++)
                            {
                                intlist[r] = int.Parse(intstrlist[r]);
                            }
                            byteb.WriteArrayInt(intlist);
                            break;
                        case ByteBase.TYPE.ARRAY_STRING:
                            string[] strlist1 = str.Split(';');
                            byteb.WriteArrayStr(strlist1);
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
                           
                            byteb.WriteString(str);
                            break;
                    }
                }
            }
            byte[] bt = byteb.GetByte();
            string pathfile = MyConfig.dataDir + filestr + ".bytes";
            File.WriteAllBytes(pathfile, bt);

            string MsgStr = pathfile + "已完成加载";
            param.completeH.Invoke(MsgStr);
        }
    }
}
