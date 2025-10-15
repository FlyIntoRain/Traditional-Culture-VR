using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class QinBronzeTripodMancontrol : MonoBehaviour
{
    private Animator m_Anim;

    // 当前动画状态
    private bool isWalking = false;
    private bool isRunning = false;
    private bool isBackwards = false;
    private bool isLeftTurning = false;
    private bool isRightTurning = false;
    private bool isSitting = false;
    private bool isStandingUp = false;
    private bool isAruging = false;

    private float baseY;

    // NPC 引用
    private MonoBehaviour currentNPC = null;
    private INPCInteractable[] allNPCs;

    // 自动转向速度
    public float faceTurnSpeed = 3f;

    // 新增：追踪是否靠近NPC但未进入对话
    private bool isNearNPC = false;
    private bool enterKeyPressedThisFrame = false;

    void Start()
    {
        m_Anim = GetComponent<Animator>();
        baseY = transform.position.y;
        allNPCs = FindObjectsOfType<MonoBehaviour>().OfType<INPCInteractable>().ToArray();
    }

    void Update()
    {
        // 重置每帧的Enter键状态
        enterKeyPressedThisFrame = false;

        // 检测Enter键输入
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            enterKeyPressedThisFrame = true;
        }

        // 若在与 NPC 对话状态中，则禁止其他控制，并持续朝向 NPC
        if (isAruging)
        {
            if (currentNPC != null)
                RotateTowardsNPC();

            // 👉 任意方向键 / Shift / Ctrl 离开对话
            if (Input.anyKeyDown)
            {
                ExitTalking();
                return;
            }

            return;
        }

  
        if (isNearNPC && currentNPC != null && !isAruging)
        { 
            // 检测Enter键进入对话
            if (enterKeyPressedThisFrame)
            {
                EnterTalking();
                return;
            }
        }

        bool anyAction = false;

        HandleUpArrowInput(ref anyAction);
        HandleDownArrowInput(ref anyAction);

        if (!anyAction && Input.GetKey(KeyCode.LeftArrow))
        {
            LeftTurn();
            anyAction = true;
        }

        if (!anyAction && Input.GetKey(KeyCode.RightArrow))
        {
            RightTurn();
            anyAction = true;
        }

        if (!anyAction && !isSitting && !isStandingUp)
        {
            SetIdleState();
        }
    }

    void LateUpdate()
    {
        // 锁定Y高度
        Vector3 pos = transform.position;
        pos.y = baseY;
        transform.position = pos;
    }

    // ================= 上箭头逻辑 ==================
    private void HandleUpArrowInput(ref bool anyAction)
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (isSitting)
            {
                StandUp();
                anyAction = true;
                return;
            }

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                Run();
            else
                Walk();

            anyAction = true;
        }

        if (Input.GetKeyUp(KeyCode.UpArrow) && !isStandingUp && !isSitting)
        {
            SetIdleState();
        }
    }

    // ================= 下箭头逻辑 ==================
    private void HandleDownArrowInput(ref bool anyAction)
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                Sit();
            else if (!isSitting)
                Backwards();

            anyAction = true;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (!isSitting)
                SetIdleState();
        }
    }

    // ================= 动画控制函数 =================
    void Walk()
    {
        if (!isWalking)
        {
            ResetAllAnimBools();
            m_Anim.SetBool("walk", true);
            isWalking = true;
        }
    }

    void Run()
    {
        if (!isRunning)
        {
            ResetAllAnimBools();
            m_Anim.SetBool("run", true);
            isRunning = true;
        }
    }

    void Backwards()
    {
        if (!isBackwards)
        {
            ResetAllAnimBools();
            m_Anim.SetBool("backwards", true);
            isBackwards = true;
        }
    }

    void LeftTurn()
    {
        if (!isLeftTurning)
        {
            ResetAllAnimBools();
            m_Anim.SetBool("left_turn", true);
            isLeftTurning = true;
        }
    }

    void RightTurn()
    {
        if (!isRightTurning)
        {
            ResetAllAnimBools();
            m_Anim.SetBool("right_turn", true);
            isRightTurning = true;
        }
    }

    void Sit()
    {
        if (!isSitting)
        {
            ResetAllAnimBools();
            m_Anim.SetBool("sit", true);
            isSitting = true;
        }
    }

    void StandUp()
    {
        if (isSitting && !isStandingUp)
        {
            ResetAllAnimBools();
            m_Anim.SetBool("standup", true);
            isStandingUp = true;
            isSitting = false;
            StartCoroutine(WaitForStandUpToFinish());
        }
    }

    IEnumerator WaitForStandUpToFinish()
    {
        yield return new WaitForSeconds(1.2f);
        isStandingUp = false;
        SetIdleState();
    }

    void SetIdleState()
    {
        if (isWalking || isRunning || isBackwards || isLeftTurning || isRightTurning || isStandingUp)
        {
            ResetAllAnimBools();
            m_Anim.SetBool("idle", true);

            isWalking = false;
            isRunning = false;
            isBackwards = false;
            isLeftTurning = false;
            isRightTurning = false;
            isStandingUp = false;
        }
    }

    private void ResetAllAnimBools()
    {
        m_Anim.SetBool("walk", false);
        m_Anim.SetBool("run", false);
        m_Anim.SetBool("backwards", false);
        m_Anim.SetBool("left_turn", false);
        m_Anim.SetBool("right_turn", false);
        m_Anim.SetBool("sit", false);
        m_Anim.SetBool("idle", false);
        m_Anim.SetBool("standup", false);
        m_Anim.SetBool("arguing", false);
    }

    // ======= 与NPC交互逻辑 =======
    public float talkRange = 10f;

    void FixedUpdate()
    {
        HandleNPCInteraction();
    }

    private void HandleNPCInteraction()
    {
        bool foundNPC = false;
        MonoBehaviour previousNPC = currentNPC;

        foreach (var npc in allNPCs)
        {
            if (npc.HasTalked) continue;
            float dist = Vector3.Distance(transform.position, ((MonoBehaviour)npc).transform.position);
            if (dist <= talkRange && !isAruging)
            {
                // 如果切换到新的NPC，隐藏上一个NPC的按钮
                if (currentNPC != (MonoBehaviour)npc && previousNPC != null)
                {
                    HideNPCButton(previousNPC); // 使用统一的方法隐藏上一个NPC的按钮
                }

                currentNPC = (MonoBehaviour)npc;
                isNearNPC = true;
                foundNPC = true;

                // 显示当前NPC的按钮
                ShowNPCButton(currentNPC);
                break;
            }
        }

        // 如果没有找到附近的NPC，重置状态
        if (!foundNPC)
        {
            isNearNPC = false;
            // 隐藏当前NPC的按钮（如果有）
            if (currentNPC != null)
            {
                HideNPCButton(currentNPC);
                currentNPC = null;
            }
        }
    }

    private void EnterTalking()
    {
        ResetAllAnimBools();
        m_Anim.SetBool("arguing", true);
        isAruging = true;

        if (currentNPC != null)
        {
            INPCInteractable npc = currentNPC as INPCInteractable;
            npc?.EnterTalking(transform);
        }

        RotateTowardsNPC(true);
    }

    private void ExitTalking()
    {
        if (isAruging)
        {
            ResetAllAnimBools();
            m_Anim.SetBool("idle", true);
            isAruging = false;
            if (currentNPC != null)
            {
                INPCInteractable npc = currentNPC as INPCInteractable;
                npc?.ExitTalking();
            }
        }
    }
    // 统一的显示NPC按钮方法
    private void ShowNPCButton(MonoBehaviour npc)
    {
        if (npc == null) return;

        // 检查所有可能的NPC类型
        npc1Control npc1 = npc.GetComponent<npc1Control>();
        npc2Control npc2= npc.GetComponent<npc2Control>();
        npc3Control npc3 = npc.GetComponent<npc3Control>();
        npc4Control npc4 = npc.GetComponent<npc4Control>();

        if (npc1 != null)
        {
            if (npc1.dialogue != null)
            {
                npc1.dialogue.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("未找到npc1对话按钮: " + npc.gameObject.name);
            }
        }
        else if (npc4 != null)
        {
            if (npc4.dialogue != null)
            {
                npc4.dialogue.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("未找到npc4对话按钮: " + npc.gameObject.name);
            }
        }
        else
        {
            Debug.LogError("未知的NPC类型: " + npc.gameObject.name);
        }
    }

    // 统一的隐藏NPC按钮方法
    private void HideNPCButton(MonoBehaviour npc)
    {
        if (npc == null) return;

        // 检查所有可能的NPC类型
        npc1Control npc1 = npc.GetComponent<npc1Control>();
        npc2Control npc2 = npc.GetComponent<npc2Control>();
        npc3Control npc3 = npc.GetComponent<npc3Control>();
        npc4Control npc4 = npc.GetComponent<npc4Control>();
        // 可以继续添加其他NPC类型...

        if (npc1 != null)
        {
            if (npc1.dialogue != null)
            {
                npc1.dialogue.gameObject.SetActive(false);
            }
        }
        else if (npc4 != null)
        {
            if (npc4.dialogue != null)
            {
                npc4.dialogue.gameObject.SetActive(false);
            }
        }
        // 可以继续添加其他NPC类型...
    }

    // ================= 面向 NPC 功能 =================
    private void RotateTowardsNPC(bool instant = false)
    {
        if (currentNPC == null) return;

        Vector3 targetDir = currentNPC.transform.position - transform.position;
        targetDir.y = 0f;
        if (targetDir == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(targetDir);

        if (instant)
            transform.rotation = targetRot;
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * faceTurnSpeed);
    }
}