using UnityEngine;

namespace Hahaha.System {
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static T _instance;
        private bool _isInstanceLoaded;

        public static bool IsInstanceLoaded => _instance != null && (_instance as Singleton<T>)!._isInstanceLoaded;

#if UNITY_EDITOR
        static Singleton() {
            var type = typeof(T);
            if (!type.IsSealed) {
                Debug.LogWarning($"{type.Name} should be a sealed class.");
            }

            if (!typeof(T).IsSubclassOf(typeof(Singleton<T>))) {
                Debug.LogError($"{type.Name} must inherit from Singleton<{type.Name}>");
            }
        }
#endif

        public static T Instance {
            get {
                if (_instance != null) {
                    return _instance;
                }

                _instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                if (_instance is not Singleton<T> singleton || singleton._isInstanceLoaded) {
                    return _instance;
                }

                singleton._isInstanceLoaded = true;
                singleton.OnInstanceInitialize();

                return _instance;
            }
        }

        protected void Awake() {
            if (_instance != null && _instance != this && _instance.GetType() == GetType()) {
                DestroyImmediate(_instance);
                Debug.LogWarningFormat($"Prior instance of {_instance.GetType()} destroyed");
            }

            _instance = this as T;

            if (_isInstanceLoaded) {
                return;
            }

            _isInstanceLoaded = true;
            OnInstanceInitialize();
        }

        protected virtual void OnInstanceInitialize() { }
    }
}
