using UnityEngine;
using System.Collections;

public class LavaManager : MonoBehaviour
{
    private Coroutine wetCoroutine;  // 3秒間水に触れているかを監視するコルーチンの状態

    // SE用
    [SerializeField] private AudioClip destroySE;

    // 何かに触れたら
    private void OnTriggerEnter(Collider other)
    {
        // 水が触れたら
        if (other.CompareTag("Water") && wetCoroutine == null)
        {
            Debug.Log("ブロックに水が触れました");

            // 3秒カウントするコルーチンを開始
            wetCoroutine = StartCoroutine(WetRoutine());
            GameManager.Instance.waterCount = true;  // 触れていることを知らせる
        }
    }

    // 何かが離れたら
    private void OnTriggerExit(Collider other)
    {
        // 水が離れたら
        if (other.CompareTag("Water") && wetCoroutine != null)
        {
            Debug.Log("ブロックから水が離れました");

            // 3秒カウントするコルーチンを中断
            StopCoroutine(wetCoroutine);
            wetCoroutine = null;  // リセット
            GameManager.Instance.waterCount = false;  // 離れたことを知らせる
        }
    }

    // 3秒間をカウントするコルーチン
    private IEnumerator WetRoutine()
    {
        Debug.Log("カウントダウン開始");

        // 3秒間待つ
        yield return new WaitForSeconds(3.0f);

        Debug.Log("3秒間経ちました!");

        // 自分自身が消えてもSEが最後まで鳴る
        AudioSource.PlayClipAtPoint(destroySE, Camera.main.transform.position, 0.5f);

        // 自分自身を消す
        Destroy(gameObject);

        // リセット
        wetCoroutine = null;
        GameManager.Instance.waterCount = false;  // 離れたことを知らせる
    }
}
