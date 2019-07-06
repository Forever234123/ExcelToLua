/* ============================================================================== 
* 功能描述：GenerateCsharp 
* 创 建 者：q
* 联系方式：null
* 创建日期：2019/7/3 17:38:31 
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
   public  class GenerateCsharp
   {
        MyParams param;
        string filestr;
        public void ToScharp(MyParams par)
        {
            param = par;
            Console.WriteLine(par);

            filestr = param.filePath.Replace(MyConfig.excelDir, "").Replace(MyConfig.xlsx, "").Replace(MyConfig.xls, "");

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

        
            string scharpstr = FileTool.Instance.ClassStr;
            scharpstr = scharpstr.Replace(TMP.classTmp, filestr);

            string propertylist = "";
            string readList = "";
            string writeList = "";

            for (int j = 0; j < table.Columns.Count; j++)
            {
                DataColumn mDc = table.Columns[j];
                string type = RowType[mDc].ToString();
                type = type.ToLower().Trim();
                string property = RowName[mDc].ToString();

                ByteBase.TYPE btype = ByteBase.GetTypeByName(type);
                string typeMethod = FileTool.GetMethodStr(btype);
                string typestr = FileTool.GetTypeStr(btype);
                if (typeMethod == null)
                {
                    errorStr = string.Format("{0}表;类型错误:{1}, 3行{2}列", filestr, RowType[mDc].ToString(), MyConfig.ColumnToA_Z(j+1));
                    param.errorH.Invoke(errorStr);
                    return;
                }

                if (RowKey[mDc].ToString().ToLower().Trim()==TMP.keystr)
                {
                    scharpstr = scharpstr.Replace(TMP.keyTypeTmp, typestr);
                    scharpstr = scharpstr.Replace(TMP.keyPropertyTmp, property);
                }
                
                string item = FileTool.Instance.PropertyStr;
                string desc = RowDes[mDc].ToString().Replace("\n", "; ");
                item = item.Replace(TMP.descTmp, desc);
                item = item.Replace(TMP.propertyTmp, property);
                item = item.Replace(TMP.typeTmp, typestr);
                propertylist = propertylist + item + "\n";

                string readItem = FileTool.Instance.ReadStr;
                readItem = readItem.Replace(TMP.propertyTmp, property);
                readItem = readItem.Replace(TMP.typeTmp, typeMethod);
                readList = readList +"           " +readItem + "\n";

                string writeItem  = FileTool.Instance.WriteStr;
                writeItem = writeItem.Replace(TMP.propertyTmp, property);
                writeItem = writeItem.Replace(TMP.typeTmp, typeMethod);
                writeList = writeList +"         "+ writeItem + "\n";
            }
            scharpstr = scharpstr.Replace(TMP.propertyListTmp, propertylist);
            scharpstr = scharpstr.Replace(TMP.readListTmp, readList);
            scharpstr = scharpstr.Replace(TMP.writeListTmp, writeList);

            string scharpFile = MyConfig.luaDir + filestr + ".cs";
            File.WriteAllText(scharpFile, scharpstr);

            string MsgStr = scharpFile + "已完成加载";
            param.completeH.Invoke(MsgStr);
        }

        public string GetFileName()
        {
            return filestr;
        }
    }
}
