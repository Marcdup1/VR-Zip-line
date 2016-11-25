using UnityEngine;
using System.Collections;

public class WandManager : MonoBehaviour
{
    public WandController wandRight;
    public WandController wandLeft;

    public Transform targetOne;
    public Transform targetTwo;
    public Transform targetThree;
    public float speedOne = 0.5f;
    public float speedTwo = 2.0f;
    public float speedThree = 2.0f;

    public float savespeedOne = 0.5f;
    public float savespeedTwo = 2.0f;
    public float savespeedThree = 2.0f;


    public GameObject platform;
    private bool isInPlatform = false;
    private bool motionStarted = false;
    private bool targetOneReached = false;
    private bool targetTwoReached = false;
    private bool targetThreeReached = false;
    private Rigidbody rigidbody;

    private Vector3 desiredVelocity;
    private float lastSqrMag;
    public Vector3 startpos;
    public Transform startcampos;

    AudioSource audio;

    public AudioClip windZiplineSound;
    public AudioClip ziplineSound;
    public AudioClip windFalling;
    public AudioClip hitGround;

    void Start()
    {

        savespeedOne = speedOne;
        savespeedTwo = speedTwo;
        savespeedThree = speedThree;

        rigidbody = GetComponent<Rigidbody>();
        startpos = startcampos.position;
        //Vector3 directionalVector = (target.position - transform.position).normalized * speed;

        lastSqrMag = Mathf.Infinity;

       audio = GetComponent<AudioSource>();

    }
    void reset(){
        audio.Stop();
        audio.Play();
        transform.position = startpos;
        wandRight.reset();
        wandLeft.reset();
        motionStarted = false;
        wandRight.increaseGrip((ushort)(0));
        wandLeft.increaseGrip((ushort)(0));
        rigidbody.velocity = (targetTwo.position - transform.position).normalized * 0;
        speedOne = savespeedOne;
        speedTwo = savespeedTwo;
        speedThree = savespeedThree;
        playingWindzipline = false;
        playingZipline = false;
        playingWindFalling = false;
        playingHitGround = false;
    }

    private bool playingWindzipline = false;
    private bool playingZipline = false;
    private bool playingWindFalling = false;
    private bool playingHitGround = false;
    void Update()
    {
        bool wandRightGrip = wandRight.getIsGripPressed();
        bool wandLeftGrip = wandLeft.getIsGripPressed();
        bool touchdown = wandLeft.istouchpadDown && wandRight.istouchpadDown;
        if (touchdown)
        {
            reset();
        }

        isInPlatform = wandRight.getIsInPlatform() && wandLeft.getIsInPlatform();
        targetOneReached = wandRight.getTargetOneReached() && wandLeft.getTargetOneReached();
        targetTwoReached = wandRight.getTargetTwoReached() && wandLeft.getTargetTwoReached();
        targetThreeReached = wandRight.getTargetThreeReached() && wandLeft.getTargetThreeReached();

        if (wandRightGrip && wandLeftGrip && isInPlatform || motionStarted && !targetThreeReached)
        {
           // audio.Stop();
            if (!playingWindzipline)
            {
                audio.PlayOneShot(windZiplineSound, 1F);
                playingWindzipline = true;
            }

            if (!playingZipline)
            {
                audio.PlayOneShot(ziplineSound, 1F);
                playingZipline = true;
            }

            if (!wandRightGrip || !wandLeftGrip)
            {
                audio.Stop();
                if (!playingWindFalling)
                {
                    audio.PlayOneShot(windFalling, 1F);
                    playingWindFalling = true;
                }
                Debug.Log("falling");
                motionStarted = false;
                
            }
            else
            {
                motionStarted = true;

                if (!targetOneReached)
                {
                    //motionStarted = false;
                    //desiredVelocity = Vector3.zero;

                    wandRight.increaseGrip((ushort) (wandRight.gripVelocity + 100 * Time.deltaTime));
                    wandLeft.increaseGrip((ushort)(wandLeft.gripVelocity + 100 * Time.deltaTime));
                    speedOne += 4 * Time.deltaTime;
                    rigidbody.velocity = (targetOne.position - transform.position).normalized * speedOne;
                }
                else if (!targetTwoReached && targetOneReached)
                {
                    //audio.clip = wind;
                   // windZipline.Play();
                    speedTwo += 23 * Time.deltaTime;
                    wandRight.increaseGrip((ushort)(wandRight.gripVelocity + 300 * Time.deltaTime));
                    wandLeft.increaseGrip((ushort)(wandLeft.gripVelocity + 300 * Time.deltaTime));
                    rigidbody.velocity = (targetTwo.position - transform.position).normalized * speedTwo;
                }
                else if (targetOneReached && targetTwoReached && !targetThreeReached)
                {
                    Debug.Log(speedThree);
                    wandRight.increaseGrip((ushort)(wandRight.gripVelocity - 800 * Time.deltaTime));
                    wandLeft.increaseGrip((ushort)(wandLeft.gripVelocity -800 * Time.deltaTime));
                    
                    if (speedThree <= 0.0f)
                    {
                        Debug.Log("YESSSS");
                        wandRight.increaseGrip((ushort)(0));
                        wandLeft.increaseGrip((ushort)(0));
                        motionStarted = false;
                    }
                    else
                    {
                        speedThree -= 30 * Time.deltaTime;
                    }
                    rigidbody.velocity = (targetThree.position - transform.position).normalized * speedThree;
                }
                else if (targetOneReached && targetTwoReached && targetThreeReached)
                {
                    motionStarted = false;
                    //desiredVelocity = Vector3.zero;
                }
                Debug.Log("both pressed");



            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {

        if (collider.tag == "mountain")
        {
            audio.Stop();
            audio.Play();
            audio.PlayOneShot(hitGround, 1F);
            playingHitGround = true;
            StartCoroutine(Example());
        }

    }

    IEnumerator Example()
    {
        yield return new WaitForSeconds(3);
        reset();
    }



    void FixedUpdate()
    {
        //rigidbody.velocity = desiredVelocity;
    }

}
