using UnityEngine;
using System.Collections;

public class BombSpark : MonoBehaviour {
	
	public string customSortingLayer = "Particle System";
	private ParticleSystem[] thisParticleSystem;
	
	private void Start () {
		thisParticleSystem = this.GetComponentsInChildren<ParticleSystem>();

		foreach(ParticleSystem ps in thisParticleSystem)
		{
			ps.renderer.sortingLayerName = customSortingLayer;
		}
	}
}
