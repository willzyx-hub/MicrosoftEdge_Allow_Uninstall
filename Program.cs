using Microsoft.Win32;
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
    }
    static void ShowVersionInfo()
    {
        System.Console.WriteLine("Microsoft Edge Allow Uninstall V1.0 ALPHA");
    }

    static void ExecuteAction()
    {
        try
        {
            string RegistryPath = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Microsoft Edge";
            string RegistryKey = "NoRemove";

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


