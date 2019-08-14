using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : Singleton<Scenes> {

    public SingleSceneLoadedCallback singleLoadedCallback;

    public void SwitchScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void SwitchSingleScene(string name, SingleSceneLoadedCallback callback)
    {
        singleLoadedCallback = callback;
        SceneManager.LoadScene(name, LoadSceneMode.Single);
        SceneManager.sceneLoaded += SingleLoadedEve;
    }

    private void SingleLoadedEve(Scene s, LoadSceneMode l)
    {
        if (singleLoadedCallback != null)
        {
            singleLoadedCallback();
        }
    }
}
