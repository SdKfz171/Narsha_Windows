using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
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
    public sealed partial class LockScreenPage : Page
    {
        public LockScreenPage()
        {
            this.InitializeComponent();

            //Debug.WriteLine("asdgsafga");

            IRandomAccessStream imageStream = LockScreen.GetImageStream();

            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(imageStream);

            //Image image = new Image { Source = bitmap };

            LockScreenImage.ImageSource = bitmap;

            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }

        private void MainGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private void MainGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Point SettingPoint = Window.Current.CoreWindow.PointerPosition;

            SettingPoint.Y = 430;
            SettingPoint.X = 750;

            var Height = ((Frame)Window.Current.Content).ActualHeight;

            Debug.WriteLine("창 높이 : " + Height);

            PointerPoint pointer = e.GetCurrentPoint(MainGrid);

            var x = (int)pointer.Position.X;
            var y = (int)pointer.Position.Y;

            Debug.WriteLine("X 좌표 : " + x);
            Debug.WriteLine("Y 좌표 : " + y);

            if (y > 750 || y < 50)
                Window.Current.CoreWindow.PointerPosition = SettingPoint;
            if(x > 1500 || x < 50)
                Window.Current.CoreWindow.PointerPosition = SettingPoint;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
