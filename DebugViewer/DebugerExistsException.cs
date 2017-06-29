using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DebugViewer
{
    /// <summary>
    /// 表示调试器已运行异常
    /// </summary>
    public class DebugerExistsException : Exception
    {
        /// <summary>
        /// 调试器已运行异常
        /// </summary>
        public DebugerExistsException()
            : base("系统已运行其它的调试器")
        {
        }
    }
}
