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
    public bool isEscaping = false;
    [HideInInspector]
    public bool initialized = false;
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

    [HideInInspector]
    public bool heartPressed = false;
    [HideInInspector]
    public bool lungsPressed = false;
    public float minHeartBeat = 10f;
    public float spamCount = 0;
    private void Awake()
    {
        HeartBeat.Initialize();
        Oxygen.Initialize();
        Energy.Initialize();
        Biomass.Initialize();
        isEscaping = false;
        initialized = false;
    }


    private void FixedUpdate()
    {
        if (isEscaping == true && initialized == false)
        {
            initialized = true;
            StartCoroutine(EnergyGeneration());
            StartCoroutine(EnergyDepletion());
            StartCoroutine(HeartBeatDepletion());
            StartCoroutine(BiomasRegeneration());


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
            if (sound.name != "Breathing"&& sound.name != "Die" && sound.name != "HeartBeat")
            {
                sound.source.volume *= 3.5f;
            }
        }
    }
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
        if (HeartBeat.CurrentVal <= minHeartBeat)
            WarnPlayer();
        if (HeartBeat.CurrentVal <= 0)
            KillPlayer();
        if (heartPressed)
        {
            HeartBeat.CurrentVal += 15;
            heartPressed = false;
        }
        if (lungsPressed && HeartBeat.CurrentVal > 0)
        {
            HeartBeat.CurrentVal -= 1;
            Oxygen.CurrentVal += 20;
            lungsPressed = false;
        }

    }

    #region Change Biometrics

    #region Reduce
    public void ReduceHeartBeat(float value)
    {
        HeartBeat.CurrentVal -= value;
    }

    public void ReduceOxygen(float value)
    {
        Oxygen.CurrentVal -= value;
    }

    public void ReduceEnergy(float value)
    {
        Energy.CurrentVal -= value;
    }

    public void ReduceBiomass(float value)
    {
        Biomass.CurrentVal -= value;
    }
    #endregion

    #region Add
    public void AddHeartBeat(float value)
    {
        HeartBeat.CurrentVal += value;
    }

    public void AddOxygen(float value)
    {
        Oxygen.CurrentVal += value;
    }

    public void AddEnergy(float value)
    {
        Energy.CurrentVal += value;
    }

    public void AddBiomass(float value)
    {
        Biomass.CurrentVal += value;
    }
    #endregion
    #endregion

    public void WarnPlayer()
    {

    }
    public void KillPlayer()
    {
    }


}
