using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public float volumeScale = 0.001f;
	public float minSpeedToPlayWind = 25f;

	[Range(0f,1f)]
	public float soundReponseness = 1f;

	float volumeToPlay = 0f;

	Rigidbody2D rb;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		rb = transform.parent.GetComponent<Rigidbody2D> ();
		audioSource = GetComponent<AudioSource> ();
	
	}
	
	// Update is called once per frame
	void Update () {
		volumeToPlay = Common.ApproachWeighted(Mathf.Clamp( volumeScale * (rb.velocity.magnitude - minSpeedToPlayWind) ,0f,1f), volumeToPlay, soundReponseness);

		audioSource.volume = volumeToPlay;
	
	}
}
