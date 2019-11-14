using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tabcoll : MonoBehaviour, ISelectHandler, IDeselectHandler {

    public void OnDeselect(BaseEventData eventData)
    {
        _instacnet = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        _instacnet = true;
    }

    EventSystem system;
    private bool _instacnet = false;

    void Start()
    {
        system = EventSystem.current;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Tab) && _instacnet)
        {
            Selectable _Nect = null;
            if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                _Nect = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                if (_Nect == null) _Nect = system.lastSelectedGameObject.GetComponent<Selectable>();
            }
            else
            {
                _Nect = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                if (_Nect == null) _Nect = system.firstSelectedGameObject.GetComponent<Selectable>();
                if(_Nect != null)
                {
                    InputField inputField = _Nect.GetComponent<InputField>();
                    system.SetSelectedGameObject(_Nect.gameObject, new BaseEventData(system));
                }
                else
                {
                    Debug.LogError("没有下一个组件");
                }
            }
        }
    }
}
