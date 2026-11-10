using System;
using UnityEngine;

namespace FX
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;
        [SerializeField] private Transform target;
        [SerializeField] private float lerpSpeed = 1f;

        private void LateUpdate()
        {
            transform.position = Vector2.Lerp(transform.position, target.position, Time.unscaledDeltaTime * lerpSpeed);
            transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        }
    }
}