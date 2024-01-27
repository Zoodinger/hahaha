using UnityEngine;

namespace Hahaha.Extensions {
    public static class VectorExtensions {
        public static Vector2 To2D(this Vector3 vector) {
            return vector;
        }

        public static Vector2 Decelerated(this Vector2 velocity, float amount) {
            var magnitude = velocity.magnitude;
            magnitude = Mathf.Max(0, magnitude - amount * Time.deltaTime);
            return velocity.normalized * magnitude;
        }

        public static Vector2 WithY(this Vector2 vector, float y) => new(vector.x, y);

        public static Vector2 WithX(this Vector2 vector, float x) => new(x, vector.y);

        public static float GetRandomValue(this Vector2 vector) {
            return Random.Range(vector.x, vector.y);
        }
    }
}
