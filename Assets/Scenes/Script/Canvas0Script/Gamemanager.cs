using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    [SerializeField]
    private FragmentHuntGameManager fragmentHuntManager;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (fragmentHuntManager != null)
            {
                fragmentHuntManager.ShowGame();
            }
            else
            {
                Debug.LogWarning("按下空格，但未找到 FragmentHuntGameManager 实例。");
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (fragmentHuntManager != null)
            {
                fragmentHuntManager.HideGame();
            }
            else
            {
                Debug.LogWarning("按下Esc，但未找到 FragmentHuntGameManager 实例。");
            }
        }
    }
}
