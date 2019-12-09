using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Attractor : MonoBehaviour
{
    private const float G = 6.67430f;
    
    [HideInInspector] public Rigidbody rb;

    public static List<Attractor> Attractors;
    protected List<Vector3> pos;

    public bool drawLine = true;

    public bool isOrbit = false;
    public bool isOrbitCirculaire = false;
    public Attractor orbitReference;

    private LineRenderer _lineRenderer;
    
    private void Awake()
    {
        if (Attractors == null)
            Attractors = new List<Attractor>();
        
        pos = new List<Vector3>();

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.widthMultiplier = 0.2f;
    }

    private void Start()
    {
        if (isOrbit)
        {
            if (orbitReference == null)
                throw new Exception("[" + GetType().Name + "] You haven't set a reference for orbit");

            float distance = (rb.position - orbitReference.rb.position - orbitReference.gameObject.transform.localScale / 2).magnitude;

            if (isOrbitCirculaire)
            {
//                float f = Mathf.Sqrt((orbitReference.rb.mass * G * 0.001f) / distance);
                float f = G * ((rb.mass * orbitReference.rb.mass) /
                               Mathf.Pow(Vector3.Distance(rb.position, orbitReference.rb.position), 2));  
                Debug.Log("F = " + f);

                rb.AddForce(Vector3.forward * 26697 * 10);
            }
            else
            {
                rb.mass = GetGravityMass(orbitReference.rb.mass,
                    (rb.position - orbitReference.rb.position).magnitude);
            
                rb.AddForce(Vector3.forward * distance);
            }
        }
    }

    private void FixedUpdate()
    {
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Time.timeScale += 1;
        }
        if (Input.GetMouseButtonDown(1))
        {
            Time.timeScale -= (Time.timeScale > 0) ? 1 : 0;
        }
           
    }

    private void OnDisable()
    {
        Attractors.Remove(this);
    }

    private void OnEnable()
    {
        Attractors.Add(this);
    }

    void Attract(Attractor objToAttract)
    {
        Rigidbody rbToAttract = objToAttract.rb;

        Vector3 direction = rb.position - rbToAttract.position - objToAttract.gameObject.transform.localScale / 2;
        float distance = direction.magnitude;

        float scale = objToAttract.gameObject.transform.localScale.magnitude / 2;

        if (distance <= scale)
        {
            rbToAttract.velocity = Vector3.zero;
            return;
        }

        float forceMagnitude = G * (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);

        Vector3 force = direction.normalized * forceMagnitude;
        
        rbToAttract.AddForce(force);
    }

    public static float GetGravityMass(float referenceMass, float d)
    {
        return ((Mathf.Pow(1, -45) / G) * Mathf.Pow(d, 2)) / referenceMass;
    }
}
