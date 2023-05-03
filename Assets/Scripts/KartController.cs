using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour
{
    [Header("Esthetic Addons")]

    public GameObject turbo;
    public GameObject humo;
    public GameObject motorSound;

    [Header("Kart Stats")]

    public float forwardAccel = 8f;
    public float reverseAccel = 4f;
    public float boostPower = 5f;
    public float turnStrenght = 180f;
    public float brakeForce = 5f;
    public float maxBoostTime = 3f;

    private float currentSpeed, currentRotate, rotation, speedInput, boostTime, actualForwardAccel;

    private bool grounded, drifting, boosting = false;

    private int driftDirection;

    [Header("Kart Physics")]
    public Rigidbody KartRB;
    public float gravityForce = 10f;
    public float dragOnGround = 3f;
    public LayerMask isGround;
    public float GroundRayLength = 0.5f;
    public Transform GroundRayPoint;

    [Header("Kart Animations")]
    public Transform LeftFrontWheel;
    public Transform RightFrontWheel;
    public float maxWheelTurn;
    public Transform KartModel;

    private GameManager spawnPoint;
    private Transform spawnPlace;

    public GameObject driftSound;


    private void Awake()
    {
        KartRB.gameObject.SetActive(false);
    }

    public void Spawn(Transform ActualTransform)
    {
        //KartSpawnSetUp
        spawnPoint = FindObjectOfType<GameManager>();
        spawnPlace = ActualTransform;

        transform.position = spawnPlace.position;
        transform.rotation = spawnPlace.rotation;

        actualForwardAccel = forwardAccel;

        //Separación del RigidBody del Modelo
        KartRB.gameObject.SetActive(true);
        KartRB.transform.parent = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            FindObjectOfType<GameManager>().Pause();
        if (Input.GetAxis("Vertical") != 0) //Input Vertical
        {
            if(Input.GetAxis("Vertical") > 0) speedInput = (Input.GetAxis("Vertical") * actualForwardAccel * 1000f);
            else if(Input.GetAxis("Vertical") < 0 && !drifting) speedInput = (Input.GetAxis("Vertical") * reverseAccel * 1000f);
        }

        if (Input.GetAxis("Horizontal") != 0) //Input Horizontal
        {
            Steer(Input.GetAxis("Horizontal") > 0 ? 1 : -1, Mathf.Abs(Input.GetAxis("Horizontal")));
        }

        if (Input.GetAxis("Vertical") != 0 && FindObjectOfType<GameManager>().GameStarted == true)
        {
            motorSound.SetActive(true);
        }
        if (Input.GetAxis("Vertical") == 0 && FindObjectOfType<GameManager>().GameStarted == true)
        {
            motorSound.SetActive(false);
        }

        if (Input.GetAxis("Horizontal") != 0 && !drifting && Input.GetKeyDown(KeyCode.Space)) //Comienzo del derrape
        {
            driftSound.SetActive(true);
            drifting = true;
            boosting = false;
            actualForwardAccel = forwardAccel;
            driftDirection = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
            StopAllCoroutines();
            humo.SetActive(true);
        }

        if (drifting) //Modificacion de input durante el derrape
        {
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, 0f, 2) : ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, 2, 0f);
            boostTime += (Time.deltaTime)/5;

            Steer(driftDirection, control);

        }
        if (Input.GetKeyUp(KeyCode.Space) && drifting) //Final del derrape
        {
            driftSound.SetActive(false);
            drifting = false;
            if (!boosting) StartCoroutine(Boost(boostTime));
            humo.SetActive(false);
        }

        //Giro del modelo de coche
        if (!drifting) 
        {
            KartModel.localEulerAngles = Vector3.Lerp(KartModel.localEulerAngles, new Vector3(KartModel.localEulerAngles.x, Mathf.LerpAngle(KartModel.localEulerAngles.y, (Input.GetAxis("Horizontal") * 15), 0.1f), KartModel.localEulerAngles.z), .2f);
        }
        else
        {
            float control = (driftDirection == 1) ? ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, .5f, 2) : ExtensionMethods.Remap(Input.GetAxis("Horizontal"), -1, 1, 2, .5f);
            KartModel.localRotation = Quaternion.Euler(KartModel.localEulerAngles.x, Mathf.LerpAngle(KartModel.localEulerAngles.y, (control) * driftDirection, .2f), KartModel.localEulerAngles.z);
        }


        //Movimiento de ruedas del modelo
        LeftFrontWheel.localRotation = Quaternion.Euler(LeftFrontWheel.localRotation.eulerAngles.x, (Input.GetAxis("Horizontal") * maxWheelTurn) , LeftFrontWheel.localRotation.eulerAngles.z);
        RightFrontWheel.localRotation = Quaternion.Euler(RightFrontWheel.localRotation.eulerAngles.x, (Input.GetAxis("Horizontal") * maxWheelTurn) , RightFrontWheel.localRotation.eulerAngles.z);

        //Actualización de rotación y velocidad
        currentSpeed = Mathf.SmoothStep(currentSpeed, speedInput, Time.deltaTime * 12f); speedInput = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotation, Time.deltaTime * 4f); rotation = 0f;

        //Iguala las posiciones del Rigidbody y el modelo
        transform.position = KartRB.transform.position;
    }

    private void FixedUpdate()
    {
        grounded = false;
        RaycastHit hit;


        //Raycast para confirmar el suelo e inclinar el vehículo para encajar con la pendiente

        if (Physics.Raycast(GroundRayPoint.position, -transform.up, out hit, GroundRayLength, isGround))
        {
            grounded = true;

            //Normal de la pendiente
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation, 0.2f);

            //Frenada si sale de pista
            if (hit.transform.gameObject.tag != "Track") actualForwardAccel = forwardAccel * 0.5f;
            else if(!boosting) actualForwardAccel = forwardAccel;
        }

        if (grounded) //Movimiento del Kart en el suelo
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + currentRotate, transform.eulerAngles.z), Time.deltaTime * 5f);
            KartRB.drag = dragOnGround;

            if (!drifting) KartRB.AddForce(transform.forward * currentSpeed);
            
            else KartRB.AddForce(((-transform.right * driftDirection + transform.forward) / 1.5f) * currentSpeed );

        }

        else //Simulación de gravedad
        {
            KartRB.drag = 0.1f;
            KartRB.AddForce(Vector3.up * -gravityForce * 100f);
        }
    }

    public void Steer(int direction, float amount) //Giro
    {
        rotation = (turnStrenght * direction) * amount;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "AI")
        Physics.IgnoreCollision(KartRB.GetComponent<Collider>(), other.gameObject.GetComponent<Collider>(), true);
    }

    IEnumerator Boost(float time) //Acelerón de velocidad
    {
        boostTime = 0;
        boosting = true;
        turbo.SetActive(true);
        actualForwardAccel += boostPower;
        yield return new WaitForSeconds(Mathf.Min(Mathf.RoundToInt(time), maxBoostTime)); //Tiempo que tarda en acabar
        actualForwardAccel -= boostPower;
        turbo.SetActive(false);
        boosting = false;
    }
}
