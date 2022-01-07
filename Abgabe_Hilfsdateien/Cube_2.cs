using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_2 : MonoBehaviour {
	public Rigidbody massCenterMarker;
	public Rigidbody impulse;

	private Rigidbody cube;
    private List<List<float>> timeSeries;
    private float currentTimeStep;
    private bool allreadyPrinted = false;

    void Start() {
        timeSeries = new List<List<float>>();
    	cube = GetComponent<Rigidbody>();

        Cube_1.instantiateMassMarker(massCenterMarker, cube, 2);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.N) | (currentTimeStep > 20f & !allreadyPrinted)) {
            print("Cube_2 to CSV");
            Cube_1.WriteTimeSeriesToCSV(timeSeries, "Cube_2");
            allreadyPrinted = true;
        }
    }

    void FixedUpdate() {
        currentTimeStep += Time.deltaTime;
        if (!Quader_3.twoThreeAreConnected) {
            timeSeries.Add(new List<float>() {
                currentTimeStep, 
                cube.position.z, 
                cube.velocity.z,
                cube.mass * cube.velocity.z, //impulse z
                0 //Drehimpuls
            });
        } else {
            timeSeries.Add(new List<float>() {
                currentTimeStep, 
                cube.position.z, 
                cube.velocity.z,
                0, //impulse is combined and messured in the quader
                0 //Drehimpuls
            });
        }

    	if (impulse != null) {
        	Cube_1.setImpulse(impulse, cube.position, cube.mass * cube.velocity);
        }
    }
}
