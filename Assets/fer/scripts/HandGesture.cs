using UnityEngine.Events;
using UnityEngine.XR.Hands.Gestures;

namespace UnityEngine.XR.Hands.Samples.GestureSample
{
    /// <summary>
    /// A gesture that detects when a hand is held in a static shape and orientation for a minimum amount of time.
    /// </summary>
    public class HandGesture : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The hand tracking events component to subscribe to for joint updates.")]
        XRHandTrackingEvents m_HandTrackingEvents;

        [SerializeField]
        [Tooltip("The hand shape or pose that must be detected for the gesture.")]
        ScriptableObject m_HandShapeOrPose;

        [SerializeField]
        [Tooltip("The target Transform used for relative orientation checks in the pose.")]
        Transform m_TargetTransform;

        [SerializeField]
        [Tooltip("Event fired when the gesture is performed.")]
        UnityEvent m_GesturePerformed;

        [SerializeField]
        [Tooltip("Event fired when the gesture is ended.")]
        UnityEvent m_GestureEnded;

        [SerializeField]
        [Tooltip("Minimum time the hand must be held in the required shape for the gesture to perform.")]
        float m_MinimumHoldTime = 0.2f;

        [SerializeField]
        [Tooltip("Interval at which gesture detection is checked.")]
        float m_GestureDetectionInterval = 0.1f;

        XRHandShape m_HandShape;
        XRHandPose m_HandPose;
        bool m_WasDetected;
        bool m_PerformedTriggered;
        float m_TimeOfLastConditionCheck;
        float m_HoldStartTime;

        public XRHandTrackingEvents handTrackingEvents
        {
            get => m_HandTrackingEvents;
            set => m_HandTrackingEvents = value;
        }

        public ScriptableObject handShapeOrPose
        {
            get => m_HandShapeOrPose;
            set => m_HandShapeOrPose = value;
        }

        public Transform targetTransform
        {
            get => m_TargetTransform;
            set => m_TargetTransform = value;
        }

        public UnityEvent gesturePerformed
        {
            get => m_GesturePerformed;
            set => m_GesturePerformed = value;
        }

        public UnityEvent gestureEnded
        {
            get => m_GestureEnded;
            set => m_GestureEnded = value;
        }

        public float minimumHoldTime
        {
            get => m_MinimumHoldTime;
            set => m_MinimumHoldTime = value;
        }

        public float gestureDetectionInterval
        {
            get => m_GestureDetectionInterval;
            set => m_GestureDetectionInterval = value;
        }

        void OnEnable()
        {
            if (m_HandTrackingEvents != null)
                m_HandTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);

            m_HandShape = m_HandShapeOrPose as XRHandShape;
            m_HandPose = m_HandShapeOrPose as XRHandPose;
            if (m_HandPose != null && m_HandPose.relativeOrientation != null)
                m_HandPose.relativeOrientation.targetTransform = m_TargetTransform;
        }

        void OnDisable()
        {
            if (m_HandTrackingEvents != null)
                m_HandTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);
        }

        void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
        {
            if (!isActiveAndEnabled || Time.timeSinceLevelLoad < m_TimeOfLastConditionCheck + m_GestureDetectionInterval)
                return;

            bool detected = false;
            if (m_HandTrackingEvents.handIsTracked)
            {
                if (m_HandShape != null && m_HandShape.CheckConditions(eventArgs))
                    detected = true;
                else if (m_HandPose != null && m_HandPose.CheckConditions(eventArgs))
                    detected = true;
            }

            if (!m_WasDetected && detected)
            {
                m_HoldStartTime = Time.timeSinceLevelLoad;
            }
            else if (m_WasDetected && !detected)
            {
                m_PerformedTriggered = false;
                m_GestureEnded?.Invoke();
            }

            m_WasDetected = detected;

            if (detected && !m_PerformedTriggered)
            {
                float holdTimer = Time.timeSinceLevelLoad - m_HoldStartTime;
                if (holdTimer > m_MinimumHoldTime)
                {
                    m_GesturePerformed?.Invoke();
                    m_PerformedTriggered = true;
                }
            }

            m_TimeOfLastConditionCheck = Time.timeSinceLevelLoad;
        }
    }
}
