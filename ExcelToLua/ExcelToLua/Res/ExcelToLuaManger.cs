/* ============================================================================== 
* 功能描述：ExcelToLuaManger 
* 创 建 者：全国祥
* 联系方式：13826503059
* 创建日期：2018/12/21 11:29:13 
* ==============================================================================*/
using ExcelToLua.Res.Scharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ExcelToLua.Res
{
    public delegate void CompleteHandler(string messge);
    public delegate void ErrorHandler(string error);

    public enum GenerateType
    {
        Lua,
        Csharp,
        Bytes,
    }
    public class ExcelToLuaManger
    {
        public static ExcelToLuaManger vInstance = new ExcelToLuaManger();
        Form1 Myform;
        SynchronizationContext _syncContext;

        List<string> fileList = new List<string>();

        int AllNum = 0;
        int CurrentNum = 0;

        GenerateType curType;

        public void  Init(Form1 form)
        {
            Myform = form;
            _syncContext = SynchronizationContext.Current;

            ThreadPool.SetMaxThreads(5, 5);
        }
        void Clear()
        {
            AllNum = 0;
            CurrentNum = 0;
            fileList.Clear();
        }
        public void Start(GenerateType gType)
        {
            curType = gType;
            Clear();
            string [] files = Directory.GetFiles(MyConfig.excelDir);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Contains("~$") &&(files[i].EndsWith(MyConfig.xls) || files[i].EndsWith(MyConfig.xlsx)))
                {
                    AllNum++;
                    MyParams par = new MyParams();
                    par.filePath = files[i];
                    par.completeH = ShowOutPut;
                    par.errorH = ShowError;

                    if (gType == GenerateType.Csharp)
                    {
                        string fileName = files[i].Replace(MyConfig.excelDir, "").Replace(MyConfig.xlsx, "").Replace(MyConfig.xls, "");
                        fileList.Add(fileName);
                    }
                    //线程
                    //Thread t = new Thread(new ParameterizedThreadStart(ThreadReadExcel));
                    //t.Start(par);

                    //线程池
                    ThreadPool.QueueUserWorkItem(ThreadReadExcel, par);
                }
            }
            if (gType == GenerateType.Csharp)
            {
                CreateConfigMgr();
            }

        }
        void ThreadReadExcel(object obj)
        {
            MyParams par = obj as MyParams;
            switch (curType)
            {
                case GenerateType.Lua:
                    GenerateLua lua = new GenerateLua();
                    lua.ToLua(par);
                    break;
                case GenerateType.Csharp:
                    GenerateCsharp csharp = new GenerateCsharp();
                    csharp.ToScharp(par);
                    break;
                case GenerateType.Bytes:
                    GenerateBytes bytes = new GenerateBytes();
                    bytes.ToByte(par);
                    break;
            }
            
        }

        void CreateConfigMgr()
        {
            string configMgrStr = FileTool.Instance.ConfigMgrStr;
            string configPropertyStr = FileTool.Instance.ConfigPropertyStr;
            string configReadStr = FileTool.Instance.ConfigReadStr;

            string configStr = "";
            string propertyStr = "";
            string readStr = "";
            for (int i = 0; i < fileList.Count; i++)
            {
                string propertyitem = configPropertyStr.Replace(TMP.classTmp, fileList[i]);
                string readItem = configReadStr.Replace(TMP.classTmp, fileList[i]);
                propertyStr = propertyStr + propertyitem + "\n";
                readStr = readStr + readItem + "\n";
            }
            configStr = configMgrStr.Replace(TMP.configPropertyItemTmp, propertyStr).Replace(TMP.configReadItemTmp, readStr);
            string configFile = MyConfig.luaDir +  "ConfigMgr.cs";
            File.WriteAllText(configFile, configStr);

            string btyeBaseStr = File.ReadAllText(FileTool.byteBaseFile);
            string btyeBaseFile = MyConfig.luaDir + "ByteBase.cs";
            File.WriteAllText(btyeBaseFile, btyeBaseStr);
        }


        public void ShowOutPut(string output)
        {
            CurrentNum++;
            _syncContext.Post(Myform.ShowOutPut, output);
            //Myform.ShowOutPut(output);
            if (AllNum == CurrentNum)
            {
                _syncContext.Post(Myform.ShowOutPut, "已全部加载完成");
            }
        }
        public void ShowError(string error)
        {
            _syncContext.Post(Myform.ShowError, error);
            //Myform.ShowError(error);
        }
    }

    public class MyParams
    {
        public string filePath;
        public CompleteHandler completeH;
        public ErrorHandler errorH;

    }
}
