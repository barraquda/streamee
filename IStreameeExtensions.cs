using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Barracuda
{

	public static class IStreameeExtensions
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
	}
}