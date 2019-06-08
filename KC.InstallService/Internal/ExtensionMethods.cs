using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.InstallServiceNS.Internal {
    public static class ExtensionMethods {
        public static string Quoted(this string self) {
            return $"\"{self?.Trim('"')}\"";
        }
    }
}
