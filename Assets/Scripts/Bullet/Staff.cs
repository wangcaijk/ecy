using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Staff : MonoBehaviourPun
{
    public GameObject shootPoint;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Attack()
    {
        ObjectPool.poolInstance.position = shootPoint.transform.position;
        ObjectPool.poolInstance.Get().transform.forward = shootPoint.transform.forward;
    }
}