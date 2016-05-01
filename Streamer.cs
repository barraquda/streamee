using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Barracuda
{
	public class Streamer<T> : IDisposable
	{
		IStreamee<T> streamee;
		IEnumerator<IStreamee<T>> enumerator;

		public Streamer(IStreamee<T> streamee)
		{
			this.streamee = streamee;
		}

		public IStreamee<T> Feed()
		{
			if (enumerator == null) {
				enumerator = streamee.GetEnumerator();
			}
			enumerator.MoveNext();
			return enumerator.Current;
		}

		public void Dispose()
		{
			if (enumerator != null) {
				enumerator.Dispose();
			}
		}
	}
}