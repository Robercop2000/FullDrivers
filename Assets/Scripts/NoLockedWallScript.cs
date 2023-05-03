using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoLockedWallScript : MonoBehaviour
{
    public bool legal;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            legal = true;
        }
    }
}
