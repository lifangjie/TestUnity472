/**
 * WebPDecoder.cs
 * Created by: lfj20
 * Created on: 2017/12/28
 */

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace _Scripts.Util {
    public static class WebPDecoder {
        //WEBP_EXTERN uint8_t* WebPDecodeRGBA(const uint8_t* data, size_t data_size, int* width, int* height);
        [DllImport("webpdecoder")]
        private static extern IntPtr WebPDecodeRGBA(IntPtr data, int dataLength, out int width, out int height);

        //WEBP_EXTERN(void) WebPFree(void* ptr);
        [DllImport("webpdecoder")]
        private static extern void WebPSafeFree(IntPtr intPtr);

        public static Texture2D DecodeFromBytes(byte[] bytes) {
            IntPtr webpData = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, webpData, bytes.Length);
            int width, height;
            IntPtr rgbaData = WebPDecodeRGBA(webpData, bytes.Length, out width, out height);
            Marshal.FreeHGlobal(webpData);
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
            byte[] textureBytes = new byte[width * height * 4];
            Marshal.Copy(rgbaData, textureBytes, 0, textureBytes.Length);
            WebPSafeFree(rgbaData);
            texture2D.LoadRawTextureData(textureBytes);
            texture2D.Apply();
            return texture2D;
        }
    }
}