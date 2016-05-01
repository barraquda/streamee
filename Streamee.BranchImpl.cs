using System;
using System.Collections.Generic;
using UnityEngine;

namespace Barracuda
{
	public static partial class Streamee
	{
		private class BranchImpl<T> : IStreamee<T>
		{
			public BranchImpl(IEnumerable<IStreamee<T>> streams)
			{
				this.streams = streams;
			}

			private IEnumerable<IStreamee<T>> streams;

			private IEnumerable<IStreamee<T>> cachedSerializedStreams;

			public IEnumerable<IStreamee<T>> GetEnumerable()
			{
				return cachedSerializedStreams = cachedSerializedStreams ?? CreateEnumerable();
			}

			private IEnumerable<IStreamee<T>> CreateEnumerable()
			{
				foreach (var stream in streams) {
					foreach (var childStream in stream.GetEnumerable()) {
						yield return childStream;
					}
				}
			}

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