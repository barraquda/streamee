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
}