using System;
using System.Linq;
using System.ServiceProcess;
using KC.InstallServiceNS;
using System.Configuration.Install;
using System.Threading;
using System.IO;
using System.ComponentModel;

namespace Example.InstallServiceNS {

    [System.ComponentModel.DesignerCategory("")]
    public class Service : ServiceBase {
        public static string Name = "Example.InstallService";

        //To try this out, you should change the debug arguments to match one of the arguments below.
        //This can be done in Visual Studio from ProjectProperties.Debug.CommandLineArguments
        //NOTE: You'll need to run Visual Studio as an admin for this to work.
        static void Main(string[] args) {
            try {
                if (args != null && args.Any(x => x == "/i")) {
                    InstallService.Install(Name);
                    return;
                }
                if (args != null && args.Any(x => x == "/stop")) {
                    InstallService.TryStopService(Name);
                    return;
                }
                if (args != null && args.Any(x => x == "/start")) {
                    InstallService.TryStopService(Name);
                    return;
                }
                if (args != null && args.Any(x => x == "/u")) {
                    InstallService.Uninstall(Name);
                    return;
                }

                if (Environment.UserInteractive) {
                    DoSomething();
                }
                else {
                    ServiceBase.Run(new Service());
                }
            }
            catch (Exception ex) {
                File.AppendAllText(Path.Combine("C:\\", "failedToStart.txt"), ex.ToString());
            }
            finally {
                if (Environment.UserInteractive) {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        public Service() {
            this.CanStop = true;
            this.ServiceName = Name;
        }

        private static bool m_Running;
        private static object lockRunning = new object();
        public static bool Running {
            get {
                lock (lockRunning)
                    return m_Running;
            }
            set {
                lock (lockRunning)
                    m_Running = value;
            }
        }

        protected override void OnStart(string[] args) {
            base.OnStart(args);

            if (Running) {
                return;
            }
            var t = new Thread(() => {
                DoSomething();
            });
            t.Name = Name;
            t.Start();
        }

        protected override void OnStop() {
            base.OnStop();
            Running = false;
            //Give the app up to 5 seconds to shut down.
            //This is coincidentally the default amount of time windows gives us
            //when the PC is rebooting, so this shouldn't go any higher.
            System.Threading.Thread.Sleep(5000);
        }

        private static void DoSomething() {
            Running = true;
            while (Running) {
                try {
                    File.AppendAllText(@"C:\InstallService.txt", $"{Environment.NewLine}{new Random().Next()}");
                }
                catch {
                    //This isn't real, so we don't need logging. We can also ignore the Thread.Sleep mechanism below for the same reason.
                }
                Thread.Sleep(3000);
            }
        }

    }

    [System.ComponentModel.DesignerCategory("")]
    [RunInstaller(true)]
    public class TheServiceInstaller : Installer {
        public TheServiceInstaller() {
            var serviceProcessInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.DisplayName = Service.Name;
            serviceInstaller.Description = Service.Name;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.DelayedAutoStart = true;
            serviceInstaller.ServiceName = Service.Name;
            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
