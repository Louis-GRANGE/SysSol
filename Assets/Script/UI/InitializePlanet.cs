using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializePlanet : MonoBehaviour
{
    private GameObject _Planete;
    public ShapeSettings _ShapeSettings;
    public ColourSettings _ColourSettings;
    public Material _AtmosphereMaterial, _PlaneteMaterial;
    public float _Radius, _Impulsion, _DistWithSun;
    public int _Resolution;
    public bool _IsOrbit;
    public string _NomDuSoleil;
    public bool _InitPlanet = false;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (_InitPlanet)
            Init();
    }

    public void Init()
    {
        if (!_ShapeSettings) 
            _ShapeSettings = Resources.Load("Script/Shape", typeof(ShapeSettings)) as ShapeSettings;
        
        if (!_ColourSettings) 
            _ColourSettings = Resources.Load("Script/Colour") as ColourSettings;
        
        if (!_AtmosphereMaterial) 
            _AtmosphereMaterial = Resources.Load("Material/Atmosphere", typeof(Material)) as Material;
        
        if (_NomDuSoleil == "") 
            _NomDuSoleil = "Sun";

        _Planete = gameObject;
        
        _Planete.transform.SetParent(GameObject.Find("Map").transform);
        
        if (!gameObject.GetComponent<Planet>())
            _Planete.AddComponent<Planet>();
        
        _Planete.GetComponent<Planet>().shapeSettings = _ShapeSettings;
        _Planete.GetComponent<Planet>().colourSettings = _ColourSettings;
        _Planete.GetComponent<Planet>().resolution = _Resolution;
        _Planete.GetComponent<Planet>().shapeSettings.planetRadius = _Radius;
        _Planete.GetComponent<Planet>().colourSettings.atmosphereMaterial = _AtmosphereMaterial;
        
        Debug.Log("[" + GetType().Name + "] Nom du material: " + _PlaneteMaterial.name);
        Debug.Log("[" + GetType().Name + "] Nom du colorsitting: " + _ColourSettings);
        
        _Planete.GetComponent<Planet>().colourSettings.planetMaterial = _PlaneteMaterial;

        if (!gameObject.GetComponent<Attractor>())
            _Planete.AddComponent<Attractor>();
        _Planete.GetComponent<Attractor>().impulsion = _Impulsion;
        _Planete.GetComponent<Attractor>().isOrbit = _IsOrbit;
        _Planete.GetComponent<Attractor>().orbitReference = GameObject.Find(_NomDuSoleil).GetComponent<Attractor>();
        //_Planete.transform.position = Vector3.zero;
        _Planete.transform.position = new Vector3(_DistWithSun, 0, 0);
        _Planete.GetComponent<Planet>().GeneratePlanet();
        Debug.Log("[" + GetType().Name + "] Impulsion donnée, is orbit= " + _IsOrbit);
        _Planete.GetComponent<Attractor>().StartImpulsion();
    }
}
