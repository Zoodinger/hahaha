using System.Collections.Generic;
using System.Linq;
using Cyens.Pooling;
using Hahaha.System;
using Teo.AutoReference;
using UnityEngine;

namespace Hahaha {
    public class Character : MonoBehaviour {
        [SerializeField, Get] private Rigidbody2D body;
        [SerializeField, Get] private new Collider2D collider;

        [SerializeField] private float axisAcceleration = 0.1f;
        [SerializeField] private float speed = 8;
        [SerializeField] private float jumpForce = 8;

        [SerializeField] private float shootRate = 0.1f;

        [SerializeField] private float gravityScale = 3;

        [SerializeField] private PrefabPool<Gas> gasPool;

        private InputActions _actions;

        private Vector2 _inputAxis;
        private Vector2 _inputVelocity;
        private float MoveVelocity => _inputVelocity.x * speed;

        private bool _isGrounded;

        private bool _isJumpPressed;
        private bool _isShootingPressed;

        private LayerMask _solidLayerMask;
        private int _solidLayer;

        private Vector2 _velocity;

        private int _direction;

        private Timer _shootTimer;

        private void Awake() {
            _actions = new InputActions();
            _actions.Enable();

            _actions.Player.Move.performed += ctx => { _inputAxis = new Vector2(ctx.ReadValue<Vector2>().x, 0); };
            _actions.Player.Move.canceled += _ => { _inputAxis = Vector2.zero; };

            _actions.Player.Jump.performed += _ => { _isJumpPressed = true; };

            _actions.Player.Shoot.performed += _ => { _isShootingPressed = true; };

            _actions.Player.Shoot.canceled += _ => { _isShootingPressed = false; };

            _solidLayer = LayerMask.NameToLayer("Solid");
            _solidLayerMask = LayerMask.GetMask("Solid");
        }

        private void Update() {
            CheckGrounded();

            ApplyRunVelocity(ref _inputVelocity.x);

            if (_isShootingPressed) {
                var elapsed = _shootTimer.Elapsed;
                if (elapsed >= shootRate) {
                    _shootTimer.Reset(Mathf.Max(0, elapsed - shootRate));
                    var gas = gasPool.Get(null, transform.position);

                    gas.Shoot(_direction);
                }
            } else {
                _shootTimer.Reset();
            }

            _direction = _inputVelocity.x switch {
                > 0 => 1,
                < 0 => -1,
                _ => _direction,
            };


            _velocity.y += Physics2D.gravity.y * Time.deltaTime * gravityScale;
            _velocity.x = MoveVelocity;

            if (_isGrounded) {
                if (_isJumpPressed) {
                    _velocity.y = jumpForce;
                }
            }

            _isJumpPressed = false;
        }

        private void OnDestroy() {
            _actions.Dispose();
        }

        private void FixedUpdate() {
            body.velocity = Vector2.zero;
            body.angularVelocity = 0;
            body.MovePosition(body.position + _velocity * Time.fixedDeltaTime);
        }

        private readonly List<ContactPoint2D> _contacts = new();

        private enum HitDirection {
            None,
            Left,
            Right,
            Up,
            Down,
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

        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.layer != _solidLayer) {
                return;
            }

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

                // switch (normal.y) {
                //     case > 0.5f when _velocity.y < 0:
                //     case < -0.5f when _velocity.y > 0:
                //         _velocity.y = 0;
                //         break;
                // }
                //
                // switch (normal.x) {
                //     case > 0.5f when _velocity.x < 0:
                //     case < -0.5f when _velocity.x > 0:
                //         _velocity.x = 0;
                //         _inputVelocity.x = 0;
                //         break;
                // }
            }
        }


        private void CheckGrounded() {
            var bounds = collider.bounds;
            collider.enabled = false;
            const float offset = 0.1f;
            var point1 = bounds.min + new Vector3(-offset, -offset);
            var point2 = new Vector3(bounds.max.x - offset, bounds.min.y + offset);
            var hit = Physics2D.OverlapArea(point1, point2, _solidLayerMask);
            collider.enabled = true;
            _isGrounded = hit;
        }

        private void ApplyRunVelocity(ref float currentVelocity) {
            var input = Mathf.Clamp(_inputAxis.x, -1, 1);

            if (currentVelocity < input) {
                currentVelocity = Mathf.Min(currentVelocity + axisAcceleration * Time.deltaTime, input);
            } else if (currentVelocity > input) {
                currentVelocity = Mathf.Max(currentVelocity - axisAcceleration * Time.deltaTime, input);
            }
        }
    }
}
