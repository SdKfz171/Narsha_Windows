using Narsha.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class LoginPage : Page
    {
        private HttpClient httpClient;

        private MemberParam memberParam;

        public LoginPage()
        {
            this.InitializeComponent();

            memberParam = new MemberParam();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingBar.Visibility = Visibility.Visible;

            LoginButton.IsEnabled = false;

            httpClient = new HttpClient();

            string uri = "http://sss3.cafe24app.com/employee/views/";

            string id = EmployeeIDBox.Password;

            string resultString = "error";

            string responsedID = null;

            await Task.Delay(1000);

            try
            {
                string JsonResponse = await httpClient.GetStringAsync(uri + id);

                Debug.WriteLine(JsonResponse);

                int firstIndex = JsonResponse.IndexOf("code") + 6;

                int lastIndex = JsonResponse.IndexOf("employee_id") - 2;

                resultString = JsonResponse.Substring(firstIndex, lastIndex - firstIndex);

                memberParam.Code = resultString;

                // "success"
                Debug.WriteLine(resultString);

                JsonResponse = JsonResponse.Remove(0, lastIndex + 15);

                Debug.WriteLine(JsonResponse);

                lastIndex = JsonResponse.IndexOf("employee_name") - 2;

                resultString = JsonResponse.Substring(0, lastIndex);

                memberParam.EmployeeId = resultString;

                // "YOUNGH0720"
                Debug.WriteLine(responsedID = resultString);

                JsonResponse = JsonResponse.Remove(0, lastIndex + 17);

                Debug.WriteLine(JsonResponse);

                lastIndex = JsonResponse.IndexOf("employee_gender") - 2;

                resultString = JsonResponse.Substring(0, lastIndex);

                memberParam.EmployeeName = resultString;

                // "최영훈"
                Debug.WriteLine(resultString);

                JsonResponse = JsonResponse.Remove(0, lastIndex + 19);

                Debug.WriteLine(JsonResponse);

                lastIndex = JsonResponse.IndexOf("employee_age") - 2;

                resultString = JsonResponse.Substring(0, lastIndex);

                memberParam.EmployeeGender = resultString;

                // "남성"
                Debug.WriteLine(resultString);

                JsonResponse = JsonResponse.Remove(0, lastIndex + 16);

                Debug.WriteLine(JsonResponse);

                lastIndex = JsonResponse.IndexOf("employee_profile") - 2;

                resultString = JsonResponse.Substring(0, lastIndex);

                memberParam.EmployeeAge = int.Parse(resultString);

                // "19"
                Debug.WriteLine(resultString);

                JsonResponse = JsonResponse.Remove(0, lastIndex + 20);

                Debug.WriteLine(JsonResponse);

                lastIndex = JsonResponse.IndexOf("employee_date") - 2;

                resultString = JsonResponse.Substring(0, lastIndex);

                memberParam.EmployeeProfile = resultString;

                // "Kirby.jpg"
                Debug.WriteLine(resultString);

                JsonResponse = JsonResponse.Remove(0, lastIndex + 17);

                Debug.WriteLine(JsonResponse);

                lastIndex = JsonResponse.IndexOf("employee_department") - 2;

                resultString = JsonResponse.Substring(0, lastIndex);

                memberParam.EmployeeDate = resultString;

                // "2017-10-02T13:16:06.000Z"
                Debug.WriteLine(resultString);

                JsonResponse = JsonResponse.Remove(0, lastIndex + 23);

                Debug.WriteLine(JsonResponse);

                Debug.WriteLine(JsonResponse);

                lastIndex = JsonResponse.IndexOf("}");

                resultString = JsonResponse.Substring(0, lastIndex);

                memberParam.EmployeeDepartment = resultString;

                // "일반사원실"
                Debug.WriteLine(resultString);

                JsonResponse = JsonResponse.Remove(0, lastIndex);

                Debug.WriteLine(JsonResponse);
            }
            catch (Exception ae)
            {
                if (resultString.Equals("error"))
                {
                    EmployeeIDBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 50, 50));
                    LoadingBar.Visibility = Visibility.Collapsed;
                    LoginButton.Visibility = Visibility.Collapsed;
                    FlyoutBase.ShowAttachedFlyout((FrameworkElement)EmployeeIDBox);
                }
            }

            Debug.WriteLine(responsedID);

            await Task.Delay(1000);

            if (responsedID == "\"" + EmployeeIDBox.Password + "\"")
            {
                Debug.WriteLine("Go SettingPage");

                LoadingBar.Visibility = Visibility.Collapsed;
                LoginGrid.Visibility = Visibility.Collapsed;

                // 세팅창으로 가기


                ((Frame)this.Parent).Navigate(typeof(SettingPage), memberParam);

                LoginButton.IsEnabled = true;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            //#FF2A6EA3
            EmployeeIDBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 42, 110, 163));
            LoadingBar.Visibility = Visibility.Visible;
            LoginButton.Visibility = Visibility.Visible;
            LoginButton.IsEnabled = true;
        }
    }
}
