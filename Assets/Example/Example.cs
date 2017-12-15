using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Example {
    public class Example : MonoBehaviour {
        private const string ServerAddress = "http://192.168.1.146/";
        public Button ClickToLoadInternet;
        public Button ClickToLoadLocal;
        public GameObject DownloadingDialog;
        public Slider Progress;

        private WWW _download;
        private string _targetLevel;

        private void Start() {
            ClickToLoadInternet.onClick.AddListener(delegate {
                _targetLevel = "downloadScene";
                StartCoroutine(StartDownload(ServerAddress + "downloadScene"));
            });
            ClickToLoadLocal.onClick.AddListener(delegate {
                _targetLevel = "localScene";
                StartCoroutine(StartDownload(Application.streamingAssetsPath + "/localScene"));
            });
        }

        private IEnumerator StartDownload(string address) {
            DownloadingDialog.SetActive(true);
            _download = WWW.LoadFromCacheOrDownload(address, 0);
            yield return null;
        }

        // Update is called once per frame
        private void Update() {
            if (DownloadingDialog.activeSelf) {
                Progress.value = _download.progress;
                if (_download.error != null) {
                    Debug.LogError(_download.error);
                    return;
                }

                if (!_download.isDone) return;
                //var bundle = _download.assetBundle;
                Application.LoadLevel(_targetLevel);
            }
        }
    }
}