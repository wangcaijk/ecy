using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Random = UnityEngine.Random;
using Cinemachine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    public float walkSpeed = 5;
    public float turnSmoothTime = 0.13f; //�趨��ɫת��ƽ��ʱ��
    float turnSmoothVelocity; //ƽ��������Ҫ��ôһ��ƽ�����ٶ�, ����ҪΪ����ֵ, ������Ҫ�������������������
    public float speedSmoothTime = 0.13f; //����ƽ���ٶ�
    float speedSmoothVelocity;
    float currentSpeed;

    public Transform cameraT;
    public GameObject ShootPoint;
    public Staff staff;


    [Header("===== other settings =====")] private Animator anim;
    private NavMeshAgent agent;

    private GameObject attackTarget;
    private CharacterStats characterStats;
    private float lastAttackTime;
    private bool isDead;
    private bool attacking = false;
    private float stopDistance;

    /*****����λ��ת��*******/
    private Vector3 direction;
    private Quaternion rotation;
    private RaycastHit hitInfo;
    private Vector3 hitInfoPosition;

    public bool IsMagic = false;


    private float ShootStateTime = 0;

    // private float attackRange;
    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        characterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;
        cameraT = FindObjectOfType<CinemachineFreeLook>().transform;
        //staff = GetComponentInChildren<Staff>();
        //  attackRange = characterStats.attackData.attackRange;
    }

    void OnEnable()
    {
        if (!photonView.IsMine)
            return;
        // MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        // MouseManager.Instance.OnShootState += Shoot;
        GameManager.Instance.RigisterPlayer(characterStats);
    }

    private void Start()
    {
        if (!photonView.IsMine)
            return;
        SaveManager.Instance.LoadPlayerData();
    }


    void OnDisable()
    {
        if (!MouseManager.IsInitialized) return;
        //MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
        // MouseManager.Instance.OnShootState -= Shoot;
    }

    void Update()
    {
        isDead = characterStats.CurrentHealth == 0;

        if (isDead)
        {
            GameManager.Instance.NotifyObservers();
        }

        ShootStateTime -= Time.deltaTime;
        if (!photonView.IsMine && !PhotonNetwork.IsConnected)
            return;

        if (photonView.IsMine)
        {
            KeyBoardInput();
            if (IsMagic) Shoot();
            SwitchAnimation();
            SwitchAttackWay();
        }

        lastAttackTime -= Time.deltaTime;
    }

    public void Shoot()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            //�����Ʒ����ת��

            //�����ӵ�

            if (Input.GetMouseButtonDown(0))
            {
                ShootStateTime = 1;
                RotateToMouseDirection(gameObject, hitInfo.point);

                photonView.RPC("AttackRpc", RpcTarget.All);
            }
        }
    }


    public void KeyBoardInput()
    {
        //***WASD����***//
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //��ȡ��������
        Vector2 inputDir = input.normalized; //�������Ƿ���һ����λ���ȵ��������

        //***��ɫ�ƶ�����***//
        float targetSpeed = (walkSpeed) * inputDir.magnitude;
        if (targetSpeed > 0)
        {
            StopAllCoroutines();
            agent.isStopped = true;
            attacking = false;
        }

        //Vector3 PlayerMovement = new Vector3(hor,0f,ver)*targetSpeed*Time.deltaTime;
        if (ShootStateTime <= 0)
        {
            transform.Translate(transform.forward * targetSpeed * Time.deltaTime, Space.World); //����Ϸ��ɫλ���ƶ�
            //transform.Translate(PlayerMovement,Space.Self);

            //***ת�򲿷�***//
            if (inputDir != Vector2.zero)
                //��������Ϊ�����Զ�����0, ��Ҳ������ʱ���ɫ�ͻ��Զ�����������, ���Լ�һ���ж�, ����Ϊ0�Ļ�����û����, ���ԾͲ�Ҫת��
            {
                //ƽ��ת�����
                float targetRotation =
                    Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg +
                    cameraT.eulerAngles.y; //����Ǹ�����Ҽ��������������Ŀ��ת��Ƕ�(y���)
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation,
                    ref turnSmoothVelocity, turnSmoothTime);
                //�ϱ���������ǽǶȽ���, Ҳ���Խ�ƽ����, ���ref��ʲô��˼������, �Ժ�϶��ܽ��, �������turnSmoothVelocity��ʲô��˼�Ժ������֪��, ������һʱ<br>��������//���ref�������ò��� , ѧ��C#��֪���� , ����������ѷ����ڵ����ݸ��������
            }
        }
        else
        {
            if (input.x != 0 && input.y != 0)
            {
                input.x = input.x * 0.6f;
                input.y = input.y * 0.6f;
            }

            this.transform.Translate(Vector3.right * input.x * Time.deltaTime * walkSpeed, Space.World);
            this.transform.Translate(Vector3.forward * input.y * Time.deltaTime * walkSpeed, Space.World);
        }

        /***λ���˶�����***/
        //���inputDir�Ǹ���λ����,inputDir.magnitude�����ĳ���,���κ������ʱ��λ�����ĳ��ȶ���1, �ڼ���û�������ʱ��������Ⱦ���0
        //��ʵ֮���Գ���������Ⱦ���Ϊ���ܹ������û�������ʱ����ٶȱ��0
        //currentSpeed = Mathf.SmoothDamp(currentSpeed,targetSpeed,ref speedSmoothVelocity,speedSmoothTime);
        //�������������SmoothDamp�Լ��ϱߵ�SmoothDampAngleһ��, ���ǵĵ�һ��������ʵ�Ǳ���ֵ��, ֱ�Ӱѿղ�������ȥ, ���ܻ�ú��ʵ�ֵ
        float animationSpeedPercent = 1f * inputDir.magnitude;
        anim.SetFloat("Speed", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }

    private void SwitchAnimation()
    {
        if (attacking)
        {
            anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        }

        anim.SetBool("Death", isDead);
    }

    private void SwitchAttackWay()
    {
        if (IsMagic)
        {
            MouseManager.Instance.OnEnemyClicked -= EventAttack;
        }
        else
        {
            MouseManager.Instance.OnEnemyClicked += EventAttack;
        }
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (isDead) return;
        agent.stoppingDistance = stopDistance;
        // characterStats.attackData.attackRange = attackRange;
        agent.isStopped = false;
        agent.destination = target;
    }

    private void EventAttack(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        attacking = true;
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;
        //characterStats.attackData.attackRange = attackTarget.GetComponent<CharacterStats>().attackData.attackRange;
        transform.LookAt(attackTarget.transform);


        while (Vector3.Distance(attackTarget.transform.position, transform.position) >
               characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        attacking = false;
        agent.isStopped = true;
        //Attack

        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //CDtime
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>() &&
                attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    #region PUN CallBack

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //����
            stream.SendNext(characterStats.CurrentHealth);
        }
        else
        {
            //����
            characterStats.CurrentHealth = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void AttackRpc()
    {
        //if (staff != null)
        staff.Attack();
    }

    #endregion


    private void RotateToMouseDirection(GameObject obj, Vector3 destination)
    {
        direction = new Vector3(destination.x - obj.transform.position.x, 0,
            destination.z - obj.transform.position.z);
        rotation = Quaternion.LookRotation(direction);
        obj.transform.localRotation = rotation;
    }
}