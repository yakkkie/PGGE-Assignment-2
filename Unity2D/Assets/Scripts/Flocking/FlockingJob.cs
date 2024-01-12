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
    
    public float weightAlignment, weightSeparation, weightCohesion, separationDistance, visibility;

    public bool useAlignmentRule, useSeparationRule, useCohesionRule;

    public int boidsCount;

    [NativeDisableContainerSafetyRestriction]
    public NativeArray<BoidData> boidDatas;

    [NativeDisableContainerSafetyRestriction]
    public NativeArray<Vector3> targetVelocity; 

    public void Execute(int i)
    {
        Vector3 flockDir = Vector3.zero;
        Vector3 separationDir = Vector3.zero;
        Vector3 cohesionDir = Vector3.zero;
        
        float speed = 0.0f;
        float separationSpeed = 0.0f;

        int count = 0;
        int separationCount = 0;
        Vector3 steerPos = Vector3.zero;


        for (int j = 0; j < boidsCount; j++)
        {
            float dist = (boidDatas[i].position - boidDatas[j].position).magnitude;
            if (i != j && dist < visibility)
            {
                speed += boidDatas[j].speed;
                flockDir += boidDatas[j].direction;
                steerPos += boidDatas[j].position;
                count++;
            }
            if (i != j && dist < separationDistance)
            {
                Vector3 targetDirection = (boidDatas[i].position - boidDatas[j].position).normalized;
                separationDir += targetDirection;
                separationSpeed += dist * weightSeparation;
            }
        }
        if (count > 0)
        {
            speed /= (float)count;
            flockDir /= (float)count;
            flockDir.Normalize();
            steerPos /= (float)count;
        }
        if (separationCount > 0)
        {
            separationSpeed /= (float)count;
            separationDir /= separationSpeed;
            separationDir.Normalize();
        }

        targetVelocity[i] = flockDir * speed * (useAlignmentRule ? weightAlignment : 0f) 
            + separationDir * separationSpeed * (useSeparationRule ? weightSeparation : 0f) 
            + (steerPos - boidDatas[i].position) * (useCohesionRule ? weightCohesion : 0f);
    }


}
