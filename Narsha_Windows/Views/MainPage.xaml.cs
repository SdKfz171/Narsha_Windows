using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;


// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace Narsha_Windows.Views
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string testMinor = "JH";

        private DispatcherTimer timer1;

        private bool ControlFlag = false;

        private int rssi;

        private string beaconData;

        private DispatcherTimer timer;

        private ApplicationView CurrentApplicationView;

        private BluetoothLEAdvertisementWatcher watcher;

        public event PropertyChangedEventHandler PropertyChanged;

        Size size = ((Frame)Window.Current.Content).DesiredSize;
        Size size2 = new Size(((Frame)Window.Current.Content).ActualWidth, ((Frame)Window.Current.Content).ActualHeight);

        public Guid Uuid { get; set; }

        public ushort Major { get; set; }

        public ushort Minor { get; set; }

        public sbyte TxPower { get; set; }

        private Guid TransMitter;

        private Guid Mobile;

        private List<int> RssiList = new List<int>();

        private List<string> DetectedHumanList = new List<string>();

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

            //ApplicationView.GetForCurrentView().DesiredBoundsMode

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

            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -50;

            watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -75;


            //watcher.MinSamplingInterval.Milliseconds = TimeSpan.FromMilliseconds(200);

            //watcher.AdvertisementFilter.Advertisement.ServiceUuids = new IList<Guid> { ""}

            watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(200);

            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(2000);

            watcher.Stopped += Watcher_Stopped;
            watcher.Received += Watcher_Received;
            watcher.Start();

            MainFrame.Navigate(typeof(NewSettingPage));

            Window.Current.CoreWindow.SizeChanged += CoreWindow_SizeChanged;
            Window.Current.CoreWindow.VisibilityChanged += CoreWindow_VisibilityChanged;
            Window.Current.Closed += Current_Closed;
        }

        private async void Current_Closed(object sender, CoreWindowEventArgs e)
        {
            Debug.WriteLine("창이 닫김");

            bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
        }

        private async void CoreWindow_VisibilityChanged(CoreWindow sender, VisibilityChangedEventArgs args)
        {
            if (!args.Visible)
            {
                Debug.WriteLine("창이 최소화됨");

                bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);

                //ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                //compactOptions.ViewSizePreference = ViewSizePreference.Custom;
                //bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default, compactOptions);
                //ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
            
        }

        private void CoreWindow_SizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            var appView = ApplicationView.GetForCurrentView();

            CurrentApplicationView = appView;

            if (appView.IsFullScreen)
            {
                //maximized

            }
            
            args.Handled = true;
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    MainFrame.Navigate(typeof(SettingPage));

        //    base.OnNavigatedTo(e);
        //}


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

        private async void Timer_Tick(object sender, object e)
        {
            string Indicator = string.Format( Convert.ToChar(Minor / 100).ToString() + Convert.ToChar(Minor % 100).ToString());

            try
            {
                if (Indicator == testMinor)
                {
                    if (RssiList.Count > 4)
                        RssiList.RemoveAt(0);

                    RssiList.Add(Rssi);
                }

                if (Indicator.Length == 2 && Rssi != -127)
                {
                    Debug.WriteLine("Uuid : " + Uuid);
                    Debug.WriteLine("Rssi : " + Rssi);
                    Debug.WriteLine("Major : " + Major);
                    Debug.WriteLine("Minor : " + Convert.ToChar(Minor / 100) + Convert.ToChar(Minor % 100));
                    Debug.WriteLine("TxPower : " + TxPower);
                    Debug.WriteLine("SamplingInterval : " + watcher.MaxSamplingInterval.Milliseconds);
                }

                if (Indicator == testMinor && RssiList.Average() <= -49 && Rssi > -127 && !ControlFlag)
                {
                    //ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                    //compactOptions.ViewSizePreference = ViewSizePreference.Default;

                    //Debug.WriteLine("//////////////////////////////////");
                    //Debug.WriteLine("size 1 : Height = {0}, Width = {1}",size.Height, size.Width);
                    //Debug.WriteLine("");
                    //Debug.WriteLine("");
                    //Debug.WriteLine("size 2 : Height = {0}, Width = {1}", size2.Height, size2.Width);
                    //Debug.WriteLine("//////////////////////////////////");

                    //bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, compactOptions);

                    Debug.WriteLine("\n\nPC Lock\n\n");

                    MainFrame.Navigate(typeof(LockScreenPage));

                    Debug.WriteLine("Rssi List Average : " + RssiList.Average());

                    RssiList.Clear();

                    ControlFlag = true;
                }

                else if (Indicator == testMinor && RssiList.Average() > -42 && Rssi > -127 && ControlFlag)
                {
                    Debug.WriteLine("\n\nPC Unlock\n\n");

                    MainFrame.Navigate(typeof(NewSettingPage));

                    RssiList.Clear();

                    ControlFlag = false;
                    
                    if(DetectedHumanList.Count != 0)
                    {
                        string totalString = "";

                        string image = "ms-appx:///Assets/warning-sign-30915_960_720.png";

                        foreach (var data in DetectedHumanList)
                        {
                            Debug.WriteLine(data);
                            totalString += data + " ";
                        }

                        totalString += "님이 당신의 컴퓨터에 접근하였습니다.";

                        //ToastNotificationManager.CreateToastNotifier()

                        //ToastVisual visual = new ToastVisual()
                        //{
                        //    BindingGeneric = new ToastBindingGeneric()
                        //    {
                        //        Children =
                        //    {
                        //        new AdaptiveText()
                        //        {
                        //            Text = "이상 접근 감지!"
                        //        },
                        //        new AdaptiveText()
                        //        {
                        //            Text = totalString
                        //        },
                        //        new AdaptiveImage()
                        //        {
                        //            Source = image
                        //        }
                        //    }
                        //    }
                        //};
                        //ToastNotification toast = new ToastNotification();
                        //ToastNotificationManager.CreateToastNotifier(visual);
                    }
                    else if (Indicator != testMinor && Rssi > -42 && Rssi > -127)
                    {
                        DetectedHumanList.Add(Indicator);
                        //TakePicture(Indicator);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        //private async void TakePicture(string str)
        //{
        //    var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
        //    var device = devices.FirstOrDefault();

        //    if (device == null)
        //        throw new Exception("No Camera Device");

        //    var media = new MediaCapture();

        //    var settings = new MediaCaptureInitializationSettings();
        //    settings.StreamingCaptureMode = StreamingCaptureMode.Video;
        //    settings.VideoDeviceId = devices[0].Id;

        //    try
        //    {
        //        await media.InitializeAsync(settings);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Access denied", ex);
        //    }


        //    await media.StartPreviewAsync();

        //    StorageFile storagefile =
        //    await ApplicationData.Current.LocalFolder.CreateFileAsync(string.Format(str + ".png"),
        //                                               CreationCollisionOption.ReplaceExisting);

        //    await media.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreatePng(), storagefile);

        //    //var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
        //    //var device = devices.FirstOrDefault();
        //    //if (device == null)
        //    //    throw new Exception("No Camera Device");

        //    //var media = new MediaCapture();

        //    //var settings = new MediaCaptureInitializationSettings();
        //    //settings.StreamingCaptureMode = StreamingCaptureMode.Video;
        //    //settings.VideoDeviceId = devices[0].Id;

        //    //try
        //    //{
        //    //    await media.InitializeAsync(settings);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw new Exception("Access denied", ex);
        //    //}

        //    //await media.StartPreviewAsync();

        //    //StorageFile storagefile =
        //    //await ApplicationData.Current.LocalFolder.CreateFileAsync(string.Format(str + ".png"),
        //    //                                           CreationCollisionOption.ReplaceExisting);

        //    //await media.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreatePng(), storagefile);


        //}
    }
}
