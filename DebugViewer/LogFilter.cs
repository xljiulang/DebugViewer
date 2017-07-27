using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DebugViewer
{
    /// <summary>
    /// 日志过滤器
    /// </summary>
    public class LogFilter
    {
        /// <summary>
        /// 正则表达式
        /// </summary>
        private Regex regex;

        /// <summary>
        /// 获取保留的进程id
        /// </summary>
        public int[] Pids { get; private set; }

        /// <summary>
        /// 获取匹配的正则表达式
        /// </summary>
        public string Pattern { get; private set; }


        /// <summary>
        /// 日志过滤器
        /// </summary>
        public LogFilter()
        {
            this.Pids = new int[0];
            this.Update(".");
        }

        /// <summary>
        /// 更新进程id
        /// </summary>
        /// <param name="pids"></param>
        public void Update(IEnumerable<int> pids)
        {
            this.Pids = pids.ToArray();
        }

        /// <summary>
        /// 更新表达式
        /// </summary>
        /// <param name="pattern"></param>
        public void Update(string pattern)
        {
            this.Pattern = string.IsNullOrEmpty(pattern) ? "." : pattern;
            this.regex = new Regex(this.Pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 是否与日志匹配
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool IsMatch(DebugLog log)
        {
            if (log == null)
            {
                return false;
            }

            return this.Pids.Contains(log.Pid) && this.regex.IsMatch(log.Message);
        }
    }
}
