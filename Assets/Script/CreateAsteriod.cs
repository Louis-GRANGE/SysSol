using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreateAsteriod : MonoBehaviour
{
    [SerializeField] private GameObject _asteriod;
    [SerializeField] private GameObject _planet;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject instanceAsteriod = Instantiate(_asteriod);
            instanceAsteriod.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            instanceAsteriod.transform.LookAt(_planet.transform);
            instanceAsteriod.transform.Translate(Vector3.forward * Random.Range(1, 100));
        }
    }
}
