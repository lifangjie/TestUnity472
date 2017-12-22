/**
 * RotateSelf.cs
 * Created by: lfj20
 * Created on: 2017/12/26
 */

using UnityEngine;

namespace Example {
    public class RotateSelf : MonoBehaviour {
        private Transform _transform;
        private float _speed;

        private void Awake() {
            _transform = transform;
            _speed = Random.value * 5;
            Material material = new Material(GetComponent<Renderer>().sharedMaterial);
            GetComponent<Renderer>().material = material;
        }

        private void Update() {
            _transform.Rotate(new Vector3(Time.deltaTime * _speed, 0));
        }
    }
}