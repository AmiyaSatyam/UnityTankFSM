using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFSM : FSM
{
    public enum FSMState
    {
        None,
        Patrol,
        Chase,
        Attack,
        Dead,
    }

    public FSMState curState;
    private float curSpeed;
    private float curRotSpeed;
    public GameObject Bullet;

    private bool isDead;
    private int health;

    protected override void Initialize()
    {
        curState = FSMState.Patrol;
        curSpeed = 150.0f;
        curRotSpeed = 2.0f;
        isDead = false;
        elapsedTime = 0.0f;
        shootRate = 3.0f;
        health = 100;

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

    protected override void FSMUpdate()
    {
        switch (curState)
        {
            case FSMState.Patrol : UpdatePatrolState(); break;
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
    // Start is called before the first frame update
    private void Start()
    {

    }
}

    // Update is called once per frame
    void Update()
    {
        
    }
}
