using Teo.AutoReference;
using UnityEngine;

namespace Hahaha {
    public class Character : MonoBehaviour {
        [SerializeField, Get] private Rigidbody2D body;
        [SerializeField, Get] private new Collider2D collider;

        [SerializeField] private float axisAcceleration = 0.1f;
        [SerializeField] private float speed = 4;
        [SerializeField] private float jumpForce = 10;

        private InputActions _actions;
        private Vector2 _inputAxis;
        private Vector2 _inputVelocity;
        private bool _jumpPressed;
        private bool _leftTheGround;
        private bool _isGrounded;

        private Vector2 _velocity;

        private void Awake() {
            _actions = new InputActions();
            _actions.Enable();

            _actions.Player.Move.performed += ctx => { _inputAxis = new Vector2(ctx.ReadValue<Vector2>().x, 0); };
            _actions.Player.Move.canceled += _ => { _inputAxis = Vector2.zero; };

            _actions.Player.Jump.performed += _ => { _jumpPressed = true; };
        }

        private void Update() {
            CheckGrounded();

            _velocity.x = ApplyRunVelocity(ref _inputVelocity.x);

            if (_isGrounded) {
                _leftTheGround = false;
                if (_jumpPressed) {
                    _velocity.y = jumpForce;
                    _leftTheGround = true;
                } else if (_velocity.y <= 0.001f) {
                    _velocity.y = Mathf.Max(-1, _velocity.y);
                }
            }

            _jumpPressed = false;

            if (!_isGrounded) {
                if (!_leftTheGround && _velocity.y < 0) {
                    _leftTheGround = true;
                    _velocity.y = 0;
                }

                _velocity.y += Physics2D.gravity.y * Time.deltaTime;
            }


        }

        private void FixedUpdate() {
            body.MovePosition(body.position + _velocity * Time.fixedDeltaTime);
        }

        private void OnDestroy() {
            _actions.Dispose();
        }

        private void CheckGrounded() {
            var bounds = collider.bounds;
            collider.enabled = false;
            const float offset = 0.1f;
            var hit = Physics2D.BoxCast(bounds.center, bounds.size, 0, Vector2.down, offset);
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
