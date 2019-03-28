using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int cherries = 0;
    [SerializeField] private Text cherryText;

    private enum State { idle, running, jumping, falling }
    private State state = State.idle;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
	}
	
	// Update is called once per frame
	void Update () {
        Movement();
        AnimationState();
        anim.SetInteger("state", (int)state);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Collectable") {
            cherries += 1;
            Destroy(collision.gameObject);
            cherryText.text = cherries.ToString();
        }
    }

    private void Movement() {
        float hDirection = Input.GetAxis("Horizontal");

        // moving left
        if (hDirection < 0) {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1); // -1 for flipping sprite

        }
        // moving right
        else if (hDirection > 0) {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);

        }

        // jumping
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground)) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            state = State.jumping;
        }
    }

    private void AnimationState() {

        if (state == State.jumping) {
            if (rb.velocity.y < -.1f) {
                state = State.falling;
            }
        }
        else if (state == State.falling) {
            if (coll.IsTouchingLayers(ground)) {
                state = State.idle;
            }
        }
        //note: Don't use 0 for comparison. Mathf.Epsilon is effectively zero in terms of float number
        else if (Mathf.Abs(rb.velocity.x) > 2f) {
            // moving
            state = State.running;
        } else {
            state = State.idle;
        }
    }
}
