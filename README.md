﻿# XPool - Object Pooling System

[![Tests](https://github.com/mackysoft/XPool/actions/workflows/tests.yaml/badge.svg)](https://github.com/mackysoft/XPool/actions/workflows/tests.yaml)
[![Build](https://github.com/mackysoft/XPool/actions/workflows/build.yaml/badge.svg)](https://github.com/mackysoft/XPool/actions/workflows/build.yaml)

![XPool_Frame](https://user-images.githubusercontent.com/13536348/154980636-f949151e-f820-4d86-b9e9-302880671d14.png)

**Created by Hiroya Aramaki ([Makihiro](https://twitter.com/makihiro_dev))**

## What is XPool ?

XPool is an object pooling library for Unity.

This was developed to be able to do all the pooling expected in application development with just this library.

- All your classes can be pooled.
- Short code, easy to use.
- Fast performance
- Scalability
- Tested. It works stably.


## <a id="index" href="#index"> Table of Contents </a>

- [📥 Installation](#installation)
- [🔰 Usage](#usage)
  - [Unity Object Pool](#unity-object-pool)
    - [GameObject Pool]()
    - [Component Pool]()
    - [ParticleSystem Pool](#particle-system-pool)
  - [Pure C# Object Pool](#object-pool)
  - [Collection Pool](#collection-pool)
  - [Non Allocated Collections](#non-allocated-collections)
  - [How to write custom pool ?](#how-to-write-custom-pool)
      - [ParticleSystemPool implementation](#particle-system-pool-implementation)
  - [Optimization](#optimization)
- [✉ Help & Contribute](#help-and-contribute)
- [📔 Author Info](#author-info)
- [📜 License](#license)

# <a id="installation" href="#installation"> 📥 Installation </a>

Download any version from releases.

Releases: https://github.com/mackysoft/XPool/releases


### Install via PackageManager

Or, you can add this package by opening PackageManager and entering

`https://github.com/mackysoft/XPool.git?path=Assets/MackySoft/MackySoft.XPool`

from the `Add package from git URL` option.


### Install via Open UPM

Or, you can install this package from the [Open UPM](https://openupm.com/packages/com.mackysoft.xpool/) registry.

More details [here](https://openupm.com/).

```
openupm add com.mackysoft.xpool
```


# <a id="usage" href="#usage"> 🔰 Usage </a>

The full Scripting API is [here](https://mackysoft.github.io/XPool/api/MackySoft.XPool.html).

Scripting API: https://mackysoft.github.io/XPool/api/MackySoft.XPool.html

## <a id="unity-object-pool" href="#unity-object-pool"> Unity Object Pool (GameObject, Component) </a>

Pooling of Unity Object (Gameobject, Component) can be performed using `GameObjectPool` or `ComponentPool<T>`.
These hierarchical objects can be rented by writing them in a similar way to Instantiate.

```cs
public class Projectile : MonoBehaviour {

	public float speed;

	void Update () {
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
	}

	void OnCollisionEnter (Collision collision) {
		Destroy(gameObject);
	}
}

public class Shooter : MonoBehaviour {

	[SerializeField]
	ComponentPool<Projectile> m_ProjectilePool = new ComponentPool<Projectile>();

	void Awake () {
		m_ProjectilePool.OnCreate += instance => {
			instance.gameObject.AddComponent<>().
		};
		m_ProjectilePool.OnRetuern += instance => instance.gameObject.SetActive(false);
	}

	public void Shoot (){
		Projectile instance = m_ProjectilePool.Rent(transform.position,transform.rotation);
	}

}
```


### <a id="particle-system-pool" href="#particle-system-pool"> ParticleSystem Pool </a>

```cs
public class HitParticleSystemEmitter : MonoBehaviour {

	[SerializeField]
	ParticleSystemPool m_HitParticleSystemPool = new ParticleSystemPool();

	void OnCollisionEnter (Collision collision) {
		// The rented ParticleSystem is automatically returned to the pool when completed.
		m_HitParticleSystemPool.Rent(collision.position,Quaternion.identity);
	}
}
```


## <a id="object-pool" href="#object-pool"> Object Pool (Pure C# Object) </a>

```cs

```


## <a id="collection-pool" href="#collection-pool"> Collection Pool (`T[]`, `List<T>`, `Qeueue<T>`, `Stack<T>`) </a>

An optimized pool is provided for the generic collections provided in .NET Framework.

```cs

```

## <a id="non-allocated-collections" href="#non-allocated-collections"> Non allocated collections </a>

You can use the TemporaryCollections API that leverages `ArrayPool<T>`.

These collections are a struct and internally use array rented from `ArrayPool<T>`.

```cs
var array = TemporaryArray<T>.Create(10);
var list = TemporaryList<T>.Create();
var queue = TemporaryQueue<T>.Create();
var stack = TemporaryStack<T>.Create();
```


## <a id="how-to-write-custom-pool" href="#how-to-write-custom-pool"> How to write custom pool ? </a>

If you want to implement a more customized pool, you can quickly create one by using the provided base classes.

The base class of the pool is in the `ObjectModel` namespace.

- `MackySoft.XPool.ObjectMode.PoolBase<T>`
- `MackySoft.XPool.Unity.ObjectModel.UnityObjectPoolBase<T>`
- `MackySoft.XPool.Unity.ObjectModel.ComponentPoolBase<T>`

```cs
using MackySoft.XPool.ObjectModel; // PoolBase<T> is here.

public class MyClass {
    public int Health;
}

public class MyPool : PoolBase<MyClass> {

    // Called when Rent is invoked and there are no instances in the pool.
    protected override MyClass Factory () {
        return new MyClass();
    }

    // Called when an instance is rented from the pool.
    // This is also the case when a new instance is created by the Factory.
    protected override void OnRent (MyClass instance) {

    }

    // Called when an instance is returned to the pool.
    protected override void OnReturn (MyClass instance) {

    }

    // Called when the capacity is exceeded and the instance cannot be returned to the pool,
    // or when the instance is released by the ReleaseInstances method.
    protected override void OnRelease (MyClass instance) {
    
    }
}
```

### <a id="particle-system-pool-implementation" href="#particle-system-pool-implementation"> ParticleSystemPool implementation </a>

As an example, `ParticleSystemPool` is implemented using `ComponentPoolBase<T>`.
Its functionality has been optimized for ParticleSystem.

```cs
using UnityEngine;
using MackySoft.XPool.Unity.ObjectModel; // ComponentPoolBase<T> is here.

public class ParticleSystemPool : ComponentPoolBase<ParticleSystem> {

	[SerializeField]
	bool m_PlayOnRent;

	public bool PlayOnRent { get => m_PlayOnRent; set => m_PlayOnRent = value; }

	// 
	protected override void OnCreate (ParticleSystem instance) {
		var main = instance.main;
		main.stopAction = ParticleSystemStopAction.Callback;
		var trigger = instance.gameObject.AddComponent<ParticleSystemStoppedTrigger>();
		trigger.Initialize(instance,this);
	}

	// 
	protected override void OnRent (ParticleSystem instance) {
		if (m_PlayOnRent) {
			instance.Play(true);
		}
	}

	// 
	protected override void OnReturn (ParticleSystem instance) {
		// 
		instance.Stop(true,ParticleSystemStopBehavior.StopEmitting);
	}

	// 
	protected override void OnRelease (ParticleSystem instance) {
		// 
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
```


# <a id="help-and-contribute" href="#help-and-contribute"> ✉ Help & Contribute </a>

I welcome feature requests and bug reports in [issues](https://github.com/mackysoft/XPool/issues) and [pull requests](https://github.com/mackysoft/XPool/pulls).


# <a id="author-info" href="#author-info"> 📔 Author Info </a>

Hiroya Aramaki is a indie game developer in Japan.

- Twitter: [https://twitter.com/makihiro_dev](https://twitter.com/makihiro_dev)
- Blog: [https://mackysoft.net/blog](https://mackysoft.net/blog)


# <a id="license" href="#license"> 📜 License </a>

This library is under the MIT License.
