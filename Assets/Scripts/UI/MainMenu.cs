using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Photon.Pun;
using Photon.Realtime;
using WebSocketSharp;

public class MainMenu : MonoBehaviourPunCallbacks
{
    Button newGameBtn;
    Button continueBtn;
    Button quitBtn;

    PlayableDirector director;

    void Awake()
    {
        newGameBtn = transform.GetChild(0).GetComponent<Button>();
        continueBtn = transform.GetChild(1).GetComponent<Button>();
        quitBtn = transform.GetChild(2).GetComponent<Button>();


        newGameBtn.onClick.AddListener(Connect);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;
    }

    void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.ConnectUsingSettings();
            newGameBtn.gameObject.SetActive(false);
            continueBtn.gameObject.SetActive(false);
            quitBtn.gameObject.SetActive(false);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            newGameBtn.gameObject.SetActive(false);
            continueBtn.gameObject.SetActive(false);
            quitBtn.gameObject.SetActive(false);
        }
    }

    void PlayTimeline()
    {
        director.Play();
    }

    void NewGame(PlayableDirector obj)
    {
        PlayerPrefs.DeleteAll();
        //转换场景
        SceneController.Instance.TransitionToFirstLevel();
    }

    void ContinueGame()
    {
        //转换场景
        SceneController.Instance.TransitionToLoadGame();
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log("退出游戏");
    }
}