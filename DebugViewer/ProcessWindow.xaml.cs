using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DebugViewer
{
    /// <summary>
    /// 进程选择窗口
    /// </summary>
    public partial class ProcessWindow : Window
    {
        /// <summary>
        /// 进程数据
        /// </summary>
        private readonly ObservableCollection<ProcessInfo> processSource;

        public ProcessWindow()
        {
            InitializeComponent();
            this.Loaded += ProcessWindow_Loaded;

            var allProcess = ProcessInfo.GetAllProcess();
            this.processSource = new ObservableCollection<ProcessInfo>(allProcess);
        }

        private void ProcessWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.dataGrid.LoadingRow += (s, ev) => ev.Row.Header = ev.Row.GetIndex() + 1;
            this.dataGrid.ItemsSource = this.processSource;
        }

        /// <summary>
        /// 选择或不选择进程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            var pid = (int)(sender as Button).Tag;
            var process = ProcessInfo.History.FirstOrDefault(item => item.Id == pid);
            process.IsSelected = !process.IsSelected;
        }
    }
}
