using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class ListAstre : MonoBehaviour
{
    [SerializeField] private GameObject _prefAstre;
    [SerializeField] private Material _mat;
    public string _SystemeSceneName;
    [HideInInspector] public List<AstresDonnees> AstresDonneesList;
    [HideInInspector] public List<GameObject> Astres;
    private bool _isLoaded;
    int nb;

    public struct AstresDonnees
    {
        public float Impulsion;
        public bool IsOrbit;
        public float Radius;
        public int Resolution;
        public float DistWithSun;
        public string Name;
        public ShapeSettings Shape;
        public ColourSettings Color;

        public AstresDonnees(float impulsion, bool isOrbit, float radius, int resolution, float distWithSun, string name, ShapeSettings shape, ColourSettings color)
        {
            Impulsion = impulsion;
            IsOrbit = isOrbit;
            Radius = radius;
            Resolution = resolution;
            DistWithSun = distWithSun;
            Name = name;
            Shape = shape;
            Color = color;
        }
    }

    public bool IsDataExist(string name)
    {
        foreach (AstresDonnees astresDonnees in AstresDonneesList)
        {
            if (astresDonnees.Name == name)
                return true;
        }

        return false;
    }

    private void Awake()
    {
        _prefAstre = Resources.Load("Prefab/Astre") as GameObject;
        AstresDonneesList = new List<AstresDonnees>();
        Astres = new List<GameObject>();
    }
    
    void Start()
    {
        nb = 0;
        _isLoaded = false;
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == _SystemeSceneName && !_isLoaded)
        {
            InstancesAstres();
            _isLoaded = true;
        }

        for (int i = 0; i < Astres.Count; i++)
        {
            if (Vector3.Distance(GravityCenter.Instance.transform.position, Astres[i].transform.position) > 6000f)
            {
                GameObject astre = Astres[i];
                Astres.Remove(Astres[i]);
                Destroy(astre);
                Destroy(GameObject.Find(astre.name + "_button"));
                AstresDonneesList.Remove(AstresDonneesList[i]);
            }
        }
    }

    public void LoadSystemScene()
    {
        SceneManager.LoadScene(_SystemeSceneName);
    }

    public void RemoveAstre(GameObject astre)
    {
        for (int i = 0; i < Astres.Count; i++)
        {
            if (Astres[i] == astre)
            {
                Astres.RemoveAt(i);
                AstresDonneesList.RemoveAt(i);
                Destroy(astre);
            }
        }
    }

    public void InstancesAstres()
    {
        for (; nb < AstresDonneesList.Count; nb++)
        {
            AstresDonnees item = AstresDonneesList[nb];
            GameObject InstanceAstre = Instantiate(_prefAstre);
            InstanceAstre.GetComponent<InitializePlanet>()._Impulsion = item.Impulsion;
            InstanceAstre.GetComponent<InitializePlanet>()._IsOrbit = item.IsOrbit;
            InstanceAstre.GetComponent<InitializePlanet>()._Radius = item.Radius;
            InstanceAstre.GetComponent<InitializePlanet>()._Resolution = item.Resolution;
            InstanceAstre.GetComponent<InitializePlanet>()._DistWithSun = item.DistWithSun;
            InstanceAstre.GetComponent<InitializePlanet>()._ShapeSettings = item.Shape;
            Material material = new Material(_mat);
            AssetDatabase.CreateAsset(material, "Assets/Resources/Material/Temp/" + item.Name + ".mat");
            InstanceAstre.GetComponent<InitializePlanet>()._PlaneteMaterial = (Resources.Load("Material/Temp/" + item.Name, typeof(Material)) as Material);
            InstanceAstre.GetComponent<InitializePlanet>()._ColourSettings = item.Color;
            InstanceAstre.name = item.Name;
            InstanceAstre.GetComponent<InitializePlanet>().Init();
            Astres.Add(InstanceAstre);
        }
        Camera.main.GetComponentInChildren<UIList>().findChildrenObject();
    }
}
