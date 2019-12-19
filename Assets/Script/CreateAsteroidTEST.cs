using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAsteroidTEST : MonoBehaviour
{
    [SerializeField] private GameObject _asteriod;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameObject instanceAsteriod = Instantiate(_asteriod);
            
            instanceAsteriod.transform.position = Camera.main.transform.position;
            instanceAsteriod.transform.LookAt(Vector3.zero);
            instanceAsteriod.GetComponent<Asteroid>()
                .Fire(Camera.main.transform.forward * 10);
        }
    }
}
