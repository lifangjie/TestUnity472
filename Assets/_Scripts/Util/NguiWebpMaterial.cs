/**
 * WebpMaterial.cs
 * Created by: lfj20
 * Created on: 2017/12/21
 */

using UnityEngine;

namespace _Scripts.Util {
    [ExecuteInEditMode]
    public class NguiWebpMaterial : MonoBehaviour {
        public Material Material;
        public TextAsset WebpBytes;
        public bool Enable;

        private void Awake() {
            if (!Enable || Material == null || WebpBytes == null) {
                return;
            }

            Material.mainTexture = WebPDecoder.DecodeFromBytes(WebpBytes.bytes);
        }
    }
}