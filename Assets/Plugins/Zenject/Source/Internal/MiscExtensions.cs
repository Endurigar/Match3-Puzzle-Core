using System;
using System.Collections.Generic;
using System.Linq;

namespace ModestTree
{
    public static class MiscExtensions
    {
        public static string Fmt(this string s, params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg == null)
                {
                    args[i] = "NULL";
                }
                else if (arg is Type)
                {
                    args[i] = ((Type)arg).PrettyName();
                }
            }

            return String.Format(s, args);
        }

        public static int IndexOf<T>(this IList<T> list, T item)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (object.Equals(list[i], item))
                {
                    return i;
                }
            }

            return -1;
        }

        public static string Join(this IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values.ToArray());
        }

        public static void AllocFreeAddRange<T>(this IList<T> list, IList<T> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                list.Add(items[i]);
            }
        }

        public static void RemoveWithConfirm<T>(this IList<T> list, T item)
        {
            bool removed = list.Remove(item);
            Assert.That(removed);
        }

        public static void RemoveWithConfirm<T>(this LinkedList<T> list, T item)
        {
            bool removed = list.Remove(item);
            Assert.That(removed);
        }

        public static void RemoveWithConfirm<TKey, TVal>(this IDictionary<TKey, TVal> dictionary, TKey key)
        {
            bool removed = dictionary.Remove(key);
            Assert.That(removed);
        }

        public static void RemoveWithConfirm<T>(this HashSet<T> set, T item)
        {
            bool removed = set.Remove(item);
            Assert.That(removed);
        }

        public static TVal GetValueAndRemove<TKey, TVal>(this IDictionary<TKey, TVal> dictionary, TKey key)
        {
            TVal val = dictionary[key];
            dictionary.RemoveWithConfirm(key);
            return val;
        }
    }
}
