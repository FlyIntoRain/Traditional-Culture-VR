using UnityEngine;
using UnityEngine.UI;

public class DisplayScreen : MonoBehaviour
{
    public Text intfoText;

    void Awake()
    {
        MultScreen();
    }

    private void Update()
    {

        // MultScreen();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();

        }
        
    }

    void MultScreen()
    {

       // Debug.Log(GetType() + "/MultScreen()/ Display.displays.Length = " + Display.displays.Length);
       // intfoText.text = "��ǰ�����Ļ����Ϊ��" + Display.displays.Length;
        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
            Screen.SetResolution(Display.displays[i].renderingWidth, Display.displays[i].renderingHeight, true);
            
        }
    }
}