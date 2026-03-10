using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WorkWatcher.Views
{
    /// <summary>
    /// FindProcessDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FindProcessDialog : Window
    {
        public FindProcessDialog()
        {
            InitializeComponent();
        }

        public string SelectedProcessName { get; set; }

        public void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (P_List.SelectedItem != null)
            {
                // 프로세스 이름이 선택된 경우, 해당 프로세스 이름을 반환
                DialogResult = true;
                SelectedProcessName = P_List.SelectedItem.ToString();
                Close();
            }
            else
            {
                MessageBox.Show("프로세스 이름을 선택해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
