using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using Microsoft.Win32;
using Windows.Media.Playback;
class MicrosoftEdge_Allow_Uninstall
{
    static void Main(string[] args)
    {
        if (args.Length == 0) {
            System.Console.WriteLine("Microsoft Edge Allow Uninstall Tool");
            System.Console.WriteLine("Please use --help to interact with the Tool");
            return;
        }
        switch (args [0]) 
        {
            case "--help":
                ShowHelp();
                break;

            case "--version":
                ShowVersionInfo();
                break;

            case "--execute":
                ExecuteAction();
                break;

            case "--force-uninstall":
                ExecuteUninstall();
                break;
            
            default:
                System.Console.WriteLine("Invalid Parameter detected use --help to show some Information");
                break;
    }
}
   static void ShowHelp()
    {
        System.Console.WriteLine("Microsoft Edge Allow Uninstall - Command-line utility to manage Microsoft Edge uninstallation.");
        System.Console.WriteLine("Usage:");
        System.Console.WriteLine("  --help      Show help information.");
        System.Console.WriteLine("  --version   Display the version of the program.");
        System.Console.WriteLine("  --execute   Execute the registry change to allow uninstallation of Microsoft Edge.");
        System.Console.WriteLine("  --force-uninstall  Force Uninstallation of MS Edge if previous Execute parameter doesn't work");
    }
    static void ShowVersionInfo()
    {
        System.Console.WriteLine("Microsoft Edge Allow Uninstall V1.0 ALPHA");
    }

    static void ExecuteAction()
    {
        try
        {
            const string RegistryPath = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Edge";
            const string RegistryKey = "NoRemove";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(RegistryPath, writable: true))
            {
                if (key == null)
                {
                    ShowErrorInfo("Registry Path not found ! You may have deleted this registry value, Exiting...");
                    return;
                }
                int CurrentValue = GetRegistryValue(key, RegistryKey);
                if (CurrentValue == -1)
                {
                    ShowErrorInfo("Registry Value was not found");
                    return;
                }
                if (CurrentValue == 0)
                {
                    ShowErrorInfo("You may already run this program or set the registry manually. You can already uninstall Edge via Settings.");
                }
                else
                {
                    SetRegistryValue(key, RegistryKey, 0);
                }


            }
        }
        catch (UnauthorizedAccessException)
        {
            ShowErrorInfo("Unless you somehow Bypassed the executable Admin permission or what, This program can't run without Admin Permission dude !, Exiting...");
        }
        catch (Exception ex)
        {
            ShowErrorInfo("Fatal error occurred: " + ex.Message);
        }
    }

    static void ExecuteUninstall()
    {
        try
        {
            string? SystemDrv = Path.GetPathRoot(Environment.SystemDirectory) ?? @"C:\\"; // Get System Drive Letter and return C:\ as fallback
            string EdgeVer = GetEdgeVersion();
            

            if (!string.IsNullOrEmpty(EdgeVer))
            {
                Console.WriteLine($"Microsoft Edge Version Detected: {EdgeVer}");
            }
            else
            {
                Console.WriteLine("Microsoft Edge version could not be detected, It's Possible the file was corrupted");
            }

            // Combine Full Path of SystemDrv and EdgeVer

            String EdgeProgramPath = Path.Combine(
                SystemDrv, @"Program Files (x86)\Microsoft\Edge\Application", EdgeVer, @"Installer\setup.exe");
            Console.WriteLine($"Installation Path: {EdgeProgramPath}");

            // Check if the file exists
            if (!File.Exists(EdgeProgramPath))
            {
                Console.WriteLine($"Setup file not found at: {EdgeProgramPath}");
                return;
            }
            // Create a new process to execute the setup.exe with parameters
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = EdgeProgramPath,
                Arguments = "--uninstall --system-level --verbose-logging --force-uninstall",
                UseShellExecute = true // UseShellExecute allows elevation prompts if needed
            };

            Process process = Process.Start(processInfo);
            if (process == null)
            {
                Console.WriteLine("Failed to start the uninstallation process.");
                return;
            }

           
            process.WaitForExit();

            Console.WriteLine("Uninstallation process executed successfully.");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal Error Occured: {ex.Message}");
        }
    }
    public static string GetEdgeVersion() // Get Edge version
    {
        string EdgeVer = string.Empty;
        const string EdgeRegKey = @"SOFTWARE\Microsoft\Edge\BLBeacon";
        const string EdgeRegVal = "version";

        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(EdgeRegKey))
        {
            if (key != null)
            {
                EdgeVer = key.GetValue(EdgeRegVal)?.ToString();
            }
            if (key == null)
            {
                System.Console.WriteLine("Microsoft Edge Installation not found !!!");
            }
        }
        return EdgeVer;
    }

    static int GetRegistryValue(RegistryKey key, string RegistryKey)
    {
        object value = key.GetValue(RegistryKey, null);
        return value != null ? (int)value : -1;
    }
    static void SetRegistryValue(RegistryKey key, string registryKey, int value)
    {
        key.SetValue("NoRemove", value, RegistryValueKind.DWord);
        System.Console.WriteLine("Registry successfully Written");
    }
    static void ShowErrorInfo(string message)
    {
        System.Console.WriteLine("Error: " + message);
    }

}


