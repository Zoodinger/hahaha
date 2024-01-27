using System.Collections.Generic;
using UnityEngine;

namespace Hahaha.Extensions {
    public static class CollectionExtensions {
        public static T GetRandom<T>(this IReadOnlyList<T> collection, ref int index) {
            var count = collection.Count;
            if (count == 0) {
                index = -1;
                return default;
            }

            if (count == 1) {
                index = 0;
                return collection[0];
            }

            var previous = index;
            do {
                index = Random.Range(0, collection.Count);
            } while (index == previous);

            return collection[index];
        }


        public static T GetRandom<T>(this IReadOnlyList<T> collection) {
            var count = collection.Count;
            if (count == 0) {
                return default;
            }

            var index = Random.Range(0, collection.Count);
            return collection[index];
        }

        public static bool TryGetRandom<T>(this IReadOnlyList<T> collection, out T result) {
            var count = collection.Count;
            if (count == 0) {
                result = default;
                return false;
            }

            var index = Random.Range(0, collection.Count);
            result = collection[index];
            return true;
        }

        public static bool TryGetRandom<T>(this IReadOnlyList<T> collection, out T result, ref int index) {
            var count = collection.Count;
            if (count == 0) {
                index = -1;
                result = default;
                return false;
            }

            if (count == 1) {
                result = collection[0];
                index = 0;
                return true;
            }

            var previous = index;
            do {
                index = Random.Range(0, collection.Count);
            } while (index == previous);

            result = collection[index];
            return true;
        }
    }
}
