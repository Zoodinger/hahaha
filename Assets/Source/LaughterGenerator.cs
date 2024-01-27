using Cyens.Pooling;
using Hahaha.System;
using UnityEngine;

namespace Hahaha {
    public sealed class LaughterGenerator : Singleton<LaughterGenerator> {
        [SerializeField] private PrefabPool<HaWord> ha1;
        [SerializeField] private PrefabPool<HaWord> ha2;
        [SerializeField] private PrefabPool<HaWord> ha3;

        public static void MakeHaAt(Vector2 point) {
            var random = Random.Range(0, 3);
            switch (random) {
                case 0:
                    Instance.ha1.Get(null, point);
                    break;
                case 1:
                    Instance.ha2.Get(null, point);
                    break;
                case 2:
                    Instance.ha3.Get(null, point);
                    break;
            }
        }
    }
}
