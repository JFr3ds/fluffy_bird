using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MovableObject
{
    public void OnInitialize()
    {
        float height = Random.Range(GameManager.Instance.minHeightPowerUp, GameManager.Instance.maxHeightPowerUp);
        transform.localPosition = Vector3.up * height;
    }
}
