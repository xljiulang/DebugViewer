using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DebugViewer
{
    /// <summary>
    /// 调试日志存储写入接口
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// 写入调试日志
        /// </summary>
        /// <param name="log">调试日志</param>
        void Write(DebugLog log);
    }
}
