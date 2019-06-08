using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KC.InstallServiceNS.Internal {
    public static class InstallUtilGetter {
        /// <summary>
        /// Find the highest version of the .net framework which has an installutil.exe file.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static FileInfo getBestInstallUtil() {
            try {
                var dotNetDir = new DirectoryInfo(@"C:\Windows\Microsoft.NET\Framework");
                if (!dotNetDir.Exists) {
                    Console.WriteLine($"Could not find system directory for service install {dotNetDir.FullName.Quoted()}");
                    return null;
                }
                FileInfo bestUtil = null;
                foreach(var subDir in dotNetDir.GetDirectories()) {
                    if (!subDir.Name.ToLower().StartsWith("v")) {
                        continue;
                    }
                    var guessUtil = new FileInfo(Path.Combine(subDir.FullName, "InstallUtil.exe"));
                    if (!guessUtil.Exists) {
                        continue;
                    }

                    var notAllNumbers = false;
                    foreach (var segment in subDir.Name.ToLower().Trim('v').Split('.')) {
                        if (segment.Trim() == "") {
                            notAllNumbers = true;
                            break;
                        }
                        if (!int.TryParse(segment, out _)) {
                            notAllNumbers = true;
                            break;
                        }
                    }
                    if (notAllNumbers) {
                        continue;
                    }

                    try {
                        if (bestUtil == null) {
                            bestUtil = guessUtil;
                        }
                        else {
                            var oldDirRev = bestUtil.Directory.Name.ToLower().Trim('v').Split('.');
                            var newDirRev = subDir.Name.ToLower().Trim('v').Split('.');
                            var minLength = Math.Min(oldDirRev.Length, newDirRev.Length);
                            var useOld = false;
                            var useNew = false;
                            for (int i = 0; i < minLength; i++) {
                                if (int.TryParse(oldDirRev[i], out var oldNum) && int.TryParse(newDirRev[i], out var newNum)) {
                                    if (oldNum > newNum) {
                                        useOld = true;
                                        break;
                                    }
                                    else if (newNum > oldNum) {
                                        useNew = true;
                                        break;
                                    }
                                }
                                else {
                                    useOld = true;
                                    break;
                                }
                            }
                            if (!(useOld || useNew)) {
                                if (oldDirRev.Length > newDirRev.Length) {
                                    useOld = true;
                                }
                                else {
                                    useNew = true;
                                }
                            }

                            if (useNew) {
                                bestUtil = guessUtil;
                            }
                        }
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Unable to check for InstallUtil in {subDir.Name.Quoted()}: {ex.ToString()}");
                    }
                }
                if (bestUtil == null) {
                    Console.WriteLine("Unable to find system file InstallUtil.exe");
                }
                else {
                    Console.WriteLine("InstallUtil.exe version: " + bestUtil.Directory.Name.Quoted());
                }
                return bestUtil;
            }
            catch (Exception ex) {
                Console.WriteLine("Failed to find InstallUtil: " + ex.Message);
                return null;
            }
        }
    }
}
