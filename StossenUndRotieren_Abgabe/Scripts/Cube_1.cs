using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Cube_1 : MonoBehaviour {
	public float springConstant = 1f;
	public float force = 0.5f;
	public Rigidbody massCenterMarker;
	public Rigidbody objectTwo;
	public Rigidbody impulseOne;


    private List<List<float>> timeSeries;
    private float currentTimeStep;
	private Rigidbody cube;
	private float springLength;
	private bool springIsWorking = false;
	private bool go = false; 
	private bool stopAccelartingObjectOne = false;
	private bool hitOneHappend = false;
    private bool allreadyPrinted;

	private int SPRING_LENGTH_RESTING = 1;

    void Start() {
        timeSeries = new List<List<float>>();
		cube = GetComponent<Rigidbody>();
		springLength = 1f;   

        instantiateMassMarker(massCenterMarker, cube, 1);     
    }

    void Update() {
        //write data to CSV
        if (Input.GetKeyDown(KeyCode.N) | (currentTimeStep > 20f & !allreadyPrinted)) {
            print("Cube_1 to CSV");
            WriteTimeSeriesToCSV(timeSeries, "Cube_1");
            allreadyPrinted = true;
        }
    }

    void FixedUpdate() {
        currentTimeStep += Time.deltaTime;
        timeSeries.Add(new List<float>() {
            currentTimeStep, 
            cube.position.z, 
            cube.velocity.z,
            cube.mass * cube.velocity.z, //impulse z
            0 //Drehimpuls
        });

		if (!hitOneHappend & go) {
    		MoveObjectOne();
    	} else if (springIsWorking) {
    		collisionOneTwo();
    	}

    	setImpulse(impulseOne, cube.position, cube.mass * cube.velocity);
    }

    //Add a Force until objectOne has a velocity of 1m/s
    //after this no force is added into the system
    private void MoveObjectOne() {
    	if (!stopAccelartingObjectOne & cube.velocity.z < 1) {
    		cube.AddForce(new Vector3(0f, 0f, force), ForceMode.Acceleration);
    	} else {
    		stopAccelartingObjectOne = true;
    	}
    }

    //let the impulse transfer to the cube two over the spring
    private void collisionOneTwo() {
    	springLength = objectTwo.position.z - cube.position.z - 1; //-1 is the offset because position of the object is in the middle
    	force = -1 * springConstant * (SPRING_LENGTH_RESTING - springLength);
    	cube.AddForce(new Vector3(0f, 0f, force), ForceMode.Acceleration);
    	objectTwo.AddForce(new Vector3(0f, 0f, -force), ForceMode.Acceleration);
    }

    public static Rigidbody instantiateMassMarker(Rigidbody marker, Rigidbody cubeToMark, int number) {
        Rigidbody massCenterMarkerCopy = 
            Instantiate(marker, cubeToMark.position, cubeToMark.rotation) as Rigidbody;
        massCenterMarkerCopy.gameObject.AddComponent<FixedJoint>();  
        massCenterMarkerCopy.gameObject.GetComponent<FixedJoint>().connectedBody = cubeToMark;
        massCenterMarkerCopy.name = "massCenterMarkerCopy" + number;
        return massCenterMarkerCopy;
    }

    public static void setImpulse(Rigidbody impulsePivot, Vector3 startPos, Vector3 impulse) {	
        if (startPos != impulse) {
            impulsePivot.transform.position = startPos;
        	impulse += startPos;	
        	impulse *= 0.02f;
    	    impulsePivot.transform.localScale = Vector3.forward * Vector3.Distance(startPos, impulse) 
    	    	+ new Vector3(0.05f, 0.05f, 0.05f);
        }
    }

    public static void WriteTimeSeriesToCSV(List<List<float>> timeSeriesToWrite, string nameOfFile) {
        using (var streamWriter = new StreamWriter(nameOfFile + "_timeSeries.csv")) {
            streamWriter.WriteLine("t,s_z(t),v_z(t),p_z(t),L(t)");
            
            foreach (List<float> timeStep in timeSeriesToWrite) {
                streamWriter.WriteLine(string.Join(",", timeStep));
                streamWriter.Flush();
            }
        }
        print(nameOfFile + ": printed!");
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "objectTwo") {
            print("Spring is touching");
            print("v of Object 1: " + cube.velocity.z);
            hitOneHappend = true;
            springIsWorking = true;
        }

        //shoot at the cube to add the startforce into the system
        if (other.gameObject.tag == "shot") {
            print("go");
            go = true;
            Destroy(GameObject.Find("plasma_gun_muzzle_flash_example(Clone)"));
        }
    }

    //spring is loose again and not working
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "objectTwo") {
            print("Spring is loose");
            print("v of Object 2: " + objectTwo.velocity.z);
            springIsWorking = false;
        }
    }
}
