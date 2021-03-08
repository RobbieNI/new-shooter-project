using UnityEngine;
using Opsive.Shared.Events;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character;

public class ModifyFOV : MonoBehaviour
{
    private UltimateCharacterLocomotion _locoScript;

    /// <summary>
    /// Initialize the default values.
    /// </summary>
    public void Awake()
    {
        _locoScript = GetComponent<UltimateCharacterLocomotion>();
        EventHandler.RegisterEvent<Ability, bool>(gameObject, "OnCharacterAbilityActive", OnAbilityActive);
    }

    /// <summary>
    /// The specified ability has started or stopped.
    /// </summary>
    /// <param name="ability">The ability that has been started or stopped.</param>
    /// <param name="activated">Was the ability activated?</param>
    private void OnAbilityActive(Ability ability, bool activated)
    {
        var runAbility = _locoScript.GetAbility<SpeedChange>();

        if (ability == runAbility)
        {
            if (activated == true)
            {
                Debug.Log(ability + " poop activated: " + activated);
            }
            else
            {
                Debug.Log(ability + " fart activated: " + activated);
            }
            
        }
    }

    /// <summary>
    /// The GameObject has been destroyed.
    /// </summary>
    public void OnDestroy()
    {
        EventHandler.UnregisterEvent<Ability, bool>(gameObject, "OnCharacterAbilityActive", OnAbilityActive);
    }
}