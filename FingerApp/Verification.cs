using DPUruNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Media;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FingerApp
{
    public partial class Verification : Form
    {
        private FingerMachine fm;

        delegate void AppendTextCallback3(string text);
        delegate void CloseDelegate();
        delegate void ClearTextCallback3();
        private Thread logThread3 = null;
        private String path = AppDomain.CurrentDomain.BaseDirectory;
        private String dataUrl;
        private String actionUrl;
        private List<FPData> fps;
        private List<Fmd> fmds = new List<Fmd>();
        public Verification()
        {
            InitializeComponent();
            fm = new FingerMachine();
        }

        public Verification(String actionUrl, String dataUrl)
        {
            InitializeComponent();
            fm = new FingerMachine();
            this.dataUrl = dataUrl;
            this.actionUrl = actionUrl;
        }

        private void Verification_Load(object sender, EventArgs e)
        {
            InitFingerMachine();
            FetchFPData();
        }

        private void InitFingerMachine()
        {
            fm.Readers = ReaderCollection.GetReaders();
            if (fm.Readers[0] == null)
            {
                MessageBox.Show("Mesin Fingerprint tidak ditemukan");
                this.Close();
                return;
            }
            string serial_reader = fm.Readers[0].Description.SerialNumber;
            fm.Reader = fm.Readers[0];
            fm.OpenReader();
            fm.StartCaptureAsync(this.OnCaptured);
            try
            {
                txtLog.Text = "";
            }
            catch
            {
                this.logThread3 = new Thread(new ThreadStart(this.ClearText));
                this.logThread3.Start();
            }
        }

        private void FetchFPData()
        {
            using (var wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var res = wc.UploadString(this.dataUrl, "");
                var d = Newtonsoft.Json.JsonConvert.DeserializeObject<FPDataResponse>(res);
                fps = d.data;
            }
        }
        public void OnCaptured(CaptureResult captureResult)
        {
            var img2 = Extensions.CreateBitmap(captureResult.Data.Views[0].RawImage, captureResult.Data.Views[0].Width, captureResult.Data.Views[0].Height);
            var img = Extensions.ImageFromRawBgraArray(captureResult.Data.Views[0].Bytes, captureResult.Data.Views[0].Width, captureResult.Data.Views[0].Width);
            picFinger.Image = img2;
            bool isfound = false;
            CompareResult compareResult;
            DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Constants.Formats.Fmd.ANSI);
            FPData dataFound = null;
            foreach (FPData res in fps)
            {
                var strFmd = System.Web.HttpUtility.UrlDecode(res.fp);
                var fmd = Fmd.DeserializeXml(strFmd);
                compareResult = Comparison.Compare(resultConversion.Data, 0, fmd, 0);
                if (compareResult.Score < (0x7FFFFFFF / 100000))
                {
                    isfound = true;
                    dataFound = res;
                    break;
                }
            }
            if (isfound)
            {
                if (dataFound != null)
                {
                    //PostToServer(dataFound.userId);
                    if (this.actionUrl.Contains("?"))
                        Process.Start(this.actionUrl + "&id=" + dataFound.userId);
                    else
                        Process.Start(this.actionUrl + "?id=" + dataFound.userId);
                    this.logThread3 = new Thread(new ThreadStart(() => this.ThreadProcSafe(DateTime.Now + " : User " + dataFound.userId + " terdeteksi\n")));
                    this.logThread3.Start();
                }
                SoundPlayer sound = new SoundPlayer(path + @"audio\success.wav");
                sound.Play();
            }
            else
            {
                this.logThread3 = new Thread(new ThreadStart(() => this.ThreadProcSafe(DateTime.Now + " : Sidik jari tidak ditemukan\n")));
                this.logThread3.Start();
                SoundPlayer sound = new SoundPlayer(path + @"audio\fail.wav");
                sound.Play();
            }
            Thread.Sleep(1000);
            if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    Close();
                }));
            }
            else
            {
                Close();
            }
        }

        private void ThreadProcSaveClose()
        {

        }

        private void TryClose()
        {
            if (this.InvokeRequired)
            {
                CloseDelegate d = new CloseDelegate(TryClose);
                if (IsHandleCreated) this.Invoke(d, new object[] { });
            }
            else
            {
                this.Close();
            }
        }

        private void ThreadProcSafe(String text)
        {
            this.AppendText(text);
        }
        private void ThreadClear()
        {
            this.ClearText();
        }
        private void AppendText(string text)
        {
            if (this.txtLog.InvokeRequired)
            {
                AppendTextCallback3 d = new AppendTextCallback3(AppendText);
                if (IsHandleCreated) this.Invoke(d, new object[] { text + "\n" });
            }
            else
            {
                if (txtLog.IsDisposed)
                {
                    txtLog = new TextBox();
                }
                this.txtLog.AppendText(text);
                this.txtLog.AppendText("\n");
            }
        }
        private void ClearText()
        {
            if (this.txtLog.InvokeRequired)
            {
                ClearTextCallback3 d = new ClearTextCallback3(ClearText);
                if (IsHandleCreated) this.Invoke(d, new object[] { });
            }
            else
            {
                this.txtLog.Text = "";
            }
        }
        private void PostToServer(string id)
        {
            using (var wc = new WebClient())
            {
                try
                {
                    String myParams = "user_id=" + id;
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    var res = wc.UploadString(this.actionUrl, myParams);

                    this.logThread3 = new Thread(new ThreadStart(() => this.ThreadProcSafe(DateTime.Now + " : OK -> " + res)));
                    this.logThread3.Start();


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
    public class FPData
    {
        public String fp { get; set; }
        public String userId { get; set; }

        //public String decodedFp()
        //{
        //    byte[] data = Convert.FromBase64String(fp.Trim());
        //    return Encoding.UTF8.GetString(data).Trim();
        //}
    }

    public class FPDataResponse
    {
        public List<FPData> data { get; set; }
    }
}
