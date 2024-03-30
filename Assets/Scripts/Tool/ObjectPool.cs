using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class ObjectPool : MonoBehaviourPunCallbacks
{
    public static ObjectPool poolInstance;

    //����λ��
    public Vector3 position;
    public Quaternion rotation;


    // ����Ҫ�洢������
    public GameObject Object;

    // �ڴ��������У�
    public Queue<GameObject> objectPool = new Queue<GameObject>();

    // ���ӵĳ�ʼ����
    public int defaultCount = 16;

    // ���ӵ��������
    public int maxCount = 5;

    private void Awake()
    {
        poolInstance = this;
    }

    // �Գ��ӽ��г�ʼ����������ʼ�������������壩
    public void Init()
    {
        GameObject obj;
        for (int i = 0; i < defaultCount; i++)
        {
            obj = PhotonNetwork.Instantiate("Projectile 13", position, Quaternion.identity, 0);
            // �����ɵĶ������
            objectPool.Enqueue(obj);
            obj.SetActive(false);
        }
    }

    // �ӳ�����ȡ������
    public GameObject Get()
    {
        GameObject tmp;
        // ��������������壬�ӳ���ȡ��һ������
        if (objectPool.Count > 0)
        {
            // ���������
            tmp = objectPool.Dequeue();
            tmp.transform.position = position;
            tmp.transform.rotation = rotation;
            tmp.SetActive(true);
        }
        // ���������û�����壬ֱ���½�һ������
        else
        {
            // tmp = PhotonNetwork.Instantiate("Projectile 13", position, Quaternion.identity, 0);
            tmp = Instantiate(Object, position, rotation);
        }

        return tmp;
    }

    // ��������ս�����
    public void Remove(GameObject obj)
    {
        // �����е�������Ŀ�������������
        if (objectPool.Count <= maxCount)
        {
            // �ö���û���ڶ�����
            if (!objectPool.Contains(obj))
            {
                // ���������
                objectPool.Enqueue(obj);
                obj.SetActive(false);
            }
        }
        // �����������������
        else
        {
            Destroy(obj);
        }
    }
}