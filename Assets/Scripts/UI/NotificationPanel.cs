using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationPanel : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] public TextMeshProUGUI message;

    public event Action notificationFinished;

    private void OnDestroy()
    {
        notificationFinished = null;
    }

    public IEnumerator ShowNotification(string designatedMessage)
    {
        gameObject.SetActive(true);
        message.text = designatedMessage;
        animator.PlayInFixedTime("Appear");

        // wait for animation finished
        float normalizedTime = 0f;
        while (normalizedTime < 1f)
        {
            yield return new WaitForEndOfFrame();
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            normalizedTime = stateInfo.normalizedTime;
        }

        animator.PlayInFixedTime("Disappear");
        normalizedTime = 0f;
        // wait for animation finished
        while (normalizedTime < 1f)
        {
            yield return new WaitForEndOfFrame();
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            normalizedTime = stateInfo.normalizedTime;
        }

        notificationFinished?.Invoke();
        gameObject.SetActive(false);
    }

}
