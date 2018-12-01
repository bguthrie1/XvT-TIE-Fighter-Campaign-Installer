using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BackupConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = null;
            string registry_key = @"SOFTWARE\WOW6432Node\LucasArts Entertainment Company\X-Wing vs. TIE Fighter\";

            // If it can't find the key, it's probably 32-bit
            if (Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registry_key) == null)
            {
                registry_key = @"SOFTWARE\LucasArts Entertainment Company\X-Wing vs. TIE Fighter\";
            }
            // If it still can't find the registry key, quit
            if (Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registry_key) == null)
            {
                Console.WriteLine("Unable to find Balance of Power! Quitting");
                //   return;
            }

            // In my case I am getting the user selected current installation folder from the registry       
            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    if (subkey_name == "2.0")
                    {
                        using (Microsoft.Win32.RegistryKey subkey = key.OpenSubKey(subkey_name))
                        {
                            path = (string)subkey.GetValue("Install Path");

                        }
                    }
                }
            }



            string sourcePath = path;
            string targetPath = System.IO.Path.Combine(path, "BOPBACKUP");
            string tempPath = System.IO.Path.Combine(path, "TEMP");

            string[] fold = new string[6];
            fold[0] = "CAMPAIGN";
            fold[1] = "FRONTRES";
            fold[2] = "MELEE";
            fold[3] = "MOVIES";
            fold[4] = "TOURN";
            fold[5] = "TRAIN";

            string[] fi = new string[11];
            fi[0] = "fronttxt.txt";
            fi[1] = "specdesc.txt";
            fi[2] = @"CAMPAIGN\imperial.lst";
            fi[3] = @"CAMPAIGN\mission.lst";
            fi[4] = @"FRONTRES\frntspec.lst";
            fi[5] = @"FRONTRES\top.lst";
            fi[6] = @"MELEE\mission.lst";
            fi[7] = @"MOVIES\cutscene.lst";
            fi[8] = @"TOURN\mission.lst";
            fi[9] = @"TRAIN\imperial.lst";
            fi[10] = @"TRAIN\mission.lst";

            // Use Path class to manipulate file and directory paths. 
            //  string sourceFile = System.IO.Path.Combine(sourcePath, fi[]);
            //  string destFile = System.IO.Path.Combine(targetPath, fi[]);

            // To copy a folder's contents to a new location: 
            // Create a new target folder, if necessary. 

            
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }


            string fileName = null;
            string destFile = null;
            string tempFile = null;


            if (System.IO.Directory.Exists(sourcePath))
            {
                
                foreach (string f in fold)
                {
                    
                    string[] files = System.IO.Directory.GetFiles(System.IO.Path.Combine(sourcePath, f));

                    if (!System.IO.Directory.Exists(System.IO.Path.Combine(targetPath, f)))
                    {
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(targetPath, f));
                    }
                    
                    // Copy the files and overwrite destination files if they already exist.
                    /*
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        fileName = System.IO.Path.GetFileName(s);
                        destFile = System.IO.Path.Combine(targetPath, f, fileName);
                        System.IO.File.Copy(s, destFile, true);
                    }
                    */
                }
                
                foreach (string sf in fi)
                {
                    fileName = System.IO.Path.Combine(sourcePath, sf);
                    destFile = System.IO.Path.Combine(targetPath, sf); 
                    tempFile = System.IO.Path.Combine(tempPath, sf);
                    System.IO.File.Copy(fileName, destFile, true);
                    System.IO.File.Copy(tempFile, fileName, true);
                }
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(targetPath);
                if ((di.Attributes & System.IO.FileAttributes.Hidden) != System.IO.FileAttributes.Hidden)
                {
                    //Add Hidden flag    
                    di.Attributes |= System.IO.FileAttributes.Hidden;
                }
            }
            else
            {
                Console.WriteLine("BOP folder does not exist!");
            }

            // Delete the temp folder
            System.IO.Directory.Delete(tempPath, true);
        }
    }
}