using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Gameplay
{
    public class FlickeringLight : MonoBehaviour
    {
        [SerializeField]
        private Light _light;

        [SerializeField]
        private Vector2 _rangeThreshold;

        [SerializeField]
        private Vector2 _valueThreshold;

        [SerializeField]
        private float _speed;

        private float _timeValue = 0f;

        private float _randomValue;

        private void Update()
        {
            _timeValue += Time.deltaTime;
            float threshold = Random.Range(_rangeThreshold.x, _rangeThreshold.y);

            if (_timeValue > threshold) {
                _timeValue = 0f;
                _randomValue = Random.Range(_valueThreshold.x, _valueThreshold.y); ;
            }

            _light.intensity = Mathf.Lerp(_light.intensity, _randomValue, Time.deltaTime * _speed);
        }

    }
}