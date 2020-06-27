

/// Original Code written and designed by Aeden C Graves.
/// Rewritten by Marnix Licht.
///
///
///CHANGE LOG:
///
/// DATE || msg: "" || Author Signature: SNG || version VERSION
///
/// 25.05/19 || msg: "orientation independ
/// 03/19/19 || msg: "Added a rudimentary slope detection system." || Author Signature: Aedan Graves || version 19.3.18a >> 19.3.19cu
/// 03/18/19 || msg: "Fixed Stamina" || Author Signature: Aedan Graves || version 19.3.11p >> 19.3.18a
/// 03/02/19 || msg: "Improved camera System" || Author Signature: Aecan Graves || version 19.3.2 >> 19.3.11p
/// 03/02/19 || msg: "Lowered maximum walk, sprint, and jump values" || Author Signature: Aedan Graves || version: 19.2.21 >> 19.3.2
/// 02/21/19 || msg: "Removed dynamic speed curve. Modified headbob logic || Author Signature: Aedan C Graves || version: 19.2.15 >> 19.2.21
/// 02/15/19 || msg: "Added Camera shake. Made it possable to disable camera movement when jumping and landing." || Author Signature: Aedan C Graves || version: 19.2.12 >> 19.2.15
/// 02/12/19 || msg: "Seperated Dynamic Footsteps from the Headbob calculations." || Author Signature: Aedan C Graves || version: 1.6b >> 19.2.12
/// 02/08/19 || msg "Added some more tooltips." || Author Signature: Aedan C Graves || version 1.6a >> 1.6b
/// 02/04/19 || msg "Changed crouch funtion to use an In Editor defined input axis" || Author Signature: Aedan C Graves || version 1.6 >> 1.6a
/// 12/13/18 || msg: "Added 'Custom' entry for Dynamic footstep system" || Author Signature: Aedan C Graves || version 1.5b >> 1.6
/// 12/11/18 || msg: "Added Volume control to Footstep and Jump/land SFX." || Author Signature: Aedan C Graves || version 1.5a >> 1.5b
/// 02/18/18 || msg: "Updated mouse rotation to allow pre-play rotiation." || Author Signature: Aedan C Graves || version 1.5 >> 1.5a
/// 01/31/18 || msg: "Changed Dynamic footstep system to use physics materials." || Author Signature: Aedan C Graves || version 1.4c >> 1.5
/// 12/19/17 || msg: "Added headbob passthrough variables" || Auther Signature: Aeden C Graves || version 1.4b >> 1.4c
/// 12/02/17 || msg: "Made camera movement toggleable" || Auther Signature: Aeden C Graves || version 1.4a >> 1.4b
/// 10/16/17 || msg: "Made all sounds optional." || Author Signature: Aedan C Graves || version 1.4 >> 1.4a
/// 10/09/17 || msg: "Added Optional FOV Kick" || Author Signature: Aedan C Graves || version 1.3b >> 1.4
/// 10/08/17 || msg: "Improved Dynamic Footsteps." || Author Signature: Aedan C Graves || version 1.3a >> 1.3b
/// 10/07/17 || msg: "BetaTesting Class" || Author Signature: Aedan C Graves || version 1.3 >> 1.3a
/// 10/07/17 || msg: "Added Optional Dynamic Footsteps. Added optional Dynamic Speed Curve." || Author Signature: Aedan C Graves || version 1.2 >> 1.3
/// 10/03/17 || msg: "Added optional Crouch." || Author Signature: Aedan C Graves || version v1.1 >> v1.2
/// 09/26/17 || msg: "Fixed Headbobbing in mid air. Added a option for head bobbing, Added optional Stamina. Added Auto Crosshair Feature." || Author Signature: Aedan C Graves|| version v1.0 >> v1.1
/// 09/21/17 || msg: "Finished SMB FPS Logic." || Author Signature: Aedan C Graves || version v0.0 >> v1.0
///
/// 
/// 
/// Made changes that you think should come "Out of the box"? E-mail the modified Script with A new entry on the top of the Change log to: modifiedassets@aedangraves.info

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

