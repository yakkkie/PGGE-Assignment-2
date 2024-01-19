using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PGGE
{
    public class TPCTrack : TPCBase
    {
        public TPCTrack(Transform cameraTransform, Transform playerTransform, LayerMask mask, Vector3 offset)
            : base(cameraTransform, playerTransform, mask, offset)
        {
        }

        public override void Update()
        {
            Vector3 targetPos = mPlayerTransform.position;
            targetPos.y += CameraConstants.CameraPositionOffset.y;
            mCameraTransform.LookAt(targetPos);
        }
    }
}
