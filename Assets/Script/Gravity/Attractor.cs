using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Attractor : MonoBehaviour
{
    private const float G = 6.674f;
    
    [HideInInspector] public Rigidbody rb;

    public static List<Attractor> Attractors;
    protected List<Vector3> pos;

    public bool drawLine = true;

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
        }
            Debug.DrawLine(pos[pos.Count - 2], point, Color.white, 3f);
        
        
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

        Vector3 direction = rb.position - rbToAttract.position;
        float distance = direction.magnitude;

        float scale = objToAttract.gameObject.transform.localScale.magnitude;

        if (distance <= scale + gameObject.transform.localScale.magnitude + 0.2f)
            return;

        float forceMagnitude = G * (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude;
        
        rbToAttract.AddForce(force);
    }
}
