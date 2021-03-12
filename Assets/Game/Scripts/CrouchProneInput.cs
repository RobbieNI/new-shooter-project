using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Input;
using System.Collections;
using UnityEngine;

public class CrouchProneInput : MonoBehaviour
{
    [Tooltip("A reference to the Ultimate Character Controller character.")]
    [SerializeField] private GameObject m_Character;

    [Header("Debug Values:")]
    [SerializeField] private bool   _cLongPress = false;
    [SerializeField] private float  _cPressTimeRequered = 0.5f;
    [SerializeField] private float  _cPressTimeReset = 0;
    [SerializeField] private float  _cPressTime = 0;
    [SerializeField] private bool   _continuousPressCheck = false;

    private PlayerInput m_PlayerInput;
    private UltimateCharacterLocomotion m_PlayerLoco;

    // Use this for initialization
    void Start()
    {
        m_PlayerInput = m_Character.GetComponent<PlayerInput>();
        m_PlayerLoco = m_Character.GetComponent<UltimateCharacterLocomotion>();
    }

    // Update is called once per frame
    void Update()
    {
        //Stance trigger timer
        if (m_PlayerInput.GetButton("Crouch") && _continuousPressCheck == false)
        {
            _cPressTimeReset += Time.deltaTime;
        }

        if (m_PlayerInput.GetButtonUp("Crouch") || _continuousPressCheck == true)
        {
            _cPressTimeReset = 0;
        }

        if (m_PlayerInput.GetButton("Crouch"))
        {
            _cPressTime += Time.deltaTime;
        }

        if (_cPressTimeReset >= _cPressTimeRequered && _continuousPressCheck == false)
        {
            _continuousPressCheck = true;
            _cLongPress = true;
        }
        else
        {
            _cLongPress = false;
        }

        if (m_PlayerInput.GetButtonUp("Crouch"))
        {
            _continuousPressCheck = false;
        }

        // Stances
        
        // Prone
        if (_cLongPress)
        {
            m_PlayerLoco.GetAbility<Prone>().IsInProne = !m_PlayerLoco.GetAbility<Prone>().IsInProne;
            m_PlayerLoco.GetAbility<HeightChange>().IsCrouched = false;

            Prone(true);
            Crouch(false);
        }

        // Crouch
        if (m_PlayerInput.GetButtonUp("Crouch") && _cPressTime < _cPressTimeRequered)
        {
            m_PlayerLoco.GetAbility<HeightChange>().IsCrouched = !m_PlayerLoco.GetAbility<HeightChange>().IsCrouched;
            m_PlayerLoco.GetAbility<Prone>().IsInProne = false;

            Crouch(true);
            Prone(false);
        }

        if (m_PlayerLoco.GetAbility<HeightChange>().IsCrouched == false && m_PlayerLoco.GetAbility<Prone>().IsInProne == false)
        {
            Crouch(false);
            Prone(false);
        }
        
    }

    // Update is called once per frame ends
    void LateUpdate()
    {
        if (m_PlayerInput.GetButtonUp("Crouch"))
        {
            _cPressTime = 0;
        }
    }

    void Crouch(bool State)
    {
        if (State)
        {
            m_PlayerLoco.TryStartAbility(m_PlayerLoco.GetAbility<HeightChange>());
        }
        else
        {
            m_PlayerLoco.TryStopAbility(m_PlayerLoco.GetAbility<HeightChange>());
        }
    }

    void Prone(bool State)
    {
        if (State)
        {
            m_PlayerLoco.TryStartAbility(m_PlayerLoco.GetAbility<Prone>());
        }
        else
        {
            m_PlayerLoco.TryStopAbility(m_PlayerLoco.GetAbility<Prone>());
        }
    }
}