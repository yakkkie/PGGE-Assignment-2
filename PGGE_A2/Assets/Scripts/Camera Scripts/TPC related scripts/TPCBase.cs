using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PGGE
{
    // The base class for all third-person camera controllers
    public abstract class TPCBase
    {
        protected Transform mCameraTransform;
        protected Transform mPlayerTransform;
        protected LayerMask mMask;
        protected Vector3 mOffset;
        protected Camera mCamera;
        

        public Transform CameraTransform
        {
            get
            {
                return mCameraTransform;
            }
        }
        public Transform PlayerTransform
        {
            get
            {
                return mPlayerTransform;
            }
        }

        public TPCBase(Transform cameraTransform, Transform playerTransform, LayerMask mask, Vector3 offset)
        {
            mCameraTransform = cameraTransform;
            mPlayerTransform = playerTransform;
            mMask = mask;
            mOffset = offset;
            mCamera = mCameraTransform.GetComponent<Camera>();
            

        }

        public void RepositionCamera()
        {
            //-------------------------------------------------------------------
            // Implement here.
            //-------------------------------------------------------------------
            //-------------------------------------------------------------------
            // Hints:
            //-------------------------------------------------------------------
            // check collision between camera and the player.
            // find the nearest collision point to the player
            // shift the camera position to the nearest intersected point
            //-------------------------------------------------------------------


            //gets the "player" so i can edit the position of it without affecting anything
            Vector3 player = mPlayerTransform.position;
            player.y += CameraConstants.playerHeight * 2;   //adds the player height so the "player" position will be at the head of the player

            float distance = Vector3.Distance(player, mCameraTransform.position); // distance between the camera and the player 

            Quaternion cameraRotation = mCameraTransform.rotation;
            Vector3 cameraForward = cameraRotation * Vector3.forward; //get the direction the camera is looking towards

            Vector3 direction = (mCameraTransform.position - player).normalized;
            Vector3 offset = mOffset;

            Debug.DrawRay(player, direction * distance, Color.red);

            if (Physics.Raycast(player, direction, out RaycastHit hit, distance, mMask)) //Raycast checks if theres an object between the player and the camera, if there is, return true
            {


                offset = hit.point; //gets the exact point that the ray cast hits the object

                CameraTransform.position = offset; //set the camera's position to the point where the ray hit the wall

            }


            








            //if (Physics.BoxCast(
            //mCameraTransform.position, CameraHalfExtends, direction, out RaycastHit hit,
            //cameraRotation, distance
            //))
            //{


            //    offset = hit.point - mPlayerTransform.position;



            //    Debug.Log(hit.point);

            //}


            //CameraConstants.CameraPositionOffset = Vector3.Lerp(CameraConstants.CameraPositionOffset, offset, 0.005f);









        }


        //Vector3 CameraHalfExtends
        //{
        //    get
        //    {
        //        Vector3 halfExtends;
        //        halfExtends.y =
        //            mCamera.nearClipPlane *
        //            Mathf.Tan(0.5f * Mathf.Deg2Rad * mCamera.fieldOfView);
        //        halfExtends.x = halfExtends.y * mCamera.aspect;
        //        halfExtends.z = 0f;
        //        return halfExtends;
        //    }
        //}

        Vector3 CameraHalfExtends
        {
            get
            {
                float fov = mCamera.fieldOfView * 0.5f; // Convert FOV to half FOV.
                float aspect = mCamera.aspect; // Aspect ratio (width / height).
                float halfHeight = Mathf.Tan(fov * Mathf.Deg2Rad) * mCamera.nearClipPlane;
                float halfWidth = halfHeight * aspect;
                float halfDepth = 0.5f * (mCamera.farClipPlane - mCamera.nearClipPlane);

                return new Vector3(halfWidth, halfHeight, halfDepth);
            }
        }


        public abstract void Update();
    }
}
