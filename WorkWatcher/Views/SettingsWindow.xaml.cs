using Microsoft.Win32;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using WorkWatcher.Models;
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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // 정규식을 사용하여 숫자(0-9)만 허용
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Add_MP_FP_Click(object sender, RoutedEventArgs e)
        {
            FindProcessDialog findProcessDialog = new FindProcessDialog();
            bool? result = findProcessDialog.ShowDialog();
            if (result == true)
            {
                // 선택된 프로세스 이름을 가져와서 처리
                string selectedProcessName = findProcessDialog.SelectedProcessName;
                if (IsValidProcessName(selectedProcessName))
                {
                    ViewModel.MonitoredPrograms.Add(new ProgramInfo { ProcessName = selectedProcessName });
                } else ProcessNameErrorMessage(selectedProcessName);
            }
        }

        private void Add_MP_D_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "실행 파일 (*.exe)|*.exe",
                Title = "작업 프로그램 선택"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                // 선택된 실행 파일의 이름을 가져와서 처리
                string fileName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                if (IsValidProcessName(fileName))
                {
                    ViewModel.MonitoredPrograms.Add(new ProgramInfo { ProcessName = fileName });
                } else ProcessNameErrorMessage(fileName);
            }
        }

        private void Remove_MP_Click(object sender, RoutedEventArgs e)
        {
            if (MP_Grid.SelectedItem != null)
            {
                ViewModel.MonitoredPrograms.Remove((ProgramInfo)MP_Grid.SelectedItem);
            }
        }

        private void Add_DP_FP_Click(object sender, RoutedEventArgs e)
        {
            FindProcessDialog findProcessDialog = new FindProcessDialog();
            bool? result = findProcessDialog.ShowDialog();
            if (result == true)
            {
                // 선택된 프로세스 이름을 가져와서 처리
                string selectedProcessName = findProcessDialog.SelectedProcessName;
                if (IsValidProcessName(selectedProcessName))
                {
                    ViewModel.DistractionPrograms.Add(new DistractionProgramInfo { ProcessName = selectedProcessName });
                } else ProcessNameErrorMessage(selectedProcessName);
            }
        }

        private void Add_DP_D_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "실행 파일 (*.exe)|*.exe",
                Title = "딴짓 프로그램 선택"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                // 선택된 실행 파일의 이름을 가져와서 처리
                string fileName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                if (IsValidProcessName(fileName))
                {
                    ViewModel.DistractionPrograms.Add(new DistractionProgramInfo { ProcessName = fileName });
                } else ProcessNameErrorMessage(fileName);
            }
        }

        private void Remove_DP_Click(object sender, RoutedEventArgs e)
        {
            if (DP_Grid.SelectedItem != null)
            {
                ViewModel.DistractionPrograms.Remove((DistractionProgramInfo)DP_Grid.SelectedItem);
            }
        }

        private bool IsValidProcessName(string input)
        {
            // 두 리스트중 하나라도 이미 존재하는지 확인
            foreach (var program in ViewModel.MonitoredPrograms)
            {
                if (string.Equals(program.ProcessName, input, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            foreach (var program in ViewModel.DistractionPrograms)
            {
                if (string.Equals(program.ProcessName, input, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return true;
        }

        private void ProcessNameErrorMessage(string processName)
        {
            MessageBox.Show($"프로세스 이름 '{processName}'은 이미 존재합니다. 다른 이름을 입력해주세요.",
                "입력 오류",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
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
