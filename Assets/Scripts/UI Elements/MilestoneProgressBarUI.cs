using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine.UI;
using UnityEngine;

public class MilestoneProgressBarUI : MonoBehaviour
{
    [Header("Progress Bar")]
    [SerializeField] private Image fillBar;

    [Header("Milestone Thresholds (0–1 range)")]
    [SerializeField] private float[] thresholds = new float[3] { 0.33f, 0.66f, 1f };

    [Header("Milestone Feedbacks")]
    [SerializeField] private MMF_Player milestone1Feedback;
    [SerializeField] private MMF_Player milestone2Feedback;
    [SerializeField] private MMF_Player milestone3Feedback;

    private int lastMilestoneTriggered = 0;

    public void UpdateProgress(float normalizedProgress)
    {
        fillBar.fillAmount = normalizedProgress;

        // Trigger milestone feedbacks
        if (lastMilestoneTriggered < 3 && normalizedProgress >= thresholds[lastMilestoneTriggered])
        {
            lastMilestoneTriggered++;
            PlayMilestoneFeedback(lastMilestoneTriggered);
        }
    }

    public void ResetProgress()
    {
        fillBar.fillAmount = 0f;
        lastMilestoneTriggered = 0;
    }

    private void PlayMilestoneFeedback(int milestone)
    {
        switch (milestone)
        {
            case 1:
                milestone1Feedback?.PlayFeedbacks();
                break;
            case 2:
                milestone2Feedback?.PlayFeedbacks();
                break;
            case 3:
                milestone3Feedback?.PlayFeedbacks();
                break;
        }
    }
}
