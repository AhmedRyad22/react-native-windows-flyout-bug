using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AdexcloudS3Library
{

    public struct Drive
    {
        public int process_id;
        public string name;
    }

    public class Managedrives
    {

        public static bool mountDrive(string remoteName, string mountPoint, Action<string> log)
        {
            string json = File.ReadAllText("drives.json");
            
            Drive[] drives = JsonConvert.DeserializeObject<Drive[]>(json);
            
            if (drives.Length > 0 )
            {
                foreach (Drive drive in drives)
                {
                    if (mountPoint == drive.name )
                    {
                        log("Mount drive error : Drive " + mountPoint + " already exist.");
                        return false;
                    }
                }
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "rclone.exe",
                Arguments = $"mount data:/{remoteName} {mountPoint} --vfs-cache-mode full",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,

            };
            try
            {
                using (var process = Process.Start(processStartInfo))
                {
                    if (!process.HasExited)
                    {

                        drives.Append(new Drive() { name = mountPoint, process_id = process.Id });
                        
                        writeConfig<Drive[]>( drives, "drives" );
                        
                        log("Drive " + mountPoint + " mounted succesfuly.");
                        
                        return true;
                    }
                    else
                    {
                        log("Mount drive error : Drive " + mountPoint + " already exist.");
                        return false;
                    }

                }

            }
            catch (Exception ex)
            {
                log($"Error mounting: {ex.Message}");
                return false;
            }


        }

        public static bool unmountDrive(string mountPoint, Action<string> log)
        {
            string json = File.ReadAllText("drives.json");
            
            Drive[] drives = JsonConvert.DeserializeObject<Drive[]>(json);
            
            bool unmounted = false;
            
            foreach (Drive drive in drives)
            {
                if (mountPoint == drive.name)
                {
                    Process.GetProcessById( drive.process_id ).Kill();

                    drives = drives.Where(x => x.name != mountPoint).ToArray();
                    
                    writeConfig<Drive[]>( drives, "drives" );
                }
            }

            return true;
        }

        public static bool unmountAllDrives(Action<string> log)
        {
            
            writeConfig<Drive[]>(Array.Empty<Drive>() , "drives");
            
            foreach (var p in Process.GetProcessesByName("rclone"))
            {
                p.Kill();
            }
            
            log("All drives are unmounted succesfuly");

            return true;
        }

        public static bool testConnection(string access_key, string secret_key)
        {

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "rclone.exe",
                Arguments = "about . --s3-env-auth",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            try
            {
                using (var process = Process.Start(processStartInfo))
                {
                    //process.OutputDataReceived += (s, args) => MessageBox.Show(args.Data, "Test Connection by line"); 
                    //process.BeginOutputReadLine();
                    string stringoutput = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    return true;
                    //process.Kill();
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string getAccessKey()
        {
            return new ConfigurationBuilder().AddJsonFile("credentials.json")
                .Build()
                .GetSection("access_key")
                .Value;
        }

        public static string getSecretkey()
        {
            return new ConfigurationBuilder().AddJsonFile("credentials.json")
                .Build()
                .GetSection("secret_key")
                .Value;
        }

        public static Drive[] getDrives()
        {
            string json = File.ReadAllText("drives.json");
            
            return JsonConvert.DeserializeObject<Drive[]>(json);
        }

        public void saveCredentials(string access_key, string secret_key)
        {
            var data = new Dictionary<string, string>();
            
            data.Add("secret_key", secret_key);
            data.Add("access_key", access_key);
            
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            
            File.WriteAllText("credentials.json", json);
        }

        public static void listFolders()
        {
            string[] buckets;
            
            var processStartInfo = new ProcessStartInfo
            {
                //FileName = "cmd.exe",
                FileName = "rclone.exe",
                //Arguments = $"/C rclone lsd {remoteName}:/data --s3-env-auth --s3-access-key-id={accessKeyId} --s3-secret-access-key={secretAccessKey} --s3-endpoint={endpoint} --s3-region={region}",
                Arguments = "lsjson data:",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            try
            {
                using (var process = Process.Start(processStartInfo))
                {
                    //process.OutputDataReceived += (s, args) => MessageBox.Show(args.Data, "List folders by line");
                    //process.BeginOutputReadLine();
                    string stringoutput = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    //process.Kill();
                }

            }
            catch (Exception ex)
            {
                
            }
        }

        public static void writeConfig<T>( T config, string file_name)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            
            File.WriteAllText(file_name, json);
        }
        
    }
}
