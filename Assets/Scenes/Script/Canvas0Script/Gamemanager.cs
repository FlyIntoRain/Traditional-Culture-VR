using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemanager : MonoBehaviour
{
    [SerializeField]
    private FragmentHuntGameManager fragmentHuntManager;
    [SerializeField]
    private Button game2Button;
    [SerializeField]
    private npc4Control game2Control;

    void Start()
    {
        if (fragmentHuntManager == null)
        {
            fragmentHuntManager = FindObjectOfType<FragmentHuntGameManager>();
            if (fragmentHuntManager == null)
            {
                Debug.LogWarning("未找到 FragmentHuntGameManager，请在Inspector拖入引用或确保场景中存在该组件。");
            }
        }
    }


    void Update()
    {
        if (fragmentHuntManager == null)
        {
            // 懒加载一次，避免初始时未赋值导致按键无效
            fragmentHuntManager = FindObjectOfType<FragmentHuntGameManager>();
        }
        if (game2Button.IsActive()&&Input.GetKeyDown(KeyCode.Return)&&game2Control.HasTalked==false)
        {
            if (fragmentHuntManager != null)
            {
                fragmentHuntManager.ShowGame();
                game2Control.HasTalked= false;
            }
            else
            {
                Debug.LogWarning("按下空格，但未找到 FragmentHuntGameManager 实例。");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (fragmentHuntManager != null)
            {
                game2Control.HasTalked = false; // 重置对话状态;
                fragmentHuntManager.HideGame();
            }
            else
            {
                Debug.LogWarning("按下Esc，但未找到 FragmentHuntGameManager 实例。");
            }
        }
    }
}
