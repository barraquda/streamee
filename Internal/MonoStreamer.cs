using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

using Barracuda.Internal;

namespace Barracuda.Internal
{
	public class MonoStreamer : MonoBehaviour, IMonoStreamer
	{
		public IStreamer Streamer { get; set; }

		void Update()
		{
			if (!Streamer.Feed()) { Dispose(); }
		}

		public void Stop()
		{
			gameObject.SetActive(false);
		}

		public void Start()
		{
			gameObject.SetActive(true);
		}

		public void Dispose()
		{
			Streamer.Dispose();
			Destroy(gameObject);
		}
	}
}