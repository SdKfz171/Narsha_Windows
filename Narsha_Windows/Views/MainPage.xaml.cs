using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace Narsha_Windows.Views
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string testMinor = "2317";

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

            List<Guid> list = new List<Guid>();

            list.Add(TransMitter); list.Add(Mobile);

            IList<Guid> guidIList = list;

            watcher = new BluetoothLEAdvertisementWatcher();

            var manufacturerData = new BluetoothLEManufacturerData();

            manufacturerData.CompanyId = 0x004C;

            

            watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData);

            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -70;

            watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -75;

            

            //watcher.MinSamplingInterval.Milliseconds = TimeSpan.FromMilliseconds(200);

            //watcher.AdvertisementFilter.Advertisement.ServiceUuids = new IList<Guid> { ""}

            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(1000);

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
            if (ControlFlag)
            {
                Debug.WriteLine(Payload);
                try
                {
                    if (Payload.Equals(null))
                        Debug.WriteLine("널 값");
                }
                catch (Exception ne)
                {
                    Debug.WriteLine("널 값");
                }

            }

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
                        Debug.WriteLine(args.BluetoothAddress);
                        Debug.WriteLine(args.Advertisement.LocalName);
                        Debug.WriteLine(args.Advertisement.DataSections);

                        this.Rssi = (int)args.RawSignalStrengthInDBm;
                        //Debug.WriteLine(this.Rssi);

                        this.BeaconData = BitConverter.ToString(data);
                        //Debug.WriteLine(this.BeaconData);

                        Uuid = new Guid(
                            BitConverter.ToInt32(data.Skip(2).Take(4).Reverse().ToArray(), 0),
                            BitConverter.ToInt16(data.Skip(6).Take(2).Reverse().ToArray(), 0),
                            BitConverter.ToInt16(data.Skip(8).Take(2).Reverse().ToArray(), 0),
                            data.Skip(10).Take(8).ToArray());
                        Major = BitConverter.ToUInt16(data.Skip(18).Take(2).Reverse().ToArray(), 0);
                        Minor = BitConverter.ToUInt16(data.Skip(20).Take(2).Reverse().ToArray(), 0);
                        TxPower = (sbyte)data[22];
                    }
                    catch
                    {
                        Debug.WriteLine("잘못된 비콘입니다.");
                    }
                    
                    
                    //foreach (var temp in data.ToList<byte>())
                    //    UUID.Add(temp.ToString("X"));

                    /*
                     * 
                     * Example : 비콘 데이타를 Hex 문자열로 변환 해 리스트로 저장하고 데이터가 정상적으로 있으면,
                     *          (비콘 UUID만 해도 16글자에 Major,Minor 합치면, 4글자)
                     *          문자열에서 Major, Minor을 따로 빼내서 문자열로 저장해 아스키 Decimal로 저장
                     *          Decimal을 아스키 문자로 변경해 문자열에 저장(完)
                     *          
                    */

                    //if (UUID.Count > 20)
                    //{
                    //    string Hex, Major, Minor;

                    //    Hex = string.Format(UUID[18] + UUID[19]);

                    //    Major = Hex;

                    //    CompareCode = Major;

                    //    Hex = string.Format(UUID[20] + UUID[21]);

                    //    Minor = Hex;

                    //    Payload = Minor;


                    //}

                    //else
                    //{
                    //    Payload = "페이로드 없음";
                    //}
                });

                Debug.WriteLine("Uuid : " + Uuid);
                Debug.WriteLine("Rssi : " + Rssi);
                Debug.WriteLine("Major : " + Major);
                Debug.WriteLine("Minor : " + Convert.ToChar( Minor / 100 ) + Convert.ToChar( Minor % 100 ));
                Debug.WriteLine("TxPower : " + TxPower);
                Debug.Write("서비스 UUIDs : ");// + watcher.AdvertisementFilter.Advertisement.ServiceUuids);
                foreach (var temp in watcher.AdvertisementFilter.Advertisement.ServiceUuids.ToList())
                    Debug.WriteLine(temp);


            }
        }

        private void Watcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            Debug.WriteLine("비콘을 잡을 수 없습니다.");
        }

        private void Timer_Tick(object sender, object e)
        {
            try
            {
                

                //if(Payload == "01")
                    //Debug.WriteLine(Payload);

                //TestModel model = new TestModel { Payload = this.Payload, Rssi = this.Rssi};

                //BeaconDataList.Items.Add(model);

                //if(Payload == testMinor && Rssi > -127)
                //    BeaconDataList.Items.Add("Payload : " + Payload + ", Rssi : " + Rssi);

                //TestBlock.Text += "Minor Value : " + Payload + ", Rssi : " + Rssi + "\n";

                if (Payload == testMinor && Rssi < -60 && Rssi > -127)
                {
                    Debug.WriteLine("\n\nPC Lock\n\n");

                    MainFrame.Navigate(typeof(LockScreenPage));
                }

                else if (Payload == testMinor && Rssi > -40 && Rssi > -127)
                {
                    Debug.WriteLine("\n\nPC Unlock\n\n");

                    MainFrame.Navigate(typeof(LoginPage));
                }

            }
            catch (Exception ex)
            {

            }

        }
    }
}
