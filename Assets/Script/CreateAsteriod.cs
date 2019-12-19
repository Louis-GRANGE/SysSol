using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreateAsteriod : MonoBehaviour
{
    [SerializeField] private GameObject _asteriod;

    [SerializeField] private Transform _positionSpawn;
    // [SerializeField] private GameObject _planet;
    
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Tab) && Camera.main.gameObject.GetComponent<MouseMovement>().target)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameObject instanceAsteriod = Instantiate(_asteriod);
            
            // instanceAsteriod.transform.SetParent(Camera.main.gameObject.GetComponent<MouseMovement>().target.transform);
            instanceAsteriod.transform.position = _positionSpawn.position;
            instanceAsteriod.GetComponent<Asteroid>().Fire(Camera.main.transform.forward);

            // instanceAsteriod.transform.position = Camera.main.transform.position;
            // instanceAsteriod.GetComponent<Asteroid>().Fire(Camera.main.transform.TransformPoint(Camera.main.transform.forward));

            // instanceAsteriod.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // instanceAsteriod.transform.LookAt(_planet.transform);
            // instanceAsteriod.transform.Translate(Vector3.forward * Random.Range(1, 100));
        }
    }
}
