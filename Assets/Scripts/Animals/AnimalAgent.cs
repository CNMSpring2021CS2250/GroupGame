using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AnimalAgent : Agent
{
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

    // The flower area the agent is in
    private FlowerArea flowerArea;

    // The nearest flower to the agent
    private Berry nearestFlower;

    // Allows for smoother pitch changes
    private float smoothPitchChange = 0f;

    // Allows for smoother yaw changes
    private float smoothYawChange = 0f;

    // Maximum angle that the bird can pitch up or down
    private const float MaxPitchAngle = 80f;

    // Maximum distance from the beak tip to accept nectar collision
    private const float BeakTipRadius = 0.008f;

    // Whether the agent is frozen (intentionally not flying)
    private bool frozen = false;

    /// <summary>
    /// The amount of nectar the agent has obtained this episode
    /// </summary>
    public float NectarObtained { get; private set; }

    /// <summary>
    /// Initializes the agent
    /// </summary>
    public override void Initialize()
    {
        rigidbody = GetComponent<Rigidbody>();
        flowerArea = GameObject.Find("Food").GetComponent<FlowerArea>();

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
            // Only reset flower in training when there is one agent per area
            flowerArea.ResetFlowers();
        }

        // Reset nectar obtained
        NectarObtained = 0f;

        // Zero out velocities so that movement stops before the new episode
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        // Default to spawning in front of a flower
        bool inFrontOfFlower = true;

        if (trainingMode)
        {
            // Spawn in front of the flower 50% of the time during training
            inFrontOfFlower = Random.value > .5f;
        }

        // Move the agent to a new random position
        MoveToSafeRandomPosition(inFrontOfFlower);

        // Recalculate the nearest flower no that the agent has moved
        UpdateNearestFlower();
    }

    /// <summary>
    /// Move the agent to a safe random position
    /// (I.E. does not collide with anything)
    /// If in front of flower, also point the beak at the flower
    /// </summary>
    /// <param name="inFrontOfFlower">Whether to choose a spot in front of a flower</param>
    private void MoveToSafeRandomPosition(bool inFrontOfFlower)
    {
        bool safePositionFound = false;
        int attemptsRemaining = 100; // PRevent infinit loop
        Vector3 potentialPosition = Vector3.zero;
        Quaternion potentialRotation = new Quaternion();

        // Loop until a safe position is found or run out of attempts
        while (!safePositionFound && attemptsRemaining > 0)
        {
            attemptsRemaining--;
            if (inFrontOfFlower)
            {
                // Pick a random flower
                Berry randomFlower = flowerArea.Flowers[Random.Range(0, flowerArea.Flowers.Count)];

                // Position 10 to 20 cm in front of the flower
                float distanceFromFlower = Random.Range(.1f, .2f);
                potentialPosition = randomFlower.transform.position + randomFlower.FlowerUpVector * distanceFromFlower;

                // Point beak at the flower
                Vector3 toFlower = randomFlower.FlowerCenterPosition - potentialPosition;
                potentialRotation = Quaternion.LookRotation(toFlower, Vector3.up);
            }
            else
            {
                // Pick a random height from the ground
                float height = Random.Range(1.2f, 2.5f);
                // Pick a random radius from the center of the flower area
                float radius = Random.Range(2f, 7f);
                // Pick a random direction rotated around the y axis
                Quaternion direction = Quaternion.Euler(0f, Random.Range(-180, 180f), 0f);
                // Combine height, radius and direction to pick position
                potentialPosition = flowerArea.transform.position = Vector3.up * height + direction * Vector3.forward * radius;

                // Choose and set random starting pitch and yaw
                float pitch = Random.Range(-60f, 60f);
                float yaw = Random.Range(-180f, 180f);
                potentialRotation = Quaternion.Euler(pitch, yaw, 0f);
            }

            // Check to see if the angent will collide with anything
            Collider[] colliders = Physics.OverlapSphere(potentialPosition, 0.05f);

            // Safe position has been found if no colliders are overlapped
            safePositionFound = colliders.Length == 0;
        }

        Debug.Assert(safePositionFound, "Could not find a safe position to spawn");
    }

    /// <summary>
    /// Update the nearest flower to the agent
    /// </summary>
    private void UpdateNearestFlower()
    {
        foreach (Berry flower in flowerArea.Flowers)
        {
            if (nearestFlower == null && flower.HasNectar)
            {
                // No current nearest flower and this flower has nectar, so set this flower
                nearestFlower = flower;
            }
            else if (flower.HasNectar)
            {
                // Calculate distances
                float distanceToFlower = Vector3.Distance(flower.transform.position, mouthPos.position);
                float distanceToCurrentNearestFlower = Vector3.Distance(nearestFlower.transform.position, mouthPos.position);

                // IF current is closer set nearest to current
                if (!nearestFlower.HasNectar || distanceToFlower < distanceToCurrentNearestFlower)
                {
                    nearestFlower = flower;
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
        Vector3 move = new Vector3(vectorAction[0], vectorAction[1], vectorAction[2]);

        // Add force in the direction of the move vector
        rigidbody.AddForce(move * moveForce);

        //Get the current location
        Vector3 rotationVector = transform.rotation.eulerAngles;

        //Calculate pitch and yaw rotation
        float pitchChange = vectorAction[3];
        float yawChange = vectorAction[4];

        //Calculate smooth rotation changes
        smoothPitchChange = Mathf.MoveTowards(smoothPitchChange, pitchChange, 2f * Time.fixedDeltaTime);
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
        if (nearestFlower == null)
        {
            sensor.AddObservation(new float[10]);
            return;
        }

        // Obvserve the agent's local rotation (4 observations)
        sensor.AddObservation(transform.localRotation.normalized);

        // Get a vector from the beak tip to the nearest flower
        Vector3 toFlower = nearestFlower.FlowerCenterPosition - mouthPos.position;

        // Observe a normalized vecotr pointing to the nearest flower (3 Observations)
        sensor.AddObservation(toFlower.normalized);

        // Observe a dot product that indicates whether the beak tip is in front of the flower
        // +1 = beak tip in front of the flower, -1 means behind (1 Observations)
        sensor.AddObservation(Vector3.Dot(toFlower.normalized, -nearestFlower.FlowerUpVector.normalized));

        // Observe a dot product that indicates whether the beak is pointing towards the flower  (1 Observations)
        // +1 = beak is pointing directly at the flower, -1 directly away
        sensor.AddObservation(Vector3.Dot(mouthPos.forward.normalized, -nearestFlower.FlowerUpVector.normalized));

        // Observe the relative distance from the beak tip to the flower (1 Observations)
        sensor.AddObservation(toFlower.magnitude / FlowerArea.AreaDiameter);

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
            Vector3 closestPointToBeakTip = collider.ClosestPoint(mouthPos.position);

            // Check if the closest collision point is close to the beak tip.
            // Note: a collision with anything but the beaktip should not count.
            if (Vector3.Distance(mouthPos.position, closestPointToBeakTip) < BeakTipRadius) 
            {
                // Get the flower for this nectar collider
                Berry flower = flowerArea.GetFlowerFromNectar(collider);

                // Attempt to take .01 nectar
                // Note: this is per fixed timestep, meaning it happens every .02 seconds
                float nectarReceived = flower.Feed(.01f);

                // Keep track of the nectar obtained
                NectarObtained += nectarReceived;
                Debug.Log("Nectar obtained: " + NectarObtained);
                if (trainingMode)
                {
                    // Calculate reward for getting nectar
                    float bonus = .02f * Mathf.Clamp01(Vector3.Dot(transform.forward.normalized, -nearestFlower.FlowerUpVector.normalized));
                    AddReward(.01f + bonus);
                }

                // if flower is empty, update the nearest flower
                if (!flower.HasNectar)
                {
                    UpdateNearestFlower();
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
        // Draw a line from the beak tip to the nearest flower
        if (nearestFlower != null)
        {
            Debug.DrawLine(mouthPos.position, nearestFlower.FlowerCenterPosition, Color.green);
        }
    }

    /// <summary>
    /// Called every .02 seconds
    /// </summary>
    private void FixedUpdate()
    {
        if (nearestFlower != null && !nearestFlower.HasNectar)
        {
            UpdateNearestFlower();
        }
    }
}
