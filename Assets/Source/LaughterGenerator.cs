using System.Collections.Generic;
using System.Linq;
using Cyens.Pooling;
using Hahaha.System;
using UnityEngine;

namespace Hahaha {
    public sealed class LaughterGenerator : Singleton<LaughterGenerator> {
        [SerializeField] private PrefabPool<HaWord> ha1;
        [SerializeField] private PrefabPool<HaWord> ha2;
        [SerializeField] private PrefabPool<HaWord> ha3;
        [SerializeField] private PrefabPool<Spirit> spirit;

        private readonly HashSet<Enemy> _curedEnemies = new();

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

        public static void SpawnSpirit(Enemy enemy) {
            var spirit = Instance.spirit.Get(null, enemy.Collider.bounds.center);
            spirit.Init(enemy);
        }

        public static void AddCured(Enemy enemy) {
            Instance._curedEnemies.Add(enemy);
        }

        public static void RemoveCured(Enemy enemy) {
            Instance._curedEnemies.Remove(enemy);
        }

        public static Enemy GetClosestCuredTo(Enemy enemy) {
            var min = Mathf.Infinity;
            Enemy selected = null;
            foreach (var e in Instance._curedEnemies) {
                if (e == enemy) {
                    continue;
                }

                var distance = Vector2.Distance(enemy.Collider.bounds.center, e.Collider.bounds.center);
                if (distance < min) {
                    min = distance;
                    selected = e;
                }
            }

            return selected;
        }
    }
}
