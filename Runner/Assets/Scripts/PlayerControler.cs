using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControler : MonoBehaviour
{
    private CharacterController controller;
    private CapsuleCollider col;
    private Animator anim;
    private Vector3 dir;
    [SerializeField] private float speed;
    [SerializeField] private float JumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private int coins;
    [SerializeField] private Text coinsText;
    [SerializeField] private GameObject losePanel;

    private bool isSliding;

    private int lineToMove = 1;
    public float lineDistance = 4;
    private const float maxSpeed = 120;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        losePanel.SetActive(false);
        controller = GetComponent<CharacterController>();
        col = GetComponent<CapsuleCollider>();
        Time.timeScale = 1;
        coins = PlayerPrefs.GetInt("coins");
        coinsText.text = coins.ToString();
        StartCoroutine(SpeedIncrease());
    }

    private void Update()
    {
        if (SwipeContoller.swipeRight)
        {
            if (lineToMove < 2)
                lineToMove++;
        }
        if (SwipeContoller.swipeLeft)
        {
            if (lineToMove > 0)
                lineToMove--;
        }
        if (SwipeContoller.swipeUp)
        {
            if (controller.isGrounded)
                Jump();
        }

        if (SwipeContoller.swipeDown)
        {
            StartCoroutine(Slide());
        }

        if (controller.isGrounded)
            anim.SetBool("Running", true);
        else
            anim.SetBool("Running", false);

        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (lineToMove == 0)
            targetPosition += Vector3.left * lineDistance;
        else if (lineToMove == 2)
            targetPosition += Vector3.right * lineDistance;

        if (transform.position == targetPosition)
            return;
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
        if (moveDir.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);
    }

    private void Jump()
    {
        
        dir.y = JumpForce;
        anim.SetTrigger("Jump");
    }

    void FixedUpdate()
    {
        dir.z = speed;
        dir.y += gravity * Time.fixedDeltaTime;
        controller.Move(dir * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "obstacle")
        {
            losePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin")
        {
            coins++;
            PlayerPrefs.SetInt("coins", coins);
            coinsText.text = coins.ToString();
            Destroy(other.gameObject);
        }
    }
    private IEnumerator SpeedIncrease()
    {
        yield return new WaitForSeconds(3);
        if (speed < maxSpeed)
        {
            speed += 2;
            StartCoroutine(SpeedIncrease());
        }
        
    }
    private IEnumerator Slide()
    {
        col.center = new Vector3(0, -0.5f, 0);
        col.height = 0;
        isSliding = true;
        anim.SetBool("Running", false);
        anim.SetTrigger("Slide");
        yield return new WaitForSeconds(1);

        col.center = new Vector3(0, 0, 0);
        col.height = 2f;
        isSliding = false;
    }
}
