using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// As the name suggest this carcontroller script controls the car model and makes it behave like a car.
public class CarController : MonoBehaviour
{
    public WheelParticles wheelParticles; // we created a variable named wheelParticles of type WheelParticles to store the references of each wheels smokeparticle.
    public WheelColliders colliders; // we created  a variable named colliders of type class wheelcolliders
                                     // that is the declared below (datastructure to hold the reference of wheelcolliders of the car model)
    public WheelMeshes wheelMeshes; // we creaated a variable named wheelMeshes of type class WheelMeshes
    public float gasInput;   // for the forward and backward movement of the car.controls the movements of the rear wheels.
    public float steeringInput;    // for the left and right side movement of the car . controls the movement of the front wheels.
    public float motorPower;    // this variable is for how much torque we should apply the engine or
                                // wheels when the user hits the forward or backward button.

    public AnimationCurve steeringCurve;  // now unity uses AnimationCurve to process Curve. 
                                          // now if you honesty ask me what this curve is for? i don't know.
                                          // i will tell you what i understand .
                                          // we can get the speed and ratio of how much we should rotate the steering angle from this curve.
                                          // so we can implement smooth steeering using that. that's all what i know now.
    public float brakePower;   // this variable store how much brake we should apply to the car.
                               // so our logic for applying brake is that .
                               // if the direction of the car and the direction the car actually moving is equal
                               // or the the angle between them is zero.
                               // which means car is moving forward. if the angle is 180 then it is moving backward .
                               // angle between these values then the car is drifting.
                               // so we apply brake according to that.
    public GameObject smokePrefab; // to store the reference of the smoke effect particle system.
    private float slipAngle;  // the angle in the above logic of brake is actually stored in this variable.
    private float brakeInput;  // to store the players input for brake.



    private float speed; // this variable is to store the velocity or speed of the car using rigidbodies velocity.
                         // the need for this variable speed is because we are going to limit the steering angle of the car according to the car.
                         // for example a car running is huge speed can't steer like a car running running in slow.
                         // so for that realistic approach we are using this varibale.
    private Rigidbody playerRB; // this rigidbody variable is to store the references of the rigidbody of the car to get the velocity.

    void Start()
    {
        playerRB = GetComponent<Rigidbody>(); // to get rigidbody reference and store it in playerRB when the game starts.
    }


    void Update()
    {
        speed = playerRB.linearVelocity.magnitude;   // calculates the speed in  every frame.
        CheckInput();
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        ApplyWheelPosition();
        InstantiateSmoke();
    }

    void ApplyWheelPosition() // this is a wrapper function to actually wrap the 4 Updatefunctions
    {                         // that actually updates the motion from wheelcollider to wheelmesh
        UpdateWheel(colliders.FRWheel,wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
    }

    void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)// actually this function takes the parameter wheelcolllider and wheel mesh
                                                                // and update the wheelmeshes motion that is position and rotation with the wheelcolliders position and rotation.
    {
        Quaternion quat; //to store rotation.
        Vector3 position; // to store position.

        coll.GetWorldPose(out position, out quat); // return the position and rotation of the wheelcollider coll.
        wheelMesh.transform.position = position;   //set the wheelmesh position with the position of wheelcollider
        wheelMesh.transform.rotation = quat; //set the wheelmesh rotation with the position of the wheelcollider
    }

    void CheckInput()  // this function take the input from the keyboard and store that  inputs in these  variables gasInput and steeringInput.
                       // so that script understand it time to move forward or steer the wheels.
    {
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        slipAngle = Vector3.Angle(transform.forward,playerRB.linearVelocity-transform.forward);// we store the angle between the directions .
                                                                                               // one direction is the car actually facing and
                                                                                               // second direction is the car actually moving.
                                                                                               // and we store that angle in slipAngle.
        if (slipAngle < 120f)               // the logic here is when slip angle is zero which means the car is moving forward
        {                                   // and when the slip angle is 180 which means the car is moving backward.
            if(gasInput < 0)                // else the other anlges the car is drifting so in that case we need to apply brake.
            {                               // i don't know exactly  the logic i think that may be the logic.
                brakeInput = Mathf.Abs(gasInput);
                gasInput = 0;
            }
        }
        else
        {
            brakeInput = 0;
        }
}
    void ApplyMotor()   // this function is to actually apply the motion to car by applying motorpower
    {                   // when the user click the forward or backward button.
        colliders.RRWheel.motorTorque = motorPower * gasInput;  // applying motorpower to rearwheels according to players input.
        colliders.RLWheel.motorTorque = motorPower * gasInput;
    }

    void ApplySteering()  // this functions is to apply steering the wheel according to player input
    {                    // and it also controls the steering angle according to the cars speed.
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speed); // i don't know what this line exactly do.
                                                                             // but i think this line will look at the speed and curve and decide the steering angle.
        colliders.FRWheel.steerAngle = steeringAngle; // just give the steer angle we got to the actual wheelcolliders of the front wheels.
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    void ApplyBrake()     // this function applies brake as the name suggest. but there is a catch.
    {                      // the front wheel 70% of the brake is applied
        colliders.FRWheel.brakeTorque = brakeInput * brakePower * 0.7f;
        colliders.FLWheel.brakeTorque = brakeInput * brakePower * 0.7f;
        colliders.RRWheel.brakeTorque = brakeInput * brakePower * 0.3f;
        colliders.RLWheel.brakeTorque = brakeInput * brakePower * 0.3f;
    }                      // and rest of the 30% of brake is applied to rear wheels to get a realistic physics.

    void InstantiateSmoke()    // this function instantiate the smoke effect in eachwheel.
    {
        wheelParticles.FRWheel = 
            Instantiate(smokePrefab, colliders.FRWheel.transform.position - Vector3.up * colliders.FRWheel.radius,
            Quaternion.identity,
            colliders.FRWheel.transform).GetComponent<ParticleSystem>();

        wheelParticles.FLWheel =
            Instantiate(smokePrefab, colliders.FLWheel.transform.position - Vector3.up * colliders.FLWheel.radius,
            Quaternion.identity,
            colliders.FLWheel.transform).GetComponent<ParticleSystem>();

        wheelParticles.RRWheel =
            Instantiate(smokePrefab, colliders.RRWheel.transform.position - Vector3.up * colliders.RRWheel.radius,
            Quaternion.identity,
            colliders.RRWheel.transform).GetComponent<ParticleSystem>();

        wheelParticles.RLWheel =
            Instantiate(smokePrefab, colliders.RLWheel.transform.position - Vector3.up * colliders.RLWheel.radius,
            Quaternion.identity,
            colliders.RLWheel.transform).GetComponent<ParticleSystem>();
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

    [System.Serializable]
    public class WheelParticles  // this datastructure is for storing the smoke particle effect when braking or drifting.
    {
        public ParticleSystem FRWheel;
        public ParticleSystem FLWheel;
        public ParticleSystem RRWheel;
        public ParticleSystem RLWheel;
    }
}
