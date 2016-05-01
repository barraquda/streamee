using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Barracuda
{
	public interface IStreamee<T>
	{
		IEnumerable<IStreamee<T>> GetEnumerable();

		IStreamee<U> Select<U>(Func<T, U> selector);

		IStreamee<U> SelectMany<U>(Func<T, IStreamee<U>> selector);

		IStreamee<T> Where(Predicate<T> predicator);

		IStreamee<T> Do(Action<T> executor);
	}

	public static class IStreamExtensions
	{
		public static IStreamee<Unit> AsUnitStream<T>(this IStreamee<T> stream)
		{
			return stream.Select(_ => Unit.Default);
		}

		public static void Run<T>(this IStreamee<T> stream, Action<T> runAction)
		{
			var runner = new GameObject("StreamRunner").AddComponent<Streamer>();
			runner.Run(stream, runAction);
		}

		private class Streamer : MonoBehaviour
		{
			public void Run<T>(IStreamee<T> stream, Action<T> runAction)
			{
				var enumerator = stream.GetEnumerable().GetEnumerator();
				StartCoroutine(RunEnumerator(enumerator, runAction));
			}

			IEnumerator RunEnumerator<T>(IEnumerator<IStreamee<T>> enumerator, Action<T> runAction)
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