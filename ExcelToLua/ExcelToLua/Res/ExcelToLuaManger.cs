/* ============================================================================== 
* 功能描述：ExcelToLuaManger 
* 创 建 者：全国祥
* 联系方式：13826503059
* 创建日期：2018/12/21 11:29:13 
* ==============================================================================*/
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

    public class ExcelToLuaManger
    {
        public static ExcelToLuaManger vInstance = new ExcelToLuaManger();
        Form1 Myform;
        SynchronizationContext _syncContext;

        int AllNum = 0;
        int CurrentNum = 0;
       
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
        }
        public void Start()
        {
            Clear();
            string [] files = Directory.GetFiles(MyConfig.excelDir);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Contains("~$") &&(files[i].EndsWith(MyConfig.xlsx) || files[i].EndsWith(MyConfig.xlsx)))
                {
                    AllNum++;
                    MyParams par = new MyParams();
                    par.filePath = files[i];
                    par.completeH = ShowOutPut;
                    par.errorH = ShowError;
                    //线程
                    //Thread t = new Thread(new ParameterizedThreadStart(ThreadReadExcel));
                    //t.Start(par);

                    //线程池
                    ThreadPool.QueueUserWorkItem(ThreadReadExcel, par);
                }
            }
        }
        void ThreadReadExcel(object obj)
        {
            MyParams par = obj as MyParams;
            GenerateLua lua = new GenerateLua();
            lua.ToLua(par);
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
