using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

using Barracuda.Internal;
using Barracuda.UISystem;

namespace Barracuda
{
	public class MonoStreamer : MonoBehaviour, IDisposable
	{
		public IStreamer Streamer { get; set; }

		[SerializeField] private Tween[] streamees;

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