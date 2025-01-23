using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnBtn : MonoBehaviour
{
    public GameObject beaglePrefab;

    public void BeagleBtn()
    {
        Instantiate(beaglePrefab);
    }
}
