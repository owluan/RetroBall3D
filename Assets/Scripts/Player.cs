using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float forceMultiplier = 5f;
    [SerializeField]
    private float maximumVelocity = 3f;
    [SerializeField]
    private ParticleSystem deathParticles;

    private Rigidbody rb;
    private CinemachineImpulseSource cinemachineImpulseSource;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        float horizontalInput = 0;
        
        if (Input.GetMouseButton(0))
        {
            var center = Screen.width / 2;
            var mousePosition = Input.mousePosition;
            if (mousePosition.x > center)
            {
                horizontalInput = 1;
            }
            else if (mousePosition.x < center)
            {
                horizontalInput = -1;
            }
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
        }        

        if (rb.velocity.magnitude <= maximumVelocity)
        {
            rb.AddForce(new Vector3(horizontalInput * forceMultiplier * Time.deltaTime, 0, 0));
        }
    }

    private void OnEnable()
    {
        transform.position = new Vector3(0, 1.68f, 0);
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            GameOver();

            Instantiate(deathParticles, transform.position, Quaternion.identity);
            cinemachineImpulseSource.GenerateImpulse();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("FallDown"))
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        GameManager.Instance.GameOver();

        gameObject.SetActive(false);
    }
}
