using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

[BurstCompile]
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

    public float weightRandom;

    public bool bounceWall;

    public bool randomRule;

    public Unity.Mathematics.Random random;



    public NativeArray<BoidData> boidDatas;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<ObstacleStruct> obstacleStructs;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<Vector3> flockDirection;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<Vector3> flockVelocity;

    public void Execute(int i, TransformAccess transform)
    {
        Vector3 targetDirection = flockDirection[i] + flockVelocity[i];
        targetDirection.Normalize();

        if (randomRule)
        {
            targetDirection = SetRandomMovement(targetDirection, transform);
        }
        
        targetDirection = CrossBorder(targetDirection, transform);
        targetDirection = AvoidObstacles(targetDirection, transform);
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


    Vector3 SetRandomMovement(Vector3 targetDirection, TransformAccess transform)
    {
        float rand = random.NextFloat(0f,2f);

        float angle = transform.rotation.eulerAngles.z;

        if (rand > 0.5f)
        {
            angle +=  45.0f;
        }
        else
        {
            angle -= 45.0f;
        }
        Vector3 dir = Vector3.zero;
        dir.x = Mathf.Cos((float)Math.PI / 180f * angle);
        dir.y = Mathf.Sin((float)Math.PI / 180f * angle);

        targetDirection += dir * weightRandom;

        return targetDirection;
    }

}
