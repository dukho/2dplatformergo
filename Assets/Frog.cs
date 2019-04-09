using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour {
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;

    [SerializeField] private float jumpLength = 15f;
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] private LayerMask ground;

    private Animator anim;

    private Collider2D coll;
    private Rigidbody2D rb;

    private bool facingLeft = true;

    // Use this for initialization
    void Start () {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (anim.GetBool("Jumping")) {
            if (rb.velocity.y < .1) {
                anim.SetBool("Falling", true);
                anim.SetBool("Jumping", false);
            }
        }

        if (coll.IsTouchingLayers(ground) && anim.GetBool("Falling")) {
            anim.SetBool("Falling", false);
        }
    }

    private void Move() {
        if (facingLeft) {
            // test to see if we are beyond the leftCap
            if (transform.position.x > leftCap) {
                // make sure sprite is facing right location, and if it is not, then face the right direction
                if (transform.localScale.x != 1) {
                    transform.localScale = new Vector3(1, 1);
                }

                // test to see if I am on the ground, it so jump
                if (coll.IsTouchingLayers(ground)) {
                    // jump to the left!
                    rb.velocity = new Vector2(-jumpLength, jumpHeight);
                    anim.SetBool("Jumping", true);
                }
            }
            else {
                facingLeft = false;
            }
        }
        else {
            // test to see if we are beyond the rightCap
            if (transform.position.x < rightCap) {
                if (transform.localScale.x != -1) {
                    transform.localScale = new Vector3(-1, 1);
                }

                // test to see if I am on the ground, it so jump
                if (coll.IsTouchingLayers(ground)) {
                    // jump to the right!
                    rb.velocity = new Vector2(jumpLength, jumpHeight);
                    anim.SetBool("Jumping", true);
                }
            }
            else {
                facingLeft = true;
            }
        }
    }

    public void JumpedOn() {
        anim.SetTrigger("Death");
    }

    private void Death() {
        Destroy(this.gameObject);
    }
}
