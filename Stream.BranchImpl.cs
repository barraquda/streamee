using System;
using System.Collections.Generic;
using UnityEngine;

namespace Barracuda
{
    public static partial class Stream
    {
        private class BranchImpl<T> : IStream<T>
        {
            public BranchImpl(IEnumerable<IStream<T>> streams)
            {
                this.streams = streams;
            }

            private IEnumerable<IStream<T>> streams;

            private IEnumerable<IStream<T>> cachedSerializedStreams;

            public IEnumerable<IStream<T>> GetEnumerable()
            {
                return cachedSerializedStreams = cachedSerializedStreams ?? CreateEnumerable();
            }

            private IEnumerable<IStream<T>> CreateEnumerable()
            {
                foreach (var stream in streams) {
                    foreach (var childStream in stream.GetEnumerable()) {
                        yield return childStream;
                    }
                }
            }

            public IStream<U> Select<U>(Func<T, U> selector)
            {
                return Stream.Branch(SelectEnumerable(selector));
            }

            private IEnumerable<IStream<U>> SelectEnumerable<U>(Func<T, U> selector)
            {
                foreach (var stream in streams) {
                    yield return stream.Select(selector);
                }
            }

            public IStream<U> SelectMany<U>(Func<T, IStream<U>> selector)
            {
                return Stream.Branch(SelectManyEnumerable(selector));
            }

            private IEnumerable<IStream<U>> SelectManyEnumerable<U>(Func<T, IStream<U>> selector)
            {
                foreach (var stream in streams) {
                    yield return stream.SelectMany(selector);
                }
            }

            public IStream<T> Where(Predicate<T> predicator)
            {
                return Stream.Branch(WhereEnumerable(predicator));
            }

            private IEnumerable<IStream<T>> WhereEnumerable(Predicate<T> predicator)
            {
                foreach (var stream in streams) {
                    yield return stream.Where(predicator);
                }
            }

            public IStream<T> Do(Action<T> executor)
            {
                return Stream.Branch(DoEnumerable(executor));
            }

            private IEnumerable<IStream<T>> DoEnumerable(Action<T> executor)
            {
                foreach (var stream in streams) {
                    yield return stream.Do(executor);
                }
            }
        }
    }
}