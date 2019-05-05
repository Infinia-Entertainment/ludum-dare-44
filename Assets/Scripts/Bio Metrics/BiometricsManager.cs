using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiometricsManager : MonoBehaviour
{
    [SerializeField]
    private Stat HeartBeat;
    [SerializeField]
    private Stat Oxygen;
    [SerializeField]
    private Stat Energy;
    [SerializeField]
    private Stat Biomass;
    public AudioManager audioManager;
    [HideInInspector]
    public bool hasEscaped = false;
    [HideInInspector]
    public bool initialized = false;

    public OrganFuncs organs;
    #region Organ Addition
    [Header("button variables")]
    [SerializeField] private float heartBeatAddition = 20f;
    [SerializeField] private float oxygenAddition = 15f;
    [SerializeField] private float heartBeatOxygenCost = 2.5f;
    #endregion

    #region Rates
    [Header("Generation rates")]
    [SerializeField] private float energyByOxygenRegenRate = 0.1f;
    [SerializeField] private float energyDepletionRate = 0.1f;
    [SerializeField] private float heartBeatDepletionRate = 0.1f;
    [SerializeField] private float biomasRegenerationRate = 3;

    #endregion

    #region Amounts
    [Header("Adding amount")]
    [SerializeField] private float energyGenerationAmount = 1;
    [Header("Depletion amount")]
    [SerializeField] private float oxygenEnergyDepletionAmount = 1.75f;
    [SerializeField] private float energyDepletionAmount = 0.175f;
    [SerializeField] private float heartBeatDepletionAmount = 0.25f;
    [SerializeField] private float biomasRegenerationAmount = 1;
    #endregion


    #region Spam Control
    [Header("Times pressed to activate Spam Delay")]
    [SerializeField] private int heartBeatSpamLimit;
    [SerializeField] private int oxygenSpamLimit;
    [SerializeField] private int energySpamLimit;
    [SerializeField] private int biomassSpamLimit;

    private int heartBeatSpamPressed = 0;
    private int oxygenSpamPressed = 0;
    private int energySpamPressed = 0;
    private int biomassSpamPressed = 0;

    private bool ishearbeatSpamDelayActive;
    private bool isOxygenSpamDelayActive;
    private bool isEnergySpamDelayActive;
    private bool isBiomassSpamDelayActive;

    [SerializeField] private float spamDelaytime;
    [SerializeField] private float spamPressedCooldowntime;

    #endregion

    [HideInInspector]
    public bool heartPressed = false;
    [HideInInspector]
    public bool lungsPressed = false;
    public float minHeartBeat = 10f;


    #region Properties
    public float GetCurrentBiomassValue()
    {
        return Biomass.CurrentVal;
    }
    public float GetCurrentEnergyValue()
    {
        return Energy.CurrentVal;
    }
    public float GetCurrentHeartBeatValue()
    {
        return HeartBeat.CurrentVal;
    }
    #endregion

    public enum VariableToChange
    {
        HeartBeat,
        Oxygen,
        Energy,
        Biomass
    }

    public enum TypeOfChange
    {
        Addition,
        Substraction,
        Multiply,
        Divide
    }



    private void Awake()
    {
        HeartBeat.Initialize();
        Oxygen.Initialize();
        Energy.Initialize();
        Biomass.Initialize();
        hasEscaped = false;
        initialized = false;

        //ButtonPressedCooldownContainer heartbeatCooldown = new ButtonPressedCooldownContainer(0, heartBeatSpamLimit);
        //ButtonPressedCooldownContainer oxygenCooldown = new ButtonPressedCooldownContainer(0, heartBeatSpamLimit);
        //ButtonPressedCooldownContainer energyCooldown = new ButtonPressedCooldownContainer(0, heartBeatSpamLimit);
        //ButtonPressedCooldownContainer biomassCooldown = new ButtonPressedCooldownContainer(0, heartBeatSpamLimit);


        lungsPressed = false;
    }


    private void FixedUpdate()
    {
        if (hasEscaped == true && initialized == false)
        {
            initialized = true;
            StartCoroutine(EnergyGeneration());
            StartCoroutine(EnergyDepletion());
            StartCoroutine(HeartBeatDepletion());
            StartCoroutine(BiomasRegeneration());
        }

        if (HeartBeat.CurrentVal <= 30f && !audioManager.GetSound("HeartBeat").source.isPlaying)
        {
            audioManager.Play("HeartBeat");
            DecreaseSounds();
        }
        else if (HeartBeat.CurrentVal > 30f && audioManager.GetSound("HeartBeat").source.isPlaying)
        {
            audioManager.GetSound("HeartBeat").source.Stop();
            IncreaseSounds();
        }

        if (Energy.CurrentVal <= 30f && !audioManager.GetSound("Breathing").source.isPlaying)
        {
            audioManager.Play("Breathing");
            DecreaseSounds();
        }
        else if (Energy.CurrentVal > 30f && audioManager.GetSound("Breathing").source.isPlaying)
        {
            audioManager.GetSound("Breathing").source.Stop();
            IncreaseSounds();
        }
    }
    void DecreaseSounds()
    {
        foreach (Sound sound in audioManager.sounds)
        {
            if (sound.name != "Breathing" && sound.name != "Die" && sound.name != "HeartBeat")
            {
                sound.source.volume /= 3.5f;
            }
        }
    }
    void IncreaseSounds()
    {
        foreach (Sound sound in audioManager.sounds)
        {
            if (sound.name != "Breathing" && sound.name != "Die" && sound.name != "HeartBeat")
            {
                sound.source.volume *= 3.5f;
            }
        }
    }


    private IEnumerator HeartBeatDepletion()
    {
        while (true)
        {

            HeartBeat.CurrentVal -= heartBeatDepletionAmount;

            yield return new WaitForSeconds(heartBeatDepletionRate);
        }
    }

    private IEnumerator EnergyDepletion()
    {
        while (true)
        {

            Energy.CurrentVal -= energyDepletionAmount;

            yield return new WaitForSeconds(energyDepletionRate);
        }
    }

    private IEnumerator EnergyGeneration()
    {
        while (true)
        {
            if (Oxygen.CurrentVal >= 1)
            {
                Oxygen.CurrentVal -= oxygenEnergyDepletionAmount;
                Energy.CurrentVal += energyGenerationAmount;
            }
            yield return new WaitForSeconds(energyByOxygenRegenRate);
        }
    }

    private IEnumerator BiomasRegeneration()
    {
        while (true)
        {
            if (Energy.CurrentVal >= 1)
            {
                Biomass.CurrentVal += biomasRegenerationAmount;
            }
            yield return new WaitForSeconds(biomasRegenerationRate);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasEscaped)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                heartPressed = true;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                lungsPressed = true;
            }

            if (HeartBeat.CurrentVal <= minHeartBeat)
                WarnPlayer();
            if (HeartBeat.CurrentVal <= 0)
                KillPlayer();
            if (heartPressed)
            {
                ChangeBiometric(VariableToChange.HeartBeat, TypeOfChange.Addition, heartBeatAddition);
                organs.HeartPressed();
                heartPressed = false;
            }
            if (lungsPressed && HeartBeat.CurrentVal > 0)
            {
                ChangeBiometric(VariableToChange.HeartBeat, TypeOfChange.Substraction, 1, true);
                ChangeBiometric(VariableToChange.Oxygen, TypeOfChange.Addition, oxygenAddition);
                organs.LungsPressed();
                lungsPressed = false;
            }
        }
    }

    #region Change Biometrics

    /// <summary>
    /// Changes a bio stat the amount a way that is specified, also control spam for the variable change
    /// </summary>
    /// <param name="variable">The variable that needs to be changed)</param>
    /// <param name="typeOfChange"> How it will be changed</param>
    /// <param name="changeVar">Variable used for the change (note: if multiply is used, this is what you multiply by) </param>
    /// <param name="ignoreSpamControl">A bool to ignore spam control if needed</param>
    public void ChangeBiometric(VariableToChange variable, TypeOfChange typeOfChange, float changeVar, bool ignoreSpamControl = false)
    {


        //ButtonPressedCooldownContainer buttonSpam = null;
        Stat statToChange = AssignStat();
        if (statToChange != null)
        {
            ChangeStat();
        }
        #region Local functions


        //Assing the Stat that will be changed to the local Stat variable
        Stat AssignStat()
        {
            switch (variable)
            {
                case VariableToChange.HeartBeat:
                    if (!ishearbeatSpamDelayActive || !ignoreSpamControl)
                    {
                        //if (heartbeatCooldown.pressResetCor != null)
                        //{
                        //    StopCoroutine(heartbeatCooldown.pressResetCor);
                        //}
                        //heartbeatCooldown.pressResetCor =  StartCoroutine(heartbeatCooldown.PressReset(spamPressedCooldowntime,heartBeatSpamLimit));
                        //if (heartbeatCooldown.hasReachedLimit == true)
                        //{
                        //    ishearbeatSpamDelayActive = true;
                        //}
                        //return HeartBeat;
                    }
                    else StartCoroutine(SpamCooldown(spamDelaytime, variable));
                    break;
                case VariableToChange.Oxygen:
                    if (!isOxygenSpamDelayActive || !ignoreSpamControl)
                    {
                        //if (heartbeatCooldown.pressResetCor != null)
                        //{
                        //    StopCoroutine(heartbeatCooldown.pressResetCor);
                        //}
                        //StopCoroutine(oxygenCooldown.pressResetCor);
                        //oxygenCooldown.pressResetCor = StartCoroutine(oxygenCooldown.PressReset(spamPressedCooldowntime,oxygenSpamLimit));
                        //if (oxygenCooldown.hasReachedLimit == true)
                        //{
                        //    isOxygenSpamDelayActive = true;
                        //}
                        //return Oxygen;
                    }
                    else StartCoroutine(SpamCooldown(spamDelaytime, variable));
                    break;
                case VariableToChange.Energy:
                    if (!isEnergySpamDelayActive || !ignoreSpamControl)
                    {
                        //Debug.Log(heartbeatCooldown);
                        //if (heartbeatCooldown.pressResetCor != null)
                        //{
                        //    StopCoroutine(heartbeatCooldown.pressResetCor);
                        //}
                        //StopCoroutine(energyCooldown.pressResetCor);
                        //energyCooldown.pressResetCor = StartCoroutine(energyCooldown.PressReset(spamPressedCooldowntime,energySpamLimit));
                        //if (energyCooldown.hasReachedLimit == true)
                        //{
                        //    isEnergySpamDelayActive = true;
                        //}
                        //return Energy;
                    }
                    else StartCoroutine(SpamCooldown(spamDelaytime, variable));
                    break;
                case VariableToChange.Biomass:
                    if (!isBiomassSpamDelayActive || !ignoreSpamControl)
                    {
                        //if (heartbeatCooldown.pressResetCor != null)
                        //{
                        //    StopCoroutine(heartbeatCooldown.pressResetCor);
                        //}
                        //StopCoroutine(biomassCooldown.pressResetCor);
                        //biomassCooldown.pressResetCor = StartCoroutine(biomassCooldown.PressReset(spamPressedCooldowntime, biomassSpamLimit));
                        //if (biomassCooldown.hasReachedLimit == true)
                        //{
                        //    isBiomassSpamDelayActive = true;
                        //}
                        return Biomass;
                    }
                    else StartCoroutine(SpamCooldown(spamDelaytime, variable));
                    break;
                default: return null;

            }

            return null;

        }

        //Applies the changes to the local Stat variable
        void ChangeStat()
        {
            switch (typeOfChange)
            {
                case TypeOfChange.Addition:
                    statToChange.CurrentVal += changeVar;
                    break;
                case TypeOfChange.Substraction:
                    statToChange.CurrentVal -= changeVar;
                    break;
                case TypeOfChange.Multiply:
                    statToChange.CurrentVal *= statToChange.MaxVal;
                    break;
                case TypeOfChange.Divide:
                    statToChange.CurrentVal /= statToChange.MinVal;
                    break;
                default:
                    break;
            }
        }

        //Starts a delay when spamming
        IEnumerator SpamCooldown(float spamDelay, VariableToChange variableToCheck)
        {
            Debug.Log("SpamDelayStarted");
            switch (variableToCheck)
            {
                case VariableToChange.HeartBeat:

                    ishearbeatSpamDelayActive = true;
                    yield return new WaitForSeconds(spamDelaytime);
                    ishearbeatSpamDelayActive = false;

                    break;
                case VariableToChange.Oxygen:

                    isOxygenSpamDelayActive = true;
                    yield return new WaitForSeconds(spamDelaytime);
                    isOxygenSpamDelayActive = false;


                    break;
                case VariableToChange.Energy:

                    isEnergySpamDelayActive = true;
                    yield return new WaitForSeconds(spamDelaytime);
                    isEnergySpamDelayActive = false;


                    break;
                case VariableToChange.Biomass:

                    isBiomassSpamDelayActive = true;
                    yield return new WaitForSeconds(spamDelaytime);
                    isBiomassSpamDelayActive = false;


                    break;
                default: yield return null;
                    break;


            }

        }

        #endregion
    }

    #endregion
}

/// <summary>
/// Contains a coroutine to acces the times pressed
/// </summary>
public class ButtonPressedCooldownContainer : MonoBehaviour
{
    public int timesPressed;
    public bool hasReachedLimit;
    public Coroutine pressResetCor;

    public ButtonPressedCooldownContainer(float cooldownTime, int buttonCooldownSpamLimit)
    {
        StartCoroutine(PressReset(cooldownTime, buttonCooldownSpamLimit));
    }


    public IEnumerator PressReset(float cooldownTime,int buttonCooldownSpamLimit)
    {
        timesPressed++;

        if (timesPressed == buttonCooldownSpamLimit)
        {
            hasReachedLimit = true;
        }
        else
        {
            hasReachedLimit = false;
            yield return new WaitForSeconds(cooldownTime);
            timesPressed = 0;
        }
        
    }
}