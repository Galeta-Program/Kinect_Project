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
        // �O����e���ɶ��Y��
        float originalTimeScale = Time.timeScale;

        // �]�m�ɶ��Y��0�ӼȰ��C��
        Time.timeScale = 0f;

        // �ϥ� unscaled time �ӵ���duration��
        yield return new WaitForSecondsRealtime(duration);

        // ��_��Ӫ��ɶ��Y��
        Time.timeScale = originalTimeScale;
    }
}