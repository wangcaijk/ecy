using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerSelectedNameUI : MonoBehaviourPunCallbacks
{
    public int owerID;
    public bool isReady = false;
    public Text PlayerName;
    // public static GameObject playerNameItem;

    private void Awake()
    {
        PlayerName = this.gameObject.GetComponentInChildren<Text>();
        if (photonView.IsMine)
        {
            PlayerName.text = PhotonNetwork.NickName;
            isReady = (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsReady"];
        }
        else
        {
            PlayerName.text = photonView.Owner.NickName;
            isReady = (bool)photonView.Owner.CustomProperties["IsReady"];
        }

        ChangeToReady(isReady);
    }


    public void ChangeToReady(bool isReady)
    {
        PlayerName.color = isReady == true ? Color.green : Color.white;
    }
}