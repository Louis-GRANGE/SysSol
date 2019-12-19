using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class ListAstre : MonoBehaviour
{
    [SerializeField] private GameObject _prefAstre;
    [SerializeField] private Material _mat;
    public string _SceneNameOfSysteme;
    public List<AstresDonnees> AstresDonneesList;
    public List<GameObject> Astres;
    private bool isDo;
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

    private void Awake()
    {
        _prefAstre = Resources.Load("Prefab/Astre") as GameObject;
        AstresDonneesList = new List<AstresDonnees>();
        Astres = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        nb = 0;
        isDo = false;
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    
    void Update()
    {
        if (SceneManager.GetActiveScene().name == _SceneNameOfSysteme && !isDo)
        {
            /*foreach (var item in Astres)
            {
                item.GetComponent<InitializePlanet>().Init();
                item.GetComponent<Attractor>().StartImpulsion();
            }
            Camera.main.GetComponentInChildren<UIList>().findChildrenObject();
            
            */
            InstancesAstres();
            /*foreach (AstresDonnees item in AstresDonneesList)
            {
                GameObject InstanceAstre = Instantiate(_prefAstre);
                InstanceAstre.GetComponent<InitializePlanet>()._Impulsion = item.Impulsion;
                InstanceAstre.GetComponent<InitializePlanet>()._IsOrbit = item.IsOrbit;
                InstanceAstre.GetComponent<InitializePlanet>()._Radius = item.Radius;
                InstanceAstre.GetComponent<InitializePlanet>()._Resolution = item.Resolution;
                InstanceAstre.GetComponent<InitializePlanet>()._DistWithSun = item.DistWithSun;
                InstanceAstre.GetComponent<InitializePlanet>()._ShapeSettings = item.Shape;
                Material material = new Material(_mat);
                AssetDatabase.CreateAsset(material, "Assets/Resources/Material/Temp/"+ item.Name +".mat");
                InstanceAstre.GetComponent<InitializePlanet>()._PlaneteMaterial = (Resources.Load("Material/Temp/" + item.Name, typeof(Material)) as Material);
                InstanceAstre.GetComponent<InitializePlanet>()._ColourSettings = item.Color;
                InstanceAstre.name = item.Name;
                InstanceAstre.GetComponent<InitializePlanet>().Init();
                Astres.Add(InstanceAstre);
            }*/
            isDo = true;
        }
    }
    
    public void ChangeScene() { SceneManager.LoadScene(_SceneNameOfSysteme); }

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
