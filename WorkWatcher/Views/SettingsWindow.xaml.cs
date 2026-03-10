using System;
using System.Windows;
using WorkWatcher.ViewModels;

namespace WorkWatcher.Views
{
    /// <summary>
    /// SettingsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private SettingsViewModel ViewModel => (SettingsViewModel)DataContext;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ValidateQuota())
            {
                ViewModel.SaveSettings();
                this.Close();
            }
            else
            {
                MessageBox.Show("시간 설정이 올바르지 않습니다. 0-59분 범위 내에서 입력해주세요.",
                    "입력 오류",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ValidateQuota())
            {
                ViewModel.SaveSettings();
                MessageBox.Show("설정이 저장되었습니다.",
                    "저장 완료",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("시간 설정이 올바르지 않습니다. 0-59분 범위 내에서 입력해주세요.",
                    "입력 오류",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
