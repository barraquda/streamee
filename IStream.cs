using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Barracuda
{
    public interface IStream<T>
    {
        IEnumerable<IStream<T>> GetEnumerable();
        IStream<U> Select<U>(Func<T, U> selector);
        IStream<U> SelectMany<U>(Func<T, IStream<U>> selector);
        IStream<T> Where(Predicate<T> predicator);
        IStream<T> Do(Action<T> executor);
    }

    public static class IStreamExtensions
    {
        public static IStream<Unit> AsUnitStream<T>(this IStream<T> stream)
        {
            return stream.Select(_ => Unit.Default);
        }

        public static void Run<T>(this IStream<T> stream, Action<T> runAction)
        {
            var runner = new GameObject("StreamRunner").AddComponent<StreamRunner>();
            runner.Run(stream, runAction);
        }

        private class StreamRunner : MonoBehaviour
        {
            public void Run<T>(IStream<T> stream, Action<T> runAction)
            {
                var enumerator = stream.GetEnumerable().GetEnumerator();
                StartCoroutine(RunEnumerator(enumerator, runAction));
            }

            IEnumerator RunEnumerator<T>(IEnumerator<IStream<T>> enumerator, Action<T> runAction)
            {
                while (enumerator.MoveNext()) {
                    enumerator.Current.Do(runAction);
                    yield return null;
                }
                Destroy(gameObject);
            }
        }
    }
}