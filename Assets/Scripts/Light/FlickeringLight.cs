using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Gameplay
{
    public class FlickeringLight : MonoBehaviour
    {

        [SerializeField]
        private Vector2 _rangeThreshold;

        [SerializeField]
        private Vector2 _valueThreshold;

        private Light _light;

        private float _timeValue = 0f;

        private void Awake()
        {
            _light = GetComponentInChildren<Light>();
        }

        private void Update()
        {
            _timeValue += Time.deltaTime;
            var threshold = Random.Range(_rangeThreshold.x, _rangeThreshold.y);

            if (_timeValue > threshold) {
                _timeValue = 0f;
                _light.intensity = Random.Range(_valueThreshold.x, _valueThreshold.y);
            }
        }


    }
}