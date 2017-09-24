using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveForce = 365f;
    public float maxSpeed = 5f;
    public float jumpForce = 1000f;
    public AudioClip pickupSound;
    public AudioClip deathSound;
    public Transform groundCheck;
    public Text countText;
    public Text winText;


    private bool facingRight = true;
    private bool jump = false;
    private bool grounded = false;
    private int count;
    private int totalCount;
    private Animator anim;
    private Rigidbody2D rb2d;
    private AudioSource audioSource;


    // Use this for initialization
    void Awake()
    {
        count = 0;
        totalCount = GameObject.FindGameObjectsWithTag("PickUp").Length;
        winText.text = "";
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent <AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        anim.SetBool("Grounded", grounded);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            jump = true;
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");

        anim.SetFloat("Speed", Mathf.Abs(h));

        if (h * rb2d.velocity.x < maxSpeed)
            rb2d.AddForce(Vector2.right * h * moveForce);

        if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);

        if (h > 0 && !facingRight)
            Flip();
        else if (h < 0 && facingRight)
            Flip();

        if (jump)
        {
            audioSource.Play();
            rb2d.AddForce(new Vector2(0f, jumpForce));
            jump = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            audioSource.PlayOneShot(pickupSound);
            other.gameObject.SetActive(false);
            count++;
            updateCountText();
            checkWon();
        }
        if (other.gameObject.CompareTag("Death"))
        {
            audioSource.PlayOneShot(deathSound);
            rb2d.AddForce(new Vector2(300f, 300f));
            anim.SetBool("Dead", true);
            StartCoroutine(reload());

        }
    }

    private void checkWon()
    {
        if (count == totalCount)
        {
            Time.timeScale = 0;
            winText.text = "You Won!";

        }
    }

    private IEnumerator reload()
    {
        yield return new WaitForSeconds(1);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void updateCountText()
    {
        countText.text = "Score: " + count.ToString();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}