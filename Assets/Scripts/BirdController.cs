using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPlayer;

    [SerializeField] private Rigidbody2D rb_2d;
    [SerializeField] private float forceUp;
    [SerializeField] private float coldDown;
    [SerializeField] private Animator anim;
    [SerializeField] private Color powerUpColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private float timePowerUp;
    [SerializeField] private SpriteRenderer mySprite;
    private float lastTimePowerUp;

    enum BirdState
    {
        Normal,
        PowerUp
    }

    private BirdState actualState;


    private Vector2 rb_velocity;
    private float lastColdDown;

    private void Awake()
    {
        if (mySprite == null)
        {
            mySprite = GetComponent<SpriteRenderer>();
        }

        if (rb_2d == null)
        {
            rb_2d = GetComponent<Rigidbody2D>();
        }

        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }

        lastColdDown = coldDown;
        actualState = BirdState.Normal;
    }

    void Update()
    {
        switch (GameManager.Instance.actualGameState)
        {
            case GameManager.GameState.Pause:
                rb_velocity = rb_2d.velocity;
                rb_2d.velocity = Vector2.zero;
                rb_2d.isKinematic = true;
                anim.speed = 0;
                return;
                break;
            case GameManager.GameState.Play:
                if (rb_2d.isKinematic)
                {
                    rb_2d.isKinematic = false;
                    rb_2d.velocity = rb_velocity;
                }

                anim.speed = 1;

                if (Input.GetButtonDown("Fire1"))
                {
                    if (lastColdDown >= coldDown)
                    {
                        lastColdDown = 0;
                        MoveUp();
                    }
                }

                lastColdDown += Time.deltaTime;
                break;
            case GameManager.GameState.Menu:
                transform.position = spawnPlayer;
                if (!rb_2d.isKinematic)
                {
                    rb_2d.isKinematic = true;
                }

                break;
            case GameManager.GameState.Dead:
                rb_2d.velocity = Vector2.down * forceUp;
                anim.speed = 0;
                break;
        }

        if (actualState == BirdState.PowerUp)
        {
            lastTimePowerUp -= Time.deltaTime;

            if (lastTimePowerUp % .2f < .1f)
            {
                mySprite.color = powerUpColor;
            }
            else
            {
                mySprite.color = normalColor;
            }

            if (lastTimePowerUp <= 0)
            {
                actualState = BirdState.Normal;
                mySprite.color = normalColor;
            }
        }
    }

    void MoveUp()
    {
        rb_2d.velocity = Vector2.up * forceUp;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string otherTag = other.tag;
        switch (otherTag)
        {
            case "CheckObstacle":
                ActionsController.OnCheckObstacle?.Invoke();
                other.gameObject.SetActive(false);
                break;
            case "Obstacle":
                if (actualState == BirdState.PowerUp)
                {
                    other.gameObject.SetActive(false);
                }
                else
                {
                    ActionsController.OnObstacle?.Invoke();
                }

                break;
            case "PowerUp":
                other.gameObject.SetActive(false);
                lastTimePowerUp = timePowerUp;
                actualState = BirdState.PowerUp;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == "Obstacle")
        {
            if (actualState == BirdState.Normal)
            {
                ActionsController.OnObstacle?.Invoke();
            }
        }
    }
}