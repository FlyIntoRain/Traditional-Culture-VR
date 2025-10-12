using UnityEngine;

public interface INPCInteractable
{
    Transform transform { get; } // 方便直接拿位置
    void EnterTalking(Transform player);
    void ExitTalking();
    bool HasTalked { get; set; }
}
