using Narsha.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace Narsha_Windows.Views
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private int OldValue;

        private BitmapImage bitmapImage;

        public SettingPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if(e.Parameter != null)
            //{
            //    Debug.WriteLine("유저 ID : " + ((MemberParam)e.Parameter).EmployeeId);
            //}
            //else
            //{

            //}
            

            ///base.OnNavigatedTo(e);
        }

        private void RssiValueSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            OldValue = App.RSSIValue;
            App.RSSIValue = (short)(e.NewValue * -1);
        }

        private void SystemTraySwitch_Toggled(object sender, RoutedEventArgs e)
        {
            //BackgroundPage backgroundPage;
            //if (SystemTraySwitch.IsOn)
            //    backgroundPage = new BackgroundPage();
            //else
            //    backgroundPage = null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //sqlite

            if (SystemTraySwitch.IsOn)
            {

            }
        }

        private async void ProfilePictureBorder_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    //ProfileImage.ImageFile = items[0] as StorageFile;

                    bitmapImage = new BitmapImage();

                    StorageFile storageFile = items[0] as StorageFile;

                    FileRandomAccessStream stream = (FileRandomAccessStream)await storageFile.OpenAsync(FileAccessMode.Read);

                    bitmapImage.SetSource(stream);

                    ProfileImage.ImageSource = bitmapImage;

                    // 웹으로 이미지 변경 정보 전송

                    // For Testing
                    //await Task.Delay(1000);
                }
            }
        }
    }
}
