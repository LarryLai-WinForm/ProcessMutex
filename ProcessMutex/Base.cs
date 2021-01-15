using System;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ProcessMutex
{

    /// <summary>
    /// GUID Mutex Lock
    /// </summary>
    abstract public class Base
    {
        /// <summary>Get The Same Running Instance</summary>
        /// <returns>return Process if it's not Exist returm null</returns>
        Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    string str = Assembly.GetExecutingAssembly().Location.Replace("/", "\\");
                    if (str == current.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }

        /// <summary>Application Run</summary>
        /// <param name="args">args</param>
        abstract protected void ApplicationRun(string[] args);

        /// <summary>Handle Running Instance</summary>
        /// <param name="instance">Instance</param>
        virtual protected void HandleRunningInstance(Process instance) { }

        delegate void OnRepeatExecution(string AssemblyFullName);
        OnRepeatExecution onRepeatExecution => new OnRepeatExecution(Func_OnRepeatExecution);
        abstract protected void Func_OnRepeatExecution(string AssemblyFullName);

        /// <summary>Run the application</summary>
        /// <param name="args">args</param>
        protected void Run(string[] args = null)
        {
            ///這裡要注意使用取得的Assembly是否正確
            ///再此類別包裝為DLL的狀況下
            ///GetAssembly,GetCallingAssembly,GetExecutingAssembly傳回的都是DLL本身
            ///會造成所有引用此DLL的程式都取得相同GUID而無法同時執行
            ///只有使用GetEntryAssembly才會正確回傳該執行程式
            Assembly asm = Assembly.GetEntryAssembly();
            Attribute guid_attr = Attribute.GetCustomAttribute(asm, typeof(GuidAttribute));
            string sGuid = ((GuidAttribute)guid_attr).Value;

            ///如果要做到跨Session唯一，名稱可加入"Global\"前綴字
            ///如此即使用多個帳號透過Terminal Service登入系統
            ///整台機器也只能執行一份
            using (Mutex m = new Mutex(false, "Global\\" + sGuid))
            {
                //檢查是否同名Mutex已存在(表示另一份程式正在執行)
                if (!m.WaitOne(0, false))
                {
                    //Get the running instance.   
                    Process instance = RunningInstance();
                    // There is another instance of this process.
                    HandleRunningInstance(instance);

                    //重複執行時想要呼叫的函式
                    onRepeatExecution?.Invoke(asm.GetName().Name);

                    return;
                }

                ///如果是Windows Form，Application.Run()要包在using Mutex範圍內
                ///以確保WinForm執行期間Mutex一直存在
                ApplicationRun(args);
            }
        }
    }

}
