using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DebugViewer
{
    /// <summary>
    /// 表示进程信息
    /// </summary>
    public class ProcessInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性变化事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 获取历史所有进程 
        /// </summary>
        public static ProcessInfo[] History { get; private set; }

        static ProcessInfo()
        {
            History = new ProcessInfo[0];
        }

        /// <summary>
        /// 获取所有进程
        /// </summary>
        /// <returns></returns>
        public static ProcessInfo[] GetAllProcess()
        {
            var allProcess = Process
                .GetProcesses()
                .Select(item => new ProcessInfo
                {
                    Id = item.Id,
                    Title = item.MainWindowTitle,
                    Name = item.ProcessName + ".exe",
                })
                .OrderBy(item => item.Name);

            var q = from c in allProcess
                    join o in History
                    on c.Id equals o.Id into g
                    from item in g.DefaultIfEmpty()
                    select item == null ? c : item;

            History = q.ToArray();
            return History;
        }

        /// <summary>
        /// 获取进程id
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 获取进程名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取进程主窗口标题
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// 进程选择状态
        /// </summary>
        private SelectState selected;

        /// <summary>
        /// 获取或设置进程选择状态
        /// </summary>
        public SelectState IsSelected
        {
            get
            {
                return this.selected;
            }
            set
            {
                this.selected = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
                }
            }
        }
    }
}
