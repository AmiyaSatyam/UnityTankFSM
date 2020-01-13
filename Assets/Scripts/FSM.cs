using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{
    protected Transform playerTransform;
    protected Vector3 destPos;
    public GameObject Bullet;
    protected GameObject[] pointList;

    protected float shootRate;
    protected float elapsedTime;


    private float curSpeed;
    private float curRotSpeed;
    private bool isDead;
    private int health;

    public enum FSMState
    {
        None,
        Patrol,
        Chase,
        Attack,
        Dead,
    }

    public FSMState curState;

    public Transform turret { get; set; }
    public Transform bulletSpawnPoint { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        FSMUpdate();
    }

    void FixedUpdate()
    {

    }

    protected void Initialize()
    {
        curState = FSMState.Patrol;
        curSpeed = 1500.0f;
        curRotSpeed = 2.0f;
        isDead = false;
        elapsedTime = 0.0f;
        shootRate = 3.0f;
        health = 100;
        destPos = new Vector3(0.0f, 0.0f, 0.0f);

        pointList = GameObject.FindGameObjectsWithTag("WandarPoint");

        FindNextPoint();
        GameObject[] objPlayer = GameObject.FindGameObjectsWithTag("PlayerTank");
        playerTransform = objPlayer[0].transform;

        if (!playerTransform)
        {
            print("Player doesn't exist..Please add one " + "with Tag named 'Player'");
        }


        turret = gameObject.transform.GetChild(0).transform;
        bulletSpawnPoint = turret.GetChild(0).transform;
    }

    protected void FSMUpdate()
    {
        switch (curState)
        {
            case FSMState.Patrol: UpdatePatrolState(); break;
            case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Attack: UpdateAttackState(); break;
            case FSMState.Dead: UpdateDeadState(); break;
        }

        elapsedTime += Time.deltaTime;

        if (health < 0)
        {
            curState = FSMState.Dead;
        }
    }

    protected void UpdatePatrolState()
    {
        if (Vector3.Distance(transform.position, destPos) <= 100.0f)
        {
            print("Reached to the destination point\n" + "Calculating the next point");

            FindNextPoint();
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) <= 300.0f)
        {
            print("Switch to Chase position");
            curState = FSMState.Chase;
        }

        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    protected void FindNextPoint()
    {
        print("Finding Next Point");
        int randIndex = Random.Range(0, pointList.Length);
        float randRadius = 10.0f;
        Vector3 randPosition = Vector3.zero;
        destPos = pointList[randIndex].transform.position + randPosition;

        if (IsInCurrentRange(destPos))
        {
            randPosition = new Vector3(Random.Range(-randRadius, randRadius), 0.0f, Random.Range(-randRadius, randRadius));
            destPos = pointList[randIndex].transform.position + randPosition;
        }
    }

    protected bool IsInCurrentRange(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);

        if (xPos <= 50 && zPos <= 50)
            return true;
        return false;
    }

    protected void UpdateChaseState()
    {
        destPos = playerTransform.position;
        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist <= 200.0f)
        {
            curState = FSMState.Attack;
        }
        else if (dist >= 300.0f)
        {
            curState = FSMState.Patrol;
        }

        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    protected void UpdateAttackState()
    {
        destPos = playerTransform.position;
        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist >= 200.0f && dist < 300.0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);

            transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
            curState = FSMState.Attack;
        }
        else if (dist >= 300.0f)
        {
            curState = FSMState.Patrol;
        }

        Quaternion turretRotation = Quaternion.LookRotation(destPos - turret.position);
        turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * curRotSpeed);

        ShootBullet();
    }

    private void ShootBullet()
    {
        if (elapsedTime >= shootRate)
        {
            Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            elapsedTime = 0.0f;
        }
    }

    protected void UpdateDeadState()
    {
        if (!isDead)
        {
            isDead = true;
            Explode();
        }
    }

    protected void Explode()
    {
        float randX = Random.Range(10.0f, 30.0f);
        float randZ = Random.Range(10.0f, 30.0f);
        for (int i = 0; i < 3; i++)
        {
            GetComponent<Rigidbody>().AddExplosionForce(10000.0f, transform.position - new Vector3(randX, 10.0f, randZ), 40.0f, 10.0f);
            GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(randX, 20.0f, randZ));
        }

        Destroy(gameObject, 1.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            health -= collision.gameObject.GetComponent<Bullet>().damage;
        }
    }
}
