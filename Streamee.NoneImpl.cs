using System;
using System.Collections.Generic;
using UnityEngine;

namespace Barracuda
{
    public static partial class Streamee
    {
        private class NoneImpl<T> : IStreamee<T>
        {
            public IEnumerable<IStreamee<T>> GetEnumerable()
            {
                yield return this;
            }

            public IStreamee<U> Select<U>(Func<T, U> selector)
            {
                return Streamee.None<U>();
            }

            public IStreamee<U> SelectMany<U>(Func<T, IStreamee<U>> selector)
            {
                return Streamee.None<U>();
            }

            public IStreamee<T> Where(Predicate<T> predicator)
            {
                return this;
            }

            public IStreamee<T> Do(Action<T> executor)
            {
                return this;
            }
        }
    }
}

