using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace Narsha_Windows.Views
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string testMinor = "DY";

        private DispatcherTimer timer1;

        private bool ControlFlag = false;


        /// <summary>
        /// 
        /// </summary>

        private int rssi;

        private string beaconData;

        private DispatcherTimer timer;

        private BluetoothLEAdvertisementWatcher watcher;

        public event PropertyChangedEventHandler PropertyChanged;

        public Guid Uuid { get; set; }

        public ushort Major { get; set; }

        public ushort Minor { get; set; }

        public sbyte TxPower { get; set; }

        private Guid TransMitter;

        private Guid Mobile;

        private List<int> RssiList = new List<int>();

        public int Rssi
        {
            get { return rssi; }
            private set
            {
                rssi = value;
                this.RaisePropertyChange("Rssi");
            }
        }

        public string BeaconData
        {
            get { return beaconData; }
            private set
            {
                beaconData = value;
                this.RaisePropertyChange("BeaconData");
            }
        }

        public string Payload { get; set; }

        public string CompareCode { get; set; }

        private void RaisePropertyChange(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary>
        /// 
        /// </summary>

        public MainPage()
        {
            this.InitializeComponent();

            timer1 = new DispatcherTimer();

            timer1.Tick += Timer1_Tick;
            timer1.Start();

            ControlFlag = true;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();

            TransMitter = new Guid("{e2c56db5-dffb-48d2-b060-d0f5a71096e0}");
            Mobile = new Guid("{6c5df2c4-7256-4563-ba20-a2507efed9bb}");

            watcher = new BluetoothLEAdvertisementWatcher();

            watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(200);

            //watcher.AdvertisementFilter.Advertisement.ServiceUuids.Add

            var manufacturerData = new BluetoothLEManufacturerData();

            manufacturerData.CompanyId = 0x004C;

            DataWriter writer = new DataWriter();

            writer.WriteUInt16(0x0215);

            //writer.WriteUInt64(0xE2C56DB5DFFB48D2);
            //writer.WriteUInt64(0xB060D0F5A71096E0);

            writer.WriteUInt64(0x6C5DF2C472564563);
            writer.WriteUInt64(0xBA20A2507EFED9BB);

            manufacturerData.Data = writer.DetachBuffer();

            //watcher.AdvertisementFilter.Advertisement.ServiceUuids.Add(TransMitter);

            //watcher.AdvertisementFilter.Advertisement.ServiceUuids.Add(Mobile);

            watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData);

            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -70;

            watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -75;
            

            //watcher.MinSamplingInterval.Milliseconds = TimeSpan.FromMilliseconds(200);

            //watcher.AdvertisementFilter.Advertisement.ServiceUuids = new IList<Guid> { ""}



            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(2000);

            watcher.Stopped += Watcher_Stopped;
            watcher.Received += Watcher_Received;
            watcher.Start();
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    MainFrame.Navigate(typeof(SettingPage));

        //    base.OnNavigatedTo(e);
        //}

        private void Timer1_Tick(object sender, object e)
        {
            //if (ControlFlag)
            //{
            //    Debug.WriteLine(Payload);
            //    try
            //    {
            //        if (Payload.Equals(null))
            //            Debug.WriteLine("널 값");
            //    }
            //    catch (Exception ne)
            //    {
            //        Debug.WriteLine("널 값");
            //    }

            //}

            //if (beaconControl.Payload == "1JDH" && beaconControl.Rssi < -90)
            //{
            //    Debug.WriteLine("\n\nPC Lock\n\n");
            //    await Task.Delay(500);
            //}
        }







        private async void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var manufacturerData = args.Advertisement.ManufacturerData;
            if (manufacturerData.Any())
            {
                var manufacturerDataSection = manufacturerData[0];
                var data = new byte[manufacturerDataSection.Data.Length];

                List<string> UUID = new List<string>();

                using (var reader = DataReader.FromBuffer(manufacturerDataSection.Data))
                {
                    reader.ReadBytes(data);
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        Debug.WriteLine("Bluetooth Address : " + args.BluetoothAddress);
                        Debug.WriteLine("Local Name : " + args.Advertisement.LocalName);
                        Debug.WriteLine("Data Sections : " + args.Advertisement.DataSections);

                        Rssi = (int)args.RawSignalStrengthInDBm;

                        Uuid = new Guid(
                            BitConverter.ToInt32(data.Skip(2).Take(4).Reverse().ToArray(), 0),
                            BitConverter.ToInt16(data.Skip(6).Take(2).Reverse().ToArray(), 0),
                            BitConverter.ToInt16(data.Skip(8).Take(2).Reverse().ToArray(), 0),
                            data.Skip(10).Take(8).ToArray());
                        Major = BitConverter.ToUInt16(data.Skip(18).Take(2).Reverse().ToArray(), 0);
                        Minor = BitConverter.ToUInt16(data.Skip(20).Take(2).Reverse().ToArray(), 0);
                        TxPower = (sbyte)data[22];

                        if(string.Format(Convert.ToChar(Minor / 100).ToString() + Convert.ToChar(Minor % 100).ToString()) == testMinor)
                        {
                            if (RssiList.Count >  4)
                            {
                                RssiList.RemoveAt(0);
                            }

                            RssiList.Add(Rssi);
                        }
                        
                    }
                    catch(ArgumentException ae)
                    {
                        Debug.WriteLine("잘못된 비콘입니다.");
                    }
                });

                

            }
        }

        private void Watcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            Debug.WriteLine("비콘을 잡을 수 없습니다.");
        }

        private void Timer_Tick(object sender, object e)
        {
            string Indicator = string.Format( Convert.ToChar(Minor / 100).ToString() + Convert.ToChar(Minor % 100).ToString());

            try
            {
                if(Indicator == "DY" && Rssi != -127)
                {
                    Debug.WriteLine("Uuid : " + Uuid);
                    Debug.WriteLine("Rssi : " + Rssi);
                    Debug.WriteLine("Major : " + Major);
                    Debug.WriteLine("Minor : " + Convert.ToChar(Minor / 100) + Convert.ToChar(Minor % 100));
                    Debug.WriteLine("TxPower : " + TxPower);
                    Debug.WriteLine("SamplingInterval : " + watcher.MaxSamplingInterval.Milliseconds);
                }

                if (Indicator == testMinor && RssiList.Average() <= -55 && Rssi > -127)
                {
                    Debug.WriteLine("\n\nPC Lock\n\n");

                    MainFrame.Navigate(typeof(LockScreenPage));

                    Debug.WriteLine("Rssi List Average : " + RssiList.Average());

                    RssiList.Clear();
                }

                else if (Indicator == testMinor && RssiList.Average() > -42 && Rssi > -127)
                {
                    Debug.WriteLine("\n\nPC Unlock\n\n");

                    MainFrame.Navigate(typeof(LoginPage));

                    RssiList.Clear();
                }

                else if(Indicator != testMinor)
                {

                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
