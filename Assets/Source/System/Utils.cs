using UnityEngine;

namespace Hahaha.System {
    public static class Utils {
        public static readonly int SolidLayer;
        public static readonly int SolidMask;

        static Utils() {
            SolidMask = LayerMask.GetMask("Solid");
            SolidLayer = LayerMask.NameToLayer("Solid");
        }
    }
}
