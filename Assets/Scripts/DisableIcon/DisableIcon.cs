using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableIcon : MonoBehaviour
{
    public GameObject disableActiveMisson;
    public GameObject disableActiveBag;
    void Start()
    {
        disableActiveMisson.SetActive(false);
        disableActiveBag.SetActive(false);
    }
    public void enableIcon()
    {
        disableActiveMisson.SetActive(true);
        disableActiveBag.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
