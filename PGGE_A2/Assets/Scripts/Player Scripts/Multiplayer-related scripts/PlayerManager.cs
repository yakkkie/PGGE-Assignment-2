using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public string[] mPlayerPrefabName;
    public PlayerSpawnPoints mSpawnPoints;

    [HideInInspector]
    public GameObject mPlayerGameObject;
    
    

    [HideInInspector]
    private ThirdPersonCamera mThirdPersonCamera;
    [HideInInspector]
    private string playerPref_CharacterKey = "Character";
    

    private void Start()
    {
        //get the character chosen by the player at the multiplayer lobby page
        string characterChosen = PlayerPrefs.GetString(playerPref_CharacterKey);
        Transform randomSpawnTransform = mSpawnPoints.GetSpawnPoint();

        
        
        
        //use the character chosen to spawn in the game world for the character to use.
        mPlayerGameObject = PhotonNetwork.Instantiate(characterChosen,
            randomSpawnTransform.position,
            randomSpawnTransform.rotation,
            0);

        //Sets the camera to track and follow the newly instantiated player
        SetCameraToFollowPlayer();
    }

    public void LeaveRoom()
    {
        Debug.LogFormat("LeaveRoom");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        //Debug.LogFormat("OnLeftRoom()");
        SceneManager.LoadScene("Menu");
    }

    public void SetCameraToFollowPlayer()
    {
        //set the reference to the main camera
        mThirdPersonCamera = Camera.main.gameObject.AddComponent<ThirdPersonCamera>();

        //set the new player game object's transform
        mThirdPersonCamera.mPlayer = mPlayerGameObject.transform;

        //add some damping to the camera
        mThirdPersonCamera.mDamping = 20.0f;

        //set the camera  type to be following and tracking the player's position and follows the player's rotations
        mThirdPersonCamera.mCameraType = CameraType.Follow_Track_Pos_Rot;
    }

}
