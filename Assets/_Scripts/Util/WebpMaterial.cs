/**
 * WebpMaterial.cs
 * Created by: lfj20
 * Created on: 2017/12/21
 */

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace _Scripts.Util {
    [ExecuteInEditMode]
    public class WebpMaterial : MonoBehaviour {
        public Material Material;
        public TextAsset WebpBytes;
        public bool Enable;

        //WEBP_EXTERN uint8_t* WebPDecodeRGBA(const uint8_t* data, size_t data_size, int* width, int* height);
        [DllImport("webpdecoder")]
        private static extern IntPtr WebPDecodeRGBA(IntPtr data, int dataLength, out int width, out int height);

        //WEBP_EXTERN(void) WebPFree(void* ptr);
        [DllImport("webpdecoder")]
        private static extern void WebPSafeFree(IntPtr intPtr);

        private void Awake() {
            if (!Enable || Material == null || WebpBytes == null) {
                return;
            }

            if (SystemInfo.maxTextureSize < 2048) {
            }

            IntPtr webpData = Marshal.AllocHGlobal(WebpBytes.bytes.Length);
            Marshal.Copy(WebpBytes.bytes, 0, webpData, WebpBytes.bytes.Length);
            int width, height;
            IntPtr rgbaData = WebPDecodeRGBA(webpData, WebpBytes.bytes.Length, out width, out height);
            Marshal.FreeHGlobal(webpData);
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
            byte[] bytes = new byte[width * height * 4];
            Marshal.Copy(rgbaData, bytes, 0, bytes.Length);
            WebPSafeFree(rgbaData);
            texture2D.LoadRawTextureData(bytes);
            texture2D.Apply();
            Material.mainTexture = texture2D;
        }
    }
}