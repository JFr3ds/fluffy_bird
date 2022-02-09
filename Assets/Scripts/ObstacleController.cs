using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleController : MovableObject
{
    [SerializeField] private GameObject checkObstacle;
    [SerializeField] private GameObject downObstacle;
    [SerializeField] private GameObject upObstacle;
    public void OnInitialize()
    {
        float height = Random.Range(GameManager.Instance.minHeight, GameManager.Instance.maxHeight);
        transform.localPosition = Vector3.up * height;
        checkObstacle.SetActive(true);
        downObstacle.SetActive(true);
        upObstacle.SetActive(true);
    }
}
