using System;
using System.Collections.Generic;
using UnityEngine;

namespace Barracuda
{
    public static partial class Streamee
    {
        private class TerminalImpl<T> : IStreamee<T>
        {
            public TerminalImpl(T value)
            {
                this.value = value;
            }

            private T value;
            public IEnumerable<IStreamee<T>> GetEnumerable()
            {
                yield return this;
            }

            public IStreamee<U> Select<U>(Func<T, U> selector)
            {
                return Streamee.Terminal(selector(value));
            }

            public IStreamee<U> SelectMany<U>(Func<T, IStreamee<U>> selector)
            {
                return selector(value);
            }

            public IStreamee<T> Where(Predicate<T> predicator)
            {
                if (predicator(value)) {
                    return this;
                } else {
                    return Streamee.None<T>();
                }
            }

            public IStreamee<T> Do(Action<T> executor)
            {
                executor(value);
                return this;
            }
        }
    }
}