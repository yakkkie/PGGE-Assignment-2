using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Patterns;
using Unity.Collections;
using UnityEngine.Jobs;
using UnityEngine.UIElements;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe;

public class FlockManager : MonoBehaviour
{

    [SerializeField]
    GameObject[] prefabObstacles;

    [SerializeField]
    BoxCollider2D Bounds;

    [SerializeField]
    private GameObject boidPrefab;

    #region Parameters

    // Time interval for flocking updates.
    public float TickDuration = 1.0f;

    // Time interval for separation rule updates.
    public float TickDurationSeparationEnemy = 0.1f;

    // Flocking weights for cohesion, alignment, and separation rules.
    public float weightCohesion = 1f;
    public float weightAlignment = 1f;
    public float weightSeparation = 1f;

    // Weight for obstacle avoidance.
    public float weightAvoidObstacles = 1f;

    // Weight for random movement.
    public float weightRandom;

    // Multiplier for obstacle avoidance radius.
    public float avoidanceRadiusMultiplier = 1f;

    // Visibility range for boid interactions.
    public float visibility = 1f;

    // Distance at which separation rule takes effect.
    public float separationDistance = 1f;

    // Boundaries for boid movement.
    public float minX, maxX, minY, maxY;

    // Maximum speed for boids.
    public float maxSpeed = 5f;

    // Rotation speed for boids.
    public float rotationSpeed;

    // Initial number of boids.
    public int BoidIncr = 100;

    // Flocking parameters.
    public int NumBatches = 1024;
    public int BatchSize = 100;
    public int boidsCount = 1;
    public int maxBoids = 1000000000;

    // Flocking rule flags.
    public bool useRandomRule;
    public bool useCohesionRule;
    public bool useAlignmentRule;
    public bool useSeparationRule;
    public bool useFlocking = false;
    #endregion

    #region Jobs
    private FlockingJob flockingJob;
    private MovementJob movementJob;
    private JobHandle flockingJobHandle;
    private JobHandle movementJobHandle;
    #endregion

  
    private System.Random random;

    #region NativeArrays
    private TransformAccessArray transformAccessArray;
    private NativeArray<BoidData> boidDatas;
    [NativeDisableContainerSafetyRestriction]
    private NativeArray<Vector3> targetVelocity;
    [NativeDisableContainerSafetyRestriction]
    private NativeArray<ObstacleStruct> obstacles;
    private NativeArray<Vector3> targetDirection;
    private NativeArray<BoidData> boidDatasLastFrame;
    #endregion

   

    private void OnDisable()
    {
        movementJobHandle.Complete();
        flockingJobHandle.Complete();
        transformAccessArray.Dispose();
        targetDirection.Dispose();
        obstacles.Dispose();
        targetVelocity.Dispose();
        boidDatas.Dispose();
        boidDatasLastFrame.Dispose();
    }

    
    void Start()
    {
        random = new System.Random();

        // Set boundaries based on the collider.
        minX = Bounds.bounds.min.x;
        maxX = Bounds.bounds.max.x;
        minY = Bounds.bounds.min.y;
        maxY = Bounds.bounds.max.y;

        // Initialize arrays for job execution.
        transformAccessArray = new TransformAccessArray(0);
        obstacles = new NativeArray<ObstacleStruct>(50, Allocator.Persistent);
        boidDatas = new NativeArray<BoidData>(maxBoids, Allocator.Persistent);
        boidDatasLastFrame = new NativeArray<BoidData>(maxBoids, Allocator.Persistent);
        targetDirection = new NativeArray<Vector3>(maxBoids, Allocator.Persistent);
        targetVelocity = new NativeArray<Vector3>(maxBoids, Allocator.Persistent);

        // Add initial boids and set obstacles.
        AddBoids(boidsCount);
        SetObstacles();

        // Start the flocking coroutine.
        StartCoroutine(Coroutine_Flocking());
    }

    

    void Update()
    {
        //ensures that the job is completed before scheduling the job again
        movementJobHandle.Complete();
        HandleInputs();
        boidDatasLastFrame.CopyFrom(boidDatas);

        // Create a new MovementJob and schedule it.
        movementJob = new MovementJob
        {
            MaxSpeed = maxSpeed,
            speed = maxSpeed,
            deltaTime = Time.deltaTime,
            bounceWall = true,
            flockDirection = targetDirection,
            flockVelocity = targetVelocity,
            rotationSpeed = rotationSpeed,
            minX = minX,
            maxX = maxX,
            minY = minY,
            maxY = maxY,
            obstacleStructs = obstacles,
            weightAvoidObstacles = weightAvoidObstacles,
            boidDatas = boidDatas,
            random = new Unity.Mathematics.Random((uint)random.Next()),
            weightRandom = weightRandom,
            randomRule = useRandomRule
        };

        movementJobHandle = movementJob.Schedule(transformAccessArray);
        JobHandle.ScheduleBatchedJobs();
    }

    

    void HandleInputs()
    {
        if (EventSystem.current.IsPointerOverGameObject() || enabled == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Add more boids on space key press.
            AddBoids(BoidIncr);
        }
    }

  

    void SetObstacles()
    {
        // Randomize obstacles placement.
        for (int i = 0; i < prefabObstacles.Length; ++i)
        {
            GameObject gameObject = prefabObstacles[i];

            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            gameObject.transform.position = new Vector3(x, y, 0.0f);

            CircleCollider2D mCollider = gameObject.GetComponent<CircleCollider2D>();

            float avoidanceRadius = 5f;

            if (mCollider != null)
            {
                avoidanceRadius = mCollider.radius;
            }

            obstacles[i + 1] = new ObstacleStruct
            {
                position = gameObject.transform.position,
                avoidanceRadius = avoidanceRadius * avoidanceRadiusMultiplier
            };
        }
    }

   

    void AddBoids(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            float x = Random.Range(Bounds.bounds.min.x, Bounds.bounds.max.x);
            float y = Random.Range(Bounds.bounds.min.y, Bounds.bounds.max.y);
            Vector3 spawnPoint = new Vector3(x, y, 0);

            // Instantiate a new boid and add it to the transformAccessArray.
            GameObject gameObject = Instantiate(boidPrefab, spawnPoint, Quaternion.identity);
            transformAccessArray.Add(gameObject.transform);

            // Initialize the boid data in the native array.
            boidDatas[boidsCount + i] = new BoidData
            {
                position = spawnPoint,
                speed = 0,
                direction = Vector3.zero
            };
        }

        // Update the total boids count.
        boidsCount = transformAccessArray.length;
    }


    #region Coroutine
    IEnumerator Coroutine_Flocking()
    {
        while (true)
        {
            // Check if flocking is enabled and alignment rule is used.
            if (useFlocking && useAlignmentRule && flockingJobHandle.IsCompleted)
            {
                // Create a new FlockingJob and schedule it.
                flockingJob = new FlockingJob
                {
                    boidsCount = transformAccessArray.length,
                    useAlignmentRule = useAlignmentRule,
                    useCohesionRule = useCohesionRule,
                    useSeparationRule = useSeparationRule,
                    weightCohesion = weightCohesion,
                    visibility = visibility,
                    weightSeparation = weightSeparation,
                    separationDistance = separationDistance,
                    weightAlignment = weightAlignment,
                    boidDatas = boidDatasLastFrame,
                    targetVelocity = targetVelocity
                };
                flockingJobHandle = flockingJob.Schedule(transformAccessArray.length, NumBatches);
            }

            // Wait for the specified time interval before the next iteration.
            yield return new WaitForSeconds(TickDuration);
        }
    }

    #endregion


}
