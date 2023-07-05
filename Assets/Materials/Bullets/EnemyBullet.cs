using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed;
    public float lifetime;
    public float distance;
    public float damage;
    public LayerMask whatIsSolid;

    void Start()
    {
        Invoke("DestroyBullet", lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward) * distance;
        Debug.DrawRay(transform.position, forward, Color.green);
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, distance, whatIsSolid);
        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.CompareTag("Player"))
            {
                //hitInfo.collider.GetComponent<Player>().
            }
            DestroyBullet();
        }

        transform.Translate(Vector2.up * speed * Time.deltaTime);    
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
