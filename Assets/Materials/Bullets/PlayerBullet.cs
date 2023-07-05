using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage;
    public float speed;
    public float lifetime;
    public float distance;
    public LayerMask whatIsSolid;
    public LayerMask whatIsTarget;

    void Start()
    {
        Invoke("DestroyBullet", lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hitInfo1 = Physics2D.Raycast(transform.position, transform.up, distance, whatIsSolid);
        RaycastHit2D hitInfo2 = Physics2D.Raycast(transform.position, transform.up, distance, whatIsTarget);

        if (hitInfo1.collider != null)
        {
            DestroyBullet();
        }
        if (hitInfo2.collider != null)
        {
            if (hitInfo2.collider.CompareTag("Enemy"))
            {
                hitInfo2.collider.GetComponent<TrooperScript>().TakeDamage(damage);
                DestroyBullet();
            }
        }

        transform.Translate(Vector2.up * speed * Time.deltaTime);    
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
