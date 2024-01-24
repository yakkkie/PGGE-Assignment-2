using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

[BurstCompile]
public struct MovementJob : IJobParallelForTransform
{
    // Maximum speed limit for boids.
    public float MaxSpeed;

    // Current speed of the boid.
    public float speed;

    // Time since the last frame.
    public float deltaTime;

    // Minimum and maximum boundaries for boid movement.
    public float minX, maxX, minY, maxY;

    // Rotation speed for boids.
    public float rotationSpeed;

    // Weight for avoiding obstacles.
    public float weightAvoidObstacles;

    // Weight for introducing random movement.
    public float weightRandom;

    // Flag to enable bouncing off walls.
    public bool bounceWall;

    // Flag to enable random movement rule.
    public bool randomRule;

    // Random number generator for introducing randomness.
    public Unity.Mathematics.Random random;

    #region Native Arrays
    // NativeArrays to store boid data and obstacle information.
    public NativeArray<BoidData> boidDatas;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<ObstacleStruct> obstacleStructs;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<Vector3> flockDirection;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<Vector3> flockVelocity;
    #endregion

    #region Execute
    public void Execute(int i, TransformAccess transform)
    {
        // Calculate the target direction based on flocking rules.
        Vector3 targetDirection = flockDirection[i] + flockVelocity[i];
        targetDirection.Normalize();

        // Apply random movement rule if enabled.
        if (randomRule)
        {
            targetDirection = SetRandomMovement(targetDirection, transform);
        }

        //apply border rules
        targetDirection = CrossBorder(targetDirection, transform);

        //apply avoid obstacles rules
        targetDirection = AvoidObstacles(targetDirection, transform);
        targetDirection.Normalize();

        //calculating the rotation of the boids
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


        //calculate the movement of the boid
        Vector3 position = transform.position;


        if (speed > MaxSpeed)
            speed = MaxSpeed;


        position += speed * deltaTime * (transform.rotation * new Vector3(1f, 0f, 0f));

        transform.position = position;

        //set the new boid position and target direction in to the boid data for further use
        boidDatas[i] = new BoidData
        {
            position = position,
            speed = speed,
            direction = targetDirection
        };
    }
    #endregion

    #region Avoid Obstacle Rule
    private Vector3 AvoidObstacles(Vector3 targetDirection, TransformAccess transform)
    {
        //for each obstacle 
        for (int i = 0; i < obstacleStructs.Length; i++)
        {
            //Check if boid is in the obstacle avoidance radius
            if ((obstacleStructs[i].position - transform.position).magnitude < obstacleStructs[i].avoidanceRadius)
            {
                //if boid is a range, calculate the boid direction to move away from the obstacle
                Vector3 normalized = (transform.position - obstacleStructs[i].position).normalized;
                targetDirection += normalized * weightAvoidObstacles;
                targetDirection.Normalize();
            }
        }

        return targetDirection;
    }
    #endregion

    #region Cross Boundary Rule

    // Handle boundary conditions (bounce or wrap around).
    private Vector3 CrossBorder(Vector3 targetDirection, TransformAccess transform)
    {
        Vector3 pos = transform.position;
        // Check if bouncing off walls is enabled.
        if (bounceWall)
        {
            // Bounce off the walls if near the boundary.
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
            // Wrap around the world if exceeding boundaries.
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
    #endregion


    #region RandomMovement
    // Introduce random movement based on a probability.
    private Vector3 SetRandomMovement(Vector3 targetDirection, TransformAccess transform)
    {
        // Generate a random value between 0 and 2.
        float rand = random.NextFloat(0f, 2f);

        // Get the current rotation angle of the boid.
        float angle = transform.rotation.eulerAngles.z;

        // Adjust the angle based on random probability.
        if (rand > 0.5f)
        {
            angle += 45.0f;
        }
        else
        {
            angle -= 45.0f;
        }

        // Convert the angle to a direction vector.
        Vector3 dir = Vector3.zero;
        dir.x = Mathf.Cos((float)Math.PI / 180f * angle);
        dir.y = Mathf.Sin((float)Math.PI / 180f * angle);

        // Adjust the target direction based on random movement.
        targetDirection += dir * weightRandom;

        return targetDirection;

    }
    #endregion
}