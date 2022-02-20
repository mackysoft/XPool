using System;
using UnityEngine;
using MackySoft.XPool.Unity.ObjectModel;

namespace MackySoft.XPool.Unity {

	[Serializable]
	public class ParticleSystemPool : ComponentPoolBase<ParticleSystem> {

		[SerializeField]
		bool m_PlayOnRent;

		public bool PlayOnRent { get => m_PlayOnRent; set => m_PlayOnRent = value; }

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