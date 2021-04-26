using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Write : MonoBehaviour {
	public string nameOfFile;
	public string nameOfObject;
	Rigidbody rigidBody;
	private List<List<float>> timeSeries;
	private float currentTimeStep;
    private bool readyToPrint = true;

    // Start is called before the first frame update
    void Start() {
        rigidBody =  GetComponent<Rigidbody>();
        timeSeries = new List<List<float>>();
    }

    // Update is called once per frame
    void Update() {
       if (Input.GetKeyDown(KeyCode.P)) {
            WriteTimeSeriesToCSV();
        }
    }

    void FixedUpdate() {
        currentTimeStep += Time.deltaTime;
        timeSeries.Add(new List<float>() {
        	currentTimeStep, 
            rigidBody.position.x, 
            rigidBody.position.y, 
            rigidBody.position.z, 
            rigidBody.velocity.z,
            rigidBody.mass
            });

        if (currentTimeStep > 20 && readyToPrint) {
            readyToPrint = false;
            WriteTimeSeriesToCSV();
        }
    }

    void OnTriggerEnter(Collider other) {
		if (other.tag == "EndOfCannon") {
			
		}
	}

     void OnCollisionEnter(Collision hit) {
        if (hit.gameObject.tag == "Ground") {
            
        }
    }

    void WriteTimeSeriesToCSV() {
    	if (nameOfFile == null) {
    		nameOfFile = nameOfObject;
    	}
        using (var streamWriter = new StreamWriter(nameOfFile + ".csv")) {
            //streamWriter.WriteLine("t,s(t),v(t)");
            
            foreach (List<float> timeStep in timeSeries) {
                streamWriter.WriteLine(string.Join(",", timeStep));
                streamWriter.Flush();
            }
        }
    }
}


