using System.Collections.Generic;
using Cyens.Pooling;
using Hahaha.Extensions;
using Hahaha.System;
using Teo.AutoReference;
using UnityEngine;

namespace Hahaha {
    public class Character : MonoBehaviour {
        [SerializeField, Get] private Rigidbody2D body;
        [SerializeField, Get] private Animator animator;
        [SerializeField, Get] private new SpriteRenderer renderer;
        [SerializeField, Get] private new Collider2D collider;

        [SerializeField, GetInChildren(SyncMode.GetIfEmpty), IgnoreSelf, Name("Left")]
        private Transform leftGun;

        [SerializeField, GetInChildren(SyncMode.GetIfEmpty), IgnoreSelf, Name("Right")]
        private Transform rightGun;

        [SerializeField] private float axisAcceleration = 0.1f;
        [SerializeField] private float speed = 8;
        [SerializeField] private float jumpForce = 8;

        [SerializeField] private float shootRate = 0.1f;

        [SerializeField] private float gravityScale = 3;

        [SerializeField] private PrefabPool<Gas> gasPool;
        [SerializeField] private float hitForce = 5;

        [SerializeField] private float minVerticalHit = 1f;
        [SerializeField] private float hitDeceleration = 1f;

        [SerializeField] private float canTakeDamageAgainTime = 2f;
        private ScaledTimer _damageTimer;

        private readonly List<ContactPoint2D> _contacts = new();

        private InputActions _actions;

        private int _direction;
        private int _enemyLayer;

        private Vector2 _hitVelocity;

        private Vector2 _inputAxis;
        private Vector2 _inputVelocity;

        private bool _isDamaged = false;

        private bool _isGrounded;

        private bool _isJumpPressed;
        private bool _isShootingPressed;

        private ScaledTimer _shootScaledTimer;
        private int _solidLayer;

        private LayerMask _solidLayerMask;

        private Vector2 _velocity;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private float MoveVelocity => _inputVelocity.x * speed;

        public Collider2D Collider => collider;

        public Vector2 GunPosition =>
            _direction switch {
                -1 => leftGun.transform.position,
                1 => rightGun.transform.position,
                _ => transform.position,
            };

        private void Awake() {
            _actions = new InputActions();
            _actions.Enable();

            _direction = 1;

            _actions.Player.Move.performed += ctx => { _inputAxis = new Vector2(ctx.ReadValue<Vector2>().x, 0); };
            _actions.Player.Move.canceled += _ => { _inputAxis = Vector2.zero; };

            _actions.Player.Jump.performed += _ => { _isJumpPressed = true; };

            _actions.Player.Shoot.performed += _ => { _isShootingPressed = true; };

            _actions.Player.Shoot.canceled += _ => { _isShootingPressed = false; };

            _solidLayer = LayerMask.NameToLayer("Solid");
            _enemyLayer = LayerMask.NameToLayer("Enemy");
            _solidLayerMask = LayerMask.GetMask("Solid", "Enemy");
        }

        private void Update() {
            _isGrounded = collider.IsGrounded(_solidLayerMask);

            ApplyRunVelocity(ref _inputVelocity.x);

            _hitVelocity = _hitVelocity.Decelerated(hitDeceleration);
            if (_hitVelocity == Vector2.zero) {
                // _isDamaged = false;
            }

            if (_isDamaged && _damageTimer.Elapsed > canTakeDamageAgainTime) {
                _isDamaged = false;
            }

            if (_isShootingPressed) {
                var elapsed = _shootScaledTimer.Elapsed;
                if (elapsed >= shootRate) {
                    _shootScaledTimer.Reset(Mathf.Max(0, elapsed - shootRate));
                    var gas = gasPool.Get(null, GunPosition);

                    gas.Shoot(_direction);
                }
            } else {
                _shootScaledTimer.Reset();
            }

            _direction = _inputVelocity.x switch {
                > 0 => 1,
                < 0 => -1,
                _ => _direction,
            };


            _velocity.y += Physics2D.gravity.y * Time.deltaTime * gravityScale;

            _velocity.y = Mathf.Max(-30, _velocity.y);
            _velocity.x = MoveVelocity;

            if (_velocity.x < 0) {
                renderer.flipX = true;
            } else if (_velocity.x > 0) {
                renderer.flipX = false;
            }

            if (_isGrounded) {
                if (_isJumpPressed) {
                    _velocity.y = jumpForce;
                }
            }

            animator.SetFloat(Speed, Mathf.Abs(_velocity.x));

            _isJumpPressed = false;
        }

        private void FixedUpdate() {
            body.velocity = Vector2.zero;
            body.angularVelocity = 0;
            var newPosition = _velocity + _hitVelocity;
            body.MovePosition(body.position + newPosition * Time.fixedDeltaTime);
        }

        private void OnDestroy() {
            _actions.Dispose();
        }

        private void OnCollisionStay2D(Collision2D other) {
            var layer = other.gameObject.layer;
            if (layer == _enemyLayer) {
                CollideWithEnemy(other);
            } else if (layer == _solidLayer) {
                CollideWithSolid(other);
            }
        }

        private void CollideWithEnemy(Collision2D other) {
            if (_isDamaged) {
                return;
            }

            var point = other.collider.bounds.center;
            Hit(point);
            _isDamaged = true;

            _damageTimer.Reset();
        }

        public void Hit(Vector2 point) {
            var direction = (collider.bounds.center.To2D() - point).normalized * hitForce;
            if (direction.y >= -0.5f && direction.y < minVerticalHit) {
                direction = direction.WithY(minVerticalHit).normalized;
            } else {
                direction = direction.normalized;
            }

            _hitVelocity += direction * hitForce;
            _hitVelocity = Vector2.ClampMagnitude(_hitVelocity, hitForce);

            _inputVelocity = Vector2.zero;
            _velocity.y = 0;
        }

        private HitDirection GetDirection(ContactPoint2D point) {
            var normal = point.normal;
            switch (normal.x) {
                case < -0.9f:
                    return HitDirection.Left;
                case > 0.9f:
                    return HitDirection.Right;
            }

            switch (normal.y) {
                case < -0.9f:
                    return HitDirection.Up;
                case > 0.9f:
                    return HitDirection.Down;
            }

            return HitDirection.None;
        }

        private void CollideWithSolid(Collision2D other) {
            _contacts.Clear();
            other.GetContacts(_contacts);

            foreach (var contact in _contacts) {
                var hitDirection = GetDirection(contact);

                if (hitDirection == HitDirection.Left && _velocity.x < 0) {
                    _velocity.x = 0;
                }

                if (hitDirection == HitDirection.Right && _velocity.x > 0) {
                    _velocity.x = 0;
                }

                if (hitDirection == HitDirection.Up && _velocity.y > 0) {
                    _velocity.y = 0;
                }

                if (hitDirection == HitDirection.Down && _velocity.y < 0) {
                    _velocity.y = 0;
                }
            }
        }

        private void ApplyRunVelocity(ref float currentVelocity) {
            var input = Mathf.Clamp(_inputAxis.x, -1, 1);

            if (currentVelocity < input) {
                currentVelocity = Mathf.Min(currentVelocity + axisAcceleration * Time.deltaTime, input);
            } else if (currentVelocity > input) {
                currentVelocity = Mathf.Max(currentVelocity - axisAcceleration * Time.deltaTime, input);
            }
        }

        private enum HitDirection {
            None,
            Left,
            Right,
            Up,
            Down,
        }
    }
}
