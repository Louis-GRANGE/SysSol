using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    //public GameObject Astre;
    public ListAstre ListeAstreDontDestroyOnLoad;
    public string _Name;
    public float _Radius, _Impulsion, _Distance;
    public int _Resolution;
    public bool _IsOrbit;
    public ShapeSettings _ShapeSetting;
    public ColourSettings _ColorSetting;

    private InputField _NameInputField, _ResolutionInputField, _RadiusInputField, _ImpulsionInputField, _DistInputField;
    private Dropdown _ShapeSettingDropDown, _ColorSettingDropDown;
    private Toggle _IsOrbitToggle;
    public Text _ListeOfAstreText;
    private Button _AddAstre;

    private Transform _PanelPlanetSetting, _PanelListAstreInSolarSystem;


    // Start is called before the first frame update
    void Start()
    {
        if (!ListeAstreDontDestroyOnLoad)
            ListeAstreDontDestroyOnLoad = GameObject.Find("DontDestroyOnLoad").GetComponent<ListAstre>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "PanelPlanetSetting") _PanelPlanetSetting = transform.GetChild(i);
            if (transform.GetChild(i).name == "PanelListAstreInSolarSystem") _PanelListAstreInSolarSystem = transform.GetChild(i);
        }
        for (int i = 0; i < _PanelPlanetSetting.childCount; i++)
        {
            if (_PanelPlanetSetting.GetChild(i).name == "Name") _NameInputField = _PanelPlanetSetting.GetChild(i).GetComponent<InputField>();
            if (_PanelPlanetSetting.GetChild(i).name == "Resolution") _ResolutionInputField = _PanelPlanetSetting.GetChild(i).GetComponent<InputField>();
            if (_PanelPlanetSetting.GetChild(i).name == "Radius") _RadiusInputField = _PanelPlanetSetting.GetChild(i).GetComponent<InputField>();
            if (_PanelPlanetSetting.GetChild(i).name == "Impulsion") _ImpulsionInputField = _PanelPlanetSetting.GetChild(i).GetComponent<InputField>();
            if (_PanelPlanetSetting.GetChild(i).name == "Distance") _DistInputField = _PanelPlanetSetting.GetChild(i).GetComponent<InputField>();
            if (_PanelPlanetSetting.GetChild(i).name == "IsOrbit") _IsOrbitToggle = _PanelPlanetSetting.GetChild(i).GetComponent<Toggle>();
            if (_PanelPlanetSetting.GetChild(i).name == "AddAstre") _AddAstre = _PanelPlanetSetting.GetChild(i).GetComponent<Button>();
            if (_PanelPlanetSetting.GetChild(i).name == "ShapeSetting") _ShapeSettingDropDown = _PanelPlanetSetting.GetChild(i).GetComponent<Dropdown>();
            if (_PanelPlanetSetting.GetChild(i).name == "ColourSetting") _ColorSettingDropDown = _PanelPlanetSetting.GetChild(i).GetComponent<Dropdown>();
        }
        for (int i = 0; i < _PanelListAstreInSolarSystem.childCount; i++)
        {
            if (_PanelListAstreInSolarSystem.GetChild(i).name == "ListeOfAstres") _ListeOfAstreText = _PanelListAstreInSolarSystem.GetChild(i).GetComponent<Text>();
        }
        _NameInputField.onEndEdit.AddListener(delegate { setName(); });
        _ResolutionInputField.onEndEdit.AddListener(delegate { setResolution(); });
        _RadiusInputField.onEndEdit.AddListener(delegate { setRadius(); });
        _ImpulsionInputField.onEndEdit.AddListener(delegate { setImpulsion(); });
        _DistInputField.onEndEdit.AddListener(delegate { setDistance(); });
        _IsOrbitToggle.onValueChanged.AddListener(delegate { setIsOrbit(); });
        _AddAstre.onClick.AddListener(delegate { AddAstre(); });

        List<string> namesshapes = new List<string>();
        namesshapes.Add("EarthShape");
        namesshapes.Add("MarsShape");
        namesshapes.Add("VenusShape");
        _ShapeSettingDropDown.options.Clear();
        _ShapeSettingDropDown.AddOptions(namesshapes);
        _ShapeSetting = Resources.Load("Script/Shapes/" + _ShapeSettingDropDown.options[_ShapeSettingDropDown.value].text, typeof(ShapeSettings)) as ShapeSettings;
        _ShapeSettingDropDown.onValueChanged.AddListener(delegate { setShape(); });

        List<string> namescolors = new List<string>();
        namescolors.Add("EarthColor");
        namescolors.Add("MarsColor");
        namescolors.Add("VenusColor");
        _ColorSettingDropDown.options.Clear();
        _ColorSettingDropDown.AddOptions(namescolors);
        _ColorSetting = Resources.Load("Script/Colors/" + _ColorSettingDropDown.options[_ColorSettingDropDown.value].text, typeof(ColourSettings)) as ColourSettings;
        _ColorSettingDropDown.onValueChanged.AddListener(delegate { setColor(); });
    }

    void AddAstre()
    {

        ListeAstreDontDestroyOnLoad.AstresDonneesList.Add(
            new ListAstre.AstresDonnees(_Impulsion, _IsOrbit, _Radius, _Resolution, _Distance, _Name, _ShapeSetting, _ColorSetting)
        );

        /*
        GameObject InstanceAstre = Instantiate(Astre);
        InstanceAstre.GetComponent<InitializePlanet>()._Impulsion = _Impulsion;
        InstanceAstre.GetComponent<InitializePlanet>()._IsOrbit = _IsOrbit;
        InstanceAstre.GetComponent<InitializePlanet>()._Radius = _Radius;
        InstanceAstre.GetComponent<InitializePlanet>()._Resolution = _Resolution;
        InstanceAstre.GetComponent<InitializePlanet>()._DistWithSun = _Distance;
        InstanceAstre.name = _Name;
        _ListeOfAstreText.text += _Name + "\n";
        ListeAstreDontDestroyOnLoad.Astres.Add(InstanceAstre);
        */

        _ListeOfAstreText.text += _Name + "\n";
    }

    void setShape() { _ShapeSetting = (Resources.Load("Script/Shapes/" + _ShapeSettingDropDown.options[_ShapeSettingDropDown.value].text, typeof(ShapeSettings)) as ShapeSettings); }

    void setColor() { _ColorSetting = (Resources.Load("Script/Colors/" + _ColorSettingDropDown.options[_ColorSettingDropDown.value].text, typeof(ColourSettings)) as ColourSettings); }

    void setName() {_Name = _NameInputField.text; }

    void setResolution()
    {
        if (_ResolutionInputField.text == "")
            _Resolution = 1;
        else
            _Resolution = int.Parse(_ResolutionInputField.text);
    }
    void setRadius()
    {
        if (_RadiusInputField.text == "")
            _Radius = 1;
        else
            _Radius = float.Parse(_RadiusInputField.text);
    }
    void setImpulsion()
    {
        if (_ImpulsionInputField.text == "")
            _Impulsion = 1;
        else
            _Impulsion = float.Parse(_ImpulsionInputField.text);
    }
    void setDistance()
    {
        if (_DistInputField.text == "")
            _Distance = 10;
        else
            _Distance = float.Parse(_DistInputField.text);
    }
    void setIsOrbit() { _IsOrbit = _IsOrbitToggle.isOn; }
}
