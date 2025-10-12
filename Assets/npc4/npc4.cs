using UnityEngine;

public class npc4Control : MonoBehaviour, INPCInteractable
{
    private Animator m_Anim;
    public bool HasTalked { get; set; } = false;

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
    }

    void Update()
    {
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
}
