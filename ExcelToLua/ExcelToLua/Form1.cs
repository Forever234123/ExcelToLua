using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ExcelToLua
{
    public partial class Form1 : Form
    {
        List<string> masageList = new List<string>();
        List<string> errorList = new List<string>();

        void Clear()
        {
            masageList.Clear();
            errorList.Clear();
            OutRichTextBox.Lines = masageList.ToArray();
            ErrorRichTextBox.Lines = errorList.ToArray();

        }
        public Form1()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            Res.ExcelToLuaManger.vInstance.Init(this);

            Res.MyConfig.ReadDir();
            this.excelTextBox.Text = Res.MyConfig.excelDir;
            this.luatextBox.Text = Res.MyConfig.luaDir;

            Clear();
        }
        /// <summary>
        /// 浏览Excel目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.SelectedPath = Res.MyConfig.excelDir.Replace("/","\\");
           // path.RootFolder = Environment.SpecialFolder.
            if (path.ShowDialog() == DialogResult.OK)
            {
                Res.MyConfig.ChangedExcelDir(path.SelectedPath);
                this.excelTextBox.Text = Res.MyConfig.excelDir;
            }
        }
        /// <summary>
        /// 浏览lua目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.SelectedPath = Res.MyConfig.luaDir.Replace("/", "\\");

            if (path.ShowDialog() == DialogResult.OK)
            {
                Res.MyConfig.ChangedLuaDir(path.SelectedPath);
                this.luatextBox.Text = Res.MyConfig.luaDir;
            }
        }
        /// <summary>
        /// 导出lua
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Clear();
            Res.ExcelToLuaManger.vInstance.Start();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        public void ShowOutPut(object output)
        {
            string[] lines = OutRichTextBox.Lines;
            List<string> list = lines.ToList();
            list.Add(output.ToString());
            OutRichTextBox.Lines = list.ToArray();
        }
        public void ShowError(object error)
        {
            string[] lines = ErrorRichTextBox.Lines;
            List<string> list = lines.ToList();
            list.Add(error.ToString());
            ErrorRichTextBox.Lines = list.ToArray();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Res.MyConfig.excelDir);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Res.MyConfig.luaDir);
        }
    }
}
