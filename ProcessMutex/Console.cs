using System;

namespace ProcessMutex
{
    /// <summary>
    /// GUID Mutex Lock for Console
    /// </summary>
    public class Console : Base
    {
        public delegate void Main(string[] args);
        Main main;

        public Console(Main _main) : base()
        {
            main = _main;
        }

        protected override void ApplicationRun(string[] args)
        {
            main?.Invoke(args);
        }

        public new void Run(string[] args)
        {
            base.Run(args);
        }

        protected override void Func_OnRepeatExecution(string AssemblyFullName)
        {
            System.Console.WriteLine(AssemblyFullName + "禁止重複執行");
        }
    }

    ////ProcessMutex_Console EX
    //class Program
    //{
    //    static void Work(string[] args)
    //    {
    //        //To Do...
    //        Console.Write("What is your name? ");
    //        var name = Console.ReadLine();
    //    }
    //    static void Main(string[] args)
    //    {
    //        new Common.WindowsAPI.ProcessMutex_Console(Work).Run(args);
    //    }
    //}

}
