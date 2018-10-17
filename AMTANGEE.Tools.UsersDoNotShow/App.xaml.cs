using System;
using System.Windows;

namespace UsersDoNotShow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            base.OnStartup(e);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyName = args.Name.Split(',')[0] + ".dll";

            string path = "";

            if (System.IO.File.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"AMTANGEE\" + assemblyName)))
                path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"AMTANGEE\" + assemblyName);
            else if (System.IO.File.Exists(@"C:\AMTANGEE\SOURCE\" + assemblyName))
                path = @"C:\AMTANGEE\SOURCE\" + assemblyName;
            else
            {
                using (Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\AMTANGEE\CRM"))
                    if (System.IO.File.Exists(System.IO.Path.Combine(reg.GetValue("Path").ToString(), assemblyName)))
                        path = System.IO.Path.Combine(reg.GetValue("Path").ToString(), assemblyName);

                using (Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AMTANGEE\CRM"))
                    if (System.IO.File.Exists(System.IO.Path.Combine(reg.GetValue("Path").ToString(), assemblyName)))
                        path = System.IO.Path.Combine(reg.GetValue("Path").ToString(), assemblyName);
            }

            System.IO.File.AppendAllText("UsersDoNotShow.log", "Resolved Assemlby path: '" + path + "'");

            if (System.IO.File.Exists(path) && !path.StartsWith("\\"))
                return System.Reflection.Assembly.LoadFile(path);

            throw new Exception("Could not resolve assembly path!");
        }
    }
}
