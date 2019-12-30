using System;
using System.Collections.Generic;
using DPUruNet;
using System.Threading;

namespace FingerApp
{
    class FingerMachine
    {
        private ReaderCollection readers;
        public ReaderCollection Readers
        {
            get
            {
                return readers;
            }
            set
            {
                readers = value;
            }
        }
        private Reader reader;
        public Reader Reader
        {
            get
            {
                return reader;
            }
            set
            {
                reader = value;
            }
        }
        private List<Fmd> preenrollmentFmds = new List<Fmd>();
        public List<Fmd> PreEnrollmentFmds
        {
            get
            {
                return preenrollmentFmds;
            }
        }
        private Dictionary<int, Fmd> fmds = new Dictionary<int, Fmd>();
        public Dictionary<int, Fmd> Fmds
        {
            get
            {
                return fmds;
            }
            set
            {
                fmds = value;
            }
        }

        private bool reset;
        public bool Reset
        {
            get
            {
                return reset;
            }
            set
            {
                reset = value;
            }
        }

        public bool OpenReader()
        {
            Reset = false;
            Constants.ResultCode result = Constants.ResultCode.DP_DEVICE_FAILURE;
            result = reader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
            if (result != Constants.ResultCode.DP_SUCCESS)
            {
                Console.WriteLine("Error : " + result.ToString());
                Reset = true;
                return false;
            }
            return true;
        }
        public void GetStatus()
        {
            Constants.ResultCode result = reader.GetStatus();
            if (result != Constants.ResultCode.DP_SUCCESS)
            {
                if (reader != null)
                {
                    Reset = true;
                    throw new Exception("" + result.ToString());
                }
            }

            if (reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_BUSY)
            {
                Thread.Sleep(50);
            }
            else if (reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION)
            {
                reader.Calibrate();
            }
            else if (reader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY)
            {
                throw new Exception("Reader Status - " + reader.Status.Status.ToString());
            }
        }
        public bool CaptureFingerAsync()
        {
            try
            {
                GetStatus();
                Constants.ResultCode captureResult = reader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, reader.Capabilities.Resolutions[0]);
                if (captureResult != Constants.ResultCode.DP_SUCCESS)
                {
                    Reset = true;
                    throw new Exception("" + captureResult.ToString());
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:  " + ex.Message);
                return false;
            }
        }
        public bool StartCaptureAsync(Reader.CaptureCallback OnCaptured)
        {
            reader.On_Captured -= OnCaptured;
            reader.On_Captured += OnCaptured;
            if (!CaptureFingerAsync())
            {
                return false;
            }
            return true;
        }
        public void CancelCaptureAndCloseReader()
        {
            if (reader != null)
            {
                if (Reset)
                {
                    reader = null;
                }
            }
        }
    }
}
