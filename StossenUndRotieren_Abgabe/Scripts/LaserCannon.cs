using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCannon : MonoBehaviour{
	public Rigidbody shot;
	public Transform pos;

	private bool hasShot = false;	
	private bool spaceHasBeenPressed = false;
    
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
    	if (!spaceHasBeenPressed & Input.GetKeyDown(KeyCode.Space)) {
        	print("space has been pressed");
            spaceHasBeenPressed = true;
        }  
    }

    void FixedUpdate() {
    	if (spaceHasBeenPressed){
    		if (!hasShot) {
    			Rigidbody shotCopy = Instantiate(shot, pos.position, pos.rotation) as Rigidbody;
    			shotCopy.AddForce(new Vector3(0f, -1000f, 40000f), ForceMode.Acceleration);
    			hasShot = true;
    		}
    	}
    }
}
