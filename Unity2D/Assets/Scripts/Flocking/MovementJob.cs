using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

public struct MovementJob : IJobParallelForTransform
{
    public float MaxSpeed;

    public float speed;

    public float deltaTime;

    public float minX;
                 
    public float maxX;
                 
    public float minY;
                 
    public float maxY;

    public float rotationSpeed;

    public float weightAvoidObstacles;

    public bool bounceWall;


    public NativeArray<BoidData> boidDatas;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<ObstacleStruct> obstacleStructs;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<Vector3> flockDirection;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<Vector3> flockDirectionRandom;

    public void Execute(int i, TransformAccess transform)
    {
        Vector3 targetDirection = flockDirection[i] + flockDirectionRandom[i];
        targetDirection.Normalize();

        
        targetDirection = AvoidObstacles(targetDirection, transform);
        targetDirection = CrossBorder(targetDirection, transform);
        targetDirection.Normalize();
        
        Vector3 rotatedVectorToTarget =
          Quaternion.Euler(0, 0, 90) *
          targetDirection;

        Quaternion targetRotation = Quaternion.LookRotation(
          forward: Vector3.forward,
          upwards: rotatedVectorToTarget);

        transform.rotation = Quaternion.RotateTowards(
          transform.rotation,
          targetRotation,
          rotationSpeed * deltaTime);

        Vector3 position = transform.position;


        if (speed > MaxSpeed)
            speed = MaxSpeed;


        position += speed * deltaTime * (transform.rotation * new Vector3(1f, 0f, 0f));

        transform.position = position;

        boidDatas[i] = new BoidData
        {
            position = position,
            speed = speed,
            direction = targetDirection
        };
    }


    private Vector3 AvoidObstacles(Vector3 targetDirection, TransformAccess transform)
    {
        for (int i = 0; i < obstacleStructs.Length; i++) 
        {
            //Check if boid is in the obstacle avoidance radius
            if ((obstacleStructs[i].position - transform.position).magnitude < obstacleStructs[i].avoidanceRadius)
            {
                Vector3 normalized = (transform.position - obstacleStructs[i].position).normalized;
                targetDirection += normalized * weightAvoidObstacles;
                targetDirection.Normalize();
            }
        }

        return targetDirection;
    }

    private Vector3 CrossBorder(Vector3 targetDirection, TransformAccess transform)
    {
        Vector3 pos = transform.position;
        if (bounceWall)
        {
               
            if (transform.position.x + 5.0f > maxX)
            {
                targetDirection.x = -1.0f;
            }
            if (transform.position.x - 5.0f < minX)
            {
                targetDirection.x = 1.0f;
            }
            if (transform.position.y + 5.0f > maxY)
            {
                targetDirection.y = -1.0f;
            }
            if (transform.position.y - 5.0f < minY)
            {
                targetDirection.y = 1.0f;
            }
            targetDirection.Normalize();
            
        }

        else 
        {
            if (pos.x > maxX)
            {
                pos.x = minX;
            }
            if (transform.position.x < minX)
            {
                pos.x = maxX;
            }
            if (pos.y > maxX)
            {
                pos.y = minY;
            }
            if (transform.position.y < minY)
            {
                pos.y = maxX;
            }
            transform.position = pos;
        }
        return targetDirection;
    }
}
