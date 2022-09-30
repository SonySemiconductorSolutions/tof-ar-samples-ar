using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryProgramming
{
    public class ScanEpanding : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 50)]
        float _maxScale = 5;
        [SerializeField, Range(0, 20)]
        float _lifeTime = 5;

        float _startTime;


        void Awake()
        {
            _startTime = Time.time;
            UpdateScale();
        }
        void Update()
        {
            UpdateScale();

            if (Time.time >= _startTime + _lifeTime)
            {
                Destroy(gameObject);
            }
        }

        void UpdateScale()
        {
            transform.localScale = Vector3.one * _maxScale * Mathf.InverseLerp(_startTime, _startTime + _lifeTime, Time.time);
        }
    }
}
