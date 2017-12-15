/**
 * AutoBindImage.cs
 * Created by: lfj20
 * Created on: 2017/12/19
 */

using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Editor {
    public class AutoBindImage : UnityEditor.AssetModificationProcessor {
        public static string[] OnWillSaveAssets(string[] paths) {
            Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
            foreach (var canvas in canvases) {
                Image[] images = canvas.gameObject.GetComponentsInChildren<Image>(true);
                foreach (var image in images) {
                    if (image == null) {
                        continue;
                    }

                    var bindSprite = image.gameObject.GetComponent<BindSprite>();
                    if (bindSprite == null) {
                        bindSprite = image.gameObject.AddComponent<BindSprite>();
                    }

                    bindSprite.SpriteName = image.sprite.name;
                }
            }

            return paths;
        }
    }
}