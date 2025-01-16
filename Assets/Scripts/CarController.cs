using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// As the name suggest this carcontroller script controls the car model and makes it behave like a car.
public class CarController : MonoBehaviour
{
    public WheelColliders colliders; // we created  a variable named colliders of type class wheelcolliders
                                     // that is the declared below (datastructure to hold the reference of wheelcolliders of the car model)
    public WheelMeshes wheelMeshes; // we creaated a variable named wheelMeshes of type class WheelMeshes
    void Start()
    {

    }


    void Update()
    {
        ApplyWheelPosition();
    }

    void ApplyWheelPosition() // this is a wrapper function to actually wrap the 4 Updatefunctions
    {                         // that actually updates the motion from wheelcollider to wheelmesh
        UpdateWheel(colliders.FRWheel,wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
    }

    void UpdateWheel(WheelCollider coll,MeshRenderer wheelMesh)// actually this function takes the parameter wheelcolllider and wheel mesh
                                                               // and update the wheelmeshes motion that is position and rotation with the wheelcolliders position and rotation.
    {
        Quaternion quat; //to store rotation.
        Vector3 position; // to store position.

        coll.GetWorldPose(out position, out quat); // return the position and rotation of the wheelcollider coll.
        wheelMesh.transform.position = position;   //set the wheelmesh position with the position of wheelcollider
        wheelMesh.transform.rotation = quat; //set the wheelmesh rotation with the position of the wheelcollider
    }


    [System.Serializable]   // This system.serializable is used to tell unity that this class is a datastructure
                            // to hold references of the wheelcolliders of car prefab.
    public class WheelColliders  // This is the datastructure or class  to hold the references of the wheelcolliders of the car prefab.
    {
        public WheelCollider FRWheel;
        public WheelCollider FLWheel;
        public WheelCollider RRWheel;
        public WheelCollider RLWheel;
    }

    [System.Serializable] // I just find out that if we don't use this then the classes references slot will be shown in the inspector.
    public class WheelMeshes  // This is the datastructure or class  to hold the references of the wheelmeshes of the car prefab
                              // (the actual visuals tires of the car model).
    {
        public MeshRenderer FRWheel;
        public MeshRenderer FLWheel;
        public MeshRenderer RRWheel;
        public MeshRenderer RLWheel;
    }
}
