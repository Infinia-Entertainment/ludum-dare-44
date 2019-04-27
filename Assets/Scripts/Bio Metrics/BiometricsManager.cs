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

    private void Awake()
    {
        HeartBeat.Initialize();
        Oxygen.Initialize();
        Energy.Initialize();
        Biomass.Initialize();
    }

    void Start()
    {
        StartCoroutine(EnergyGeneration());
        StartCoroutine(EnergyDepletion());
        StartCoroutine(HeartBeatDepletion());
        StartCoroutine(BiomasRegeneration());

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
        if (Input.GetKeyDown(KeyCode.H))
        {
            HeartBeat.CurrentVal += 10;
        }
        if (Input.GetKeyDown(KeyCode.O) && HeartBeat.CurrentVal > 0)
        {
            HeartBeat.CurrentVal -= 1;
            Oxygen.CurrentVal += 10;
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

    public void AddReduceOxygen(float value)
    {
        Oxygen.CurrentVal += value;
    }

    public void AddReduceEnergy(float value)
    {
        Energy.CurrentVal += value;
    }

    public void AddReduceBiomass(float value)
    {
        Biomass.CurrentVal += value;
    }
    #endregion
    #endregion

    public void KillPlayer()
    {
        Destroy(gameObject);
    }
}
