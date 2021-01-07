﻿using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	public AudioClip deathClip;
    float timeBeforeRespwan = 0.2f;
    Vector3 lastCheckpointPosition;
    bool isAlive;

	bool ropeIsEnabled = false;

	// Use this for initialization
	void Start () {
        lastCheckpointPosition = this.gameObject.transform.position;
        isAlive = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Kill ()
    {
        StartCoroutine(KillProcess());
    }

    IEnumerator KillProcess()
    {
         GameObject camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];

		PlayDeathSound ();

        if (camera != null)
        {
            camera.GetComponent<CameraController>().Shake(0.2f,0.5f);
        }
        
        DisablePlayer();

        yield return new WaitForSeconds(timeBeforeRespwan);

        this.gameObject.transform.position = lastCheckpointPosition;

        EnablePlayer();    
    }

    void DisablePlayer()
    {
        isAlive = false;

        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
		ropeIsEnabled = this.gameObject.GetComponent<NewRope> ().enabled;
        this.gameObject.GetComponent<NewRope>().enabled = false;
        this.gameObject.GetComponent<NewPlayer2>().enabled = false;
        this.gameObject.GetComponent<Collider2D>().enabled = false;

        this.gameObject.GetComponent<Rigidbody2D>().Sleep();
    }

    void EnablePlayer()
    {
        isAlive = true;

        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
		this.gameObject.GetComponent<NewRope>().enabled = ropeIsEnabled;
        this.gameObject.GetComponent<NewPlayer2>().enabled = true;
        this.gameObject.GetComponent<Collider2D>().enabled = true;

        this.gameObject.GetComponent<Rigidbody2D>().WakeUp();
    }

    public bool IsPlayerAlive()
    {
        return isAlive;
    }

    public void SetCheckpointPosition(Vector3 newCheckpoint)
    {
        lastCheckpointPosition = newCheckpoint;
    }

    public Vector3 GetCheckpointPosition()
    {
        return lastCheckpointPosition;
    }

	public void PlayDeathSound()
	{
		GetComponent<AudioSource> ().PlayOneShot (deathClip);
	}

}
