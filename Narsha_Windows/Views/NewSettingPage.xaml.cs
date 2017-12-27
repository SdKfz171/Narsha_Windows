using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class NewSettingPage : Page
    {
        public NewSettingPage()
        {
            this.InitializeComponent();

            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var BorderHeight = ClockBorder.ActualHeight;
            var BorderWidth = ClockBorder.ActualWidth;

            var Height = ((Frame)Window.Current.Content).ActualHeight;
            var Width = ((Frame)Window.Current.Content).ActualWidth;

            //ClockBorder.Margin = new Thickness(200,Height - 500 - BorderHeight,Width - 200 - BorderWidth, 500);

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            DateTime CurrentDateTime = DateTime.Now;
            DateBlock.Text = CurrentDateTime.ToString("yyyy . MM . dd");
            TimeBlock.Text = CurrentDateTime.ToString("tt hh : mm : ss");
        }
    }
}
