using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTankController : MonoBehaviour
{
    public GameObject Bullet;

    private Transform Turret;
    private Transform bulletSpawnPoint;
    private float curSpeed, targetSpeed, rotSpeed;
    private float turretRotSpeed = 10.0f;
    private float maxForwardSpeed = 300.0f;
    private float maxBackwardSpeed = -300.0f;

    protected float shootRate = 0.5f;
    protected float elapsedTime;
    // Start is called before the first frame update
    void Start()
    {
        rotSpeed = 150.0f;
        Turret = gameObject.transform.GetChild(0).transform;
        bulletSpawnPoint = Turret.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWeapon();
        UpdateControl();
    }
}
