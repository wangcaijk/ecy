using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using JetBrains.Annotations;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class CharacterSelection : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject selectingCharacter = null;
    [SerializeField] private GameObject PlayerName;
    [SerializeField] public GameObject[] playerList;

    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;


    private GameObject DefaultSelected;
    public Transform defaultPos;

    public Button readyBtn;
    private bool IsReady = false;

    ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();


    void Awake()
    {
        MouseManager.Instance.OnCharacterClicked += EventSelected;
        PhotonNetwork.ConnectUsingSettings();
        readyBtn.onClick.AddListener(OnReadyBtn);
        table.Add("IsReady", IsReady);
        table.Add("Character", 1001);
        PhotonNetwork.LocalPlayer.SetCustomProperties(table);
    }

    private void Start()
    {
    }

    public void OnPlayerReady(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        playerList = GameObject.FindGameObjectsWithTag("PlayerName");
        if (!PlayerName)
        {
            DefaultSelected = PhotonNetwork.Instantiate("PlayerSelectCanvas",
                new Vector3(defaultPos.position.x, defaultPos.position.y + (float)1.7, defaultPos.position.z),
                Quaternion.identity, 0);
            PlayerName = DefaultSelected;
            PlayerSelectedNameUI item = PlayerName.GetComponent<PlayerSelectedNameUI>();
            item.owerID = PhotonNetwork.LocalPlayer.ActorNumber;
            selectingCharacter = defaultPos.gameObject;
        }
        else
        {
            UpdateReady(targetPlayer, changedProps);
        }
    }

    private void UpdateReady(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            PhotonView item = playerList[i].GetComponent<PhotonView>();
            if (item.Owner.ActorNumber == targetPlayer.ActorNumber)
            {
                PlayerSelectedNameUI PSNitem = item.gameObject.GetComponent<PlayerSelectedNameUI>();
                PSNitem.isReady = (bool)changedProps["IsReady"];
                PSNitem.ChangeToReady((bool)changedProps["IsReady"]);
            }
        }
    }

    private void OnReadyBtn()
    {
        IsReady = !IsReady;
        table["IsReady"] = IsReady;
        if (selectingCharacter == player1)
        {
            table["Character"] = (int)1001;
        }
        else if (selectingCharacter == player2)
        {
            table["Character"] = (int)1002;
        }
        else if (selectingCharacter == player3)
        {
            table["Character"] = (int)1003;
        }
        else if (selectingCharacter == player4)
        {
            table["Character"] = (int)1004;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(table); //设置自定义参数
    }


    private void EventSelected(GameObject character)
    {
        if (IsReady)
        {
            return;
        }

        if (!character)
        {
            return;
        }

        if (DefaultSelected)
        {
            PhotonNetwork.Destroy(DefaultSelected.gameObject);
        }

        if (character != selectingCharacter)
        {
            if (selectingCharacter != null)
            {
                PhotonNetwork.Destroy(PlayerName.gameObject);
            }

            UpdateNamePos(character);
        }

        else if (selectingCharacter == character)
        {
            PhotonNetwork.Destroy(PlayerName.gameObject);
            selectingCharacter = null;
        }
    }

    private void UpdateNamePos(GameObject character)
    {
        if (character)
        {
            PlayerName = PhotonNetwork.Instantiate("PlayerSelectCanvas",
                new Vector3(character.transform.position.x, character.transform.position.y + (float)1.7,
                    character.transform.position.z), Quaternion.identity, 0);

            PlayerName.transform.SetParent(character.transform);
        }

        PlayerSelectedNameUI item = PlayerName.GetComponent<PlayerSelectedNameUI>();
        item.owerID = PhotonNetwork.LocalPlayer.ActorNumber;
        selectingCharacter = character;
    }


    private void OnDestroy()
    {
        // MouseManager.Instance.OnCharacterClicked -= EventSelected;
    }
}