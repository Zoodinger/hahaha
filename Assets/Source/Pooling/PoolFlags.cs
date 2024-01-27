using System;

namespace Cyens.Pooling {
    [Flags]
    public enum PoolFlags {
        None = 0,
        Initialized = 1 << 0,
        Pooled = 1 << 1,
        IsReadyToPool = 1 << 2,
    }

    public static class PoolFlagsExtensions {
        public static bool Has(this PoolFlags flags, PoolFlags flag) {
            return (flags & flag) == flag;
        }

        public static PoolFlags Without(this PoolFlags flags, PoolFlags with) {
            return flags & ~with;
        }

        public static PoolFlags With(this PoolFlags flags, PoolFlags with) {
            return flags | with;
        }

        public static void Set(this ref PoolFlags flags, PoolFlags set) {
            flags |= set;
        }

        public static void Remove(this ref PoolFlags flags, PoolFlags add) {
            flags &= ~add;
        }
    }
}
