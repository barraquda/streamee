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

		public bool Feed(Action<T> action = null)
		{
			if (enumerator == null) {
				enumerator = streamee.GetEnumerator();
			}
			var continuing = enumerator.MoveNext();
			if (continuing && action != null) {
				enumerator.Current.Do(action);
			}
			return continuing;
		}

		public void Dispose()
		{
			if (enumerator != null) {
				enumerator.Dispose();
			}
		}
	}
}