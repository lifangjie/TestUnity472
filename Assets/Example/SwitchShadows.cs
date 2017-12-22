using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchShadows : MonoBehaviour {
    // Use this for initialization
    private int _qualityLevel;

    private void Awake() {
        QualitySettings.SetQualityLevel(0);
        GetComponent<Button>().onClick.AddListener(delegate { QualitySettings.SetQualityLevel(++_qualityLevel % 3); });
    }
}