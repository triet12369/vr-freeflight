using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ControllerState
{
    public struct State
    {
        public OVRInput.Controller controllerType;
        public bool One, Two; // 2 buttons next to joystick
        public float PrimaryIndexTrigger, PrimaryHandTrigger;
    }
    public State leftHand;
    public State rightHand;
    public ControllerState()
    {
        leftHand.controllerType = OVRInput.Controller.LTouch;
        rightHand.controllerType = OVRInput.Controller.RTouch;
        leftHand.One = false;
        rightHand.One = false;
        leftHand.Two = false;
        rightHand.Two = false;
        leftHand.PrimaryIndexTrigger = 0;
        rightHand.PrimaryIndexTrigger = 0;
        leftHand.PrimaryHandTrigger = 0;
        rightHand.PrimaryHandTrigger = 0;
    }
}
public class HandPoseController : MonoBehaviour
{

    // references
    public GameObject leftHandAnchor;
    public GameObject rightHandAnchor;
    public GameObject trackingSpace;

    // on state change
    public delegate void StateEnterHandler();

    // State change handlers
    public StateEnterHandler OnLPointingStart = (() => { });
    public StateEnterHandler OnRPointingStart = (() => { });
    public StateEnterHandler OnRPointingEnd = (() => { });
    public StateEnterHandler OnFiring = (() => { });


    private Vector3 forwardDirection;
    public enum POSES
    {
        IDLE,
        GRIPPING,
        OK,
        POINTING
    };

    public struct HandState {
        public POSES state;
        public Quaternion rotation;
        public Vector3 position;
        // state getters
        public bool GetIsIdle()
        {
            return state == POSES.IDLE;
        }
        public bool GetIsGripping()
        {
            return state == POSES.GRIPPING;
        }
        public bool GetIsOK()
        {
            return state == POSES.OK;
        }
    };

    public HandState leftHand;
    public HandState rightHand;
    private float TRIGGER_THRESHOLD = 0.5f;
    private ControllerState controllerState = new ControllerState();
    //
    // Start is called before the first frame update
    void Start()
    {
        leftHand.state = POSES.IDLE;
        rightHand.state = POSES.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateControllerStates();
        UpdateHandStates();
        forwardDirection = rightHandAnchor.transform.forward;
        //Vector3 localDirection = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
        //forwardDirection = localDirection;
    }

    // Handle state change callbacks
    private void HandleOnStateChange(POSES previousState, POSES nextState, OVRInput.Controller type)
    {
        if (previousState != nextState)
        {
            switch (nextState)
            {
                case POSES.POINTING:
                    if (type == OVRInput.Controller.RTouch) OnRPointingStart();
                    break;
                default:
                    break;
            }
            if (previousState == POSES.POINTING)
            {
                // firing
                if (controllerState.rightHand.PrimaryIndexTrigger > TRIGGER_THRESHOLD) OnFiring();
                if (type == OVRInput.Controller.RTouch) OnRPointingEnd();
            }
        }
    }

    // Hand pose detection

    private void UpdateHandStates()
    {
        UpdateHandState(ref leftHand, ref controllerState.leftHand, leftHandAnchor);
        UpdateHandState(ref rightHand, ref controllerState.rightHand, rightHandAnchor);
    }

    private void UpdateHandState(ref HandState handState, ref ControllerState.State controllerState, GameObject handVisual)
    {
        POSES previous = handState.state;
        POSES nextState = UpdateHandPoseState(ref controllerState);
        HandleOnStateChange(previous, nextState, controllerState.controllerType);
        // test
        handState.rotation = OVRInput.GetLocalControllerRotation(controllerState.controllerType);
        handState.position = trackingSpace.transform.TransformPoint(OVRInput.GetLocalControllerPosition(controllerState.controllerType));
        handState.state = nextState;
    }
    private void UpdateControllerStates()
    {
        UpdateControllerState(OVRInput.Controller.LTouch, ref controllerState.leftHand);
        UpdateControllerState(OVRInput.Controller.RTouch, ref controllerState.rightHand);
    }
    private void UpdateControllerState(OVRInput.Controller type, ref ControllerState.State state)
    {
        state.One = OVRInput.Get(OVRInput.Button.One, type);
        state.Two = OVRInput.Get(OVRInput.Button.Two, type);
        state.PrimaryIndexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, type);
        state.PrimaryHandTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, type);
    }
    private POSES UpdateHandPoseState(ref ControllerState.State state)
    {
        bool isHandGripping = IsHandGripping(ref state);
        bool isHandOK = IsHandOK(ref state);
        bool isHandPointing = IsHandPointing(ref state);
        // return next state
        if (isHandGripping)
        {
            return POSES.GRIPPING;
        }
        else if (isHandOK)
        {
            return POSES.OK;
        }
        else if (isHandPointing)
        {
            return POSES.POINTING;
        } else
        {
            return POSES.IDLE;
        }
    }

    // Gripping
    private bool IsHandGripping(ref ControllerState.State state)
    {
        return ((state.One || state.Two) &&
                 state.PrimaryHandTrigger > TRIGGER_THRESHOLD &&
                 state.PrimaryIndexTrigger > TRIGGER_THRESHOLD);
    }

    // OK Pose
    private bool IsHandOK(ref ControllerState.State state)
    {
        return (state.PrimaryIndexTrigger > TRIGGER_THRESHOLD && (
            !state.One && !state.Two && !(state.PrimaryHandTrigger > TRIGGER_THRESHOLD))
            );
    }

    // Pointing Pose
    private bool IsHandPointing(ref ControllerState.State state)
    {
        return (!(state.PrimaryIndexTrigger > TRIGGER_THRESHOLD) && (
                  state.PrimaryHandTrigger > TRIGGER_THRESHOLD)
            );
    }
    public Vector3 GetForwardDirection()
    {
        return forwardDirection;
    }
}
