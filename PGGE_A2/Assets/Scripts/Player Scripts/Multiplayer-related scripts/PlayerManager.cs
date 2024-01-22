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
        string characterChosen = PlayerPrefs.GetString(playerPref_CharacterKey);
        Transform randomSpawnTransform = mSpawnPoints.GetSpawnPoint();

        
        
        
        
        mPlayerGameObject = PhotonNetwork.Instantiate(characterChosen,
            randomSpawnTransform.position,
            randomSpawnTransform.rotation,
            0);

        mThirdPersonCamera = Camera.main.gameObject.AddComponent<ThirdPersonCamera>();

        //mPlayerGameObject.GetComponent<PlayerMovement>().mFollowCameraForward = false;
        mThirdPersonCamera.mPlayer = mPlayerGameObject.transform;
        mThirdPersonCamera.mDamping = 20.0f;
        mThirdPersonCamera.mCameraType = CameraType.Follow_Track_Pos_Rot;
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

}
