﻿using UnityEngine;
using System.Collections;

public class LevelRestarter : MonoBehaviour {

	// Use this for initialization
	void Start () {
			
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.R))
		   Application.LoadLevel(Application.loadedLevel);	
	}
}