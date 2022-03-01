using System;
using UnityEngine;
using MackySoft.XPool.Unity.ObjectModel;

namespace MackySoft.XPool.Unity {

	/// <summary>
	/// Optimized pool for <see cref="ParticleSystem"/>.
	/// </summary>
	[Serializable]
	public class ParticleSystemPool : ComponentPoolBase<ParticleSystem> {

		[Tooltip("If true, ParticleSystem will play when the it is rented.")]
		[SerializeField]
		bool m_PlayOnRent = true;

		/// <summary>
		/// If true, <see cref="ParticleSystem"/> will play when the it is rented.
		/// </summary>
		public bool PlayOnRent { get => m_PlayOnRent; set => m_PlayOnRent = value; }

		public ParticleSystemPool () {
		}

		/// <param name="original"> The original object from which the pool will instantiate a new instance. </param>
		/// <param name="capacity"> The pool capacity. If less than 0, <see cref="ArgumentOutOfRangeException"/> will be thrown. </param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public ParticleSystemPool (ParticleSystem original,int capacity) : base(original,capacity) {
		}

		protected override void OnCreate (ParticleSystem instance) {
			var main = instance.main;
			main.stopAction = ParticleSystemStopAction.Callback;
			var trigger = instance.gameObject.AddComponent<ParticleSystemStoppedTrigger>();
			trigger.Initialize(instance,this);
		}

		protected override void OnRent (ParticleSystem instance) {
			if (m_PlayOnRent) {
				instance.Play(true);
			}
		}

		protected override void OnReturn (ParticleSystem instance) {
			instance.Stop(true,ParticleSystemStopBehavior.StopEmitting);
		}

		protected override void OnRelease (ParticleSystem instance) {
			UnityEngine.Object.Destroy(instance.gameObject);
		}

		public class ParticleSystemStoppedTrigger : MonoBehaviour {

			ParticleSystem m_ParticleSystem;
			IPool<ParticleSystem> m_Pool;

			internal void Initialize (ParticleSystem ps,IPool<ParticleSystem> pool) {
				m_ParticleSystem = ps;
				m_Pool = pool;
			}

			void OnParticleSystemStopped () {
				m_Pool?.Return(m_ParticleSystem);
			}

		}
	}
}