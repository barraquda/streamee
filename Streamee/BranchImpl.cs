using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Barracuda
{
	public static partial class Streamee
	{
		private class BranchImpl<T> : IStreamee<T>
		{
			IEnumerator<IStreamee<T>> IEnumerable<IStreamee<T>>.GetEnumerator()
			{
				foreach (var streamee in streams) {
					var enumerator = streamee.GetEnumerator();
					while (enumerator.MoveNext()) {
						yield return enumerator.Current;
					}
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				var enumerator = ((IEnumerable<IStreamee<T>>)this).GetEnumerator();
				while (enumerator.MoveNext()) {
					yield return enumerator.Current;
				}
			}

			public BranchImpl(IEnumerable<IStreamee<T>> streams)
			{
				this.streams = streams;
			}

			private IEnumerable<IStreamee<T>> streams;

			private IEnumerable<IStreamee<T>> cachedSerializedStreams;

			public IStreamee<U> Select<U>(Func<T, U> selector)
			{
				return Streamee.Branch(SelectEnumerable(selector));
			}

			private IEnumerable<IStreamee<U>> SelectEnumerable<U>(Func<T, U> selector)
			{
				foreach (var stream in streams) {
					yield return stream.Select(selector);
				}
			}

			public IStreamee<U> SelectMany<U>(Func<T, IStreamee<U>> selector)
			{
				return Streamee.Branch(SelectManyEnumerable(selector));
			}

			private IEnumerable<IStreamee<U>> SelectManyEnumerable<U>(Func<T, IStreamee<U>> selector)
			{
				foreach (var stream in streams) {
					yield return stream.SelectMany(selector);
				}
			}

			public IStreamee<T> Where(Predicate<T> predicator)
			{
				return Streamee.Branch(WhereEnumerable(predicator));
			}

			private IEnumerable<IStreamee<T>> WhereEnumerable(Predicate<T> predicator)
			{
				foreach (var stream in streams) {
					yield return stream.Where(predicator);
				}
			}

			public IStreamee<T> Do(Action<T> executor)
			{
				return Streamee.Branch(DoEnumerable(executor));
			}

			private IEnumerable<IStreamee<T>> DoEnumerable(Action<T> executor)
			{
				foreach (var stream in streams) {
					yield return stream.Do(executor);
				}
			}
		}
	}
}