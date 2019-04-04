/* ============================================================================== 
* 功能描述：CheckError 
* 创 建 者：全国祥
* 联系方式：13826503059
* 创建日期：2018/12/22 9:07:34 
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToLua.Res
{
    public class CheckError
    {
        /// <summary>
        /// 检查关联ID是否有错误
        /// </summary>
        /// <returns></returns>
        public static bool CheckConnectIdIsError(string key, DataColumn column, DataRowCollection rows)
        {
            return false;
        }
        /// <summary>
        /// 检查行重复
        /// </summary>
        /// <returns></returns>
        public static bool CheckRepeatDataRow(string filePath, DataRow row, DataColumnCollection columns,int rowIndex,out string errorMsg)
        {
            errorMsg = "";
            Dictionary<string, int> RepeatDic = new Dictionary<string, int>();
            for (int i = 0; i < columns.Count; i++)
            {
                DataColumn column = columns[i];
                string key = row[column].ToString();
                int columnIndex = i + 1;
                if (string.IsNullOrEmpty(key)) continue;
                if (RepeatDic.ContainsKey(key))
                {
                    errorMsg = filePath +"表"+ rowIndex + "行" + MyConfig.ColumnToA_Z(columnIndex) + "列;"+ rowIndex + "行" + MyConfig.ColumnToA_Z(RepeatDic[key]) + "列;" + "\"" + key + "\" 重复";
                    return true;
                }
                else {
                    RepeatDic.Add(key, columnIndex);
                }
            }
            return false;
        }

        /// <summary>
        /// 检查行重复
        /// </summary>
        /// <returns></returns>
        public static bool CheckRepeatDataColumn(string filePath, DataColumn column, DataRowCollection rows, int columnIndex, out string errorMsg)
        {
            errorMsg = "";
            Dictionary<string, int> RepeatDic = new Dictionary<string, int>();
            for (int i = 0; i < rows.Count; i++)
            {
                DataRow row = rows[i];
                string key = row[column].ToString();
                int rowIndex = i + 1;
                if (string.IsNullOrEmpty(key)) continue;
                if (RepeatDic.ContainsKey(key))
                {
                    errorMsg = filePath + "表" + rowIndex + "行" + MyConfig.ColumnToA_Z(columnIndex) + "列;" + RepeatDic[key] + "行" + MyConfig.ColumnToA_Z(columnIndex) + "列;" + "\"" + key + "\" 重复";
                    return true;
                }
                else
                {
                    RepeatDic.Add(key, rowIndex);
                }
            }
            return false;
        }

       
    }

    
}
