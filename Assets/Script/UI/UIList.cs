using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIList : MonoBehaviour
{
    private GameObject Map;
    private List<GameObject> childrens;
    public GameObject Button, Slider;
    public Vector2 pos;
    // private Vector3 offset;
    // public float smoothCam;
    private string ObjName;
    private GameObject button, slider;
    // private Quaternion DefaultRotation;

    private List<string> _listButtonName;

    void Start()
    {
        _listButtonName = new List<string>();
        
        ObjName = "Sun";
        // DefaultRotation = Camera.main.transform.rotation;
        // offset = new Vector3(0,200,0);
        pos = new Vector2();
        // smoothCam = 2;
        childrens = new List<GameObject>();
    }

    
    void Update()
    {
        CameraFollowTo();
        if(slider)
            Time.timeScale = slider.GetComponent<Slider>().value;
    }

    public void findChildrenObject()
    {
        // ListAstre ListeAstreDontDestroyOnLoad = GameObject.Find("DontDestroyOnLoad").GetComponent<ListAstre>();
        
        if (GameObject.Find("Map"))
        {
            Map = GameObject.Find("Map");
            string childName;
            
            for (int i = 0; i < Map.transform.childCount; i++)
            {
                childName = Map.transform.GetChild(i).gameObject.name;

                // if (!ListeAstreDontDestroyOnLoad.IsGameObjectExist(childName))
                if (!_listButtonName.Contains(childName))
                {
                    CreateButton(childName, transform, pos, () => { FonctionButton(childName); });
                    pos.y -= Button.GetComponent<RectTransform>().rect.size.y * Button.GetComponent<RectTransform>().localScale.y;
                }
            }
            
            // foreach (var item in childrens)
            // {
            //     CreateButton(item.name, transform, _positions, () => { FonctionButton(item.name); });
            //     _positions.y -= Button.GetComponent<RectTransform>().rect.size.y * Button.GetComponent<RectTransform>().localScale.y;
            // }
            
            CreateSlider(transform, pos);
        }
    }
    public void CreateButton(string buttonname, Transform panel, Vector2 position, UnityAction method)
    {
        button = Instantiate(Button);
        button.name = buttonname + "_button";
        button.transform.parent = panel;
        button.transform.position = position;
        button.GetComponent<RectTransform>().anchoredPosition = position;
        button.GetComponent<Button>().onClick.AddListener(method);
        button.GetComponentInChildren<Text>().text = buttonname;
        
        _listButtonName.Add(buttonname);
    }

    public void CreateSlider(Transform panel, Vector2 position)//, UnityEvent method
    {
        slider = Instantiate(Slider);
        slider.name = "_slider";
        slider.transform.parent = panel;
        slider.transform.position = position;
        slider.GetComponent<Slider>().minValue = 0;
        slider.GetComponent<Slider>().maxValue = 24;
        slider.GetComponent<Slider>().value = 1;
        for (int i = 0; i < slider.transform.childCount; i++)
        {
            if (slider.transform.GetChild(i).name == "min")
                slider.transform.GetChild(i).GetComponent<Text>().text = slider.GetComponent<Slider>().minValue.ToString();
            if (slider.transform.GetChild(i).name == "max")
                slider.transform.GetChild(i).GetComponent<Text>().text = slider.GetComponent<Slider>().maxValue.ToString();
        }
        slider.GetComponent<RectTransform>().anchoredPosition = position;
    }

    void FonctionButton(string objname) { ObjName = objname; }
    // void FonctionSlider(float value) { smoothCam = value; }

    void CameraFollowTo()
    {
        if (GameObject.Find(ObjName))
        {
            Transform obj = GameObject.Find(ObjName).transform;
            Camera.main.GetComponentInChildren<MouseMovement>().target = obj;
        }
    }
}
