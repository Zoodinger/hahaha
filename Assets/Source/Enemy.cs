using Hahaha.Pooling;
using Teo.AutoReference;
using UnityEngine;

namespace Hahaha {
    public class Enemy : Poolable {
        private static readonly int Saturation = Shader.PropertyToID("_Saturation");
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        [SerializeField, Get] private Rigidbody2D body;
        [SerializeField, Get] private new SpriteRenderer renderer;
        [SerializeField] private float maxLife = 1;
        [SerializeField] private float damagePerSecond = 0.25f;
        [SerializeField] private Color color = Color.white;
        private int _gasLayer;
        private float _life;
        private Material _material;

        private void Awake() {
            _gasLayer = LayerMask.NameToLayer("Gas");
            _material = renderer.material;
            _material.SetColor(Color1, color);

            OnGet();
        }

        private void OnTriggerStay2D(Collider2D other) {
            if (other.gameObject.layer == _gasLayer) {
                var gas = other.gameObject.GetComponent<Gas>();

                _life -= damagePerSecond * gas.Damage * Time.deltaTime;
                _material.SetFloat(Saturation, 1 - _life / maxLife);
            }
        }

        private void FixedUpdate() {
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
        }

        protected override void OnGet() {
            _life = maxLife;
            _material.SetFloat(Saturation, 0);
            _material.SetColor(Color1, color);
        }
    }
}
