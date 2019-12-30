using Microsoft.Win32;
using System;

namespace FingerApp
{
    class ProtocolRegister
    {
        const String PROTOCOL_KEY = "fingerapp";
        public static void RegisterMyProtocol(String appPath)
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(PROTOCOL_KEY);
            if (key == null)
            {
                key = Registry.ClassesRoot.CreateSubKey(PROTOCOL_KEY);
                key.SetValue(string.Empty, "URL: " + PROTOCOL_KEY + " Protocol");
                key.SetValue("URL Protocol", string.Empty);

                var subKey = key.CreateSubKey(@"shell\open\command");
                subKey.SetValue(string.Empty, appPath + " " + "%1");
                subKey.Close();
            }
            key.Close();
        }
    }
}
