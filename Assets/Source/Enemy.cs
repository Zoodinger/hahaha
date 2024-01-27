using System;
using Hahaha.Extensions;
using Hahaha.Pooling;
using Hahaha.System;
using Teo.AutoReference;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Hahaha {
    public class Enemy : Poolable {
        private static readonly int Saturation = Shader.PropertyToID("_Saturation");
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        [SerializeField, Get] private Rigidbody2D body;
        [SerializeField, Get] private new SpriteRenderer renderer;
        [SerializeField, Get] private new Collider2D collider;
        [SerializeField] private float maxLife = 1;
        [SerializeField] private float damagePerSecond = 0.25f;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private float laughForce = 4;
        [SerializeField] private float laughRate = 0.25f;
        [SerializeField] private float gravityScale = 3;

        [SerializeField] private float haOffsetY = 0.25f;
        [SerializeField] private float detectionDistance = 10;
        [SerializeField] private float detectionVerticalDistance = 2;

        [SerializeField] private float chaseSpeed = 4;
        [SerializeField] private float laughSpeed = 1;
        [SerializeField] private float normalSpeed = 2;

        [SerializeField] private Vector2 roamDurationRange = new Vector2(2, 4);
        private float _nextRoamTime;

        private int _gasLayer;

        private int _isTakingDamage = 0;
        private float _life;
        private Material _material;

        private Character _player;


        private ScaledTimer _laughBounceTimer;

        private ScaledTimer _roamTimer;

        private Vector2 _velocity;
        private int _solidMask;

        private enum Direction {
            Left,
            Right,
        }

        private Direction _direction;

        private void Awake() {
            _gasLayer = LayerMask.NameToLayer("Gas");
            _material = renderer.material;
            _material.SetColor(Color1, color);


            if (_player == null) {
                _player = FindObjectOfType<Character>();
            }

            _solidMask = LayerMask.GetMask("Solid", "Player");
            OnGet();
        }

        private void ChasePlayer(in Vector2 seeVector) {
            var speed = _isTakingDamage > 0 ? laughSpeed : chaseSpeed;

            if (Mathf.Abs(seeVector.x) < 0.05f) {
                _velocity.x = 0;
            } else if (seeVector.x < 0) {
                if (!collider.IsGroundedLeft(_solidMask)) {
                    _velocity.x = 0;
                } else {
                    _velocity.x = -speed;
                }
            } else {
                if (!collider.IsGroundedRight(_solidMask)) {
                    _velocity.x = 0;
                } else {
                    _velocity.x = speed;
                }
            }
        }

        private void SwitchDirection() {
            _roamTimer.ResetFromCurrentInterval(_nextRoamTime);
            _nextRoamTime = roamDurationRange.GetRandomValue();
            _direction = (Direction)(((int)_direction + 1) % 2);
        }

        private void RoamAround() {
            if (_roamTimer.Elapsed > _nextRoamTime) {
                SwitchDirection();
                return;
            }

            switch (_direction) {
                case Direction.Left:
                    if (!collider.IsGroundedLeft()) {
                        SwitchDirection();
                    } else {
                        _velocity.x = -normalSpeed;
                    }

                    break;
                case Direction.Right:
                    if (!collider.IsGroundedRight()) {
                        SwitchDirection();
                    } else {
                        _velocity.x = normalSpeed;
                    }

                    break;
            }
        }

        private void Update() {
            if (CanSeePlayer(out var seeVector)) {
                ChasePlayer(seeVector);
            } else {
                RoamAround();
            }

            if (_isTakingDamage == 0) {
                _velocity.y += Physics2D.gravity.y * Time.deltaTime * gravityScale;
                _velocity.y = Mathf.Max(-10, _velocity.y);
                return;
            }

            if (_laughBounceTimer.Elapsed >= laughRate) {
                _laughBounceTimer.ResetFromCurrentInterval(laughRate);

                MakeHaHa();
            }

            var isGrounded = collider.IsGrounded(_solidMask);

            if (isGrounded) {
                _velocity.y = laughForce;
            } else {
                _velocity.y += Physics2D.gravity.y * Time.deltaTime * gravityScale;
                _velocity.y = Mathf.Max(-10, _velocity.y);
            }
        }

        private void FixedUpdate() {
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
            body.MovePosition(body.position + _velocity * Time.fixedDeltaTime);
        }

        //
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.layer == _gasLayer) {
                _isTakingDamage += 1;
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.gameObject.layer == _gasLayer) {
                _isTakingDamage -= 1;
            }
        }

        private void OnTriggerStay2D(Collider2D other) {
            if (other.gameObject.layer != _gasLayer) {
                return;
            }

            var gas = other.gameObject.GetComponent<Gas>();

            _life -= damagePerSecond * gas.Damage * Time.deltaTime;
            _material.SetFloat(Saturation, 1 - _life / maxLife);
        }

        private void MakeHaHa() {
            LaughterGenerator.MakeHaAt(collider.GetTop() + new Vector2(0, haOffsetY));
        }

        private bool CanSeePlayer(out Vector2 vector) {
            var pos = collider.bounds.center;
            var otherPos = _player.Collider.bounds.center;
            vector = otherPos - pos;
            var distance = vector.magnitude;

            if (distance >= detectionDistance) {
                return false;
            }

            if (Mathf.Abs(vector.y) > detectionVerticalDistance) {
                return false;
            }

            var direction = vector.normalized;

            return !Physics2D.Raycast(pos, direction, distance, Utils.SolidMask);
        }

        protected override void OnGet() {
            _life = maxLife;
            _material.SetFloat(Saturation, 0);
            _material.SetColor(Color1, color);
            _direction = (Direction)Random.Range(0, 2);
            _nextRoamTime = roamDurationRange.GetRandomValue();
        }
    }
}
