using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartIA : MonoBehaviour
{
    public bool win = false;
    public int counter = 1;

    [Header("Kart AI Setup")]
    private GameManager spawnPoint;
    private Transform spawnPlace;


    [Header("Kart Stats")]
    public float forwardAccel = 8f;
    public float reverseAccel = 4f;
    public float boostPower = 5f;
    public float turnStrenght = 180f;
    public float brakeForce = 5f;
    public float maxBoostTime = 3f;
    public float maxSpeed = 50f;


    private float currentSpeed, currentRotate;
    private float currentMaxspeed;

    private bool grounded;

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

    [Header("Kart Path")]
    public List<Transform> Checkpoints;
    private int currentCheckpoint = 0;
    private Vector3 randomFactor;
    public Transform[] pathTransforms;

    void Start()
    {
        //KartSpawn SetUp
        KartRB.transform.parent = null;
        randomFactor = new Vector3(Random.insideUnitSphere.x * 3, 0, Random.insideUnitSphere.z * 3);
        Physics.IgnoreCollision(KartRB.GetComponent<Collider>(), KartRB.GetComponent<Collider>(), true);
    }

    void Update()
    {
        //Iguala las posiciones del Rigidbody y el modelo
        transform.position = KartRB.transform.position;
        CheckWaypointDistance();
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
            if (hit.transform.gameObject.tag != "Track") currentMaxspeed = maxSpeed * 0.8f;  
            else currentMaxspeed = maxSpeed;
        }

        if (grounded)        //Caracteristicas del Kart en Ground
        {
            KartRB.drag = dragOnGround;
        }

        else                 //Simulacion de gravedad
        {
            KartRB.drag = 0.1f;
            KartRB.AddForce(Vector3.up * -gravityForce * 100f);
        }
        ApplySteer();
        Drive();
    }

    private void ApplySteer() //Giro de la IA
    {
        //Cálculo de ángulo de giro
        Vector3 relativeVector = transform.InverseTransformPoint(Checkpoints[currentCheckpoint].position + randomFactor);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * turnStrenght;

        //Interpolación para suavizar el giro
        currentRotate = Mathf.Lerp(currentRotate, newSteer, Time.deltaTime * 10 / (currentSpeed/750));

        //Giro del vehículo
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + currentRotate, transform.eulerAngles.z), Time.deltaTime * 5f);
        KartModel.localEulerAngles = Vector3.Lerp(KartModel.localEulerAngles, new Vector3(KartModel.localEulerAngles.x, Mathf.LerpAngle(KartModel.localEulerAngles.y, (currentRotate), 0.2f), KartModel.localEulerAngles.z), .2f);

    }

    private void Drive()     //Aceleración de la IA
    {
        //Interpolación de aceleración
        currentSpeed = Mathf.SmoothStep(currentSpeed, currentMaxspeed * 100, Time.deltaTime * forwardAccel);
        if (currentSpeed <= currentMaxspeed * 100)
        {
            KartRB.AddForce(transform.forward * currentSpeed);
        }
        else currentSpeed = maxSpeed - 100;
    }

    private void CheckWaypointDistance() //Cálculo de distancia y avance de los checkpoints
    {
        if (Vector3.Distance(transform.position, (Checkpoints[currentCheckpoint].position + randomFactor)) < 10f) //Siguiente Checkpoint
        {
            if (currentCheckpoint == Checkpoints.Count - 1) //Reseteo Checkpoints
            {
                currentCheckpoint = 0;
                counter++;
                if (counter == 3)
                {
                    FindObjectOfType<GameManager>().LooseGame();
                }
            }
            else //Avance Checkpoint
            {
                currentCheckpoint++;
                randomFactor = new Vector3(Random.insideUnitSphere.x * 5, 0, Random.insideUnitSphere.z * 5); //Factor de trazado aleatorio
            }
        }
    }

    public void Difficulty(int difficulty)
    {
        if (difficulty == 1)
        {
            forwardAccel = 4f;
            reverseAccel = 2f;
            turnStrenght = 100f;
            brakeForce = 5f;
            maxBoostTime = 1f;
            maxSpeed = 20f;
        }
        if (difficulty == 2)
        {
            forwardAccel = 6.5f;
            reverseAccel = 3.5f;
            turnStrenght = 145f;
            brakeForce = 5f;
            maxBoostTime = 2f;
            maxSpeed = 40f;
        }
        if (difficulty == 3)
        {
            forwardAccel = 6f;
            reverseAccel = 3f;
            turnStrenght = 145f;
            brakeForce = 5f;
            maxBoostTime = 2f;
            maxSpeed = 36.5f;
        }
        if (difficulty == 4)
        {
            forwardAccel = 4.25f;
            reverseAccel = 2.25f;
            turnStrenght = 80f;
            brakeForce = 5f;
            maxBoostTime = 1f;
            maxSpeed = 22f;
        }
        if (difficulty == 5)
        {
            forwardAccel = 3.75f;
            reverseAccel = 1.75f;
            turnStrenght = 80f;
            brakeForce = 5f;
            maxBoostTime = 1f;
            maxSpeed = 18f;
        }
        if (difficulty == 6)
        {
            forwardAccel = 6.25f;
            reverseAccel = 3.5f;
            turnStrenght = 145f;
            brakeForce = 5f;
            maxBoostTime = 2f;
            maxSpeed = 42.5f;
        }
        if (difficulty == 7)
        {
            forwardAccel = 6f;
            reverseAccel = 3.75f;
            turnStrenght = 300f;
            brakeForce = 5f;
            maxBoostTime = 2f;
            maxSpeed = 50f;
        }
        if (difficulty == 8)
        {
            forwardAccel = 5.5f;
            reverseAccel = 3.75f;
            turnStrenght = 275f;
            brakeForce = 5f;
            maxBoostTime = 1f;
            maxSpeed = 40f;
        }
        if (difficulty == 9)
        {
            forwardAccel = 6.25f;
            reverseAccel = 3.75f;
            turnStrenght = 275f;
            brakeForce = 5f;
            maxBoostTime = 1f;
            maxSpeed = 55f;
        }
        if (difficulty == 10)
        {
            forwardAccel = 9.5f;
            reverseAccel = 3f;
            turnStrenght = 250f;
            brakeForce = 5f;
            maxBoostTime = 1f;
            maxSpeed = 55f;
        }
        if (difficulty == 11)
        {
            forwardAccel = 9f;
            reverseAccel = 2.5f;
            turnStrenght = 250f;
            brakeForce = 5f;
            maxBoostTime = 1f;
            maxSpeed = 50f;
        }
        if (difficulty == 12)
        {
            forwardAccel = 7.5f;
            reverseAccel = 2f;
            turnStrenght = 250f;
            brakeForce = 5f;
            maxBoostTime = 1f;
            maxSpeed = 40f;
        }
    }
}
