using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Chrome
{
    public partial class MainForm : Form
    {
        private readonly AppSettings _settings;
        private UserActivityHook actHook;
        private Service service;

        public MainForm(AppSettings settings)
        {
            InitializeComponent();
            _settings = settings;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            service = new Service(_settings.ServiceSettings);
            actHook = new UserActivityHook(false, true);
            actHook.KeyPress += new KeyPressEventHandler(MyKeyPress);
        }

        public void MyKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == _settings.HotKeyCode)
            {
                Action();
            }
        }

        private Bitmap PrintScreen()
        {
            Bitmap printScreen = new Bitmap(_settings.ScreenWidth, _settings.ScreenHeight);

            Graphics graphics = Graphics.FromImage(printScreen);

            graphics.CopyFromScreen(0, 0, 0, 0, printScreen.Size);

            return printScreen;
        }

        private void Action()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var ps = PrintScreen();
                ps.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                service.UploadPhoto(ms, $"image{DateTime.Now.ToLongTimeString()}.jpg");
            }
        }
    }
}
