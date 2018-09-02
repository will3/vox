using UnityEngine;
using System.Collections;

public class Commander : MonoBehaviour
{
    public bool isBuilding;
    public static Commander instance;

    // Use this for initialization
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) {
            isBuilding = !isBuilding;
        }
    }
}
