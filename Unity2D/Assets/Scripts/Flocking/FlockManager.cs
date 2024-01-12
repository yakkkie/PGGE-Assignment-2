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

    public float TickDuration = 1.0f;
    public float TickDurationSeparationEnemy = 0.1f;
    public float TickDurationRandom = 1.0f;
    public float weightCohesion = 1f;
    public float weightAlignment = 1f;
    public float weightSeparation = 1f;
    public float weightAvoidObstacles = 1f;
    public float avoidanceRadiusMultiplier = 1f;
    public float visibility = 1f;
    public float separationDistance = 1f;
    public float minX;                     
    public float maxX;                     
    public float minY;                     
    public float maxY;
    public float maxSpeed = 5f;

    public int BoidIncr = 100;
    public int NumBatches = 1024;
    public int BatchSize = 100;
    public int boidsCount = 1;
    public int maxBoids = 1000000000;

    public bool useRandomRule;
    public bool useCohesionRule;
    public bool useAlignmentRule;
    public bool useSeparationRule;
    public bool useFlocking = false;
    


    private FlockingJob flockingJob;
    private MovementJob movementJob;
    private JobHandle flockingJobHandle;
    private JobHandle movementJobHandle;


    private TransformAccessArray transformAccessArray;
    private NativeArray<BoidData> boidDatas;
    [NativeDisableContainerSafetyRestriction]
    private NativeArray<Vector3> targetVelocity;
    [NativeDisableContainerSafetyRestriction]
    private NativeArray<ObstacleStruct> obstacles;
    private NativeArray<Vector3> flockDirectionRandom;


    private void OnDisable()
    {
        movementJobHandle.Complete();
        flockingJobHandle.Complete();
        transformAccessArray.Dispose();
        flockDirectionRandom.Dispose();
        obstacles.Dispose();
        targetVelocity.Dispose();
        boidDatas.Dispose();
       
    }


    void Start()
    {
        


        minX = Bounds.bounds.min.x; 
        maxX = Bounds.bounds.max.x; 
        minY = Bounds.bounds.min.y; 
        maxY = Bounds.bounds.max.y;

        Debug.Log(maxX);

        transformAccessArray = new TransformAccessArray(0);
        obstacles = new NativeArray<ObstacleStruct>(50, Allocator.Persistent);
        boidDatas = new NativeArray<BoidData>(maxBoids, Allocator.Persistent);
        flockDirectionRandom = new(maxBoids, Allocator.Persistent);
        targetVelocity = new(maxBoids, Allocator.Persistent);

        AddBoids(boidsCount);
        
        SetObstacles();
        
        StartCoroutine(Coroutine_Flocking());



    }

    

    void Update()
    {
        movementJobHandle.Complete();
        HandleInputs();
        movementJob = new MovementJob
        {
            MaxSpeed = maxSpeed,
            speed = maxSpeed,
            deltaTime = Time.deltaTime,
            bounceWall = true,
            flockDirection = targetVelocity,
            flockDirectionRandom = flockDirectionRandom,
            rotationSpeed = 100f,
            minX = minX,
            maxX = maxX,
            minY = minY,
            maxY = maxY,
            obstacleStructs = obstacles,
            weightAvoidObstacles = weightAvoidObstacles,
            boidDatas = boidDatas
        };
        movementJobHandle = movementJob.Schedule(transformAccessArray);
        JobHandle.ScheduleBatchedJobs();


    }

    void HandleInputs()
    {
        if (EventSystem.current.IsPointerOverGameObject() ||
           enabled == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
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


            GameObject gameObject = Instantiate(boidPrefab, spawnPoint, Quaternion.identity ); 
            transformAccessArray.Add(gameObject.transform);
            boidDatas[boidsCount + i] = new BoidData
            {
                position = spawnPoint,
                speed = 0,
                direction = Vector3.zero

            };

        }

        boidsCount = transformAccessArray.length;
        
    }

    

    


    


    IEnumerator Coroutine_Flocking()
    {
        while (true)
        {
            if (useFlocking && useAlignmentRule)
            {
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
                    boidDatas = boidDatas,
                    targetVelocity = targetVelocity
                    
                };
                flockingJobHandle = flockingJob.Schedule(transformAccessArray.length,NumBatches );
            }
            yield return new WaitForSeconds(TickDuration);
        }
    }


    

}
