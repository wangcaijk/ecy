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
    public float turnSmoothTime = 0.13f; //设定角色转向平滑时间
    float turnSmoothVelocity; //平滑函数需要这么一个平滑加速度, 不需要为他赋值, 但是需要把这个变量当参数传入
    public float speedSmoothTime = 0.13f; //用于平滑速度
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

    /*****发射位置转向*******/
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
            //鼠标控制发射点转向

            //发射子弹

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
        //***WASD输入***//
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //获取键盘输入
        Vector2 inputDir = input.normalized; //反正就是返回一个单位长度的这个变量

        //***角色移动部分***//
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
            transform.Translate(transform.forward * targetSpeed * Time.deltaTime, Space.World); //让游戏角色位置移动
            //transform.Translate(PlayerMovement,Space.Self);

            //***转向部分***//
            if (inputDir != Vector2.zero)
                //可能是因为键盘自动收入0, 玩家不输入的时候角色就会自动面向正方向, 所以加一个判定, 输入为0的话就是没输入, 所以就不要转向
            {
                //平滑转向代码
                float targetRotation =
                    Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg +
                    cameraT.eulerAngles.y; //这就是根据玩家键盘输入算出来的目标转向角度(y轴的)
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation,
                    ref turnSmoothVelocity, turnSmoothTime);
                //上边这个函数是角度渐变, 也可以叫平滑吧, 这个ref是什么意思还存疑, 以后肯定能解决, 还有这个turnSmoothVelocity是什么意思以后迟早能知道, 不急于一时<br>　　　　//这个ref就是引用参数 , 学点C#就知道了 , 用这个参数把方法内的数据给保存出来
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

        /***位置运动部分***/
        //这个inputDir是个单位向量,inputDir.magnitude是他的长度,有任何输入的时候单位向量的长度都是1, 在键盘没有输入的时候这个长度就是0
        //其实之所以乘以这个长度就是为了能够在玩家没有输入的时候把速度变成0
        //currentSpeed = Mathf.SmoothDamp(currentSpeed,targetSpeed,ref speedSmoothVelocity,speedSmoothTime);
        //看起来这个函数SmoothDamp以及上边的SmoothDampAngle一样, 他们的第一个参数其实是被赋值的, 直接把空参数传进去, 就能获得合适的值
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
            //发送
            stream.SendNext(characterStats.CurrentHealth);
        }
        else
        {
            //接受
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