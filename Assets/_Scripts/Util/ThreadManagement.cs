/**
 * ThreadManagement.cs
 * Created by: lfj20
 * Created on: 2017/12/29
 */

using System.Threading;

namespace _Scripts.Util {
    public static class ThreadManagement {
        public static int MainThreadId { get; private set; }

        public static int GetCurrentThreadId() {
            return Thread.CurrentThread.ManagedThreadId;
        }

        public static void Initialize() {
            MainThreadId = GetCurrentThreadId();
        }

        public static bool IsMainThread {
            get { return GetCurrentThreadId() == MainThreadId; }
        }
    }
}