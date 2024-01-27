using System.Collections.Generic;
using UnityEngine;

namespace Cyens.Pooling {
    public class PrefabPoolData : ScriptableObject {

        private static bool _isCreating;

        private static Transform _commonParent;
        private static Transform _defaultTransform;
        private readonly List<Component> _allItems = new();

        private readonly List<Component> _pool = new();

        private Transform _gatheredTransform;
        private Component _prototype;

        private void Awake() {
            if (_isCreating) {
                return;
            }

            Debug.LogError(
                $"{nameof(PrefabPoolData)} must be created only through the {nameof(Create)} static function"
            );
            Destroy(this);
        }

        private void OnDestroy() {
            if (_gatheredTransform != null) {
                Destroy(_gatheredTransform.gameObject);
            }
        }

        private void MakeParent(string parentName) {
            if (_commonParent == null) {
                var commonParent = new GameObject("Gathered");
                commonParent.SetActive(false);
                DontDestroyOnLoad(commonParent);
                _commonParent = commonParent.transform;
            }

            if (string.IsNullOrEmpty(parentName)) {
                parentName = "Pool";
            }

            var obj = new GameObject(parentName);
            _gatheredTransform = obj.transform;
            _gatheredTransform.SetParent(_commonParent, false);
        }

        public static PrefabPoolData Create(Component prototype, int preconstruct, string parentName = null) {
            _isCreating = true;
            var pool = CreateInstance<PrefabPoolData>();
            _isCreating = false;

            pool.MakeParent(parentName);

            pool._prototype = prototype;
            pool._pool.ReserveCapacity(preconstruct);
            pool._allItems.ReserveCapacity(preconstruct);

            for (var i = 0; i < preconstruct; ++i) {
                var obj = Instantiate(prototype, pool._gatheredTransform, true);
                pool._allItems.Add(obj);
                pool._pool.Add(obj);
            }

#if UNITY_EDITOR
            if (pool._pool.Count > 0) {
                pool._gatheredTransform.gameObject.hideFlags &= ~HideFlags.HideInHierarchy;
            } else {
                pool._gatheredTransform.gameObject.hideFlags |= HideFlags.HideInHierarchy;
            }
#endif

            return pool;
        }

        public void RePool(Component obj) {
            if (obj == null) {
                return;
            }

            var transform = obj.transform;

            if (transform.parent == _gatheredTransform) {
                return;
            }
#if UNITY_EDITOR
            if (_pool.Count == 0) {
                _gatheredTransform.gameObject.hideFlags &= ~HideFlags.HideInHierarchy;
            }
#endif

            transform.SetParent(_gatheredTransform, false);
            _pool.Add(obj);
        }

        public Component Get(Transform parentTransform = null, Vector3 position = default) {
            if (_pool.TryPop(out var outValue)) {
                var newTransform = outValue.transform;
                newTransform.SetParent(parentTransform, false);
                outValue.gameObject.SetActive(true);
                newTransform.position = position;

#if UNITY_EDITOR
                if (_pool.Count == 0) {
                    _gatheredTransform.gameObject.hideFlags |= HideFlags.HideInHierarchy;
                }
#endif
                return outValue;
            }

            // No objects left in the pool, we need to instantiate a new one.
            outValue = Instantiate(_prototype, parentTransform, false);
            outValue.transform.position = position;

            _allItems.Add(outValue);

            return outValue;
        }
    }
}
