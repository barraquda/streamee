using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Barracuda
{
    public static partial class Stream
    {
        public static IStream<T> Branch<T>(IEnumerable<IStream<T>> streams)
        {
            return new BranchImpl<T>(streams);
        }

        public static IStream<T> Terminal<T>(T value)
        {
            return new TerminalImpl<T>(value);
        }

        private static class NoneCacher<T>
        {
            private static IStream<T> cache;
            public static IStream<T> Cache {
                get { return cache = cache ?? new NoneImpl<T>(); }
            }
        }

        public static IStream<T> None<T>()
        {
            return NoneCacher<T>.Cache;
        }

        private static IStream<Unit> unitEmpty = new TerminalImpl<Unit>(Unit.Default);
        public static IStream<Unit> UnitEmpty {
            get { return unitEmpty; }
        }

        public static IEnumerable<IStream<T>> Serialize<T>(IEnumerable<IStream<T>> streams)
        {
            return Branch(streams).GetEnumerable();
        }

        public static IStream<Unit> Wait(System.TimeSpan timeSpan)
        {
            return new BranchImpl<Unit>(WaitEnumerable(timeSpan));
        }

        private static IEnumerable<IStream<Unit>> WaitEnumerable(System.TimeSpan timeSpan)
        {
            var time = Time.time;
            var seconds = (float)timeSpan.TotalSeconds;
            while (Time.time - time < seconds) {
                yield return unitEmpty;
            }
        }

        public static IStream<Unit> Loop(System.TimeSpan timeSpan, System.Action<float> callback)
        {
            return new BranchImpl<Unit>(LoopEnumerable(timeSpan, callback));
        }

        private static IEnumerable<IStream<Unit>> LoopEnumerable(System.TimeSpan timeSpan, System.Action<float> callback)
        {
            var time = Time.time;
            var seconds = (float)timeSpan.TotalSeconds;
            while (Time.time - time < seconds) {
                callback(Time.time - time);
                yield return unitEmpty;
            }
            callback(seconds);
            yield return unitEmpty;
        }

        public static IStream<WWW> FetchWWW(string url)
        {
            return new BranchImpl<WWW>(WWWEnumerable(url));
        }

        private static IEnumerable<IStream<WWW>> WWWEnumerable(string url)
        {
            var wwwFetcher = WWWFetcher.CreateInstance();
            wwwFetcher.Initialize(url);
            wwwFetcher.FetchAsync();
            while (!wwwFetcher.WWW.isDone) {
                yield return Stream.Terminal(wwwFetcher.WWW);
            }
            yield return Stream.Terminal(wwwFetcher.WWW);
        }

        public static IStream<V> Merge<T, U, V>(IStream<T> lhs, IStream<U> rhs, Func<T, U, V> converter)
        {
            var left = lhs.GetEnumerable();
            var right = rhs.GetEnumerable();
            return Stream.Branch(MergeEnumerable(left, right, converter));
        }

        public static IStream<Unit> MergeToUnit<T, U>(IStream<T> lhs, IStream<T> rhs)
        {
            var left = lhs.GetEnumerable();
            var right = lhs.GetEnumerable();
            return Stream.Branch(MergeEnumerable(left, right, (_1, _2) => Unit.Default));
        }

        private static IEnumerable<IStream<V>> MergeEnumerable<T, U, V>
        (IEnumerable<IStream<T>> lhs, IEnumerable<IStream<U>> rhs, Func<T, U, V> converter)
        {
            using (var left = lhs.GetEnumerator())
            using (var right = rhs.GetEnumerator()) {
                while (left.MoveNext() && right.MoveNext()) {
                    left.Current.SelectMany<V>(leftValue =>
                        right.Current.Select<V>(rightValue =>
                            converter(leftValue, rightValue)
                        )
                    );
                    yield return null;
                }
            }
        }

        /// <summary>
        /// WWWでWebからリソースを取得するゲームオブジェクト
        /// </summary>
        private class WWWFetcher : MonoBehaviour
        {
            private WWW www;
            private Coroutine coroutine;

            public WWW WWW { get { return www; } }

            public static WWWFetcher CreateInstance()
            {
                return new GameObject("WWWFetcher").AddComponent<WWWFetcher>();
            }

            /// <summary>
            /// 取得したいURLを渡して初期化
            /// </summary>
            public void Initialize(string url)
            {
                this.www = new WWW(url);
            }

            /// <summary>
            /// コネクション開始する
            /// ブロッキングしない
            /// </summary>
            public void FetchAsync()
            {
                coroutine = StartCoroutine(WWWEnumerator(www));
            }

            IEnumerator WWWEnumerator(WWW www)
            {
                yield return www;
                Destroy(gameObject);
            }

            void OnDestroy()
            {
                if (www != null) {
                    www.Dispose();
                }
                if (coroutine != null) {
                    StopCoroutine(coroutine);
                }
            }
        }
    }
}