using System;
using Photon.Pun;
using UnityEngine;

public class NetworkManger : MonoBehaviourPunCallbacks
{
    public Transform reborn1;
    public Transform reborn2;
    public Transform reborn3;
    public Transform reborn4;


    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log((int)PhotonNetwork.LocalPlayer.CustomProperties["Character"]);
        if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Character"] == 1001)
        {
            PhotonNetwork.Instantiate("Player1", reborn1.position, Quaternion.identity, 0);
        }
        else if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Character"] == 1002)
        {
            PhotonNetwork.Instantiate("Player2", reborn2.position, Quaternion.identity, 0);
        }
        else if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Character"] == 1003)
        {
            PhotonNetwork.Instantiate("Player3", reborn3.position, Quaternion.identity, 0);
        }
        else if ((int)PhotonNetwork.LocalPlayer.CustomProperties["Character"] == 1004)
        {
            PhotonNetwork.Instantiate("Player4", reborn4.position, Quaternion.identity, 0);
        }


        #region Enemy instantiate

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Slime", new Vector3(278.91896f, 3.31525993f, -44.251976f), Quaternion.identity,
                0);
            PhotonNetwork.Instantiate("Slime", new Vector3(269.337585f, 3.52012062f, -49.8059654f), Quaternion.identity,
                0);
            PhotonNetwork.Instantiate("TurtleShell", new Vector3(265.623596f, 3.92834282f, -51.9772682f),
                Quaternion.identity,
                0);
            PhotonNetwork.Instantiate("TurtleShell", new Vector3(269.874664f, 3.94546413f, -32.2069092f),
                Quaternion.identity,
                0);
            PhotonNetwork.Instantiate("Grunt", new Vector3(345.59671f, 7.27507401f, -44.8674164f), Quaternion.identity,
                0);
            PhotonNetwork.Instantiate("Grunt", new Vector3(355.078644f, 7.10513687f, -51.5740204f), Quaternion.identity,
                0);
            PhotonNetwork.Instantiate("Grunt", new Vector3(357.379395f, 8.24986458f, -41.7532196f), Quaternion.identity,
                0);
            PhotonNetwork.Instantiate("Grunt_GUARD", new Vector3(429.850006f, 11.9675884f, -28.2000008f),
                Quaternion.identity,
                0);
            PhotonNetwork.Instantiate("Grunt_GUARD", new Vector3(432.829987f, 11.9675846f, -44.6599998f),
                Quaternion.identity,
                0);
            PhotonNetwork.Instantiate("Grunt", new Vector3(412.865265f, 11.9675865f, -45.9782753f), Quaternion.identity,
                0);
            PhotonNetwork.Instantiate("Grunt", new Vector3(418.375488f, 11.9675884f, -32.4980774f), Quaternion.identity,
                0);
            PhotonNetwork.Instantiate("Polyart_Golem", new Vector3(103.636467f, 35.5420036f, 102.97451f),
                Quaternion.Euler(0, 87.4565125f, 0),
                0);
        }

        #endregion


        SaveManager.Instance.SavePlayerData();

        InventoryManager.Instance.SaveData();
    }
}