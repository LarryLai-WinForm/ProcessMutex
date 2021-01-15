using System.Diagnostics;
using System.Windows.Forms;

namespace ProcessMutex
{
    using WindowsAPI;

    /// <summary>
    /// 使用GUID判斷重複執行
    /// </summary>
    public class WinForm<TForm> : Base where TForm : Form,new()
    {
        protected sealed override void HandleRunningInstance(Process instance)
        {
            Func.HandleRunningInstance(instance);
        }
        protected sealed override void ApplicationRun(string[] args)
        {
            Application.Run(new TForm());
        }

        public void Run()
        {
            base.Run();
        }

        protected override void Func_OnRepeatExecution(string AssemblyFullName)
        {
            MessageBox.Show(AssemblyFullName + "禁止重複執行");
        }
    }

    ////ProcessMutex_WinForm EX
    //static class Program
    //{
    //    /// <summary>
    //    /// 應用程式的主要進入點。
    //    /// </summary>
    //    [STAThread]
    //    static void Main()
    //    {
    //        Application.EnableVisualStyles();
    //        Application.SetCompatibleTextRenderingDefault(false);
    //        //Application.Run(new Form());
    //        new Common.WindowsAPI.ProcessMutex_WinForm<Form>().Run();
    //    }
    //}
}
