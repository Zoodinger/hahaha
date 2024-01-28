using Hahaha.Extensions;
using Hahaha.Pooling;
using Hahaha.System;
using Teo.AutoReference;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Hahaha {
    public class HaWord : Poolable {
        [SerializeField, GetInChildren(SyncMode.GetIfEmpty), Name("From")]
        private Transform from;

        [SerializeField, GetInChildren(SyncMode.GetIfEmpty), Name("To")]
        private Transform to;

        [SerializeField, Get] private new SpriteRenderer renderer;
        [SerializeField] private float lifetime = 2;

        [SerializeField] private float startScale;
        [SerializeField] private Quaternion startRotation;

        [SerializeField] private float startScaleRelative = 1.5f;
        [SerializeField] private float endScaleRelative = 5f;
        [SerializeField] private Vector2 speedRange = new(2, 4);
        private float _speed;

        private ScaledTimer _scaledTimer;
        private Vector3 _direction;

        private void Update() {
            transform.position += _direction * _speed * Time.deltaTime;

            var ratio = _scaledTimer.Elapsed / lifetime;
            if (ratio >= 1) {
                RePool();
            }

            var scale = Mathf.Lerp(startScale * startScaleRelative, startScale * endScaleRelative, ratio);
            transform.localScale = new Vector3(scale, scale, scale);

            renderer.color = renderer.color.WithAlpha(1 - ratio);
        }

        protected override void OnGet() {
            _speed = Random.Range(speedRange.x, speedRange.y);
            _scaledTimer.Reset();
            renderer.color = renderer.color.WithAlpha(1);

            var angle = Random.Range(-30.0f, 30.0f);
            var newRotation = Quaternion.Euler(0, 0, angle);

            var t = transform;
            t.rotation = startRotation * newRotation;

            _direction = (to.position - from.position).normalized;

            t.localScale = new Vector3(startScale, startScale, startScale);

            t.position += new Vector3(_direction.x, 0, 0);
        }

        private void OnDrawGizmos() {
            var t = transform;
            startScale = t.localScale.x;
            startRotation = t.rotation;

            if (from == null || to == null) {
                return;
            }

            Gizmos.DrawLine(from.position, to.position);
        }
    }
}
