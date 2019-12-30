using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FingerApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var path = Environment.CurrentDirectory;
            var args = Environment.GetCommandLineArgs();
            Debug.WriteLine(path);
            Debug.WriteLine(args);
            Debug.WriteLine(Application.ExecutablePath);
            try
            {
                ProtocolRegister.RegisterMyProtocol(Application.ExecutablePath);
            }
            catch
            {

            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 1)
            {
                var strParam = args[1].Replace("fingerapp:", "");
                var _params = strParam.Split(new char[] { ';' }, StringSplitOptions.None);
                String action = _params[0];
                String actionUrl = _params[1];
                String dataUrl = "";
                if (_params.Length > 2)
                {
                    dataUrl = _params[2];
                    byte[] _data = Convert.FromBase64String(dataUrl);
                    dataUrl = Encoding.UTF8.GetString(_data);
                }
                byte[] data = Convert.FromBase64String(actionUrl);
                actionUrl = Encoding.UTF8.GetString(data);
                //MessageBox.Show(actionUrl + "\n" + dataUrl);
                if (action.ToLower() == "enrollment")
                    Application.Run(new Enrollment(@actionUrl));
                else if (action.ToLower() == "verification")
                    Application.Run(new Verification(@actionUrl, @dataUrl));
            }
            else
                Application.Run(new Hello());
            //else
            //    Application.Run(new Verification(@"http://myworks.me/flexcode/welcome/ver", @"http://myworks.me/flexcode/welcome/fp"));
        }
    }
}
