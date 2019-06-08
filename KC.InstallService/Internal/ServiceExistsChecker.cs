using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace KC.InstallServiceNS.Internal {
    public static class ServiceExistsChecker {
        public static bool ServiceExists(string serviceName) {
            ServiceController service = null;
            try {
                service = ServiceController.GetServices().FirstOrDefault(x => x.ServiceName == serviceName);
                return service != null;
            }
            catch (Exception ex) {
                Console.WriteLine($"Failed to Find the {serviceName.Quoted()} Service: " + ex.Message);
                return false;
            }
        }
    }
}
