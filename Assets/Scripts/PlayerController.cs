using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
public class PlayerController : MonoBehaviour
{

    BiometricsManager bioManager;
    //Move
    public float energyMoveReduction = 0.1f;
    public float maxSpeed = 10f;
    float currentSpeed;

    //Jump
    public float energyJumpReduction = 10f;
    public float maxJumpVelocity = 10f;
    float startJumpVelocity;
    float currentJumpVelocity;
    public float rotationSpeed = 0.1f;
    public GameObject jumpPoint;
    public GameObject arrow;
    public float fallGravityMultiplier = 2.5f;
    public float jumpForceIncrease = 0.1f;
    public float airSpeedIncrease = 1.25f;

    public float minY = 0;
    public float maxY = 1.5f;

    bool inLiquid = true;
    bool isJumping = false;
    bool hasEnergy = true;
    bool isAirbone = false;
    float rotationAngle;
    bool isDead = false;
    public float delay = 3f;
    public float heartDeathDelay = 3;
    float heartDeathDelayCounter;
    public float scoreMultiplier = 1f;
    public float deathDelay = 5f;
    float deathDelayCounter;
    Vector2 initialPos;
    Rigidbody2D rb;
    CinemachineImpulseSource hurtImpulse;
    public CinemachineImpulseSource startImpulse;
    public FilterManager filterManager;

    public SpriteRenderer sprite;
    public Animator animator;

    public AudioManager audioManager;

    public GameObject youDiedText;
    public TMP_Text scoreText;
    public TMP_Text highestScoreText;


