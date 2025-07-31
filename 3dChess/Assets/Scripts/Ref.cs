using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ref : MonoBehaviour
{
    public static Ref Instance;

    public ManageTiles manageTiles;
    public static ManageTiles ManageTiles { get {  return Instance.manageTiles; } }


    public Camera cam;
    public static Camera Camera { get { return Instance.cam; } }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
}
