using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Attractor : MonoBehaviour
{
    private const float G = 6.67430f;
    
    [HideInInspector] public Rigidbody _rigidbody;

    public static List<Attractor> Attractors;
    protected List<Vector3> pos;

    public bool drawLine = true;

    public float impulsion = 100;

    public bool isOrbit = false;
    public bool isOrbitCirculaire = false;
    public Attractor orbitReference;

    private LineRenderer _lineRenderer;
    
    private void Awake()
    {
        if (Attractors == null)
            Attractors = new List<Attractor>();

        Attractors.Add(this);
        
        pos = new List<Vector3>();

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;

        if (gameObject.GetComponent<LineRenderer>())
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
        else
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    private void Start()
    {
        //StartImpulsion();
    }

    private void FixedUpdate()
    {
        // Debug.Log("[" + GetType().Name + "] Attractor object.name: " + gameObject.name);
        foreach (Attractor attractor in Attractors)
        {
            if (attractor != this)
                Attract(attractor);
        }
        
        AddPoint(gameObject.transform.position);
    }
    
    private void AddPoint(Vector3 point)
    {
        pos.Add(point);
        if (drawLine && pos.Count > 1)
        {
            _lineRenderer.positionCount = pos.Count;
            _lineRenderer.SetPositions(pos.ToArray());
            Debug.DrawLine(pos[pos.Count - 2], point, Color.white, 3f);
        }
    }

    
    private void OnDestroy()
    {
        Attractors.Remove(this);
    }


    void Attract(Attractor objToAttract)
    {
        Rigidbody rbToAttract = objToAttract._rigidbody;

        Vector3 direction = _rigidbody.position - rbToAttract.position - objToAttract.gameObject.transform.localScale / 2;
        float distance = direction.magnitude;

        float scale = objToAttract.gameObject.transform.localScale.magnitude / 2;

        if (distance <= scale)
        {
            rbToAttract.velocity = Vector3.zero;
            return;
        }

        float forceMagnitude = G * (_rigidbody.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);

        Vector3 force = direction.normalized * forceMagnitude;
        
        rbToAttract.AddForce(force);
    }

    public static float GetGravityMass(float referenceMass, float d)
    {
        return ((Mathf.Pow(1, -45) / G) * Mathf.Pow(d, 2)) / referenceMass;
    }

    public void StartImpulsion()
    {
        if (isOrbit)
        {
            _rigidbody.mass = 1000;
            Debug.Log("Orbit mass: " + orbitReference._rigidbody.mass);
            //isOrbitCirculaire = true;
            if (orbitReference == null)
                throw new Exception("[" + GetType().Name + "] You haven't set a reference for orbit");

            float distance = (_rigidbody.position - orbitReference._rigidbody.position - orbitReference.gameObject.transform.localScale / 2).magnitude;

            if (isOrbitCirculaire)
            {
                //                float f = Mathf.Sqrt((orbitReference._rigidbody.mass * G * 0.001f) / distance);
                float f = G * ((_rigidbody.mass * orbitReference._rigidbody.mass) /
                               Mathf.Pow(Vector3.Distance(_rigidbody.position, orbitReference._rigidbody.position), 2));
                Debug.Log("F = " + f);

                _rigidbody.AddForce(Vector3.forward * 26697 * 10);
            }
            else
            {
                _rigidbody.mass = GetGravityMass(orbitReference._rigidbody.mass, (_rigidbody.position - orbitReference._rigidbody.position).magnitude);

                _rigidbody.AddForce(Vector3.forward * impulsion);
                Debug.Log("[" + GetType().Name + "] Impulsion faite avec: (Vector3.forward * impulsion)");
                //                _rigidbody.AddForce(Vector3.forward * distance);
            }
        }
    }
}
