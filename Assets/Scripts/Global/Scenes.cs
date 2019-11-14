using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : Singleton<Scenes> {

    private Dictionary<string, List<SingleSceneLoadedCallback>> singleSceneLoadedDict = new Dictionary<string, List<SingleSceneLoadedCallback>>();

    public void SwitchScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void SwitchSingleScene(string name, SingleSceneLoadedCallback callback)
    {
        if(singleSceneLoadedDict.ContainsKey(name))
        {
            List<SingleSceneLoadedCallback> cbList = singleSceneLoadedDict[name];
            if(!cbList.Contains(callback)){
                cbList.Add(callback);
            }
        }
        else
        {
            List<SingleSceneLoadedCallback> cbList = new List<SingleSceneLoadedCallback>();
            cbList.Add(callback);
            singleSceneLoadedDict.Add(name, cbList);
        }
        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    public void AfterSingleSceneLoaded(string name)
    {
        if((singleSceneLoadedDict != null) && singleSceneLoadedDict.ContainsKey(name))
        {
            List<SingleSceneLoadedCallback> cbList = singleSceneLoadedDict[name];
            foreach(var item in cbList)
            {
                item();
            }
            singleSceneLoadedDict.Remove(name);
        }
    }
}
