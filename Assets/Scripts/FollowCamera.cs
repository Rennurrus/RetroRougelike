using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public float pointX;
    public float pointY;
    public float pointZ;

    public Joystick aimJoystick;
    public Joystick moveJoystick;
    public Transform target;
    public float smoothTime = 0.3F;
    public float flipMult = 1f;

    private Vector3 velocity = Vector3.zero;

    void Start() {
        //target          = GameObject.FindGameObjectWithTag("Player").transform;
        moveJoystick    = GameObject.Find("Canvas").GetComponent<JoysticksObjects>().moveJoystick;
        aimJoystick     = GameObject.Find("Canvas").GetComponent<JoysticksObjects>().aimJoystick;
    }
    

    void Update()
    {
        /*if (target == null)
        {
            target          = GameObject.FindGameObjectWithTag("Player").transform;
        }*/
        // Define a target position above and behind the target transform
        Vector3 targetPosition = new Vector3(pointX + target.position.x + (aimJoystick.Horizontal ) + (moveJoystick.Horizontal * 2f), pointY + target.position.y + aimJoystick.Vertical  * 2f, pointZ);
        //Vector3 targetPosition = target.TransformPoint( new Vector3(pointX + (flipMult *  aimJoystick.Horizontal ) + (flipMult *  moveJoystick.Horizontal * 2f), pointY + aimJoystick.Vertical  * 2f, pointZ));

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        //transform.position = target.position + new Vector3(pointX + aimJoystick.Horizontal, pointY + aimJoystick.Vertical, pointZ);

    }

    public void setTarget(Transform target)
    {
        this.target = target;
    }
}
