using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DebugViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, ILogWriter, IDisposable
    {
        private bool isCatching = false;

        private DateTime lastScrollTime = DateTime.Now;

        private readonly Viewer viewerUser;

        private readonly Viewer viewerKernel;

        private readonly LogFilter filter = new LogFilter();

        private readonly LogContainer logContainer = new LogContainer();

        private readonly ObservableCollection<DebugLog> itemSource = new ObservableCollection<DebugLog>();

        public MainWindow()
        {
            InitializeComponent();

            this.viewerUser = new Viewer(false, this);
            this.viewerKernel = new Viewer(true, this);

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.dataGrid.LoadingRow += (s, ev) => ev.Row.Header = ev.Row.GetIndex() + 1;
            this.dataGrid.ItemsSource = this.itemSource;

            this.btnStart.Click += btnStart_Click;
            this.btnFilter.Click += btnFilter_Click;

            this.ckScroll.Click += ckScroll_Click;
        }

        private void ckScroll_Click(object sender, RoutedEventArgs e)
        {
            var alternationCount = this.ckScroll.IsChecked == true ? 1 : 2;
            this.dataGrid.AlternationCount = alternationCount;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (this.isCatching == false)
            {
                this.Start();
                this.isCatching = true;
                this.btnStart.Content = "暂停";
            }
            else
            {
                this.isCatching = false;
                this.btnStart.Content = "监听";
            }
        }

        private void Start()
        {
            try
            {
                this.viewerUser.Start();
                this.viewerKernel.Start();
            }
            catch (DebugerExistsException)
            {
                MessageBox.Show("请检查系统是否存在其它调试器", "无法调动调试器");
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "无法调动调试器");
                Application.Current.Shutdown();
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            var pid = 0;
            int.TryParse(this.txtPid.Text.Trim(), out pid);
            var pattern = this.txtPattern.Text.Trim();
            this.filter.Update(pid, pattern);

            var logs = this.logContainer.Filter(this.filter);
            this.itemSource.Clear();
            foreach (var item in logs)
            {
                this.itemSource.Add(item);
            }
        }

        void ILogWriter.Write(DebugLog log)
        {
            if (this.isCatching == false)
            {
                return;
            }

            this.logContainer.Add(log);

            if (this.filter.IsMatch(log) == true)
            {
                this.AddLogToUi(log);
            }
        }



        private void AddLogToUi(DebugLog log)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.itemSource.Add(log);
                if (this.ckScroll.IsChecked == true && DateTime.Now.Subtract(this.lastScrollTime).TotalSeconds > 1d)
                {
                    this.lastScrollTime = DateTime.Now;
                    this.dataGrid.ScrollIntoView(log);
                }
            }));
        }

        public void Dispose()
        {
            this.viewerUser.Dispose();
            this.viewerKernel.Dispose();
        }
    }
}
