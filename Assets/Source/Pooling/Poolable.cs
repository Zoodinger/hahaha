using Cyens.Pooling;
using UnityEngine;

namespace Hahaha.Pooling {
    public abstract class Poolable : MonoBehaviour {
        private PrefabPool _parent;
        // private bool _isReadyToPool;
        // private bool _isPooled = true;

        private PoolFlags _poolFlags;

        public PoolFlags PoolFlags => _poolFlags;
        //
        // public bool IsReadyToPool => _isReadyToPool;
        //
        // public bool IsPooled => _isPooled;

        public PrefabPool ParentPool => _parent;

        public void RePool() {
            if (_poolFlags.Has(PoolFlags.IsReadyToPool)) {
                Debug.LogError("Cannot pool an object that's already pooled.");
                return;
            }

            if (_parent.InnerType != GetType()) {
                Debug.LogError("Invalid pooled object type.");
                return;
            }

            _poolFlags.Set(PoolFlags.IsReadyToPool);
            if (!_parent.RePool(this)) {
                return;
            }

            OnRePool();

            _poolFlags.Remove(PoolFlags.IsReadyToPool);
            _poolFlags.Set(PoolFlags.Pooled);
        }

        public void InitFromPool(PrefabPool pool) {
            if (!_poolFlags.Has(PoolFlags.Pooled) && _poolFlags.Has(PoolFlags.Initialized)) {
                Debug.LogError("Object is already initialized.");
                return;
            }

            if (pool.InnerType != GetType()) {
                Debug.LogError("Invalid pooled object type.");
                return;
            }

            if (!pool.IsReadyToPool) {
                Debug.LogError("Cannot init from outside a pool.");
                return;
            }

            _parent = pool;

            _poolFlags.Set(PoolFlags.Initialized);
            _poolFlags.Remove(PoolFlags.Pooled);
            OnGet();
        }

        protected virtual void OnGet() { }

        protected virtual void OnRePool() { }
    }
}
