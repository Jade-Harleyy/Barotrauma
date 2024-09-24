using System;

namespace Barotrauma
{
    static class PredicateExtensions
    {
        public static bool Evaluate<T>(this Predicate<T> predicate, T obj) => predicate == null || predicate(obj);
    }
}
