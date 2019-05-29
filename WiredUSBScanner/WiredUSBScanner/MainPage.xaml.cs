using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Com.Unistrong.Pin;
using Com.Unistrong.Player;
using Com.Unistrong.Qrcode;
using Android.Content;
using Java.Lang;
using Android.OS;
using StringBuilder = System.Text.StringBuilder;
namespace WiredUSBScanner
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        #region
        private ScanBroadcastReceiver scanBroadcastReceiver;
        USBQRscanFactory usbScan;
        private bool mWorkingStateFlag = false;
        private bool mPowerStateFlag = true;
        private Context mCtx;
        private Player mPlayer;




        public class OnScanListener : Java.Lang.Object, IOnScanListener
        {

            H myH = new H();
            public void ScanReport(byte[] byteArray)
            {
                lock (myH)
                {
                    if (null != byteArray && byteArray.Length > 0)
                    {
                        myH.SendMessage(myH.ObtainMessage(0, byteArray));
                    }
                }
            }


            public void StatusReport(int i)
            {
                lock (myH)
                {
                    myH.SendEmptyMessage(i);
                }

            }


        }



        #endregion
        public MainPage()
        {
            usbScan = USBQRscanFactory.CreateInstance();
            usbScan.PowerOn();
            



            InitializeComponent();

        }







        int count = 0;
        private void scanBtn_Clicked(object sender, EventArgs e)
        {
           

            count++;
            //usbScan.Init(OnScanListener);
            Barcode.Text = "";
            OnScanListener myOnScanListener = new OnScanListener();
            usbScan.Init(myOnScanListener);
            openScanner(true);
            usbScan.Scan_start();
        }

        private void contScanBtn_Clicked(object sender, EventArgs e)
        {

        }


        //Open Scanner
        private void openScanner(bool open)
        {
            if (open == mWorkingStateFlag) return;
            if (open)
            {
                try
                {
                    Java.Lang.Thread.Sleep(50);
                    usbScan.Open();
                    usbScan.EnableAddKeyValue(0);
                }
                catch (Java.Lang.InterruptedException e)
                {
                    // TODO Auto-generated catch block
                    e.PrintStackTrace();
                }
            }

        }
        //

    }
    public class H : Handler
    {
        public string BytesToHex(byte[] bArr)
        {
            if (bArr == null)
            {
                return null;
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bArr)
            {
                if (b != (byte)0)
                {
                    stringBuilder.Append(string.Format("{0:x2} ", new object[] { Convert.ToByte(bArr) }));
                }
            }
            return stringBuilder.ToString();
        }

        public void handleMessage(Android.OS.Message msg)
        {
            int i = msg.What;
            if (i == 0)

            {

                byte[] byteArray = (byte[])msg.Obj;
                if (byteArray != null)
                {

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("Result:");
                    stringBuilder.Append(new Java.Lang.String(byteArray));
                    stringBuilder.Append("\nHEX:");

                    stringBuilder.Append(BytesToHex(byteArray));
                    // access$000.setText(stringBuilder.toString());
                    Char[] array = System.Text.Encoding.UTF8.GetString(byteArray).ToCharArray();
                    new Android.App.Instrumentation().SendStringSync(new string(array));

                }
            }
        }

    }


}
