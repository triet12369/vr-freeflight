using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI status;
    // on state change
    public delegate void StateEnterHandler();

    // State change handlers
    public StateEnterHandler OnFlyingForwardStart = (() => { });

    // hand poses
    private HandPoseController HandPose;


    public enum PLAYER_STATE
    {
        IDLE,
        HOVERING,
        FLYING_FORWARD
    };

    public PLAYER_STATE CurrentState = PLAYER_STATE.IDLE;
    public string GetCurrentStateString()
    {
        switch (CurrentState)
        {
            case PLAYER_STATE.IDLE:
                return "IDLE";
            case PLAYER_STATE.HOVERING:
                return "HOVERING";
            case PLAYER_STATE.FLYING_FORWARD:
                return "FLYING_FORWARD";
            default:
                return "null";
        }
    }
    // State setters
    public void SetIsIdle()
    {
        CurrentState = PLAYER_STATE.IDLE;
    }
    public void SetIsHovering()
    {
        CurrentState = PLAYER_STATE.HOVERING;
    }
    public void SetIsFlyingForward()
    {
        CurrentState = PLAYER_STATE.FLYING_FORWARD;
    }
    // Start is called before the first frame update
    void Start()
    {
        HandPose = GetComponent<HandPoseController>();
    }

    // Update is called once per frame
    void Update()
    {
        PLAYER_STATE previous = CurrentState;
        PLAYER_STATE nextState = MapVRInputToPlayerState();
        // Trigger onChange events if possible
        HandleOnStateChange(previous, nextState);
        SetState(nextState);
        status.text = GetCurrentStateString();
    }

    private PLAYER_STATE MapVRInputToPlayerState()
    {
        bool isLeftHandGripping = HandPose.leftHand.GetIsGripping();
        bool isRightHandGripping = HandPose.rightHand.GetIsGripping();
        bool isLeftHandOK = HandPose.leftHand.GetIsOK();
        if (isRightHandGripping || (isLeftHandGripping && isRightHandGripping))
        {
            return PLAYER_STATE.FLYING_FORWARD;
        } else if (isLeftHandOK)
        {
            return PLAYER_STATE.HOVERING;
        }
        return PLAYER_STATE.IDLE;
    }

    private void HandleOnStateChange(PLAYER_STATE previousState, PLAYER_STATE nextState)
    {
        if (previousState != nextState)
        {
            switch(nextState)
            {
                case PLAYER_STATE.FLYING_FORWARD:
                    OnFlyingForwardStart();
                    break;
                default:
                    return;
            }
        }
    }

    private void SetState(PLAYER_STATE state)
    {
        CurrentState = state;
    }
}
