using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Barracuda
{

	public class Streamer : MonoBehaviour
	{
		public void Feed<T>(IStreamee<T> streamee)
		{

		}

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