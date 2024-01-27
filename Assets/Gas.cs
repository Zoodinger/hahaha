using Hahaha.Extensions;
using Hahaha.Pooling;
using Hahaha.System;
using Teo.AutoReference;
using UnityEngine;

namespace Hahaha {
    public class Gas : Poolable {
        [SerializeField, Get] private new SpriteRenderer renderer;
        [SerializeField] private float horizontalSpeed = 2;
        [SerializeField] private float lifeTime = 3.0f;
        [SerializeField] private float damage = 1;

        [SerializeField] private float startAlpha = 0.8f;
        [SerializeField] private float endAlpha = 0.1f;

        [SerializeField] private Vector2 verticalSpeedRange = new(-0.1f, 0.3f);
        [SerializeField] private Vector2 scaleRange = new(0.1f, 1.1f);

        private float _verticalSpeed;

        private Timer _timer;
        private float _direction;

        public float Damage => damage;

        protected override void OnGet() {
            _timer.Reset();
        }

        public void Shoot(int direction) {
            _timer.Reset();
            _direction = Mathf.Sign(direction);
            _verticalSpeed = Random.Range(verticalSpeedRange.x, verticalSpeedRange.y);
        }

        private void Update() {
            if (_timer.Elapsed >= lifeTime) {
                RePool();
                return;
            }

            var ratio = _timer.Elapsed / lifeTime;

            var t = transform;
            t.position += new Vector3(_direction * horizontalSpeed, _verticalSpeed, 0)  * Time.deltaTime;
            t.localScale = Vector3.one * Mathf.Lerp(scaleRange.x, scaleRange.y, ratio);

            renderer.color = renderer.color.WithAlpha(Mathf.Lerp(startAlpha, endAlpha, ratio));
        }
    }
}