    PostProcessVolume volume;
    ColorGrading colorGrading;
    Vignette vignette;
    float score = 0;
    float highestScore = 0;
    [HideInInspector]
    public bool hasEscaped = false;
    [HideInInspector]
    public bool isRunning = false;
    [HideInInspector]
    public bool isInvincibile = false;
    public GameObject fireParticles;
    // Start is called before the first frame update
    void Awake()
    {
        scoreText.text = score.ToString();
        startJumpVelocity = maxJumpVelocity / 2;
        currentSpeed = maxSpeed;
        currentJumpVelocity = startJumpVelocity;
        initialPos = jumpPoint.transform.localPosition;
        arrow.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
        bioManager = GetComponent<BiometricsManager>();
        hurtImpulse = GetComponent<CinemachineImpulseSource>();
        heartDeathDelayCounter = 0;
        volume = Camera.main.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out colorGrading);
        volume.profile.TryGetSettings(out vignette);
        highestScore = PlayerPrefs.GetFloat("HighestScore");
        highestScoreText.text = highestScoreText.text + " " + ((int)highestScore).ToString();
        

    }
    private void Start()
    {
        audioManager.Play("MainMenu");
        hasEscaped = false;
        isRunning = false;
        isInvincibile = false;
        scoreText.gameObject.SetActive(false);
    }
    public void Escaping()
    {
        hasEscaped = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasEscaped)
        {
            if (!isRunning)
            {
                isRunning = true;
                bioManager.isEscaping = true;

                StartCoroutine(IncreaseScore());
                EarthQuake();
            }
            if (!isDead)
            {
                MoveHorizontally();
                if (!hasEnergy)
                {
                    rb.velocity = new Vector2(rb.velocity.x / 5, rb.velocity.y);
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (hasEscaped)
        {
            if (!isDead)
            {
                if (bioManager.GetCurrentEnergyValue() <= 1)
                {
                    hasEnergy = false;
                }
                else if (bioManager.GetCurrentEnergyValue() > 0)
                {
                    hasEnergy = true;
                }
                QuickerFall();
                Jump();
            }

            if (bioManager.GetCurrentEnergyValue() <= 0 && !isDead)
            {
                colorGrading.temperature.Interp(colorGrading.temperature, -50, Time.fixedDeltaTime * 2);
                colorGrading.tint.Interp(colorGrading.tint, 100, Time.fixedDeltaTime * 2);
            }
            else if (bioManager.GetCurrentEnergyValue() > 0 || isDead && colorGrading.temperature != 0)
            {
                colorGrading.temperature.Interp(colorGrading.temperature, 0, Time.fixedDeltaTime * 2);
                colorGrading.tint.Interp(colorGrading.tint, 0, Time.fixedDeltaTime * 2);
            }
            if (bioManager.GetCurrentHeartBeatValue() <= 0 && !isDead)
            {
                colorGrading.mixerRedOutRedIn.Interp(colorGrading.mixerRedOutRedIn.value, 200, Time.fixedDeltaTime * 2);

                vignette.intensity.Interp(vignette.intensity.value, 0.5f, Time.fixedDeltaTime * 2);

                if (heartDeathDelayCounter < heartDeathDelay)
                {
                    heartDeathDelayCounter += Time.deltaTime;
                }
                else if (heartDeathDelayCounter >= heartDeathDelay)
                {
                    heartDeathDelayCounter = 0;

                    StartCoroutine(Die());
                }
            }
            else if (bioManager.GetCurrentHeartBeatValue() > 0)
            {
                colorGrading.mixerRedOutRedIn.Interp(colorGrading.mixerRedOutRedIn.value, 100, Time.fixedDeltaTime * 2);

                vignette.intensity.Interp(vignette.intensity.value, 0.3f, Time.fixedDeltaTime * 2);
                heartDeathDelayCounter = 0;
            }
            else if (bioManager.GetCurrentHeartBeatValue() > 0 && colorGrading.mixerRedOutRedIn.value != 100)
            {
                colorGrading.mixerRedOutRedIn.value = 100;
                vignette.intensity.value /= 1.5f;
            }
        }

    }

    public void GetHurt(float dmg, bool shake = true, bool deathSound = false, bool tempInvincibility = false)
    {
        if (!isInvincibile)
        {
            if (shake)
            {
                hurtImpulse.GenerateImpulse();
            }
            if (audioManager.GetSound("Hit").source.isPlaying == false)
            {
                audioManager.Play("Hit");
            }

            bioManager.ReduceBiomass(dmg);
            bioManager.ReduceHeartBeat(dmg / 2);
            if (bioManager.GetCurrentBiomassValue() <= 0 && !isDead)
            {
                StartCoroutine(Die());
            }
            animator.SetTrigger("Hurt");
        }
        if (tempInvincibility && !isInvincibile)
        {
            StartCoroutine(TempInvincibility());
        }
    }
    IEnumerator TempInvincibility()
    {
        isInvincibile = true;
        yield return new WaitForSeconds(0.5f);
        isInvincibile = false;
    }
    void MoveHorizontally()
    {

        if (!isJumping)
        {

            float xValue = Input.GetAxis("Horizontal");
            if (xValue != 0)
            {
                bioManager.ReduceEnergy(energyMoveReduction);
            }
            rb.velocity = new Vector2(xValue * currentSpeed, rb.velocity.y);

            if (!animator.GetBool("IsRunning"))
                animator.SetBool("IsRunning", true);
            if (xValue < 0 && sprite.flipX == false)
            {
                sprite.flipX = true;
            }
            else if (xValue > 0 && sprite.flipX == true)
            {
                sprite.flipX = false;
            }
        }

        if (rb.velocity.x == 0)
        {

            if (animator.GetBool("IsRunning"))
                animator.SetBool("IsRunning", false);
        }

    }

    void Jump()
    {
        JumpApply();
        JumpControl();
        QuickerFall();
    }

    void JumpApply()
    {
        if (Input.GetButtonUp("Jump") && isJumping && !isAirbone)
        {
            bioManager.ReduceEnergy(energyJumpReduction);
            animator.SetBool("IsJumping", true);
            arrow.SetActive(false);
            Vector3 jumpDir = Quaternion.AngleAxis(rotationAngle, Vector3.forward) * Vector3.right;
            rb.velocity = jumpDir * new Vector2(currentSpeed * airSpeedIncrease, currentJumpVelocity);
            isAirbone = true;

        }
    }
    void QuickerFall()
    {
        //Increase gravity to have faster fall
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics2D.gravity * (fallGravityMultiplier - 1) * Time.fixedDeltaTime;
    }
    void JumpControl()
    {
        if (Input.GetButton("Jump") && isJumping && !isAirbone)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (isJumping && !isAirbone)
        {

            float xValue = Input.GetAxis("Horizontal");
            rotationAngle = ArcTangent(jumpPoint.transform.position, transform.position);
            if (rotationAngle <= 85 && xValue < 0)
            {
                jumpPoint.transform.position = RotateByRadians(transform.position, jumpPoint.transform.position, -rotationSpeed * Time.fixedDeltaTime);
                arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationAngle - 90));
            }
            if (rotationAngle >= 25 && xValue > 0)
            {
                jumpPoint.transform.position = RotateByRadians(transform.position, jumpPoint.transform.position, rotationSpeed * Time.fixedDeltaTime);
                arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationAngle - 90));
            }
        }

        if (Input.GetButton("Jump") && isJumping && !isAirbone && currentJumpVelocity < maxJumpVelocity)
        {
            arrow.SetActive(true);
            float proportionalConstant = minY + (maxY - minY) * ((currentJumpVelocity - startJumpVelocity) / maxJumpVelocity);
            if (currentJumpVelocity + jumpForceIncrease != maxJumpVelocity)
            {
                arrow.transform.localScale = new Vector3(arrow.transform.localScale.x, proportionalConstant, 1);

                currentJumpVelocity += jumpForceIncrease;
            }
            else
            {
                currentJumpVelocity = maxJumpVelocity;
            }

        }
        if (rb.velocity.y == 0 && isAirbone || inLiquid && isAirbone)
        {
            isAirbone = false;
            isJumping = false;
            animator.SetBool("IsJumping", false);
            animator.SetTrigger("Land");
            currentJumpVelocity = startJumpVelocity;
            jumpPoint.transform.localPosition = initialPos;
            rb.velocity = Vector2.zero;
        }
        if (Input.GetButton("Jump") && !isJumping && !isAirbone && hasEnergy)
        {
            isJumping = true;
            inLiquid = false;
            rb.velocity = Vector2.zero;
        }
    }
    IEnumerator IncreaseScore()
    {
        Vector2 prevPos = transform.position;
        scoreText.gameObject.SetActive(true);
        while (true)
        {
            if (transform.position.x > prevPos.x)
            {
                prevPos = transform.position;
            }
            yield return new WaitForSeconds(0.25f);
            float xDifference = transform.position.x - prevPos.x;
            if (xDifference > 0)
            {
                score += xDifference * (scoreMultiplier + 10 * fireParticles.GetComponent<FireMove>().currentSpeed);
                Debug.Log(score);
                scoreText.text = ((int)score).ToString();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Liquid"))
        {
            inLiquid = true;
        }
    }

    float ArcTangent(Vector2 A, Vector2 B)
    {
        Vector2 difference = A - B;
        difference.Normalize();
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        return rotZ;
    }

    public Vector2 RotateByRadians(Vector2 Center, Vector2 A, float angle)
    {
        //Move calculation to 0,0
        Vector2 v = A - Center;

        //rotate x and y
        float x = v.x * Mathf.Cos(angle) + v.y * Mathf.Sin(angle);
        float y = v.y * Mathf.Cos(angle) - v.x * Mathf.Sin(angle);

        //move back to center
        Vector2 B = new Vector2(x, y) + Center;

        return B;
    }
    IEnumerator Die()
    {
        highestScore = score;
        PlayerPrefs.SetFloat("HighestScore", highestScore);
        isDead = true;
        rb.velocity = Vector2.zero;
        Time.timeScale /= 2;
        filterManager.DeathFilter();
        audioManager.Play("HeartBeat");
        foreach (Sound sound in audioManager.sounds)
        {
            if (sound.name != "Die" && sound.name != "HeartBeat")
            {
                sound.source.volume /= 2f;
                sound.source.pitch -= 0.2f;
            }
        }
        yield return new WaitForSeconds(0.2f);
        youDiedText.SetActive(true);

        yield return new WaitForSeconds(3);
        Time.timeScale *= 2;
        SceneManager.LoadScene(0);
    }
    void EarthQuake()
    {
        StartCoroutine(PlayMusic());
        startImpulse.GenerateImpulse();
        StartCoroutine(PlayWarningSounds());
    }

    IEnumerator PlayMusic()
    {
        audioManager.GetSound("MainMenu").source.Stop();
        audioManager.Play("BattleStart");
        AudioSource battleStartSource = audioManager.GetSound("BattleStart").source;
        yield return new WaitUntil(() => battleStartSource.isPlaying == false);
        audioManager.Play("BattleLoop");
    }

    IEnumerator PlayWarningSounds()
    {
        audioManager.Play("Explosion");

        Instantiate(fireParticles);
        yield return new WaitForSeconds(1f);
        audioManager.Play("WarningStart");
        AudioSource warningStartSource = audioManager.GetSound("WarningStart").source;
        yield return new WaitUntil(() => warningStartSource.isPlaying == false);
        audioManager.Play("WarningLoop");

    }
}
