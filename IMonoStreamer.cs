using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

using Barracuda.Internal;

namespace Barracuda
{
	public interface IMonoStreamer : IDisposable
	{
		void Stop();
		void Start();
	}
}