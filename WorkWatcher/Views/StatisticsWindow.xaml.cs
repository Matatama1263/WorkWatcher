using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// StatisticsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true; // 창이 실제로 닫히는 것을 취소
            this.Hide();     // 대신 숨김
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ViewModels.StatisticsViewModel;
            if (viewModel != null)
            {
                viewModel.SortSessions();
            }
        }

        internal void UpdateStatistics()
        {
            var viewModel = DataContext as ViewModels.StatisticsViewModel;
            if (viewModel != null)
            {
                viewModel.LoadStatistics();
                viewModel.LoadSessions();
            }
        }
    }
}