[AddComponentMenu("First Person AIO")]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class FirstPersonAIO : MonoBehaviour {
    
    #region Script Header and Cosmetics
    [Header("            Aedan Graves' First Person All-in-One v19.3.19cu", order = 0)]
    [Space(30, order = 1)]
    #endregion

    #region Variables

    #region Input Settings

    #endregion

    #region Look Settings
    [Header("Mouse Rotation Settings", order = 2)]
    
    [Space(8, order = 3)]
    [Tooltip("Determines whether the player can move camera or not.")] public bool enableCameraMovement;
    [Tooltip("from center, how much - range in degrees - does the camera have to move up and down.")][Range(0,180)] public float rotationRange = 170;
    [Tooltip("Determines how sensitive the mouse is.")] [Range(0.01f, 35)] public float mouseSensitivity = 10f;
    [Tooltip("Mouse Smoothness.")] [Range(1, 100)] public float cameraSmoothing = 5f;
    [Tooltip("For Debuging or if You don't plan on having a pause menu or quit button.")] public bool lockAndHideCursor = false;
    [Tooltip("Camera that you wish to rotate.")] public Transform playerCamera;
    [Tooltip("Call this Coroutine externaly with duration ranging from 0.01 to 1, and a magnitude of 0.01 to 0.5.")] public bool enableCameraShake=false;
    internal Vector3 cameraStartingPosition;


    //[SerializeField] [Tooltip("Automatically Create Crosshair")] private bool autoCrosshair;
    public Sprite Crosshair;

    [HideInInspector]
    public Vector3 targetAngles;
    private Vector3 followAngles;
    private Vector3 followVelocity;
    private Quaternion originalRotation;
    public bool hasTeleported = false;
    private Vector3 velocityAfterTeleport;
    private Vector3 dampVelocity;
    private float startTime;

    [Space(15, order = 4)]

    #endregion

    #region Movement Settings
    [Header("Movement Settings", order = 5)]
    [Space(8, order = 6)]
        
    [Tooltip ("Determines whether the player can move.")] public bool playerCanMove = true;
    [Tooltip("If true; Left shift = Sprint. If false; Left Shift = Walk.")] [SerializeField] private bool walkByDefault = true;
    [Tooltip("Determines how fast Player walks.")] [Range(0.1f, 10)] public float walkSpeed = 4f;
    [Tooltip("Determines how fast Player Sprints.")] [Range(0.1f, 20)] public float sprintSpeed = 8f;
    [Tooltip("Determines how high Player Jumps.")] [Range(0.1f, 15)] public float jumpPower = 5f;
    [Tooltip("Determines if the jump button needs to be pressed down to jump, or if the player can hold the jump button to automaticly jump every time the it hits the ground.")] public bool canHoldJump;
    [Tooltip("Determines whether to use Stamina or not.")] [SerializeField] private bool useStamina = true;
    [Tooltip("Determines how quickly the players stamina runs out")] [SerializeField] [Range(0.1f, 9)] private float staminaDepletionSpeed = 2f;
    [Tooltip("Determines how much stamina the player has")] [SerializeField] [Range(0, 100)] private float Stamina = 50;
    [HideInInspector] public float speed;
    [HideInInspector] public float backgroundStamina;
    
    [System.Serializable]
    public class CrouchModifiers {
        [Tooltip("Determines whether to use Crouch or not.")] public bool useCrouch = true;
        [SerializeField] [Tooltip("Name of the Input Axis you wish to use for crouching, this must be set up in the InputManager.")]public string CrouchInputAxis;
        [SerializeField] [Range(0.1f, 4f)] internal float walkSpeedWhileCrouching = 2f;
        [SerializeField] [Range(0.1f, 8f)] internal float sprintSpeedWhileCrouching = 2f;
        [SerializeField] [Range(0f, 5f)] internal float jumpPowerWhileCrouching = 0f;
        internal float defaultWalkSpeed;
        internal float defaultSprintSpeed;
        internal float defaultStrafeSpeed;
        internal float defaultJumpPower;
    }
    public CrouchModifiers _crouchModifiers = new CrouchModifiers();
    [System.Serializable]
    public class FOV_Kick
    {
        [SerializeField] internal bool useFOVKick = false;
        [SerializeField] [Range(0, 10)] internal float FOVKickAmount = 4;
        [SerializeField] [Range(0.01f, 5)] internal float changeTime = 0.1f;
        [SerializeField] internal AnimationCurve KickCurve;
        internal bool fovKickReady = true;
        internal float fovStart;
    }
    public FOV_Kick fOVKick = new FOV_Kick();
    [System.Serializable]
    public class AdvancedSettings {
        [Tooltip("Changes the multiplication factor of the Engine's current gravitational force")] [Range(0.01f, 5.0f   )] public float gravityMultiplier = 1.0f;
        public PhysicMaterial zeroFrictionMaterial;
        public PhysicMaterial highFrictionMaterial;
        [Space(10)]
        [Tooltip("Currently buggy; Determines if the slope detection/limiting system should be used.")]public bool useSlopeDetection = true;
        [Range(0,89)] public float maxSlopeAngle=70;
        [HideInInspector] public bool tooSteep;
        [HideInInspector] public RaycastHit surfaceAngleCheck;
    }
    [SerializeField] private AdvancedSettings advanced = new AdvancedSettings();
    private CapsuleCollider capsule;
    private const float jumpRayLength = 1.4f;
    public bool IsGrounded { get; private set; }
    Vector2 inputXY;
    public bool isCrouching { get; private set; }
    bool isSprinting = false;

    [HideInInspector] public Rigidbody fps_Rigidbody;
    [Space(15, order = 7)]

    #endregion

    #region Headbobbing Settings
    [Header("Headbobbing Settings", order = 8)]
    [Space(8, order = 9)]

    [Tooltip("Determines Whether to use headbobing or not")] public bool useHeadbob = true;
    [SerializeField] [Tooltip("Parent Of Player Camera")] private Transform head;
    [Tooltip("Overall Speed of Headbob")] [Range(0.1f, 10)] public float headbobFrequency = 1.5f;
    [Tooltip("Headbob Sway Angle")] [Range(0,10)] public float headbobSwayAngle = 5f;
    [Tooltip("Headbob Height")][Range(0,10)]  public float headbobHeight = 3f;
    [Tooltip("Headbob Side Movement")][Range(0,10)]  public float headbobSideMovement =5f;
    [HideInInspector] public float headbobSpeedMultiplier = 3f;
  
    [Tooltip("Determines if the headbob system will react to jumping and lading")]public bool useJumdLandMovement = true;
    [Tooltip("Determines how much the head jolts when Jumping")][Range(0,10)] public float jumpAngle =3f;
    [Tooltip("Determines how much the head jolts when landing")] public float landAngle = 60;
    private Vector3 originalLocalPosition;
     Vector3 previousPosition;
    Vector3 previousVelocity = Vector3.zero;
    bool previousGrounded;
    AudioSource audioSource;

    private Vector3 dMove = Vector3.zero;

    [Space(15, order = 10)]
    #endregion

    #region Audio Settings
    [Header("Audio/SFX Settings", order = 11)]
    [Space(4, order = 12)]
    
    //[SerializeField] [Tooltip("Volume to play the Footsteps with.")] [Range(0,10)] private float Volume = 5f;
    [Space(4, order = 13)]
    [SerializeField] [Tooltip("The Sound made when jumping. Not Used in Dynamic Foot Steps mode.")] private GameObject jumpSound;
    [SerializeField] [Tooltip("The Sound made when landing from a jump or a fall. Not Used in Dynamic Foot Steps mode.")] private GameObject landSound;
    [SerializeField] [Tooltip("Determines Whether to use movement Sounds.")] private bool _useFootStepSounds = false;
    [SerializeField] [Tooltip("Foot step Sounds. Will also act as a fall back for the Dynamic Foot Steps.")] private AudioClip[] footStepSounds;
 
    [System.Serializable]
    public class DynamicFootStep
    {

        //Not Easily changeable at the moment
        [Tooltip("Should the controller use dynamic footsteps? For this to work properly, A physics material must be assigned to both this scipt and the collider you wish give sound fx to. I.e: To use the grass fx, assign a physics material to the 'Grass' slot below, as well as the collider you wish to act as a grassy area")]public bool useDynamicFootStepProcess;
        public PhysicMaterial _Wood;
        public PhysicMaterial _metalAndGlass;
        public PhysicMaterial _Grass;
        public PhysicMaterial _dirtAndGravle;
        public PhysicMaterial _rockAndConcrete;
        public PhysicMaterial _Mud;
        public PhysicMaterial _CustomMaterial;
        internal AudioClip[] qikAC;

        [Tooltip("Audio clips to be played while walking on the Wood physics material")] public AudioClip[] _woodFootsteps;
        [Tooltip("Audio clips to be played while walking on the Metal or Glass physics material")] public AudioClip[] _metalAndGlassFootsteps;
        [Tooltip("Audio clips to be played while walking on the Grass physics material")] public AudioClip[] _GrassFootsteps;
        [Tooltip("Audio clips to be played while walking on the Dirt or Gravelphysics material")] public AudioClip[] _dirtAndGravelFootsteps;
        [Tooltip("Audio clips to be played while walking on the Rock or Concrete physics material")] public AudioClip[] _rockAndConcreteFootsteps;
        [Tooltip("Audio clips to be played while walking on the Mud physics material")] public AudioClip[] _MudFootsteps;
        [Tooltip("Audio clips to be played while walking on the Custom physics material")] public AudioClip[] _CustomMaterialFoorsteps;

    }
    public DynamicFootStep dynamicFootstep = new DynamicFootStep();

    #endregion

    #region BETA Settings
    /*
     [System.Serializable]
public class BETA_SETTINGS{

}

            [Space(15)]
    [Tooltip("Settings in this feild are currently in beta testing and can prove to be unstable.")]
    [Space(5)]
    public BETA_SETTINGS betaSettings = new BETA_SETTINGS();
     */

    #endregion

    #endregion

    [Header("AnimationSettings")]
    public Animator anim;
    private Animator animC;
    public bool isFalling;
    private bool isTouching;

    public GameObject MoonTranstiion;
    public GameObject SunTransition;

    private void Awake()
    {


        #region Look Settings - Awake
        originalRotation = transform.localRotation;

        #endregion

        #region Movement Settings - Awake
        capsule = GetComponent<CapsuleCollider>();
        IsGrounded = true;
        isCrouching = false;
        fps_Rigidbody = GetComponent<Rigidbody>();
        _crouchModifiers.defaultWalkSpeed = walkSpeed;
        _crouchModifiers.defaultSprintSpeed = sprintSpeed;
        _crouchModifiers.defaultStrafeSpeed = walkSpeed;
        _crouchModifiers.defaultJumpPower = jumpPower;
        #endregion

        #region Headbobbing Settings - Awake

        #endregion

        #region BETA_SETTINGS - Awake
    
#endregion

    }

    private void Start()
    {
        startTime = Time.time;
        #region Look Settings - Start

        //if (autoCrosshair)
        //{
        //    GameObject qui = new GameObject("AutoCrosshair");
        //    qui.AddComponent<RectTransform>();
        //    qui.AddComponent<Canvas>();
        //    qui.AddComponent<CanvasScaler>();
        //    qui.AddComponent<GraphicRaycaster>();
        //    qui.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        //    GameObject quic = new GameObject("Crosshair");
        //    quic.AddComponent<Image>().sprite = Crosshair;

        //    qui.transform.SetParent(this.transform);
        //    qui.transform.position = Vector3.zero;
        //    quic.transform.SetParent(qui.transform);
        //    quic.transform.position = Vector3.zero;
        //}
        cameraStartingPosition = playerCamera.localPosition;
        if(lockAndHideCursor) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        #endregion

        #region Movement Settings - Start
        backgroundStamina = Stamina*10;

        #endregion

        #region Headbobbing Settings - Start
        originalLocalPosition = head.localPosition;
        if(GetComponent<AudioSource>() == null) { gameObject.AddComponent<AudioSource>(); }
        previousPosition = fps_Rigidbody.position;
        audioSource = GetComponent<AudioSource>();
        #endregion

        #region BETA_SETTINGS - Start
        fOVKick.fovStart = playerCamera.GetComponent<Camera>().fieldOfView;
        #endregion

    }
    
    public void portalRotation(Quaternion rotation, Vector3 velocityOut)
    {
        originalRotation = rotation;
        followAngles = new Vector3(followAngles.x, 0f, 0f);
        targetAngles = new Vector3(targetAngles.x, 0f, 0f);
        hasTeleported = true;
        velocityAfterTeleport = velocityOut;
    }

    private void LateUpdate()
    {
        if(animC == null && transform.childCount == 3)
        {
            animC = transform.GetChild(2).GetComponent<Animator>();
        }

        #region Animation Settings - Update
        if (Input.GetAxis("Vertical") != 0 && Input.GetAxis("Horizontal") != 0)
        {
            //anim.SetFloat("jogSpeed", Input.GetAxis("Vertical"));
            if (Input.GetAxis("Horizontal") > 0 && Input.GetAxis("Vertical") > 0)
            {
                anim.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
                anim.SetInteger("movementState", 6);
                if (animC != null)
                {
                    animC.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
                    animC.SetInteger("movementState", 6);
                }
   
            }
            else if (Input.GetAxis("Horizontal") < 0 && Input.GetAxis("Vertical") > 0)
            {
                anim.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
                anim.SetInteger("movementState", 7);
                if (animC != null)
                {
                    animC.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
                    animC.SetInteger("movementState", 7);
                }
            }
            else if (Input.GetAxis("Horizontal") > 0 && Input.GetAxis("Vertical") < 0)
            {
                anim.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
                anim.SetInteger("movementState", 7);
                if (animC != null)
                {
                    animC.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
                    animC.SetInteger("movementState", 7);
                }
            }
            else if (Input.GetAxis("Horizontal") < 0 && Input.GetAxis("Vertical") < 0)
            {
                anim.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
                anim.SetInteger("movementState", 6);
                if (animC != null)
                {
                    animC.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
                    animC.SetInteger("movementState", 6);
                }
            }
        }
        else if (Input.GetAxis("Vertical") != 0)
        {
            anim.SetInteger("movementState", 1);
            if (animC != null)
                animC.SetInteger("movementState", 1);
            anim.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
            if (animC != null)
                animC.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                anim.SetInteger("movementState", 5);
                if (animC != null)
                    animC.SetInteger("movementState", 5);
            }
            else
            {
                anim.SetInteger("movementState", 4);
                anim.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
                if (animC != null)
                    animC.SetInteger("movementState", 4);
                if (animC != null)
                    animC.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
            }
        }
        //Deze zet ik uit
        //else if (Input.GetAxis("Horizontal") == 0 && Input.GetKey(KeyCode.LeftShift) || Input.GetAxis("Vertical") != 0 && Input.GetKey(KeyCode.LeftShift))
        //{
        //    anim.SetInteger("movementState", 2);
        //    if (animC != null)
        //        animC.SetInteger("movementState", 2);
        //    anim.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
        //    if (animC != null)
        //        animC.SetFloat("walkSpeed", Input.GetAxis("Vertical"));
        //}
        //else if (Input.GetAxis("Horizontal") <= 0 && Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") != 0)
        //{
        //    anim.SetInteger("movementState", 7);
        //    if (animC != null)
        //        animC.SetInteger("movementState",7);
        //}
        //else if (Input.GetAxis("Horizontal") >= 0 && Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") != 0)
        //{
        //    anim.SetInteger("movementState", 6);
        //    if (animC != null)
        //        animC.SetInteger("movementState", 6);
        //}
        else
        {
            anim.SetInteger("movementState", 0);
            if (animC != null)
                animC.SetInteger("movementState", 0);
            anim.SetFloat("rotateSpeed", Input.GetAxis("Mouse X") * mouseSensitivity);
            if (animC != null)
                animC.SetFloat("rotateSpeed", Input.GetAxis("Mouse X") * mouseSensitivity);
        }

        //Debug.Log((Vector3.Dot(fps_Rigidbody.velocity, -transform.up)));
        //if ((Vector3.Dot(fps_Rigidbody.velocity, -transform.up) > 8.2f && !IsGrounded) || isFalling)
        //{
        //    anim.SetBool("Falling", true);
        //    anim.SetBool("Jumping", false);
        //    if (animC != null)
        //    {
        //        animC.SetBool("Falling", true);
        //        animC.SetBool("Jumping", false);
        //    }

        //    isFalling = true;
        //    head.localPosition = Vector3.Lerp(head.localPosition, new Vector3(0.3f, 1.573f, 0.207f), 0.05f);
        //    head.localEulerAngles = Vector3.Lerp(head.localEulerAngles, new Vector3(7.5f, 0f, 0f), 0.05f);
        //}
        if(IsGrounded)
        {
            isFalling = false;
            anim.SetBool("Jumping", false);
            anim.SetBool("Falling", false);
            if (animC != null)
            {
                animC.SetBool("Jumping", false);
                animC.SetBool("Falling", false);
            }
                

            head.localPosition = Vector3.Lerp(head.localPosition, new Vector3(0f, 2.3f, 0), 0.1f);
            head.localEulerAngles = Vector3.Lerp(head.localEulerAngles, new Vector3(0.0f, 0f, 0f), 0.1f);
        }
        #endregion



        if (enableCameraMovement)
        {
            float mouseXInput;
            float mouseYInput;
            mouseXInput = Input.GetAxis("Mouse Y");
            mouseYInput = Input.GetAxis("Mouse X");
            if(targetAngles.y > 180) { targetAngles.y -= 360; followAngles.y -= 360; } else if(targetAngles.y < -180) { targetAngles.y += 360; followAngles.y += 360; }
            if(targetAngles.x > 180) { targetAngles.x -= 360; followAngles.x -= 360; } else if(targetAngles.x < -180) { targetAngles.x += 360; followAngles.x += 360; }
            targetAngles.y += mouseYInput * mouseSensitivity;
            targetAngles.x += mouseXInput * mouseSensitivity;
            targetAngles.y = Mathf.Clamp(targetAngles.y, -0.5f * Mathf.Infinity, 0.5f * Mathf.Infinity);
            targetAngles.x = Mathf.Clamp(targetAngles.x, -0.5f * rotationRange, 0.5f * rotationRange);
            followAngles = Vector3.SmoothDamp(followAngles, targetAngles, ref followVelocity, cameraSmoothing/100);

            Vector3 lookAngle = originalRotation * followAngles;
            
            transform.localRotation = originalRotation * Quaternion.Euler(new Vector3(0, followAngles.y, followAngles.z));
            playerCamera.localRotation = Quaternion.Euler(new Vector3(-followAngles.x, 0, 0));

            Physics.gravity = 9.8f * -transform.up;



        }                         


        #region Movement Settings - FixedUpdate

        if (Input.GetButton("Jump") && IsGrounded && isTouching)
        {
            landSound.SetActive(false);
            jumpSound.SetActive(false);
            jumpSound.SetActive(true);
            anim.SetBool("Jumping", true);
            if (animC != null)
            {
                animC.SetBool("Jumping", true);
            }

                //turned it off for a bit
                //fps_Rigidbody.AddForce(transform.up * 6.5f);
                //anim.SetBool("Jumping", true); //turned it off for a bit
            }

        bool wasWalking = !isSprinting;
        if (useStamina)
        {
            if (backgroundStamina > 0) { if (!isCrouching) { isSprinting = Input.GetKey(KeyCode.LeftShift); } } else { isSprinting = false; }
            if (isSprinting == true && backgroundStamina > 0) { backgroundStamina -= staminaDepletionSpeed; } else if (backgroundStamina < (Stamina * 10) && !Input.GetKey(KeyCode.LeftShift)) { backgroundStamina += staminaDepletionSpeed / 2; }
        }
        else { }
        //isSprinting = Input.GetKey(KeyCode.LeftShift);

        advanced.tooSteep = false;
        float inrSprintSpeed;
        inrSprintSpeed = sprintSpeed;
        Vector3 dMove = Vector3.zero;
        speed = walkByDefault ? isCrouching ? walkSpeed : (isSprinting ? inrSprintSpeed : walkSpeed) : (isSprinting ? walkSpeed : inrSprintSpeed);
        Ray ray = new Ray(transform.position, -transform.up);
        if ((isTouching && IsGrounded) || ((Vector3.Dot(fps_Rigidbody.velocity, transform.up)) < 0.05)) {
            RaycastHit[] hits = Physics.RaycastAll(ray, jumpRayLength);
            float nearest = float.PositiveInfinity;
            IsGrounded = false;

            for (int i = 0; i < hits.Length; i++) {
                if (!hits[i].collider.isTrigger && hits[i].distance < nearest && (hits[i].transform.tag == "LevelParts" || hits[i].transform.tag == "DirectionalLight")) {
                    IsGrounded = true;

                    landSound.SetActive(true);
                    nearest = hits[i].distance;                    
                }
            }
        }
        Debug.DrawLine(transform.position, transform.position - transform.up * jumpRayLength, Color.cyan);



        if (advanced.useSlopeDetection)
        {
            if (Physics.Raycast(transform.position + originalRotation * new Vector3(0f, - 0.75f, 0.1f), -transform.up, out advanced.surfaceAngleCheck, 1f))
            {

                if (Vector3.Angle(advanced.surfaceAngleCheck.normal, transform.up) < 89)
                {
                    advanced.tooSteep = false;
                    dMove = transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeed;
                    if (Vector3.Angle(advanced.surfaceAngleCheck.normal, transform.up) > advanced.maxSlopeAngle)
                    {
                        advanced.tooSteep = true;
                        isSprinting = false;
                        //dMove = originalRotation * (-4f * transform.up);

                    }
                    else if (Vector3.Angle(advanced.surfaceAngleCheck.normal, transform.up) > 44)
                    {
                        advanced.tooSteep = true;
                        isSprinting = false;
                        dMove = (transform.forward * inputXY.y * speed + transform.right * inputXY.x);// + ((Vector3.Angle(advanced.surfaceAngleCheck.normal, transform.up) / advanced.maxSlopeAngle) * (-4f * transform.up));
                    }
                }
            }

            else if (Physics.Raycast(transform.position + originalRotation * new Vector3(- 0.086f, - 0.75f, - 0.05f), -transform.up, out advanced.surfaceAngleCheck, 1f))
            {
                if (Vector3.Angle(advanced.surfaceAngleCheck.normal, transform.up) < 89)
                {
                    advanced.tooSteep = false;
                    dMove = transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeed;
                    if (Vector3.Angle(advanced.surfaceAngleCheck.normal, transform.up) > 70)
                    {
                        advanced.tooSteep = true;
                        isSprinting = false;
                        //dMove = originalRotation * new Vector3(0, -4, 0);

                    }
                    else if (Vector3.Angle(advanced.surfaceAngleCheck.normal, transform.up) > 45)
                    {
                        advanced.tooSteep = true;
                        isSprinting = false;
                        dMove = (transform.forward * inputXY.y * speed + transform.right * inputXY.x);// + ((Vector3.Angle(advanced.surfaceAngleCheck.normal, transform.up) / advanced.maxSlopeAngle) * (-4f * transform.up));

                    }
                }
                else if (Physics.Raycast(transform.position + originalRotation * new Vector3(0.086f, - 0.75f, - 0.05f), -transform.up, out advanced.surfaceAngleCheck, 1f))
                {

                    if (Vector3.Angle(advanced.surfaceAngleCheck.normal, transform.up) < 89)
                    {
                        advanced.tooSteep = false;
                        dMove = transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeed;
                        if (Vector3.Angle(advanced.surfaceAngleCheck.normal, transform.up) > 70)
                        {
                            advanced.tooSteep = true;
                            isSprinting = false;
                            //dMove = originalRotation * new Vector3(0, -4, 0);

                        }
                        else if (Vector3.Angle(advanced.surfaceAngleCheck.normal, transform.up) > 45)
                        {
                            advanced.tooSteep = true;
                            isSprinting = false;
                            dMove = (transform.forward * inputXY.y * speed + transform.right * inputXY.x);// + ((Vector3.Angle(advanced.surfaceAngleCheck.normal, transform.up) / advanced.maxSlopeAngle) * (-4f * transform.up));
                        }
                    }
                }
            }
            else
            {
                advanced.tooSteep = false;
                dMove = transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeed;
            }
        }
        else
        {  
            advanced.tooSteep = false;
            dMove = transform.forward * inputXY.y * speed + transform.right * inputXY.x * walkSpeed;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        inputXY = new Vector2(horizontalInput, verticalInput);
        if(inputXY.magnitude > 1) { inputXY.Normalize(); }

        //Max fall speed;
        if (fps_Rigidbody.velocity.magnitude > 20)
        {
            fps_Rigidbody.velocity = 20f * Vector3.Normalize(fps_Rigidbody.velocity);
        }

        //Velocity direction change after teleport;
        if (hasTeleported)
        {
            fps_Rigidbody.velocity = velocityAfterTeleport;
            hasTeleported = false;
            if (gameObject.layer == 12)
            {
                MoonTranstiion.SetActive(true);
                SunTransition.SetActive(false);
            }
            if (gameObject.layer == 11)
            {
                MoonTranstiion.SetActive(false);
                SunTransition.SetActive(true);
            }
        }

        //float yv = (Quaternion.Inverse(originalRotation) * fps_Rigidbody.velocity).y;





        float yv = Vector3.Dot(fps_Rigidbody.velocity, transform.up);

        bool didJump = canHoldJump?Input.GetButton("Jump"): Input.GetButtonDown("Jump");

        if (isTouching && IsGrounded && didJump && jumpPower > 0)
        {
            yv += jumpPower;
            IsGrounded = false;
            didJump=false;
        }

        if(playerCanMove)
        {         
            fps_Rigidbody.velocity = dMove + transform.up * yv;              
        } else{fps_Rigidbody.velocity = Vector3.zero;}

        if(dMove.magnitude > 0 || !IsGrounded || advanced.tooSteep) {
            GetComponent<Collider>().material = advanced.zeroFrictionMaterial;
        } else { GetComponent<Collider>().material = advanced.highFrictionMaterial; }

        if (IsGrounded)
        {
            fps_Rigidbody.AddForce(Physics.gravity * (2f));
        }
        else
        {
            fps_Rigidbody.AddForce(Physics.gravity * (advanced.gravityMultiplier - 1));
            //Debug.Log(fps_Rigidbody.velocity.magnitude);
        }


        if(_crouchModifiers.useCrouch &&  _crouchModifiers.CrouchInputAxis != string.Empty) {


            if(Input.GetButton(_crouchModifiers.CrouchInputAxis)) { if(isCrouching == false) {
                    capsule.height /= 2;
                  
                        walkSpeed = _crouchModifiers.walkSpeedWhileCrouching;
                        sprintSpeed = _crouchModifiers.sprintSpeedWhileCrouching;
                        jumpPower = _crouchModifiers.jumpPowerWhileCrouching;
                    
                     
                    isCrouching = true;

                } } else if(isCrouching == true) {
                capsule.height *= 2;
           
                walkSpeed = _crouchModifiers.defaultWalkSpeed;
                sprintSpeed = _crouchModifiers.defaultSprintSpeed;
                jumpPower = _crouchModifiers.defaultJumpPower;
          
                isCrouching = false;

            }

        }

        #endregion

        #region BETA_SETTINGS - FixedUpdate

        #endregion

    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "LevelParts")
        {
            isTouching = true;
        }            
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "LevelParts")
        {
            isTouching = false;
        }
    }

}




