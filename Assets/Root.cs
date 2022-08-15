using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    public GameMode gameMode;
    // Start is called before the first frame update
    void Awake()
    {
        AppConst.GameMode = this.gameMode;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
