using UnityEngine;
using System.Collections;

public class PauseEvent : MonoBehaviour
{
    public void Pause(float duration)
    {
        StartCoroutine(PauseRoutine(duration));
    }

    private IEnumerator PauseRoutine(float duration)
    {
        // 記錄當前的時間縮放
        float originalTimeScale = Time.timeScale;

        // 設置時間縮放為0來暫停遊戲
        Time.timeScale = 0f;

        // 使用 unscaled time 來等待duration秒
        yield return new WaitForSecondsRealtime(duration);

        // 恢復原來的時間縮放
        Time.timeScale = originalTimeScale;
    }
}