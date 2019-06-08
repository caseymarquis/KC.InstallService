using KC.InstallServiceNS.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KC.InstallServiceNS {
    public class InstallService {
        /// <summary>
        /// Install the service with a given name. Optionally, if the executing assembly is
        /// not the assembly containing the Service and ServiceInstaller, then you can specify
        /// the correct assembly.
        /// </summary>
        public static void Install(string serviceName, Assembly serviceAssembly = null) {
            if (serviceAssembly == null) {
                serviceAssembly = Assembly.GetEntryAssembly();
            }
            try {
                Console.WriteLine("Starting Installation");
                if (!ServiceStopper.StopTheService(serviceName)) {
                    return;
                }
                
                if (ServiceExistsChecker.ServiceExists(serviceName)) {
                    Console.WriteLine("Service Already Installed");
                }
                else {
                    Console.WriteLine("Installing Service");
                    var bestInstallUtil = InstallUtilGetter.getBestInstallUtil();
                    if (bestInstallUtil == null) {
                        return;
                    }
                    var execFilePath = serviceAssembly.Location;
                    if (!RunCmdProcess.RunProcess(bestInstallUtil.FullName, execFilePath.Quoted(), Path.GetFullPath(".\\"))) {
                        Console.WriteLine("Error installing service.");
                        Console.WriteLine("To manually install the service, run:");
                        Console.WriteLine($"{bestInstallUtil.FullName.Quoted()} {execFilePath.Quoted()}");
                        return;
                    }
                    else {
                        Console.WriteLine("Service Installed Successfully.");
                    }
                }

                if (ServiceStarter.StartTheService(serviceName)) {
                    Console.WriteLine("Successfully installed!");
                }
                else {
                    Console.WriteLine($"Successfully installed, but unable to start {serviceName} Service.");
                    Console.WriteLine("Try starting the service manually.");
                    Console.WriteLine($"If unable to start the service, contact {serviceName} support.");
                }

            }
            catch (Exception ex) {
                Console.WriteLine("Installation failed with error: " + ex.Message); 
            }
        }

        public static void TryStopService(string serviceName) {
            Console.WriteLine("Stopping Service");
            ServiceStopper.StopTheService(serviceName);
        }

        public static void TryStartService(string serviceName) {
            Console.WriteLine("Starting Service");
            if (ServiceStarter.StartTheService(serviceName)) {
                Console.WriteLine("Service Started");
            }
            else {
                Console.WriteLine($"Unable to start {serviceName} Service.");
            }
        }

        public static void Uninstall(string serviceName) {
            Console.WriteLine("Starting Uninstall");
            ServiceStopper.StopTheService(serviceName);
            Console.WriteLine($"Uninstalling Service: {serviceName}");
            RunCmdProcess.RunProcess("sc", $"delete {serviceName}", Path.GetFullPath(".\\"));
        }
    }
}
