using System;

namespace Barracuda
{
	public static partial class Streamee
	{
		private class LazyTerminalImpl<T> : IStreamee<T>
		{
			private Func<T> generator;

			private bool isGenerated;
			private T value;
			private T Value {
				get {
					if (!isGenerated) {
						isGenerated = true;
						value = generator();
					}
					return value;
				}
			}

			public LazyTerminalImpl(Func<T> generator)
			{
				this.generator = generator;
			}

			public IStreamee<U> Select<U>(System.Func<T, U> selector)
			{
				return new LazyTerminalImpl<U>(() => selector(Value));
			}

			public IStreamee<U> SelectMany<U>(System.Func<T, IStreamee<U>> selector)
			{
				return selector(Value);
			}

			public IStreamee<T> Where(System.Predicate<T> predicator)
			{
				if (predicator(Value)) {
					return this;
				} else {
					return Streamee.None<T>();
				}
			}

			public IStreamee<T> Do(System.Action<T> executor)
			{
				return new LazyTerminalImpl<T>(() => {
					executor(Value);
					return Value;
				});
			}

			public System.Collections.Generic.IEnumerator<IStreamee<T>> GetEnumerator()
			{
				yield return this;
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				yield return this;
			}
		}
	}
}
