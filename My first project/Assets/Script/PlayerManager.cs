﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerManager : MonoBehaviour
{        
    public float movePower = 10f;
    public float KickBoardMovePower = 15f;
    public float jumpPower = 20f;
    public Tilemap ground2;
    //지형효과 적용 후 이동&점프 파워
    private float FmovePower;
    private float FjumpPower;
    private Rigidbody2D rb;
    private Animator anim;
    Vector3 movement;
    private int direction = 1;
    bool isJumping = false;
    bool isGrounded = false;
    private bool alive = true;
    private int deathdepth = -100;
    public InteractionObject interObj {set {_interObj = value;}}
    private InteractionObject _interObj;
    private GimmicGround currentGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (currentGround != null)
        {
            //currentGround의 효과를 받아와서 적용
            FmovePower = currentGround.groundSpeedValue * movePower;
            FjumpPower = currentGround.groundJumpValue * jumpPower;
        }
        
        else
        {
            FmovePower = movePower;
            FjumpPower = jumpPower;
        }
        
        if (!PauseMenu.GameIsPaused && alive)
        {
            Run();
            Jump();
            Die();
            if (Input.GetKeyDown(KeyCode.F))
            {
                    Interaction();
            }
        }

        if (transform.position.x < -100 && transform.position.x > -102 && transform.position.y < -45 && transform.position.y > -49) {
            // Tilemap이 속한 GameObject를 가져옴
            GameObject tilemapGameObject = ground2.gameObject;

            // Tilemap을 활성화 또는 비활성화
            tilemapGameObject.SetActive(false);
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        anim.SetBool("isJump", false);
    }

    private void Interaction()
    {
        if(_interObj == null) return;
        _interObj.Interaction();
        anim.SetTrigger("attack");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 객체에 GimmicGround 컴포넌트가 있다면
        GimmicGround gimmicGround = collision.gameObject.GetComponent<GimmicGround>();
        if (gimmicGround != null)
        {
            // currentGround를 충돌한 객체의 GimmicGround로 설정
            currentGround = gimmicGround;
            isGrounded = true;
        }
        else if (collision.gameObject.tag == "Box") {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // 충돌이 끝난 객체가 currentGround라면 currentGround를 null로 설정
        GimmicGround gimmicGround = collision.gameObject.GetComponent<GimmicGround>();
        if (gimmicGround == currentGround)
        {
            currentGround = null;
            isGrounded = false;
        }
        else if(collision.gameObject.tag == "Box") {
            isGrounded = false;
        }
    }
    void Run()
    {
            Vector3 moveVelocity = Vector3.zero;
            anim.SetBool("isRun", false);
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                SoundManager.instance.PlaySFX("Walk");
                direction = -1;
                moveVelocity = Vector3.left;
                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }
            
            else if (Input.GetAxisRaw("Horizontal") > 0)
            {
                SoundManager.instance.PlaySFX("Walk");
                direction = 1;
                moveVelocity = Vector3.right;
                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }
            transform.position += moveVelocity * FmovePower * Time.deltaTime;
    }
    void Jump()
    {
        if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0)
        && !anim.GetBool("isJump") && isGrounded)
        {
            SoundManager.instance.PlaySFX("Jump");
            isJumping = true;
            anim.SetBool("isJump", true);
        }
        if (!isJumping)
        {
            return;
        }
        rb.velocity = Vector2.zero;
        Vector2 jumpVelocity = new Vector2(0, FjumpPower);
        rb.AddForce(jumpVelocity, ForceMode2D.Impulse);
        isJumping = false;
    }
    void Hurt()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim.SetTrigger("hurt");
            if (direction == 1)
                rb.AddForce(new Vector2(-5f, 1f), ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(5f, 1f), ForceMode2D.Impulse);
        }
    }
    void Die()
    {
        if (transform.position.y <= deathdepth)
        {
            // y 좌표가 일정 이하라면 사망
            alive = false;
            anim.SetBool("isKickBoard", false);
            anim.SetTrigger("die");
            GameOver.Gamebool = true;
        }
    }
}