using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    //public GameObject Astre;
    private ListAstre ListeAstreDontDestroyOnLoad;
    public string _Name;
    public float _Radius, _Impulsion, _Distance;
    public int _Resolution;
    public bool _IsOrbit;
    public ShapeSettings _ShapeSetting;
    public ColourSettings _ColorSetting;

    [SerializeField] private Text _errorText;

    private InputField _NameInputField, _ResolutionInputField, _RadiusInputField, _ImpulsionInputField, _DistInputField;
    private Dropdown _ShapeSettingDropDown, _ColorSettingDropDown;
    private Toggle _IsOrbitToggle;
    public Text _ListeOfAstreText;
    private Button _AddAstre;

    private Transform _PanelPlanetSetting, _PanelListAstreInSolarSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!_errorText)
            throw new Exception("You must define an error message text !");
        
        _errorText.enabled = false;
        
        _IsOrbit = true;
        if (!ListeAstreDontDestroyOnLoad)
            ListeAstreDontDestroyOnLoad = GameObject.Find("DontDestroyOnLoad").GetComponent<ListAstre>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "PanelPlanetSetting") _PanelPlanetSetting = transform.GetChild(i);
            if (transform.GetChild(i).name == "PanelListAstreInSolarSystem") _PanelListAstreInSolarSystem = transform.GetChild(i);
        }
        for (int i = 0; i < _PanelPlanetSetting.childCount; i++)
        {
            if (_PanelPlanetSetting.GetChild(i).name == "Name") 
                _NameInputField = _PanelPlanetSetting.GetChild(i).GetComponent<InputField>();
            if (_PanelPlanetSetting.GetChild(i).name == "Resolution") 
                _ResolutionInputField = _PanelPlanetSetting.GetChild(i).GetComponent<InputField>();
            if (_PanelPlanetSetting.GetChild(i).name == "Radius") 
                _RadiusInputField = _PanelPlanetSetting.GetChild(i).GetComponent<InputField>();
            if (_PanelPlanetSetting.GetChild(i).name == "Impulsion") 
                _ImpulsionInputField = _PanelPlanetSetting.GetChild(i).GetComponent<InputField>();
            if (_PanelPlanetSetting.GetChild(i).name == "Distance") 
                _DistInputField = _PanelPlanetSetting.GetChild(i).GetComponent<InputField>(); 
            if (_PanelPlanetSetting.GetChild(i).name == "AddAstre") 
                _AddAstre = _PanelPlanetSetting.GetChild(i).GetComponent<Button>();
            if (_PanelPlanetSetting.GetChild(i).name == "ShapeSetting") 
                _ShapeSettingDropDown = _PanelPlanetSetting.GetChild(i).GetComponent<Dropdown>();
            if (_PanelPlanetSetting.GetChild(i).name == "ColourSetting")
                _ColorSettingDropDown = _PanelPlanetSetting.GetChild(i).GetComponent<Dropdown>();
        }
        
        for (int i = 0; i < _PanelListAstreInSolarSystem.childCount; i++)
        {
            if (_PanelListAstreInSolarSystem.GetChild(i).name == "ListeOfAstres") 
                _ListeOfAstreText = _PanelListAstreInSolarSystem.GetChild(i).GetComponent<Text>();
        }
        
        _NameInputField.onEndEdit.AddListener(delegate { setName(); });
        _ResolutionInputField.onEndEdit.AddListener(delegate { setResolution(); });
        _RadiusInputField.onEndEdit.AddListener(delegate { setRadius(); });
        _ImpulsionInputField.onEndEdit.AddListener(delegate { setImpulsion(); });
        _DistInputField.onEndEdit.AddListener(delegate { setDistance(); });
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
        if (_Name == "" || ListeAstreDontDestroyOnLoad.IsDataExist(_Name)
            || _Distance < 70 || _Distance > 5000
            || _Resolution < 2 || _Resolution > 100
            || _Radius < 1 || _Radius > 20
            || _Impulsion < 0 || _Impulsion > 200)
            _errorText.enabled = true;
        else
        {
            _errorText.enabled = false;
            
            ListeAstreDontDestroyOnLoad.AstresDonneesList.Add(
                new ListAstre.AstresDonnees(_Impulsion, _IsOrbit, _Radius, _Resolution, _Distance, _Name, _ShapeSetting, _ColorSetting)
            );
        
            _ListeOfAstreText.text += _Name + "\n";
        }
    }

    public void LauchTestImpactScene()
    {
        SceneManager.LoadScene("ImpactTest");
    }

    void setShape()
    {
        _ShapeSetting = (Resources.Load("Script/Shapes/" + _ShapeSettingDropDown.options[_ShapeSettingDropDown.value].text, typeof(ShapeSettings)) as ShapeSettings);
    }

    void setColor()
    {
        _ColorSetting = (Resources.Load("Script/Colors/" + _ColorSettingDropDown.options[_ColorSettingDropDown.value].text, typeof(ColourSettings)) as ColourSettings);
    }

    void setName()
    {
        _Name = _NameInputField.text;
    }

    void setResolution()
    {
        if (_ResolutionInputField.text == "")
            _Resolution = 2;
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
            _Impulsion = 0;
        else
            _Impulsion = float.Parse(_ImpulsionInputField.text);
        
    }
    void setDistance()
    {
        if (_DistInputField.text == "")
            _Distance = 60;
        else
            _Distance = float.Parse(_DistInputField.text);
    }
}
