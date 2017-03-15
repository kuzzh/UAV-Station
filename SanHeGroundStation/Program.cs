using SanHeGroundStation.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SanHeGroundStation
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GMap.NET.GMaps.Instance.PrimaryCache = new MyImageCache();//对主存储器初始化
            Application.Run(new MainForm());
            //Application.Run(new Form1());
        }
    }
}
