using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Jobs;
using System.Threading.Tasks;
using System.ComponentModel;
using Unity.Collections;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst;

[BurstCompile]
public struct FlockingJob : IJobParallelFor
{

    // Weights for different flocking rules.
    public float weightAlignment, weightSeparation, weightCohesion;

    // Distance at which separation rule takes effect.
    public float separationDistance;

    // Visibility range for boid interactions.
    public float visibility;

    // Flags to enable/disable specific flocking rules.
    public bool useAlignmentRule, useSeparationRule, useCohesionRule;

    // Total number of boids in the flock.
    public int boidsCount;

    // NativeArrays to store boid data and target velocity.
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<BoidData> boidDatas;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<Vector3> targetVelocity;

    #region Execute
    // Execute method from the IJobParallelFor interface.
    public void Execute(int i)
    {
        // Initialize variables for alignment, separation, and cohesion directions.
        Vector3 flockDir = Vector3.zero;
        Vector3 separationDir = Vector3.zero;
        Vector3 cohesionDir = Vector3.zero;

        // Initialize variables for boid speed, separation speed, and steering position.
        float speed = 0.0f;
        float separationSpeed = 0.0f;
        int count = 0;
        int separationCount = 0;
        Vector3 steerPos = Vector3.zero;

        // Iterate through all boids to calculate flocking behaviors.
        for (int j = 0; j < boidsCount; j++)
        {
            // Calculate the distance between the current boid and others.
            float dist = (boidDatas[i].position - boidDatas[j].position).magnitude;

            // Apply alignment and cohesion rules within visibility range.
            if (i != j && dist < visibility)
            {
                speed += boidDatas[j].speed;
                flockDir += boidDatas[j].direction;
                steerPos += boidDatas[j].position;
                count++;
            }

            // Apply separation rule within separation distance.
            if (i != j && dist < separationDistance)
            {
                Vector3 targetDirection = (boidDatas[i].position - boidDatas[j].position).normalized;
                separationDir += targetDirection;
                separationSpeed += dist * weightSeparation;
                separationCount++;
            }
        }

        // Calculate average values for alignment and cohesion.
        if (count > 0)
        {
            speed /= (float)count;
            flockDir /= (float)count;
            flockDir.Normalize();
            steerPos /= (float)count;
        }

        // Normalize separation direction and speed.
        if (separationCount > 0)
        {
            separationSpeed /= (float)count;
            separationDir /= separationSpeed;
            separationDir.Normalize();
        }

        // Calculate the final target velocity based on flocking rules.
        targetVelocity[i] = flockDir * speed * (useAlignmentRule ? weightAlignment : 0f)
            + separationDir * separationSpeed * (useSeparationRule ? weightSeparation : 0f)
            + (steerPos - boidDatas[i].position) * (useCohesionRule ? weightCohesion : 0f);
    }

    #endregion

}
