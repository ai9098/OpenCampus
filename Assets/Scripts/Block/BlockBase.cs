using UnityEngine;
using System.Collections;

public class BlockBase : MonoBehaviour
{
    // 3秒間触れているかを監視するコルーチンの状態
    protected Coroutine activeCoroutine = null;  

    // SE用
    [SerializeField] protected AudioClip destroySE;

    // 指定したタグと接触したか判定し、コルーチンを開始する
    protected void HandleTriggerEnter(Collider other, string targetTag)
    {
        // 炎が触れたら
        if (other.CompareTag(targetTag) && activeCoroutine == null)
        {
            Debug.Log($"ブロックに{targetTag}が触れました");

            // 3秒カウントするコルーチンを開始
            activeCoroutine = StartCoroutine(CountDownRoutine(targetTag));
            SetGameManagerFlag(targetTag, true);
        }
    }

    // 指定したタグが離れたか判定し、コルーチンを停止する
    protected void HandleTriggerExit(Collider other, string targetTag)
    {
        // 炎が離れたら
        if (other.CompareTag(targetTag) && activeCoroutine != null)
        {
            Debug.Log($"ブロックから{targetTag}が離れました");

            // 3秒カウントするコルーチンを中断
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;  // コルーチンリセット

            SetGameManagerFlag(targetTag, false);
        }
    }

    // 3秒間をカウントするコルーチン
    private IEnumerator CountDownRoutine(string targetTag)
    {
        Debug.Log($"{targetTag}のカウントダウン開始");

        // 3秒間待つ
        yield return new WaitForSeconds(3.0f);

        Debug.Log("3秒間経ちました!");

        // 自分自身が消えてもSEが最後まで鳴る
        if (destroySE != null)
        {
            AudioSource.PlayClipAtPoint(destroySE, Camera.main.transform.position, 0.5f);
        }

        // 自分自身を消す
        Destroy(gameObject);

        // リセット
        activeCoroutine = null;
        SetGameManagerFlag(targetTag, false);
    }

    // GameManagerへのフラグ通知（タグに応じて切り替え）
    private void SetGameManagerFlag(string targetTag, bool isTouching)
    {
        // バグ防止
        if (GameManager.Instance == null) return;

        // ターゲットによって切り替える
        if (targetTag == "Fire")
        {
            GameManager.Instance.fireCount = isTouching;
        }
        if (targetTag == "Water")
        {
            GameManager.Instance.waterCount = isTouching;
        }
    }
}

