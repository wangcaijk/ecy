using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class NetworkLauncher : MonoBehaviourPunCallbacks
{
    public GameObject loginUI;
    public GameObject nameUI;
    public InputField roomName;
    public InputField playerName;

    public GameObject roomListUI;

    private void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        nameUI.SetActive(true);
        Debug.Log("Connected to The Master");
        PhotonNetwork.JoinLobby();
    }

    public void PlayButton()
    {
        nameUI.SetActive(false);
        PhotonNetwork.NickName = playerName.text;
        loginUI.SetActive(true);
        if (PhotonNetwork.InLobby)
        {
            roomListUI.SetActive(true);
        }
    }

    public void JoinOrCreateButton()
    {
        if (roomName.text.Length < 2)
            return;

        loginUI.SetActive(false);

        var options = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom(roomName.text, options, default);
        Debug.Log("Create the Room");
    }


    public override void OnJoinedRoom()
    {
        SceneController.Instance.LoginToFirstLevel();
        Debug.Log("join the Room");
    }
}