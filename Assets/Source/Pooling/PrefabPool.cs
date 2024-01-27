using System;
using Hahaha.Pooling;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cyens.Pooling {
    [Serializable]
    public abstract class PrefabPool : IDisposable {
        private PrefabPoolData _prefabPoolData;
        private readonly ReadyProxy _readyContext;

        protected abstract PrefabPoolData CreateData();

        public virtual Type InnerType => typeof(Component);

        public void Initialize() {
            _prefabPoolData = Data;
        }

        protected PrefabPool() {
            _readyContext = new ReadyProxy(this);
        }

        protected IDisposable MakeReadyContext() {
            IsReadyToPool = true;
            return _readyContext;
        }

        public bool IsReadyToPool { get; private set; }


        private class ReadyProxy : IDisposable {
            private readonly PrefabPool _owner;

            public ReadyProxy(PrefabPool owner) {
                _owner = owner;
            }

            public void Dispose() {
                _owner.IsReadyToPool = false;
            }
        }

        protected PrefabPoolData Data {
            get {
                if (_prefabPoolData == null) {
                    _prefabPoolData = CreateData();
                }

                return _prefabPoolData;
            }
        }

        public void Dispose() {
            if (_prefabPoolData != null) {
                Object.Destroy(_prefabPoolData);
            }
        }

        public Component Get(Transform parent = null, Vector3 position = default) =>
            _prefabPoolData.Get(parent, position);

        public bool RePool(Poolable behaviour) {
            if (behaviour.ParentPool._prefabPoolData != _prefabPoolData) {
                Debug.LogError("Invalid pool target.");
                return false;
            }

            if (!behaviour.PoolFlags.Has(PoolFlags.IsReadyToPool) || behaviour.PoolFlags.Has(PoolFlags.Pooled)) {
                Debug.LogError("Object is not ready for pooling.");
                return false;
            }

            _prefabPoolData.RePool(behaviour);
            return true;
        }
    }

    [Serializable]
    public class PrefabPool<T> : PrefabPool where T : Poolable {
        [SerializeField] private T prototype;
        [SerializeField] private int preconstruct;
        [SerializeField] private bool debug;

        public override Type InnerType => typeof(T);

        public bool IsInitialized => Data != null;

        protected override PrefabPoolData CreateData() {
            return PrefabPoolData.Create(prototype, preconstruct, typeof(T).Name);
        }

        public void RePool(T obj) {
            var data = Data;
            if (data == null) {
                return;
            }

            using (MakeReadyContext()) {
                #if UNITY_EDITOR
                if (debug) {
                    Object.Destroy(obj);
                } else {
                    data.RePool(obj);
                }
                #else
                data.RePool(obj);
                #endif

            }
        }

        private T GetFromData(Transform parent, Vector3 position) => (T)Data.Get(parent, position);

        public new T Get(Transform parent = null, Vector3 position = default) {
            var instance = GetFromData(parent, position);

            using (MakeReadyContext()) {
                instance.InitFromPool(this);
            }

            return instance;
        }

        public T Get(string name, Transform parent = null, Vector3 position = default) {
            var instance = GetFromData(parent, position);
            instance.name = name;

            using (MakeReadyContext()) {
                instance.InitFromPool(this);
            }

            return instance;
        }

        public T GetLocal(Transform parent = null, Vector3 position = default) {
            var instance = GetFromData(parent, default);
            instance.transform.localPosition = position;

            using (MakeReadyContext()) {
                instance.InitFromPool(this);
            }

            return instance;
        }

        public T GetLocal(string name, Transform parent = null, Vector3 position = default) {
            var instance = GetFromData(parent, default);
            instance.name = name;
            instance.transform.localPosition = position;
            using (MakeReadyContext()) {
                instance.InitFromPool(this);
            }

            return instance;
        }

        protected bool Equals(PrefabPool<T> other) => Equals(Data, other.Data);

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            return obj.GetType() == GetType() && Equals((PrefabPool<T>)obj);
        }

        public override int GetHashCode() => Data != null ? Data.GetHashCode() : 0;
    }
}
