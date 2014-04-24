using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour {

	public bool toDestroy = true;
	private ParticleSystem[] thisParticleSystem;

	private void Start () {
		thisParticleSystem = this.GetComponentsInChildren<ParticleSystem>();
		
		foreach(ParticleSystem ps in thisParticleSystem)
		{
			ps.renderer.sortingLayerName = "Particle System";
		}

		if (!thisParticleSystem[0].loop && toDestroy) {
			Destroy(this.gameObject, thisParticleSystem[0].duration + 0.5f);
		}
	}
}
