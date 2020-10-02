using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float forwardForce = 2000f;
    public float sidewaysForce = 1500f;

    private void FixedUpdate()
    {
        rb.AddForce(0, 0, forwardForce * Time.deltaTime);
              
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.AddForce(- sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }     
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.AddForce(sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
        }

        if (rb.position.y <= 0)
        {
            FindObjectOfType<GameManager>().EndGame();
        }
    }
}
