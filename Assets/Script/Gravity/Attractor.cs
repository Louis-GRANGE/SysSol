using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Attractor : MonoBehaviour
{
    private const float G = 6.67430f;
    
    [HideInInspector] public Rigidbody rigidbody;

    public static List<Attractor> Attractors;
    protected List<Vector3> _positions;

    public bool drawLine = true;

    public float impulsion = 100;

    public bool isOrbit = false;
    public bool isOrbitCirculaire = false;
    public Attractor orbitReference;

    private LineRenderer _lineRenderer;

    protected void Awake()
    {
        if (Attractors == null)
            Attractors = new List<Attractor>();

        Attractors.Add(this);
        
        _positions = new List<Vector3>();

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;

        if (gameObject.GetComponent<LineRenderer>())
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
        else
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    private void FixedUpdate()
    {
        AddPoint(gameObject.transform.position);
        if (_lineRenderer.positionCount > 1)
        {
            _lineRenderer.positionCount = _positions.Count;
            for (int i = 0; i < _positions.Count; i++)
            {
                if (Vector3.Distance(_positions[i], gameObject.transform.position) > (gameObject.transform.localScale.x * 1000f))
                    _positions.Remove(_positions[i]);
            }
            
            _lineRenderer.positionCount = _positions.Count;
            _lineRenderer.SetPositions(_positions.ToArray());
        }
        
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
        _positions.Add(point);
        if (drawLine && _positions.Count > 1)
        {
            _lineRenderer.positionCount = _positions.Count;
            _lineRenderer.SetPositions(_positions.ToArray());
            Debug.DrawLine(_positions[_positions.Count - 2], point, Color.white, 3f);
        }
    }

    private void OnDestroy()
    {
        Attractors.Remove(this);
    }


    void Attract(Attractor objToAttract)
    {
        Rigidbody rbToAttract = objToAttract.rigidbody;

        Vector3 direction = rigidbody.position - rbToAttract.position - objToAttract.gameObject.transform.localScale / 2;
        float distance = direction.magnitude;

        float scale = objToAttract.gameObject.transform.localScale.magnitude / 2;

        if (distance <= scale)
        {
            rbToAttract.velocity = Vector3.zero;
            return;
        }

        float forceMagnitude = G * (rigidbody.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);

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
            rigidbody.mass = 1000;
            Debug.Log("[" + GetType().Name + "] Orbit mass: " + orbitReference.rigidbody.mass);
            //isOrbitCirculaire = true;
            if (orbitReference == null)
                throw new Exception("[" + GetType().Name + "] You haven't set a reference for orbit");

            float distance = (rigidbody.position - orbitReference.rigidbody.position - orbitReference.gameObject.transform.localScale / 2).magnitude;

            if (isOrbitCirculaire)
            {
                //                float f = Mathf.Sqrt((orbitReference.rigidbody.mass * G * 0.001f) / distance);
                float f = G * ((rigidbody.mass * orbitReference.rigidbody.mass) /
                               Mathf.Pow(Vector3.Distance(rigidbody.position, orbitReference.rigidbody.position), 2));
                Debug.Log("F = " + f);

                rigidbody.AddForce(Vector3.forward * 26697 * 10);
            }
            else
            {
                rigidbody.mass = GetGravityMass(orbitReference.rigidbody.mass, (rigidbody.position - orbitReference.rigidbody.position).magnitude);

                rigidbody.AddForce(Vector3.forward * impulsion);
                Debug.Log("[" + GetType().Name + "] Impulsion faite avec: (Vector3.forward * impulsion)");
                //                rigidbody.AddForce(Vector3.forward * distance);
            }
        }
    }
}
