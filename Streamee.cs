using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Barracuda
{
	public static partial class Streamee
	{
		public static IStreamee<T> Branch<T>(IEnumerable<IStreamee<T>> streams)
		{
			return new BranchImpl<T>(streams);
		}

		public static IStreamee<T> Terminal<T>(T value)
		{
			return new TerminalImpl<T>(value);
		}

		private static class NoneCacher<T>
		{
			private static IStreamee<T> cache;

			public static IStreamee<T> Cache {
				get { return cache = cache ?? new NoneImpl<T>(); }
			}
		}

		public static IStreamee<T> None<T>()
		{
			return NoneCacher<T>.Cache;
		}

		private static IStreamee<Unit> unitEmpty = new TerminalImpl<Unit>(Unit.Default);

		public static IStreamee<Unit> UnitEmpty {
			get { return unitEmpty; }
		}

		public static IEnumerable<IStreamee<T>> Serialize<T>(IEnumerable<IStreamee<T>> streams)
		{
			var enumerator = Branch(streams).GetEnumerator();
			while (enumerator.MoveNext()) {
				yield return enumerator.Current;
			}
		}

		public static IStreamee<T> ToStreamee<T>(this IEnumerable<IStreamee<T>> enumerable)
		{
			return Branch(enumerable);
		}

		public static IStreamee<Unit> Wait(System.TimeSpan timeSpan)
		{
			return new BranchImpl<Unit>(WaitEnumerable(timeSpan));
		}

		private static IEnumerable<IStreamee<Unit>> WaitEnumerable(System.TimeSpan timeSpan)
		{
			var time = Time.time;
			var seconds = (float)timeSpan.TotalSeconds;
			while (Time.time - time < seconds) {
				yield return unitEmpty;
			}
		}

		public static IStreamee<Unit> Loop(System.TimeSpan timeSpan, System.Action<float> callback)
		{
			return new BranchImpl<Unit>(LoopEnumerable(timeSpan, callback));
		}

		private static IEnumerable<IStreamee<Unit>> LoopEnumerable(System.TimeSpan timeSpan, System.Action<float> callback)
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

		public static IStreamee<WWW> FetchWWW(string url)
		{
			return new BranchImpl<WWW>(WWWEnumerable(url));
		}

		private static IEnumerable<IStreamee<WWW>> WWWEnumerable(string url)
		{
			var wwwFetcher = WWWFetcher.CreateInstance();
			wwwFetcher.Initialize(url);
			wwwFetcher.FetchAsync();
			while (!wwwFetcher.WWW.isDone) {
				yield return Streamee.Terminal(wwwFetcher.WWW);
			}
			yield return Streamee.Terminal(wwwFetcher.WWW);
		}

		public static IStreamee<V> Merge<T, U, V>(IStreamee<T> lhs, IStreamee<U> rhs, Func<T, U, V> converter)
		{
			return Streamee.Branch(MergeEnumerable(lhs, rhs, converter));
		}

		public static IStreamee<Unit> MergeToUnit<T, U>(IStreamee<T> lhs, IStreamee<T> rhs)
		{
			return Streamee.Branch(MergeEnumerable(lhs, rhs, (_1, _2) => Unit.Default));
		}

		private static IEnumerable<IStreamee<V>> MergeEnumerable<T, U, V>(IStreamee<T> left, IStreamee<U> right, Func<T, U, V> converter)
		{
			using(var lhs = left.GetEnumerator())
			using(var rhs = right.GetEnumerator())
			while (lhs.MoveNext() && rhs.MoveNext()) {
				yield return lhs.Current.SelectMany<V>(leftValue => rhs.Current.Select<V>(rightValue => converter(leftValue, rightValue)));
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