using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : Singleton<Scenes> {

    public void SwitchScene(string name)
    {
        SceneManager.LoadScene(name);
    }

}
