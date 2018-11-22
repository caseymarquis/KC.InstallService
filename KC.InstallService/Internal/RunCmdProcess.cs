using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.InstallServiceNS.Internal {
    public static class RunCmdProcess {
        public static bool RunProcess(string fileName, string args, string workingDir) {
            try {
                var proc = new Process() {
                    StartInfo = new ProcessStartInfo() {
                        FileName = fileName,
                        WorkingDirectory = workingDir,
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                    }
                };
                proc.Start();
                var stdOut = "";
                while (!proc.StandardOutput.EndOfStream) {
                    stdOut += proc.StandardOutput.ReadLine() + Environment.NewLine;
                    Console.Write(".");
                }
                Console.WriteLine();
                if (proc.ExitCode == 0) {
                    return true;
                }
                Console.WriteLine(stdOut);
                try {
                    while (!proc.StandardError.EndOfStream) {
                        Console.WriteLine(proc.StandardError.ReadLine());
                    }
                }
                catch { }
                return false;
            }
            catch (Exception ex) {
                Console.WriteLine($"Error Running Process{fileName} {args} in {workingDir}: " + ex.Message);
                return false;
            }
        }
    }
}
