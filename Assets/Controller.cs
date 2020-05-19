using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controler : MonoBehaviour
{
    
    private Vector2 vel;
    private bool isSpinning = false;
    private bool canSpin = false;
    public GameObject Phase1;
    public GameObject Phase2;
    public float moveSpeed = 3;
    public float jumpSize = 3;
    public int maxJump = 2;
    private int jumpCount = 0;
    private int _currentPhase = 1;
    public Vector3 direction = new Vector3(1, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        ResetPhase();
    }
    void ResetPhase()
    {
        Phase1.SetActive(_currentPhase == 1);
        Phase2.SetActive(_currentPhase == 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSpinning)
        {
            var scaledMoveSpeed = moveSpeed * Time.deltaTime;
            transform.position += direction * scaledMoveSpeed * vel.x;
        }

    }
    public void OnCollisionEnter(Collision collision)
    {
        jumpCount = 0;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SpinEnable")
        {
            canSpin = true;
            this.GetComponentInChildren<ParticleSystem>().Play();
        }
        if (other.tag == "Finish")
        {
            Debug.Log("Winner");
        }
    }
    public void OnTriggerExit(Collider other)
    {
        canSpin = false;
        this.GetComponentInChildren<ParticleSystem>().Stop();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        vel =  context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Performed)
        {
            if (vel.y>0 && !isSpinning && canSpin)
            {
                isSpinning = true;
                StartCoroutine("Spin");
            }
            //Debug.Log("performed:" + context.ToString());
        }       
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && jumpCount< maxJump)
        {
            this.GetComponent<Rigidbody>().AddForce(Vector3.up* jumpSize, ForceMode.Impulse);
            jumpCount++;
        }
    }
    IEnumerator Spin()
    {

        Vector3 start = transform.rotation.eulerAngles;
        Phase1.SetActive(true);
        Phase2.SetActive(true);

        if (_currentPhase == 1)
        {
            for (float f = 0; f <= 90; f += 5)
            {
                transform.rotation = Quaternion.Euler(start.x, start.y + f, start.z);
                yield return new WaitForSeconds(0.02f);
            }
            direction = Quaternion.AngleAxis(90, Vector3.up) * direction;
        }
        else
        {
            for (float f = 0; f >= -90; f -= 5)
            {
                transform.rotation = Quaternion.Euler(start.x, start.y + f, start.z);
                yield return new WaitForSeconds(0.02f);
            }
            direction = Quaternion.AngleAxis(-90, Vector3.up) * direction;
        }
       
        _currentPhase = _currentPhase == 1 ? 2 : 1;
        ResetPhase();
        isSpinning = false;
    }
}