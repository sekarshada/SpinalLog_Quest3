using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThreeBtnSwitch : MonoBehaviour
{
    public GameObject Btn1;
    public GameObject Btn2;

    public GameObject Btn3;
    // Start is called before the first frame update
    void Start()
    {
        Btn1.GetComponent<Button>().onClick.AddListener(OnButtonClick1);
        Btn2.GetComponent<Button>().onClick.AddListener(OnButtonClick2);
        Btn3.GetComponent<Button>().onClick.AddListener(OnButtonClick3);
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    void OnButtonClick1()
    {
        Debug.Log("Btn1 clicked");
        Btn1.SetActive(false);
        Btn2.SetActive(true);
        Btn3.SetActive(false);
    }
     void OnButtonClick2()
    {
        Debug.Log("Btn2 clicked");
        Btn1.SetActive(false);
        Btn2.SetActive(false);
        Btn3.SetActive(true);
    }

       void OnButtonClick3()
    {
        Debug.Log("Btn3 clicked");
        Btn1.SetActive(true);
        Btn2.SetActive(false);
        Btn3.SetActive(false);
    }

}
