using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quader_3: MonoBehaviour {
	public Rigidbody objectTwo;
    public Rigidbody massCenterMarker;
    public Rigidbody impulse;
    public Rigidbody velocityPhaseThree;

    public static bool twoThreeAreConnected = false;

	private Rigidbody objectThree;
    private List<List<float>> timeSeries;
    private float currentTimeStep;
    private bool allreadyAdded = false;
    private Rigidbody massCenterMarkerCopy;
    private Rigidbody combinedMassCenterMarkerCopy;

    private float INERTIA_QUADER;
    private float INERTIA_CUBE2;
    private float relativeInertiaQuader;
    private float relativeInertiaCubeTwo;
    private float totalInertia;
    private bool allreadyPrinted = false;
    private Vector3 impulseValue = new Vector3(0f, 0f, 0f);

	void Start() {
        timeSeries = new List<List<float>>();
		objectThree = GetComponent<Rigidbody>();
        INERTIA_QUADER = objectThree.inertiaTensor.z;
        INERTIA_CUBE2 = objectTwo.inertiaTensor.z;

        massCenterMarkerCopy = Cube_1.instantiateMassMarker(massCenterMarker, objectThree, 3);
        
        combinedMassCenterMarkerCopy = 
            Instantiate(massCenterMarker, calculateCombinedMassCenter(), objectTwo.rotation) as Rigidbody;
	}

    void Update() {
        //print the TimeSeries into a CSV
        if (Input.GetKeyDown(KeyCode.N) | (currentTimeStep > 20f & !allreadyPrinted)) {
            print("Quader_3 to CSV");
            Cube_1.WriteTimeSeriesToCSV(timeSeries, "Quader_3");
            allreadyPrinted = true;
        }

        if (!allreadyAdded & twoThreeAreConnected) {
            //Destroy unused Objects because 2 and 3 are combined now
            Destroy(GameObject.Find("massCenterMarkerCopy2"));
            Destroy(GameObject.Find("massCenterMarkerCopy3"));
            Destroy(GameObject.Find("Impulse_2"));

            velocityPhaseThree.gameObject.AddComponent<FixedJoint>();  
            velocityPhaseThree.gameObject.GetComponent<FixedJoint>().connectedBody = objectThree;

            allreadyAdded = true;

            //print some infos for debugging
            print("Inertia Cube2: " + INERTIA_CUBE2 + "; Inertia Quader: " + INERTIA_QUADER);
            print("Inertia Cube2': " + relativeInertiaCubeTwo + "; Inertia Quader': " + relativeInertiaQuader);

            print("Drehimpuls: " + calculateAngularMomentManuel());
            print("angular moment: " + calculateAngularMoment());

            print("omegaUnity: " + (objectTwo.angularVelocity + objectThree.angularVelocity).magnitude);

        }

        if (Input.GetKeyDown(KeyCode.A)) {
            print("v_cube: " + objectTwo.velocity.z + "; v_quader: " + objectThree.velocity.z + "; combined: " + velocityPhaseThree.velocity.z);
        }
    }

    void FixedUpdate() {
        currentTimeStep += Time.deltaTime;
        combinedMassCenterMarkerCopy.position = calculateCombinedMassCenter();

        relativeInertiaQuader = INERTIA_QUADER + objectThree.mass * Mathf.Pow((combinedMassCenterMarkerCopy.position - objectThree.position).magnitude, 2); //I' = I + m * d^2
        relativeInertiaCubeTwo = INERTIA_CUBE2 + objectTwo.mass * Mathf.Pow((objectTwo.position - combinedMassCenterMarkerCopy.position).magnitude, 2);
        totalInertia = relativeInertiaQuader + relativeInertiaCubeTwo;

        combinedMassCenterMarkerCopy.transform.localScale = calculateAngularMoment() + new Vector3(0.05f, 0.05f, 0.05f);
 
        if (twoThreeAreConnected) { 
            impulseValue = (objectTwo.mass + objectThree.mass) * velocityPhaseThree.velocity;

            Vector3 position = combinedMassCenterMarkerCopy.position;
            Cube_1.setImpulse(impulse, position, impulseValue);
        }

        timeSeries.Add(new List<float>() {
                currentTimeStep, 
                objectThree.position.z, 
                objectThree.velocity.z,
                impulseValue.magnitude, //impulse z
                calculateAngularMoment().y //Drehimpuls
            });
    }

    private Vector3 calculateCombinedMassCenter() {
        return (objectTwo.position * objectTwo.mass + objectThree.position * objectThree.mass) / (objectTwo.mass + objectThree.mass);
    }

    //calculate angular moment with superposition
    private Vector3 calculateAngularMoment() {
        Vector3 angularVelocity = (objectTwo.angularVelocity + objectThree.angularVelocity);
        return totalInertia * angularVelocity;
    }

    //claculate angular moment with manual calculation with formula
    private Vector3 calculateAngularMomentManuel() {
        Vector3 R = objectTwo.position - combinedMassCenterMarkerCopy.position;
        Vector3 v = objectTwo.velocity;
        float RSquared = Mathf.Pow(Vector3.Cross(R,v).magnitude / v.magnitude, 2);
        Vector3 omega = Vector3.Cross(R, v) / RSquared;

        print("R: " + R + "; Rs: " + RSquared + "; v: " + v + "; omega: " + omega);
        print(v.x + ", " + v.y + ", " + v.z);

        return totalInertia * omega;
    }

    void OnCollisionEnter(Collision hit) {
        if (hit.gameObject.tag == "objectTwo") {
            print("Two and Three are now connected");
            gameObject.AddComponent<FixedJoint>();  
         	gameObject.GetComponent<FixedJoint>().connectedBody = objectTwo;
         	twoThreeAreConnected = true;
        }
    }
}
