using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Laps")]
    public NoLockedWallScript noLockedWallModifier;
    private int counter;
    public Text lap;
    private string laps = "/3";

    [Header("Velocimeter")]
    public Image image;
    public Rigidbody rb;
    public float MagnitudeVelocity;

    void Start()
    {
        counter = 1;
        lap.text = counter.ToString() + laps;
    }

    void Update()
    {
        float speed = rb.velocity.magnitude * MagnitudeVelocity;
        image.transform.eulerAngles = new Vector3(0, 0, speed * -4 + 130);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && noLockedWallModifier.legal == true)
        {
            noLockedWallModifier.legal = false;
            counter++;
            lap.text = counter.ToString() + laps;

            if (counter == 4) FindObjectOfType<GameManager>().WinGame();
        }
    }
}
