using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

namespace Chrome
{
    static class Program
    {
        private const string AppSettingFilePath = "AppSettings.json";

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var jsonString = File.ReadAllText(AppSettingFilePath);
            var settings = JsonConvert.DeserializeObject<AppSettings>(jsonString);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(settings));
        }
    }
}
