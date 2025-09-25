using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideosPlay : MonoBehaviour
{
    public VideoPlayer[] videoPlayer; // ����VideoPlayer���
    public AudioSource audioSource; // ��Ӷ�AudioSource������
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
   
        if (Input.GetMouseButtonDown(0)) // ����������
        {

            for (int i=0; i<videoPlayer.Length; i++)
            {
                if (videoPlayer[i].isPlaying)
                {
                    videoPlayer[i].Pause();
                    audioSource.Pause();
                }
               
                else
                { videoPlayer[i].Play();
                    audioSource.UnPause();
                }
                
            }
          

        }
        else if(Input.GetMouseButtonDown(1)) // ����Ҽ�����
        {
            for (int i = 0; i < videoPlayer.Length; i++)
            {
                videoPlayer[i].time = 0f; // ���ò���ʱ�䵽��ͷ
                audioSource.time = 0f;
                if (!videoPlayer[i].isPlaying)
                {
                    videoPlayer[i].Play(); // �����Ƶ��ͣ����ʼ����
                    audioSource.Play();
                }

            }
             
        }
    }
}
