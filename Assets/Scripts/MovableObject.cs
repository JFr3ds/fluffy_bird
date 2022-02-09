using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    public virtual void Update()
    {
        if (GameManager.Instance.actualGameState != GameManager.GameState.Play)
        {
            return;
        }
        transform.Translate(Vector3.left * GameManager.Instance.movementSpeed * Time.deltaTime);
        if (GameManager.Instance.maxLenght > transform.position.x)
        {
            gameObject.SetActive(false);
        }
    }
}
