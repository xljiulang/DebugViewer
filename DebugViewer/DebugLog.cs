using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DebugViewer
{
    /// <summary>
    /// 表示调试输出的消息
    /// </summary>
    public class DebugLog
    {
        /// <summary>
        /// 获取进程id
        /// </summary>
        public int Pid { get; set; }

        /// <summary>
        /// 获取时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 获取消息内容
        /// </summary>
        public string Message { get; set; }


        /// <summary>
        /// 调试输出的消息
        /// </summary>
        /// <param name="pid">进程id</param>
        /// <param name="message">消息内容</param>
        public DebugLog(int pid, string message)
        {
            this.Pid = pid;
            this.Message = message;
            this.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Message;
        }
    }
}
