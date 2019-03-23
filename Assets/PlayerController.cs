using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody2D rb;
    private Animator anim;

    private enum State { idle, running, jumping }
    private State state = State.idle;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        float hDirection = Input.GetAxis("Horizontal");

        if (hDirection < 0) {
            rb.velocity = new Vector2(-5, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1); // -1 for flipping sprite
            
        } else if (hDirection > 0) {
            rb.velocity = new Vector2(5, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
            
        } else {
            
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            rb.velocity = new Vector2(rb.velocity.x, 10);
            state = State.jumping;
        }

        VelocityState();
        anim.SetInteger("state", (int)state);
    }

    private void VelocityState() {

        if (state == State.jumping) {

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
