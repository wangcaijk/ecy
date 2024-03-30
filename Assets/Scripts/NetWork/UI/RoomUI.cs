using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class RoomUI : MonoBehaviourPunCallbacks
{
    public GameObject NamePrefab;
    public Text roomNum;
    public Button closeBtn;
    public Button startBtn;
    public Transform gridLayout;
    public CharacterSelection characterSelection;

    //private List<string> p = new List<string>();

    private void Awake()
    {
        // PhotonNetwork.ConnectUsingSettings();
        UpdatePlayerList();
        roomNum.text = "Room:" + PhotonNetwork.CurrentRoom.Name;
        closeBtn.onClick.AddListener(onCloseBtn);
        startBtn.onClick.AddListener(onStartBtn);

        PhotonNetwork.AutomaticallySyncScene = true; //loadleve时，让房间里其他玩家都加载场景
    }

    void onCloseBtn()
    {
        //断开连接
        PhotonNetwork.Disconnect();
        SceneController.Instance.TransitionToMain();
    }

    void onStartBtn()
    {
        //加载场景 让房间里的玩家也加载场景
        PhotonNetwork.LoadLevel(2);
    }

    private void UpdatePlayerList()
    {
        //清理玩家
        for (int i = 0; i < gridLayout.childCount; i++)
        {
            Destroy(gridLayout.GetChild(i).gameObject);
        }

        if (PhotonNetwork.PlayerList.Length == 0)
            return;
        //生产玩家列表
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            GameObject obj = Instantiate(NamePrefab, gridLayout.position, Quaternion.identity);
            obj.transform.SetParent(gridLayout);
            Text nameText = obj.transform.gameObject.GetComponent<Text>();
            nameText.text = PhotonNetwork.PlayerList[i].NickName;
        }
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newplayer)
    {
        base.OnPlayerEnteredRoom(newplayer);
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UpdatePlayerList();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        characterSelection.OnPlayerReady(targetPlayer, changedProps);
        Debug.Log((int)PhotonNetwork.LocalPlayer.CustomProperties["Character"]);
        canStart();
    }


    private void canStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (!(bool)PhotonNetwork.PlayerList[i].CustomProperties["IsReady"])
                {
                    startBtn.gameObject.SetActive(false);
                    return;
                }
            }

            startBtn.gameObject.SetActive(true);
            Debug.Log("游戏开始");
        }
    }
}