using UnityEngine;
using Opsive.Shared.Events;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character;
using MalbersAnimations.Scriptables;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using System.Collections;

public class ModifyFOV : MonoBehaviour
{
    [SerializeField] private FloatReference _FOV;
    [SerializeField] private float _changeDuration;
    [SerializeField] private float _fovToAddToSprint;

    private float _cachedFOV;
    private float _sprintFOV;
    private WaitForSeconds _fovResetTime;
    private UltimateCharacterLocomotion _locoScript;

    public void Awake()
    {
        _cachedFOV = _FOV.Value;
        _locoScript = GetComponent<UltimateCharacterLocomotion>();
        _fovResetTime = new WaitForSeconds(_changeDuration + .5f);

        EventHandler.RegisterEvent<Ability, bool>(gameObject, "OnCharacterAbilityActive", OnAbilityActive);
    }

    private void OnAbilityActive(Ability ability, bool activated)
    {
        var runAbility = _locoScript.GetAbility<SpeedChange>();

        if (ability == runAbility)
        {
            if (activated == true)
            {
                SetSprintFOV();
            }
            else
            {
                ResetFOV();
            }   
        }
    }

    private void SetSprintFOV()
    {
        _sprintFOV = _FOV.Value + _fovToAddToSprint;
        LeanTween.value(gameObject, _FOV.Value, _sprintFOV, _changeDuration).setOnUpdate((float val) =>
        {
            _FOV.Value = val;
        });
    }

    private void ResetFOV()
    {
        LeanTween.value(gameObject, _FOV.Value, _cachedFOV, _changeDuration).setOnUpdate((float val) =>
        {
            _FOV.Value = val;
        });

        // If for whatever the FOV didn't reset, force it to. 
        StartCoroutine(ForceFOVReset());
    }

    IEnumerator ForceFOVReset()
    {
        yield return _fovResetTime;
        _FOV.Value = _cachedFOV;
    }

    public void OnDestroy()
    {
        EventHandler.UnregisterEvent<Ability, bool>(gameObject, "OnCharacterAbilityActive", OnAbilityActive);
    }
}