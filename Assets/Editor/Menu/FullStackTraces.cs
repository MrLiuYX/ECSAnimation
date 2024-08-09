using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEditor;
using Unity.Collections;

namespace Assets.Scripts.Editor
{
    public class Leak
    {
        [MenuItem("Custom/Show Leak Detection Mode")]
        static void ShowLeakDetection()
        {
            EditorUtility.DisplayDialog("内存泄漏检测设置", string.Format("NativeLeakDetection.Mode：{0}", NativeLeakDetection.Mode.ToString()), "OK");
        }

        [MenuItem("Custom/Leak Detection Enabled")]
        static void LeakDetectionEnabled()
        {
            NativeLeakDetection.Mode = NativeLeakDetectionMode.Enabled;
        }

        [MenuItem("Custom/Leak Detection Enabled", true)]  //第二个参数表示本函数是菜单是否可用的验证函数
        static bool ValidateLeakDetectionEnabled()
        {
            return NativeLeakDetection.Mode != NativeLeakDetectionMode.Enabled;
        }

        [MenuItem("Custom/Leak Detection Enabled With StackTrace")]
        static void LeakDetectionEnabledWithStackTrace()
        {
            NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
        }

        [MenuItem("Custom/Leak Detection Enabled With StackTrace", true)]  //第二个参数表示本函数是菜单是否可用的验证函数
        static bool ValidateLeakDetectionEnabledWithStackTrace()
        {
            return NativeLeakDetection.Mode != NativeLeakDetectionMode.EnabledWithStackTrace;
        }

        [MenuItem("Custom/Leak Detection Disable")]
        static void LeakDetectionDisable()
        {
            NativeLeakDetection.Mode = NativeLeakDetectionMode.Disabled;
        }

        [MenuItem("Custom/Leak Detection Disable", true)]  //第二个参数表示本函数是菜单是否可用的验证函数
        static bool ValidateLeakDetectionDisable()
        {
            return NativeLeakDetection.Mode != NativeLeakDetectionMode.Disabled;
        }
    }
}
