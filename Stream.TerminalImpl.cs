using System;
using System.Collections.Generic;
using UnityEngine;

namespace Barracuda
{
    public static partial class Stream
    {
        private class TerminalImpl<T> : IStream<T>
        {
            public TerminalImpl(T value)
            {
                this.value = value;
            }

            private T value;
            public IEnumerable<IStream<T>> GetEnumerable()
            {
                yield return this;
            }

            public IStream<U> Select<U>(Func<T, U> selector)
            {
                return Stream.Terminal(selector(value));
            }

            public IStream<U> SelectMany<U>(Func<T, IStream<U>> selector)
            {
                return selector(value);
            }

            public IStream<T> Where(Predicate<T> predicator)
            {
                if (predicator(value)) {
                    return this;
                } else {
                    return Stream.None<T>();
                }
            }

            public IStream<T> Do(Action<T> executor)
            {
                executor(value);
                return this;
            }
        }
    }
}