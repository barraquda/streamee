using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

using Barracuda.Internal;

namespace Barracuda
{
	public class Streamer<T> : IStreamer
	{
		IStreamee<T> streamee;
		IEnumerator<IStreamee<T>> enumerator;
		Action<T> subscriber;

		IDisposable monoStreamerDisposable;

		public Streamer(IStreamee<T> streamee)
		{
			this.streamee = streamee;
		}

		public IMonoStreamer Run()
		{
			return Run(null);
		}

		public IMonoStreamer Run(Action<T> subscriber)
		{
			if (monoStreamerDisposable != null) {
				monoStreamerDisposable.Dispose();
			}

			this.subscriber = subscriber;
			var gameObject = new GameObject("MonoStreamer");
			var monoStreamer = gameObject.AddComponent<MonoStreamer>();
			monoStreamer.Streamer = this;

			monoStreamerDisposable = monoStreamer;

			return monoStreamer;
		}

		public bool Feed()
		{
			return Feed(null);
		}

		public bool Feed(Action<T> action)
		{
			if (enumerator == null) {
				enumerator = streamee.GetEnumerator();
			}
			var continuing = enumerator.MoveNext();
			if (continuing) {
				if (action != null) {
					enumerator.Current.Do(action);
				}
				if (subscriber != null) {
					enumerator.Current.Do(subscriber);
				}
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