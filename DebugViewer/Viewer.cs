using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DebugViewer
{
    /// <summary>
    /// 调试查看器
    /// </summary>
    public class Viewer : IDisposable
    {
        private static readonly int BUFFER_SIZE = 4096;

        private readonly bool global;

        private readonly ILogWriter writer;


        private bool inited = false;

        private EventWaitHandle bufferReadyHandle;

        private EventWaitHandle dataReadyHandle;

        private MemoryMappedFile mappedFile;

        /// <summary>
        /// 获取是否正在运行
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 调试查看器
        /// </summary>
        /// <param name="global"></param>
        /// <param name="writer"></param>
        public Viewer(bool global, ILogWriter writer)
        {
            this.global = global;
            this.writer = writer;
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="Exception"></exception>
        public void Start()
        {
            if (this.IsRunning == true)
            {
                return;
            }
            this.Init();
            ThreadPool.QueueUserWorkItem((state) => this.TryRun());
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <exception cref="DebugerExistsException"></exception>
        /// <exception cref="Exception"></exception>
        private void Init()
        {
            if (this.inited == true)
            {
                return;
            }

            var bufferReadyName = "DBWIN_BUFFER_READY";
            var dataReadyName = "DBWIN_DATA_READY";
            var bufferName = "DBWIN_BUFFER";

            if (global == true)
            {
                var globalPrefix = "Global\\";
                bufferReadyName = globalPrefix + bufferReadyName;
                dataReadyName = globalPrefix + dataReadyName;
                bufferName = globalPrefix + bufferName;
            }

            var createdNew = false;
            var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            var eventSecurity = new EventWaitHandleSecurity();
            eventSecurity.AddAccessRule(new EventWaitHandleAccessRule(everyone, EventWaitHandleRights.Modify | EventWaitHandleRights.Synchronize, AccessControlType.Allow));

            this.bufferReadyHandle = new EventWaitHandle(false, EventResetMode.AutoReset, bufferReadyName, out createdNew, eventSecurity);
            if (createdNew == false)
            {
                throw new DebugerExistsException();
            }

            this.dataReadyHandle = new EventWaitHandle(false, EventResetMode.AutoReset, dataReadyName, out createdNew, eventSecurity);
            if (createdNew == false)
            {
                throw new DebugerExistsException();
            }

            var memoryMapSecurity = new MemoryMappedFileSecurity();
            memoryMapSecurity.AddAccessRule(new AccessRule<MemoryMappedFileRights>(everyone, MemoryMappedFileRights.ReadWrite, AccessControlType.Allow));
            this.mappedFile = MemoryMappedFile.CreateOrOpen(bufferName, BUFFER_SIZE, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, memoryMapSecurity, HandleInheritability.None);

            this.inited = true;
        }

        /// <summary>
        /// 尝试运行
        /// </summary>
        private void TryRun()
        {
            try
            {
                this.Run();
            }
            catch (Exception ex)
            {
                var log = new DebugLog(Process.GetCurrentProcess().Id, ex.Message);
                this.writer.Write(log);
            }
        }

        /// <summary>
        /// 运行
        /// </summary>
        private void Run()
        {
            this.bufferReadyHandle.Set();
            var byteArray = new byte[BUFFER_SIZE];

            using (var view = this.mappedFile.CreateViewAccessor())
            {
                while (this.dataReadyHandle.WaitOne())
                {
                    view.ReadArray<byte>(0L, byteArray, 0, byteArray.Length);
                    int processId = BitConverter.ToInt32(byteArray, 0);
                    int terminator = Array.IndexOf<byte>(byteArray, 0, sizeof(int));
                    var length = terminator < 0 ? byteArray.Length : terminator - sizeof(int);
                    var message = Encoding.Default.GetString(byteArray, sizeof(int), length);

                    var log = new DebugLog(processId, message);
                    this.writer.Write(log);
                    this.bufferReadyHandle.Set();
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this.bufferReadyHandle != null)
            {
                this.bufferReadyHandle.Dispose();
                this.bufferReadyHandle = null;
            }

            if (this.dataReadyHandle != null)
            {
                this.dataReadyHandle.Dispose();
                this.dataReadyHandle = null;
            }

            if (this.mappedFile != null)
            {
                this.mappedFile.Dispose();
                this.mappedFile = null;
            }
        }
    }
}
