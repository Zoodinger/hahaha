using UnityEngine;

namespace Hahaha.Extensions {
    public static class ColliderExtensions {
        private static readonly int SolidLayerMask;

        static ColliderExtensions() {
            SolidLayerMask = LayerMask.GetMask("Solid");
        }

        public static bool IsGrounded(this Collider2D collider) {
            var bounds = collider.bounds;
            const float offset = 0.1f;
            var point1 = bounds.min + new Vector3(-offset, -offset);
            var point2 = new Vector3(bounds.max.x - offset, bounds.min.y + offset);
            var hit = Physics2D.OverlapArea(point1, point2, SolidLayerMask);
            return hit;
        }
    }
}
