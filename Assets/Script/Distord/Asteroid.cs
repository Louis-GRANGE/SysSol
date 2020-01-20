using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Asteroid : MonoBehaviour
{
    private LineRenderer _line;
    private List<Vector3> _position;
    private Mesh _mesh;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _line = gameObject.AddComponent<LineRenderer>();
        _position = new List<Vector3>();
    }

    private void Start()
    {
        gameObject.transform.localScale = Vector3.one * Random.Range(1, 10);
        _rigidbody.mass = gameObject.transform.localScale.magnitude * Random.Range(1, 10);
        Destroy(gameObject, 10);
    }

    public void Fire(Vector3 direction)
    {
        _rigidbody.velocity = direction * Random.Range(1, 10);
        CreateLine();
    }
    
    private void Update()
    {
        AddPoint(gameObject.transform.position);
        if (_position.Count > 1)
        {
            _line.positionCount = _position.Count;
            for (int i = 0; i < _position.Count; i++)
            {
                if (Vector3.Distance(_position[i], gameObject.transform.position) > (gameObject.transform.localScale.x * 2.5f))
                    _position.Remove(_position[i]);
            }
            CreateLine();
            _line.positionCount = _position.Count;
            _line.SetPositions(_position.ToArray());
        }
    }

    private void AddPoint(Vector3 point)
    {
        _position.Add(point);
    }

    private void CreateLine()
    {
        _line.material = new Material(Shader.Find("Sprites/Default"));
        _line.startColor = Color.red;
        _line.endColor = Color.yellow;
        _line.widthMultiplier = 0.2f;
        _line.startWidth = 0;
        _line.endWidth = gameObject.transform.localScale.x;
    }
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("[" + GetType().Name + "] Collision avec " + collider.gameObject.name);
        
        Ray ray;
        if (collider.transform.parent != null)
        {
            Debug.Log("[" + GetType().Name + "] Parent: " + collider.gameObject.transform.parent.name);
            Vector3 direction = (collider.transform.parent.position - transform.position).normalized;
            ray = new Ray(transform.position, direction);
        }
        else
        {
            Debug.Log("[" + GetType().Name + "] Pas de parent");
            Vector3 direction = (collider.transform.position - transform.position).normalized;
            ray = new Ray(transform.position, direction);
        }

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("L_Distortable")))
        {
            Debug.Log(hit.collider.name);
            Debug.DrawRay(hit.transform.position, transform.position, Color.red, 10);

            Vector3 impactStrength = (_rigidbody.mass / gameObject.transform.localScale.x) * _rigidbody.velocity;
            
            hit.collider.GetComponent<PlanetDistord>().CrashAsteroid(hit.triangleIndex, impactStrength, transform.localScale.x);
        }
        
        Destroy(gameObject);
    }
}
