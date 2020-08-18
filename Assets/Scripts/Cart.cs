using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{

    /// <summary>
    /// Contains parameters that can adjust the kart's behaviors temporarily.
    /// </summary>
    public float mouse;
    public float zooms;

    private Vector2 center;
    private float rotation;

    [System.Serializable]
    public class StatPowerup
    {
        public Stats modifiers;
        public string PowerUpID;
        public float ElapsedTime;
        public float MaxTime;
    }

    /// <summary>
    /// Contains a series tunable parameters to tweak various karts for unique driving mechanics.
    /// </summary>
    [System.Serializable]
    public struct Stats
    {
        [Header("Movement Settings")]
        [Tooltip("The maximum speed forwards")]
        public float TopSpeed;

        [Tooltip("How quickly the Kart reaches top speed.")]
        public float Acceleration;

        [Tooltip("The maximum speed backward.")]
        public float ReverseSpeed;

        [Tooltip("The rate at which the kart increases its backward speed.")]
        public float ReverseAcceleration;

        [Tooltip("How quickly the Kart starts accelerating from 0. A higher number means it accelerates faster sooner.")]
        [Range(0.2f, 1)]
        public float AccelerationCurve;

        [Tooltip("How quickly the Kart slows down when going in the opposite direction.")]
        public float Braking;

        [Tooltip("How quickly to slow down when neither acceleration or reverse is held.")]
        public float CoastingDrag;

        [Range(0, 1)]
        [Tooltip("The amount of side-to-side friction.")]
        public float Grip;

        [Tooltip("How quickly the Kart can turn left and right.")]
        public float Steer;

        [Tooltip("Additional gravity for when the Kart is in the air.")]
        public float AddedGravity;

        [Tooltip("How much the Kart tries to keep going forward when on bumpy terrain.")]
        [Range(0, 1)]
        public float Suspension;

        // allow for stat adding for powerups.
        public static Stats operator +(Stats a, Stats b)
        {
            return new Stats
            {
                Acceleration = a.Acceleration + b.Acceleration,
                AccelerationCurve = a.AccelerationCurve + b.AccelerationCurve,
                Braking = a.Braking + b.Braking,
                CoastingDrag = a.CoastingDrag + b.CoastingDrag,
                AddedGravity = a.AddedGravity + b.AddedGravity,
                Grip = a.Grip + b.Grip,
                ReverseAcceleration = a.ReverseAcceleration + b.ReverseAcceleration,
                ReverseSpeed = a.ReverseSpeed + b.ReverseSpeed,
                TopSpeed = a.TopSpeed + b.TopSpeed,
                Steer = a.Steer + b.Steer,
                Suspension = a.Suspension + b.Suspension
            };
        }
    }

    public Stats baseStats = new Stats
    {
        TopSpeed = 10f,
        Acceleration = 5f,
        AccelerationCurve = 4f,
        Braking = 10f,
        ReverseAcceleration = 5f,
        ReverseSpeed = 5f,
        Steer = 5f,
        CoastingDrag = 4f,
        Grip = .95f,
        AddedGravity = 1f,
        Suspension = .2f
    };
    // can the kart move?
    bool canMove = true;
    List<StatPowerup> activePowerupList = new List<StatPowerup>();
    GameObject lastGroundCollided = null;
    Stats finalStats;
    public Rigidbody Rigidbody { get; private set; }
    public PortHandler Ports { get; private set; }
    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Ports= GameObject.Find("EventSystem").GetComponent<PortHandler>();
        center = new Vector2(Screen.width/2, Screen.height/2);
    }


    void FixedUpdate()
    {
       // ResetIfStuck();

        // apply our powerups to create our finalStats
        TickPowerups();

        // gather inputs

        // float accel = Ports.kg;
        float accel = Input.mousePosition.y / 500;
        mouse = (Input.mousePosition.y / 20);
        zooms = Rigidbody.velocity.magnitude;
        //float turn = Input.x;

        if (canMove)
        {
            MoveVehicle(accel, 0);
            Rotate();
        }

    }

    void Rotate()
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        //float accel = (y > center.y ? y - center.y :( center.y - y) * -1) / 10;
        float direction = (x > center.x ? x - center.x : (center.x - x) * -1) / 10;
        float offset = x > center.x ? (x - center.x) / center.x : -1 * (center.x - x) / center.x;
        rotation = offset * 90;
        Rigidbody.transform.Rotate(new Vector3(0, rotation, 0), Math.Abs(offset));
    }

    void TickPowerups()
    {
        // remove all elapsed powerups
        activePowerupList.RemoveAll((p) => { return p.ElapsedTime > p.MaxTime; });

        // zero out powerups before we add them all up
        var powerups = new Stats();

        // add up all our powerups
        for (int i = 0; i < activePowerupList.Count; i++)
        {
            var p = activePowerupList[i];

            // add elapsed time
            p.ElapsedTime += Time.deltaTime;

            // add up the powerups
            powerups += p.modifiers;
        }

        // add powerups to our final stats
        finalStats = baseStats + powerups;

        // clamp values in finalstats
        finalStats.Grip = Mathf.Clamp(finalStats.Grip, 0, 1);
        finalStats.Suspension = Mathf.Clamp(finalStats.Suspension, 0, 1);
    }

    void MoveVehicle(float accelInput, float turnInput)
    {
        // manual acceleration curve coefficient scalar
        float accelerationCurveCoeff = 5;
        Vector3 localVel = transform.InverseTransformVector(Rigidbody.velocity);

        bool accelDirectionIsFwd = accelInput >= 0;
        bool localVelDirectionIsFwd = localVel.z >= 0;

        // use the max speed for the direction we are going--forward or reverse.
        float maxSpeed = accelDirectionIsFwd ? finalStats.TopSpeed : finalStats.ReverseSpeed;
        float accelPower = accelDirectionIsFwd ? finalStats.Acceleration : finalStats.ReverseAcceleration;

        float accelRampT = Rigidbody.velocity.magnitude / maxSpeed;
        float multipliedAccelerationCurve = finalStats.AccelerationCurve * accelerationCurveCoeff;
        float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);

        bool isBraking = accelDirectionIsFwd != localVelDirectionIsFwd;

        // if we are braking (moving reverse to where we are going)
        // use the braking acceleration instead
        float finalAccelPower = isBraking ? finalStats.Braking : accelPower;

        float finalAcceleration = finalAccelPower * accelRamp;

        // apply inputs to forward/backward
        float turningPower = turnInput * finalStats.Steer;

        Quaternion turnAngle = Quaternion.AngleAxis(turningPower, Rigidbody.transform.up);
        Vector3 fwd = turnAngle * Rigidbody.transform.forward;

        Vector3 movement = fwd * accelInput * finalAcceleration /* * GroundPercent*/;

        // simple suspension allows us to thrust forward even when on bumpy terrain
        //  fwd.y = Mathf.Lerp(fwd.y, 0, finalStats.Suspension);

        // forward movement
        float currentSpeed = Rigidbody.velocity.magnitude;
        bool wasOverMaxSpeed = currentSpeed >= maxSpeed;

        // if over max speed, cannot accelerate faster.
        if (wasOverMaxSpeed && !isBraking) movement *= 0;

        Vector3 adjustedVelocity = Rigidbody.velocity + movement * Time.deltaTime;

        adjustedVelocity.y = Rigidbody.velocity.y;

        //  clamp max speed if we are on ground
        /* if (GroundPercent > 0)
          {
              if (adjustedVelocity.magnitude > maxSpeed && !wasOverMaxSpeed)
              {
                  adjustedVelocity = Vector3.ClampMagnitude(adjustedVelocity, maxSpeed);
              }
          }*/

        // coasting is when we aren't touching accelerate
        bool isCoasting = Mathf.Abs(accelInput) < .01f;

        if (isCoasting)
        {
            Vector3 restVelocity = new Vector3(0, Rigidbody.velocity.y, 0);
            adjustedVelocity = Vector3.MoveTowards(adjustedVelocity, restVelocity, Time.deltaTime * finalStats.CoastingDrag);
        }

        Rigidbody.velocity = adjustedVelocity;



        //if (GroundPercent > 0)
        //{
        //manual angular velocity coefficient
        //float angularVelocitySteering = .4f;
        //float angularVelocitySmoothSpeed = 20f;

        ////turning is reversed if we're going in reverse and pressing reverse
        //if (!localVelDirectionIsFwd && !accelDirectionIsFwd) angularVelocitySteering *= -1;
        //var angularVel = Rigidbody.angularVelocity;

        //// move the Y angular velocity towards our target
        //angularVel.y = Mathf.MoveTowards(angularVel.y, turningPower * angularVelocitySteering, Time.deltaTime * angularVelocitySmoothSpeed);

        ////apply the angular velocity
        //Rigidbody.angularVelocity = angularVel;

        ////rotate rigidbody's velocity as well to generate immediate velocity redirection
        //// manual velocity steering coefficient
        //float velocitySteering = 25f;
        ////rotate our velocity based on current steer value
        //Rigidbody.velocity = Quaternion.Euler(0f, turningPower * velocitySteering * Time.deltaTime, 0f) * Rigidbody.velocity;
    
        

        // apply simplified lateral ground friction
        // only apply if we are on the ground at all
         //if (GroundPercent > 0f)
         //   {
         //       // manual grip coefficient scalar
         //       float gripCoeff = 30f;
         //       // what direction is our lateral friction in?
         //       // it is the direction the wheels are turned, our forward
         //       Vector3 latFrictionDirection = Vector3.Cross(fwd, transform.up);
         //       // how fast are we currently moving in our friction direction?
         //       float latSpeed = Vector3.Dot(Rigidbody.velocity, latFrictionDirection);
         //       // apply the damping
         //       Vector3 latFrictionDampedVelocity = Rigidbody.velocity - latFrictionDirection * latSpeed * finalStats.Grip * gripCoeff * Time.deltaTime;

         //       // apply the damped velocity
         //       Rigidbody.velocity = latFrictionDampedVelocity;
         //   }

    }

    public void AddPowerup(StatPowerup statPowerup)
    {
        activePowerupList.Add(statPowerup);
    }

    public float LocalSpeed()
    {
        if (canMove)
        {
            float dot = Vector3.Dot(transform.forward, Rigidbody.velocity);
            if (Mathf.Abs(dot) > 0.1f)
            {
                float speed = Rigidbody.velocity.magnitude;
                return dot < 0 ? -(speed / finalStats.ReverseSpeed) : (speed / finalStats.TopSpeed);
            }
            return 0f;
        }
        else
        {
            // use this value to play kart sound when it is waiting the race start countdown.
            // return  Ports.kg;
            return Input.mousePosition.y / 10;
        }
    }

    public void SetCanMove(bool move)
    {
        canMove = move;
    }

}
