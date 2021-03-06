﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;
    
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int cherries = 0;
    [SerializeField] private TextMeshProUGUI cherryTextPro;
    [SerializeField] private float hurtForce = 10f;
    [SerializeField] private AudioSource cherry;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private int health;
    [SerializeField] private Text healthAmount;

    private enum State { idle, running, jumping, falling, hurt }
    private State state = State.idle;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        healthAmount.text = health.ToString();
	}
	
	// Update is called once per frame
	void Update () {
        if (state != State.hurt) {
            Movement();
        }
        AnimationState();
        anim.SetInteger("state", (int)state);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Collectable") {
            cherry.Play();
            cherries += 1;
            Destroy(collision.gameObject);
            cherryTextPro.text = cherries.ToString();
        } else if (collision.tag == "Powerup") {
            Destroy(collision.gameObject);
            jumpForce = 25f;
            GetComponent<SpriteRenderer>().color = Color.yellow;
            StartCoroutine(ResetPower());
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Enemy") {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (state == State.falling) {
                //Destroy(other.gameObject);
                enemy.JumpedOn();
                Jump();
            } else {
                state = State.hurt;
                HandleHealth(); // deals with health update ui resets level if health < 0
                if (other.gameObject.transform.position.x > transform.position.x) {
                    // enemy is to my right: get damage, move to the left
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else {
                    // enemy is to my left: get damage, move to the right
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }

        }
    }

    private void HandleHealth() {
        health -= 1;
        healthAmount.text = health.ToString();
        if (health <= 0) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
            Jump();
        }
    }

    private void Jump() {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
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
        else if (state == State.hurt) {
            if (Mathf.Abs(rb.velocity.x) < .1f) {
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

    private void Footstep() {
        footstep.Play();
    }

    private IEnumerator ResetPower() {
        yield return new WaitForSeconds(5);
        jumpForce = 15;
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
