using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;

public class SceneController : Singleton<SceneController>, IEndGameObserver
{
    //public GameObject playerPrefab;
    public SceneFader sceneFaderPrefab;

    //public NetworkLauncher networkLauncher;
    private bool fadeFinished;
    private GameObject player;
    private NavMeshAgent playerAgent;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }

    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;

            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));

                break;
        }
    }

    private IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        //TODO;保存数据
        SaveManager.Instance.SavePlayerData();
        InventoryManager.Instance.SaveData();

        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            //yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position,GetDestination(destinationTag).transform.rotation);
            //读取数据
            SaveManager.Instance.LoadPlayerData();
            yield break;
        }

        player = GameManager.Instance.playerStats.gameObject;
        playerAgent = player.GetComponent<NavMeshAgent>();
        playerAgent.enabled = false;
        player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position,
            GetDestination(destinationTag).transform.rotation);
        playerAgent.enabled = true;
        yield return null;
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        for (var i = 0; i < entrances.Length; i++)
            if (entrances[i].destinationTag == destinationTag)
                return entrances[i];


        return null;
    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("OutSide"));
    }

    public void LoginToFirstLevel()
    {
        StartCoroutine(NetLoadLevel());
    }

    private IEnumerator LoadLevel(string scene)
    {
        var fade = Instantiate(sceneFaderPrefab);
        if (scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(2f));
            yield return SceneManager.LoadSceneAsync(scene);
            //yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position,GameManager.Instance.GetEntrance().rotation);

            //保存数据

            yield return StartCoroutine(fade.FadeIn(2f));
            yield break;
        }
    }

    private IEnumerator NetLoadLevel()
    {
        var fade = Instantiate(sceneFaderPrefab);

        yield return StartCoroutine(fade.FadeOut(0.5f));
        PhotonNetwork.LoadLevel(1);
        //yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position,GameManager.Instance.GetEntrance().rotation);
        //保存数据
        yield return StartCoroutine(fade.FadeIn(0.5f));
    }

    private IEnumerator LoadMain()
    {
        var fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2f));
        PhotonNetwork.LoadLevel(0);
        yield return StartCoroutine(fade.FadeIn(2f));
    }
}