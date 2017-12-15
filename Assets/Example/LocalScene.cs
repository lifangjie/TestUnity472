using UnityEngine;
using UnityEngine.UI;

namespace Example {
    public class LocalScene: MonoBehaviour {
        private void Start () {
            GetComponent<Text>().text = "This is the local scene.";
        }
    }
}
