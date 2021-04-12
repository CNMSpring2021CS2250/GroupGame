using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AnimalAgent : Agent
{
    [Tooltip("Maximum height berry to go after")]
    public float maxHeightBerry = 2.2f;

    [Tooltip("Minimum height berry to go after")]
    public float minHeightBerry = 1.7f;

    [Tooltip("Force to apply when moving")]
    public float moveForce = 2f;

    [Tooltip("Speed to pitch up or down")]
    public float pitchSpeed = 100f;

    [Tooltip("Speed to roate around the up axis")]
    public float yawSpeed = 100f;

    [Tooltip("Transform of the mouth")]
    public Transform mouthPos;

    [Tooltip("Whether this is training mode or gameplay mode")]
    public bool trainingMode;

    // The rigid body of the agent
    new private Rigidbody rigidbody;

    // The berry area the agent is in
    private BerryArea berryArea;

    // The nearest berry to the agent
    private Berry nearestBerry;

    // Allows for smoother pitch changes
    private float smoothPitchChange = 0f;

    // Allows for smoother yaw changes
    private float smoothYawChange = 0f;

    // Maximum angle that the bird can pitch up or down
    private const float MaxPitchAngle = 80f;

    // Maximum distance from the mouth to accept food collision
    private const float MouthRadius = 0.008f;

    // Whether the agent is frozen (intentionally not flying)
    private bool frozen = false;

    /// <summary>
    /// The amount of food the agent has obtained this episode
    /// </summary>
    public float FoodObtained { get; private set; }

    /// <summary>
    /// Initializes the agent
    /// </summary>
    public override void Initialize()
    {
        rigidbody = GetComponent<Rigidbody>();
        berryArea = GameObject.Find("Food").GetComponent<BerryArea>();

        // If not training mode, no max step, play forever
        if (!trainingMode) MaxStep = 0;
    }

    /// <summary>
    /// Reset the agent when an episode begins
    /// </summary>
    public override void OnEpisodeBegin()
    {
        if (trainingMode)
        {
            // Only reset berry in training when there is one agent per area
            berryArea.ResetBerries();
        }

        // Reset food obtained
        FoodObtained = 0f;

        // Zero out velocities so that movement stops before the new episode
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        // Recalculate the nearest berry no that the agent has moved
        UpdateNearestBerry();
    }

    /// <summary>
    /// Returns true if the value is between the lower and upper limits
    /// </summary>
    /// <param name="num">The number to check against</param>
    /// <param name="lower">The lower bound</param>
    /// <param name="upper"><The upper bound/param>
    /// <param name="inclusive">Whether num can match the lower or upper bound, true to match</param>
    /// <returns>True if num is between the values, false otherwise.</returns>
    public bool Between(float num, float lower, float upper, bool inclusive = false)
        => inclusive
            ? lower <= num && num <= upper
            : lower < num && num < upper;

    /// <summary>
    /// Update the nearest berry to the agent
    /// </summary>
    private void UpdateNearestBerry()
    {
        foreach (Berry berry in berryArea.Berries)
        {
            if (Between(berry.transform.position.y, minHeightBerry, maxHeightBerry, true))
            {
                if (nearestBerry == null && berry.HasFood)
                {
                    // No current nearest berry and this berry has food, so set this berry
                    nearestBerry = berry;
                }
                else if (berry.HasFood)
                {
                    // Calculate distances
                    float distanceToBerry = Vector3.Distance(berry.transform.position, mouthPos.position);
                    float distanceToCurrentNearestBerry = Vector3.Distance(nearestBerry.transform.position, mouthPos.position);

                    // IF current is closer set nearest to current
                    if (!nearestBerry.HasFood || distanceToBerry < distanceToCurrentNearestBerry)
                    {
                        nearestBerry = berry;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called when an action is received from either the player input (heuristic) or the neural network
    /// 
    /// vectaorAction[i] represents
    /// Index 0: move vector x (+1 = right, -1 = left)
    /// Index 1: move vector y (+1 = up, -1 = down)
    /// Index 2: move vector z (+1 = forward, -1 = backwards)
    /// Index 3: pitch angle (+1 = up, -1 = down)
    /// Index 4: yaw angle (+1 turn right, -1 = turn left)
    /// </summary>
    /// <param name="vectorAction">The actions to take</param>
    public override void OnActionReceived(float[] vectorAction)
    {
        // Don't take actions if frozen
        if (frozen) return;

        // Calculate movement vector
        Vector3 move = new Vector3(vectorAction[0], 0, vectorAction[2]);

        // Add force in the direction of the move vector
        rigidbody.AddForce(move * moveForce);

        //Get the current location
        Vector3 rotationVector = transform.rotation.eulerAngles;

        //Calculate pitch and yaw rotation
        //float pitchChange = vectorAction[3];
        float yawChange = vectorAction[4];

        //Calculate smooth rotation changes
        smoothPitchChange = Mathf.MoveTowards(smoothPitchChange, 0, 2f * Time.fixedDeltaTime);
        smoothYawChange = Mathf.MoveTowards(smoothYawChange, yawChange, 2f * Time.fixedDeltaTime);

        //Calculate new pitch and yaw based on smoothed values
        //Clamp pitch to avoid flipping upside down
        float pitch = rotationVector.x + smoothPitchChange * Time.fixedDeltaTime * pitchSpeed;
        if (pitch > 180f) pitch -= 360f;
        pitch = Mathf.Clamp(pitch, -MaxPitchAngle, MaxPitchAngle);

        float yaw = rotationVector.y + smoothYawChange * Time.fixedDeltaTime * yawSpeed;

        //Apply the new rotation
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

    }

    /// <summary>
    /// When the behavior type is set to "Heuristic Only" on the agent's behavior parameters
    /// this function will be called. I'ts return values will be fed into
    /// <see cref="OnActionReceived(float[])"/> instead of using the neural network
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(float[] actionsOut)
    {
        // Create placeholders for all movements/turning
        Vector3 forward = Vector3.zero;
        Vector3 left = Vector3.zero;
        Vector3 up = Vector3.zero;
        float pitch = 0f;
        float yaw = 0f;

        // Convert keyboard inputs to movement and turning
        // All values should be betwee -1 and +1

        // forward / backward
        if (Input.GetKey(KeyCode.W)) forward = transform.forward;
        else if (Input.GetKey(KeyCode.S)) forward = -transform.forward;

        // Left/right
        if (Input.GetKey(KeyCode.A)) left = -transform.right;
        else if (Input.GetKey(KeyCode.D)) left = transform.right;

        // up/down
        if (Input.GetKey(KeyCode.E)) up = transform.up;
        else if (Input.GetKey(KeyCode.C)) up = -transform.up;

        // turn left/right
        if (Input.GetKey(KeyCode. UpArrow)) pitch = 1f;
        else if (Input.GetKey(KeyCode.DownArrow)) pitch = -1f;

        if (Input.GetKey(KeyCode.LeftArrow)) yaw = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) yaw = 1;

        // Combine the movment vectors and normalize
        Vector3 combined = (forward + left + up).normalized;

        // Set the values into the actions out array
        actionsOut[0] = combined.x;
        actionsOut[1] = combined.y;
        actionsOut[2] = combined.z;
        actionsOut[3] = pitch;
        actionsOut[4] = yaw;
    }

    /// <summary>
    /// Collect vector observations from the environment
    /// </summary>
    /// <param name="sensor">The vector sensor</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        if (nearestBerry == null)
        {
            sensor.AddObservation(new float[10]);
            return;
        }

        // Obvserve the agent's local rotation (4 observations)
        sensor.AddObservation(transform.localRotation.normalized);

        // Get a vector from the mouth to the nearest berry
        Vector3 toBerry = nearestBerry.BerryCenterPosition - mouthPos.position;

        // Observe a normalized vecotr pointing to the nearest berry (3 Observations)
        sensor.AddObservation(toBerry.normalized);

        // Observe a dot product that indicates whether the mouth is in front of the berry
        // +1 = mouthin front of the berry, -1 means behind (1 Observations)
        sensor.AddObservation(Vector3.Dot(toBerry.normalized, -nearestBerry.BerryUpVector.normalized));

        // Observe a dot product that indicates whether the mouth is pointing towards the berry  (1 Observations)
        // +1 = mouth pointing directly at the berry, -1 directly away
        sensor.AddObservation(Vector3.Dot(mouthPos.forward.normalized, -nearestBerry.BerryUpVector.normalized));

        // Observe the relative distance from the mouth to the berry (1 Observations)
        sensor.AddObservation(toBerry.magnitude / BerryArea.AreaDiameter);

        // 10 total observations
    }

    /// <summary>
    /// Prevent the agent from moving and taking actions
    /// </summary>
    public void FreezeAgent()
    {
        Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training mode.");
        frozen = true;
        rigidbody.Sleep();
    }

    /// <summary>
    /// Resume agent movement and actions
    /// </summary>
    public void UnFreezeAgent()
    {
        Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training mode.");
        frozen = false;
        rigidbody.WakeUp();
    }

    /// <summary>
    /// Called when the agent's collider enters a trigger collider
    /// </summary>
    /// <param name="other">The trigger collider</param>
    private void OnTriggerEnter(Collider other)
    {
        TriggerEnterOrStay(other);
    }

    /// <summary>
    /// Called when the agent's collider stays in a trigger collider
    /// </summary>
    /// <param name="other">The trigger collider</param>
    private void OnTriggerStay(Collider other)
    {
        TriggerEnterOrStay(other);
    }

    /// <summary>
    /// Handles when the agent's collider enters or stays in a trigger collider
    /// </summary>
    /// <param name="collider">The trigger collider</param>
    private void TriggerEnterOrStay(Collider collider)
    {
        if (collider.CompareTag("berry"))
        {
            Vector3 closestPointToMouth = collider.ClosestPoint(mouthPos.position);

            // Check if the closest collision point is close to the mouth.
            // Note: a collision with anything but the mouth should not count.
            if (Vector3.Distance(mouthPos.position, closestPointToMouth) < MouthRadius) 
            {
                // Get the berry for this food collider
                Berry berry = berryArea.GetBerryFromFood(collider);

                // Attempt to take .01 food
                // Note: this is per fixed timestep, meaning it happens every .02 seconds
                float foodReceived = berry.Feed(.01f);

                // Keep track of the food obtained
                FoodObtained += foodReceived;
                Debug.Log("Food obtained: " + FoodObtained);
                if (trainingMode)
                {
                    // Calculate reward for getting food
                    float bonus = .02f * Mathf.Clamp01(Vector3.Dot(transform.forward.normalized, -nearestBerry.BerryUpVector.normalized));
                    AddReward(.01f + bonus);
                }

                // if berry is empty, update the nearest berry
                if (!berry.HasFood)
                {
                    UpdateNearestBerry();
                }
            }
        }
    }

    ///// <summary>
    ///// Called when the agent collides with something solid
    ///// </summary>
    ///// <param name="collision">The collision info</param>
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (trainingMode && collision.collider.CompareTag("boundary"))
    //    {
    //        // Collision with area boundary give negative reward
    //        AddReward(-.5f);
    //    }
    //}

    /// <summary>
    /// Called every frame
    /// </summary>
    private void Update()
    {
        // Draw a line from the mouth to the nearest berry
        if (nearestBerry != null)
        {
            Debug.DrawLine(mouthPos.position, nearestBerry.BerryCenterPosition, Color.green);
        }
    }

    /// <summary>
    /// Called every .02 seconds
    /// </summary>
    private void FixedUpdate()
    {
        if (nearestBerry != null && !nearestBerry.HasFood)
        {
            UpdateNearestBerry();
        }
    }
}
