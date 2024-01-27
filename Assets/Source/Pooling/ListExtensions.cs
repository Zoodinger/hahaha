using System;
using System.Collections.Generic;

namespace Cyens.Pooling {
    public static class ListExtensions {
        public static void ReverseNoAlloc<T>(this List<T> list) {
            var count = list.Count;
            for (var i = 0; i < count / 2; i++) {
                (list[i], list[count - i - 1]) = (list[count - i - 1], list[i]);
            }
        }

        public static T Pop<T>(this List<T> list) {
            var lastIndex = list.Count - 1;
            var value = list[lastIndex];
            list.RemoveAt(lastIndex);
            return value;
        }

        public static bool TryPop<T>(this List<T> list, out T value) {
            if (list.Count == 0) {
                value = default;
                return false;
            }

            value = list.Pop();
            return true;
        }

        public static T Pop<T>(this List<T> list, int index) {
            var value = list[index];
            list.RemoveAt(index);
            return value;
        }

        public static void ReserveCapacity<T>(this List<T> list, int capacity) {
            if (list.Capacity < capacity) {
                list.Capacity = capacity;
            }
        }

        public static List<T> SplitInto<T>(this List<T> list, List<T> splitInto, Predicate<T> predicate) {
            splitInto.Clear();
            for (var i = list.Count - 1; i >= 0 && i < list.Count; --i) {
                var element = list[i];
                if (!predicate(element)) {
                    continue;
                }

                list.RemoveAt(++i);
                splitInto.Add(element);
            }

            return splitInto;
        }

        public static List<T> WhereInto<T>(this List<T> list, List<T> into, Predicate<T> predicate) {
            // Garbage-free Where variant (depending on predicate) that uses an output list as an argument.
            into.Clear();
            foreach (var element in list) {
                if (predicate(element)) {
                    into.Add(element);
                }
            }

            return into;
        }

        public static List<TOut> SelectInto<T, TOut>(this List<T> list, List<TOut> into, Func<T, TOut> func) {
            // Garbage-free Where variant (depending on selection function) that uses an output list as an argument.
            into.Clear();
            foreach (var element in list) {
                into.Add(func(element));
            }

            return into;
        }

        public static ListView<T> AsView<T>(this List<T> list) => new(list);

        public static bool RemoveLast<T>(this List<T> list, T actor) {
            for (var i = list.Count - 1; i >= 0; --i) {
                if (Equals(list[i], actor)) {
                    list.RemoveAt(i);

                    return true;
                }
            }

            return false;
        }
    }


    // Struct version of a read only list which produces no garbage
    public readonly struct ListView<T> {
        private readonly List<T> _innerList;

        public ListView(List<T> list) {
            _innerList = list;
        }

        public Enumerable GetEnumerator() => new(_innerList);

        public T this[int key] => _innerList[key];

        public struct Enumerable {
            private readonly List<T> _innerList;
            private int _index;

            public Enumerable(List<T> innerList) {
                _innerList = innerList;
                _index = 0;
            }

            public T Current => _innerList[_index - 1];

            public bool MoveNext() {
                _index++;
                return _innerList != null && _innerList.Count >= _index;
            }

            public void Reset() {
                _index = 0;
            }
        }
    }
}
