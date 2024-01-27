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

        [SerializeField] private PrefabPool<Gas> gasPool;

        private InputActions _actions;
        private Vector2 _inputAxis;
        private Vector2 _inputVelocity;
        private bool _jumpPressed;
        private bool _leftTheGround;
        private bool _isGrounded;

        private bool _isShooting;

        private LayerMask _solidLayerMask;

        private Vector2 _velocity;

        private int _direction;

        private Timer _shootTimer;

        private void Awake() {
            _actions = new InputActions();
            _actions.Enable();

            _actions.Player.Move.performed += ctx => { _inputAxis = new Vector2(ctx.ReadValue<Vector2>().x, 0); };
            _actions.Player.Move.canceled += _ => { _inputAxis = Vector2.zero; };

            _actions.Player.Jump.performed += _ => { _jumpPressed = true; };

            _actions.Player.Shoot.performed += _ => { _isShooting = true; };

            _actions.Player.Shoot.canceled += _ => { _isShooting = false; };

            _solidLayerMask = LayerMask.GetMask("Solid");
        }

        private void Update() {
            CheckGrounded();

            var x = ApplyRunVelocity(ref _inputVelocity.x);

            var velocity = body.velocity;
            velocity.x = x;

            body.velocity = velocity;

            if (_isShooting) {
                var elapsed = _shootTimer.Elapsed;
                if (elapsed >= shootRate) {
                    _shootTimer.Reset(Mathf.Max(0, elapsed - shootRate));
                    var gas = gasPool.Get(null, transform.position);

                    gas.Shoot(_direction);
                }
            } else {
                _shootTimer.Reset();
            }

            _direction = x switch {
                > 0 => 1,
                < 0 => -1,
                _ => _direction,
            };

            if (_isGrounded) {
                _leftTheGround = false;
                if (_jumpPressed) {
                    body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
            }

            _jumpPressed = false;
        }

        private void OnDestroy() {
            _actions.Dispose();
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

        private float ApplyRunVelocity(ref float currentVelocity) {
            var input = Mathf.Clamp(_inputAxis.x, -1, 1);

            if (currentVelocity < input) {
                currentVelocity = Mathf.Min(currentVelocity + axisAcceleration * Time.deltaTime, input);
            } else if (currentVelocity > input) {
                currentVelocity = Mathf.Max(currentVelocity - axisAcceleration * Time.deltaTime, input);
            }

            return currentVelocity * speed;
        }
    }
}
