using Hahaha.Extensions;
using Hahaha.Pooling;
using Hahaha.System;
using Teo.AutoReference;
using UnityEngine;

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
        [SerializeField] private float gravityScale = 3;

        [SerializeField] private Vector2 _velocity;

        private int _gasLayer;
        private float _life;
        private Material _material;

        private int _isTakingDamage = 0;

        private void Awake() {
            _gasLayer = LayerMask.NameToLayer("Gas");
            _material = renderer.material;
            _material.SetColor(Color1, color);

            OnGet();
        }


        private Timer _timer;

        private void Update() {
            if (_isTakingDamage == 0) {
                _velocity.y += Physics2D.gravity.y * Time.deltaTime * gravityScale;
                _velocity.y = Mathf.Max(-10, _velocity.y);
                return;
            }

            var isGrounded = collider.IsGrounded();

            if (isGrounded) {
                _velocity.y = laughForce;
            } else {
                _velocity.y += Physics2D.gravity.y * Time.deltaTime * gravityScale;
                _velocity.y = Mathf.Max(-10, _velocity.y);
            }
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

        private void FixedUpdate() {
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
            body.MovePosition(body.position + _velocity * Time.fixedDeltaTime);
        }

        protected override void OnGet() {
            _life = maxLife;
            _material.SetFloat(Saturation, 0);
            _material.SetColor(Color1, color);
        }
    }
}
