using System;
using System.Collections.Generic;
using UnityEngine;

namespace Barracuda
{
    public static partial class Stream
    {
        private class NoneImpl<T> : IStream<T>
        {
            public IEnumerable<IStream<T>> GetEnumerable()
            {
                yield return this;
            }

            public IStream<U> Select<U>(Func<T, U> selector)
            {
                return Stream.None<U>();
            }

            public IStream<U> SelectMany<U>(Func<T, IStream<U>> selector)
            {
                return Stream.None<U>();
            }

            public IStream<T> Where(Predicate<T> predicator)
            {
                return this;
            }

            public IStream<T> Do(Action<T> executor)
            {
                return this;
            }
        }
    }
}

