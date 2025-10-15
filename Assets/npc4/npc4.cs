using UnityEngine;
using UnityEngine.UI;

public class npc4Control : MonoBehaviour, INPCInteractable
{
    private Animator m_Anim;
    public bool HasTalked { get; set; } = false;
    [SerializeField]
    public Button dialogue;
    [SerializeField]
    public GameObject taskWarning;

    // 对话状态
    private bool isTalking = false;

    // 对话时面向的主角
    private Transform talkingTarget = null;

    // NPC 转向速度
    public float faceTurnSpeed = 3f;

    void Start()
    {
        m_Anim = GetComponent<Animator>();
        PlayDancing();

        // 确保按钮初始状态正确
        if (dialogue != null)
        {
            dialogue.gameObject.SetActive(false);
        }

        // 确保任务警告初始状态正确
        if (taskWarning != null)
        {
            taskWarning.SetActive(false);
        }
    }

    void Update()
    {
        // 根据对话状态显示/隐藏任务警告
        if (taskWarning != null)
        {
            if (!HasTalked)
            {
                taskWarning.SetActive(true);
            }
            else
            {
                taskWarning.SetActive(false);
            }
        }

        // 如果在对话状态，持续面向主角
        if (isTalking && talkingTarget != null)
        {
            RotateTowardsTarget();
        }
    }

    void PlayDancing()
    {
        m_Anim.SetBool("stretch", true);
        m_Anim.SetBool("thank", false);
        isTalking = false;
        talkingTarget = null;
    }

    public void EnterTalking(Transform target)
    {
        if (HasTalked) return;
        HasTalked = true;
        m_Anim.SetBool("stretch", false);
        m_Anim.SetBool("thank", true);
        isTalking = true;

        talkingTarget = target; // 保存主角 Transform
    }

    public void ExitTalking()
    {
        PlayDancing();
    }

    private void RotateTowardsTarget()
    {
        if (talkingTarget == null) return;

        Vector3 targetDir = talkingTarget.position - transform.position;
        targetDir.y = 0f; // 忽略高度差
        if (targetDir == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * faceTurnSpeed);
    }

    //显示对话按钮
    public void ShowDialogueButton()
    {
        if (dialogue != null)
        {
            dialogue.gameObject.SetActive(true);
        }
    }

    // 隐藏对话按钮
    public void HideDialogueButton()
    {
        if (dialogue != null)
        {
            dialogue.gameObject.SetActive(false);
        }
    }
}