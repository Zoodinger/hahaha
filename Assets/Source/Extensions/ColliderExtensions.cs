using Hahaha.System;
using UnityEngine;

namespace Hahaha.Extensions {
    public static class ColliderExtensions {
        private const float Offset = 0.15f;
        private const float RayDistance = Offset * 3;

        public static bool IsGrounded(this Collider2D collider, LayerMask mask) {
            var bounds = collider.bounds;
            var point1 = bounds.min + new Vector3(-Offset, -Offset);
            var point2 = new Vector3(bounds.max.x - Offset, bounds.min.y + Offset);
            var hit = Physics2D.OverlapArea(point1, point2, mask);
            return hit;
        }

        public static bool IsGrounded(this Collider2D collider) {
            return collider.IsGrounded(Utils.SolidMask);
        }

        public static Vector2 GetTop(this Collider2D collider) {
            var bounds = collider.bounds;
            return new Vector2(bounds.center.x, bounds.center.y + bounds.extents.y);
        }

        public static Vector2 GetBottom(this Collider2D collider) {
            var bounds = collider.bounds;
            return new Vector2(bounds.center.x, bounds.center.y - bounds.extents.y);
        }

        public static bool IsGroundedLeft(this Collider2D collider, float offset = Offset) {
            return collider.IsGroundedLeft(Utils.SolidMask, offset);
        }

        public static bool IsGroundedRight(this Collider2D collider, float offset = Offset) {
            return collider.IsGroundedRight(Utils.SolidMask, offset);
        }

        public static bool IsGroundedLeft(this Collider2D collider, LayerMask layerMask, float offset = Offset) {
            var bounds = collider.bounds;

            var point = new Vector2(bounds.min.x - offset, bounds.min.y + Offset);
            var hit = Physics2D.Raycast(point, Vector2.down, RayDistance, layerMask);

            return hit.collider != null;
        }

        public static bool IsGroundedRight(this Collider2D collider, LayerMask layerMask, float offset = Offset) {
            var bounds = collider.bounds;
            var point = new Vector2(bounds.max.x + offset, bounds.min.y + Offset);
            var hit = Physics2D.Raycast(point, Vector2.down, RayDistance, layerMask);
            return hit.collider != null;
        }
    }
}
