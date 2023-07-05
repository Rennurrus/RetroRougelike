using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aiming : MonoBehaviour
{

    public Joystick joystick;
    public float offset;
    public float rotMult = 1;

    public float timeCooldownBtwShots;
    public float startTimeCooldownBtwShots;
    public GameObject bullet;
    public Transform shotPoint;
    

    private void Start() 
    {
        joystick          = GameObject.Find("Canvas").GetComponent<JoysticksObjects>().aimJoystick;
        bullet            = (GameObject) Resources.Load ("Bullets/PlayerBullet", typeof(GameObject));
        shotPoint         = transform.GetChild(2).transform; 
    }

    void Update()
    {
        float rotationZ = Mathf.Atan2(rotMult * joystick.Vertical ,  rotMult * joystick.Horizontal) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f , 0f, rotMult == -1 && rotationZ == -180 ? 0 : rotationZ );

        Shot(rotationZ, joystick);
    }

    void Shot(float rotationZ, Joystick joystick)
    {
        if (timeCooldownBtwShots <= 0 && (joystick.Vertical != 0 || joystick.Horizontal != 0))
        {   
            Instantiate(bullet, shotPoint.position, Quaternion.Euler(0f , 0f, rotationZ + (rotMult == 1 ? -90 : +90)));

            timeCooldownBtwShots = startTimeCooldownBtwShots;
        }
        else if (timeCooldownBtwShots > 0)
        {
            timeCooldownBtwShots -= Time.deltaTime;
        }
    }
}
