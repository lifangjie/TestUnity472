/**
 * BakeUiRef.cs
 * Created by: lfj20
 * Created on: 2017/12/19
 */

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Editor {
    public static class BakeUiRef {
        [MenuItem("Build/Bake UI Reference")]
        static void BakeUiReference() {
            var objects = Selection.objects;
            var length = Selection.objects.Length;
            // todo: mark all sprite's info into text
        }
    }
}