using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Barracuda.Internal
{
	public interface IStreamer : IDisposable
	{
		bool Feed();
	}
}