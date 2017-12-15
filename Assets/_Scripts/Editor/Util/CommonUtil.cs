using System.Reflection;

namespace _Scripts.Editor.Util {
    public static class CommonUtil {
        public static void ClearConsole() {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.ActiveEditorTracker));
            var type = assembly.GetType("UnityEditorInternal.LogEntries");
            var method = type.GetMethod("Clear");
            if (method != null) method.Invoke(new object(), null);
        }
    }
}