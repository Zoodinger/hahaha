namespace Cyens.Pooling {
    // [Serializable]
    // public abstract class PoolInfo {
    //     public abstract PrefabPool BasePool { get; }
    // }
    //
    // [Serializable]
    // public class PoolInfo<T> : PoolInfo, IDisposable where T : Poolable {
    //     [SerializeField] private T prototype;
    //     [SerializeField] private int prespawn;
    //     private PrefabPool<T> _pool;
    //
    //     public PoolInfo(T prototype, int prespawn) {
    //         this.prototype = prototype;
    //         this.prespawn = prespawn;
    //     }
    //
    //     public PrefabPool<T> Pool {
    //         get {
    //             if (_pool is not { IsInitialized: true }) {
    //                 _pool = new PrefabPool<T>(prototype, prespawn);
    //             }
    //
    //             return _pool;
    //         }
    //     }
    //
    //     public override PrefabPool BasePool => Pool;
    //
    //     public void Dispose() {
    //         if (_pool is not { IsInitialized: true }) {
    //             return;
    //         }
    //
    //         _pool.Destroy();
    //     }
    // }
}
