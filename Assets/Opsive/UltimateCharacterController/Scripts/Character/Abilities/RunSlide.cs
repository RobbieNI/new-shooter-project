using Opsive.Shared.Events;
using Opsive.UltimateCharacterController.Character.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSlide : Ability
{
    private RaycastHit m_RaycastHit;

    [Tooltip("Specifies the cast distance to ensure there is enough space for the character to roll.")]
    [SerializeField] protected float m_StartCastDistance = 2;

    [Tooltip("Specifies the speed of the slide.")]
    [SerializeField] protected float m_slideSpeed = 50;

    [Tooltip("Do we only want this ability to be active when the Speed Change Ability is active? If false then use a different key binding than crouch.")]
    [SerializeField] protected bool m_OnlySlideWhenSprinting;

    public float StartCastDistance { get { return m_StartCastDistance; } set { m_StartCastDistance = value; } }
    public float SlideSpeed { get { return m_slideSpeed; } set { m_slideSpeed = value; } }
    public bool OnlySlideWhenSprinting { get { return m_OnlySlideWhenSprinting; } set { m_OnlySlideWhenSprinting = value; } }

    public override void Awake()
    {
        base.Awake();
        EventHandler.RegisterEvent(m_GameObject, "OnAnimatorSlideComplete", OnComplete);
        EventHandler.RegisterEvent(m_GameObject, "OnAnimatorSlide", AddSlideForce);
    }

    public override bool CanStartAbility()
    {
        // An attribute may prevent the ability from starting.
        if (!base.CanStartAbility())
        {
            return false;
        }

        // The character has to be on the ground.
        if (!m_CharacterLocomotion.Grounded)
        {
            return false;
        }

        if (!m_CharacterLocomotion.Moving)
        {
            return false;
        }

        // The character can't roll on steep slopes.
        var slope = Vector3.Angle(m_CharacterLocomotion.Up, m_CharacterLocomotion.GroundRaycastHit.normal);
        if (slope > m_CharacterLocomotion.SlopeLimit)
        {
            return false;
        }

        // Don't slide if an object would get in the way.
        if (m_StartCastDistance > 0)
        {
            var direction = new Vector3(m_CharacterLocomotion.InputVector.x, 0, m_CharacterLocomotion.InputVector.y);
            // If the direction is zero then the character will roll foward.
            if (direction.sqrMagnitude == 0)
            {
                direction.z = 1;
            }
            if (m_CharacterLocomotion.SingleCast(m_Transform.TransformDirection(direction).normalized * m_StartCastDistance, Vector3.zero, m_CharacterLayerManager.SolidObjectLayers, ref m_RaycastHit))
            {
                return false;
            }
        }

        // Do we only want to start this ability when the speed change ability is active?
        if (m_OnlySlideWhenSprinting)
        {
            var speedChangeAbility = m_CharacterLocomotion.GetAbility<SpeedChange>();
            if (speedChangeAbility.IsActive)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }

    }

    protected override void AbilityStarted()
    {
        base.AbilityStarted();
        m_CharacterLocomotion.AddForce(m_CharacterLocomotion.transform.forward * (m_slideSpeed / 2));
    }

    private void AddSlideForce()
    {
        m_CharacterLocomotion.AddForce(m_CharacterLocomotion.transform.forward * m_slideSpeed);
    }

    private void OnComplete()
    {
        StopAbility();
        // Go into crouch when this ability is over, if we are still sprinting then the sprint will override the crouch.
        m_CharacterLocomotion.TryStartAbility(m_CharacterLocomotion.GetAbility<HeightChange>());
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        EventHandler.UnregisterEvent(m_GameObject, "OnAnimatorSlideComplete", OnComplete);
        EventHandler.UnregisterEvent(m_GameObject, "OnAnimatorSlide", AddSlideForce);
    }
}
