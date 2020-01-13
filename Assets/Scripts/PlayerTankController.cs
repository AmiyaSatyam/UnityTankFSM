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
    private float maxForwardSpeed = 30.0f;
    private float maxBackwardSpeed = -30.0f;

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

    void UpdateWeapon()
    {
        if (Input.GetMouseButtonDown(0))
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= shootRate)
            {
                elapsedTime = 0.0f;
                Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            }
        }
    }

    void UpdateControl()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position, new Vector3(0, 0, 0));
        Ray RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

        float HitDistance = 0;
        if (playerPlane.Raycast(RayCast, out HitDistance))
        {
            Vector3 RayHitPoint = RayCast.GetPoint(HitDistance);
            Quaternion targetRotation = Quaternion.LookRotation(RayHitPoint - transform.position        );

            Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);
        }

        if (Input.GetKey(KeyCode.W))
        {
            targetSpeed = maxForwardSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            targetSpeed = maxBackwardSpeed;
        }
        else
        {
            targetSpeed = 0;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -rotSpeed * Time.deltaTime, 0.0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, rotSpeed * Time.deltaTime, 0.0f);
        }

        curSpeed = Mathf.Lerp(curSpeed, targetSpeed, 7.0f * Time.deltaTime);

        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }
}
