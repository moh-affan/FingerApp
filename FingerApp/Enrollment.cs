using DPUruNet;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace FingerApp
{
    public partial class Enrollment : Form
    {
        private int count;
        private string fp;
        private FingerMachine fm;
        delegate void AppendTextCallback2(string text);
        delegate void ClearTextCallback2();
        private Thread logThread2 = null;
        private String url;
        public Enrollment()
        {
            InitializeComponent();
            fm = new FingerMachine();
        }

        public Enrollment(String url)
        {
            InitializeComponent();
            fm = new FingerMachine();
            this.url = url;
        }
        private void Enrollment_Load(object sender, EventArgs e)
        {
            InitFingerMachine();
            //this.logThread2 = new Thread(new ThreadStart(() => this.ThreadProcSafe(this.url)));
            //this.logThread2.Start();
            //this.label1.Text = this.url;
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
            count = 0;
            fm.OpenReader();
            fm.StartCaptureAsync(this.OnCaptured);
            try
            {
                txtLog.Text = "";
            }
            catch
            {
                this.logThread2 = new Thread(new ThreadStart(this.ClearText));
                this.logThread2.Start();
            }
        }
        public void OnCaptured(CaptureResult captureResult)
        {
            count += 1;
            DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Constants.Formats.Fmd.ANSI);
            if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
            {
                Console.WriteLine("Gagal");
            }
            else
            {
                fm.PreEnrollmentFmds.Add(resultConversion.Data);
                Debug.WriteLine(captureResult.Data.Views[0].Width);
                Debug.WriteLine(captureResult.Data.Views[0].Height);
                Debug.WriteLine(captureResult.Data.Views.Count);

                var img = Extensions.ImageFromRawBgraArray(captureResult.Data.Views[0].Bytes, captureResult.Data.Views[0].Width, captureResult.Data.Views[0].Height);
                var img2 = Extensions.CreateBitmap(captureResult.Data.Views[0].RawImage, captureResult.Data.Views[0].Width, captureResult.Data.Views[0].Height);
                picFinger.Image = img2;
                Debug.WriteLine(img.Size);
                this.logThread2 = new Thread(new ThreadStart(() => this.ThreadProcSafe(DateTime.Now + " : OK -> Sidik jari ke-" + count + "\n")));
                this.logThread2.Start();
            }
            if (count >= 4)
            {
                DataResult<Fmd> resultEnrollment = DPUruNet.Enrollment.CreateEnrollmentFmd(Constants.Formats.Fmd.ANSI, fm.PreEnrollmentFmds);
                if (resultEnrollment.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    fp = Fmd.SerializeXml(resultEnrollment.Data);
                    fp = System.Web.HttpUtility.UrlEncode(fp);
                    //var fmd = Fmd.DeserializeXml(Extensions.Base64Decode(fp));
                    //fp = Fid.SerializeXml(captureResult.Data);
                    this.logThread2 = new Thread(new ThreadStart(() => this.ThreadProcSafe(DateTime.Now + " : OK -> Sidik jari cocok")));
                    this.logThread2.Start();
                    String myParams = "fp_data=" + fp;
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        var res = wc.UploadString(this.url, myParams);
                        this.logThread2 = new Thread(new ThreadStart(() => this.ThreadProcSafe(DateTime.Now + " : OK -> "+res)));
                        this.logThread2.Start();
                        this.logThread2 = new Thread(new ThreadStart(() => this.ThreadProcSafe(DateTime.Now + " : OK -> " + fp)));
                        this.logThread2.Start();
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
                }
                else
                {
                    MessageBox.Show("Jari yang Anda taruh tidak cocok", "Gagal", MessageBoxButtons.OK);
                    this.Reset();
                }
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
                AppendTextCallback2 d = new AppendTextCallback2(AppendText);
                if (IsHandleCreated) this.Invoke(d, new object[] { text + "\n" });
            }
            else
            {
                this.txtLog.AppendText(text);
                this.txtLog.AppendText("\n");
            }
        }
        private void ClearText()
        {
            if (this.txtLog.InvokeRequired)
            {
                ClearTextCallback2 d = new ClearTextCallback2(ClearText);
                if (IsHandleCreated) this.Invoke(d, new object[] { });
            }
            else
            {
                this.txtLog.Text = "";
            }
        }

        private void Reset()
        {
            picFinger.Image = null;
            fm.PreEnrollmentFmds.Clear();
            fm.CancelCaptureAndCloseReader();
            fm = new FingerMachine();
            InitFingerMachine();
        }


    }
}
