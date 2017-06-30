using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DebugViewer
{
    /// <summary>
    /// 表示日志容器
    /// </summary>
    public class LogContainer
    {
        /// <summary>
        /// 日志同步加锁
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// 所有日志
        /// </summary>
        private readonly List<DebugLog> logList = new List<DebugLog>();

        /// <summary>
        /// 获取记录条数
        /// </summary>
        public int Length
        {
            get
            {                
                return this.logList.Count;
            }
        }

        /// <summary>
        /// 添加记录到尾部
        /// </summary>
        /// <param name="log"></param>
        public void Add(DebugLog log)
        {
            lock (this.syncRoot)
            {
                this.logList.Add(log);
            }
        }

        /// <summary>
        /// 过滤日志
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public DebugLog[] Filter(LogFilter filter)
        {
            lock (this.syncRoot)
            {
                return this.logList.Where(item => filter.IsMatch(item)).ToArray();
            }
        }

        /// <summary>
        /// 清除日志
        /// </summary>
        public void Clear()
        {
            lock (this.syncRoot)
            {
                this.logList.Clear();
            }
        }
    }
}
