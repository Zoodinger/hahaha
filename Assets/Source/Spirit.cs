using Hahaha.Pooling;
using Hahaha.System;
using Teo.AutoReference;
using UnityEngine;

namespace Hahaha {
    public class Spirit : Poolable {
        [SerializeField] private float bobHeight = 0.2f;
        [SerializeField] private float bobSpeed = 5;
        [SerializeField, GetInChildren] private new Collider2D collider;
        [SerializeField] private float rotationSpeed = 0.18f;
        [SerializeField] private Vector2 rotationRadius = new(0.25f, 0.1f);
        [SerializeField] private float goBackToHostTime = 10;

        [SerializeField] private float spawnTime = 2;
        [SerializeField] private float initialSpeed = 3;
        [SerializeField] private float moveSpeed = 2;

        private float _phase;

        private ScaledTimer _initSpawnTimer;
        private ScaledTimer _targetTimer;
        private ScaledTimer _bobTimer;
        private ScaledTimer _rotationTimer;
        private Enemy _previousTarget;
        private Enemy _newTarget;

        protected override void OnGet() {
            _bobTimer.Reset(Random.Range(0, bobSpeed));
            _rotationTimer.Reset(Random.Range(0, rotationSpeed));
            _previousTarget = null;
            _newTarget = null;
            _chasing = false;
        }

        private bool _chasing;

        private void Update() {
            var y = Mathf.Sin(_bobTimer.Elapsed * bobSpeed) * bobHeight;

            var xRotation = Mathf.Cos(_rotationTimer.Elapsed * rotationSpeed) * rotationRadius.x;
            var yRotation = Mathf.Sin(_rotationTimer.Elapsed * rotationSpeed) * rotationRadius.y;

            var t = collider.transform;
            var pos = t.localPosition;
            pos.y = y + yRotation;
            pos.x = xRotation;

            t.localPosition = pos;

            if (_initSpawnTimer.Elapsed < spawnTime) {
                transform.position += new Vector3(0, initialSpeed * Time.deltaTime, 0);
                return;
            }

            _chasing = true;

            if (_newTarget == null) {
                if (_targetTimer.Elapsed > goBackToHostTime) {
                    _newTarget = _previousTarget;
                } else {
                    _newTarget = LaughterGenerator.GetClosestCuredTo(_previousTarget);
                }
            }

            if (_newTarget != null && !_newTarget.IsCured) {
                _newTarget = null;
            }

            if (_newTarget != null) {
                transform.position = Vector2.MoveTowards(transform.position, _newTarget.Collider.bounds.center,
                    moveSpeed * Time.deltaTime);
            }
        }

        public Enemy Target => _newTarget;
        public bool IsChasing => _chasing;

        protected override void OnRePool() {
            collider.transform.localPosition = Vector3.zero;
            _chasing = false;
        }


        public void Init(Enemy enemy) {
            _previousTarget = enemy;
            _initSpawnTimer.Reset();
            _targetTimer.Reset();
        }
    }
}
