using UnityEngine;
using System.Collections;

public class LineRangeManager : MonoBehaviour
{
    private Coroutine LineCoroutine;  // 3秒間赤線に触れているかを監視するコルーチンの状態
    private int touchingCount = 0;  // エリア内にあるブロックの数

    // 何かに触れたら
    private void OnTriggerEnter(Collider other)
    {
        // ブロックが触れたら
        if (other.CompareTag("Target"))
        {
            touchingCount++;
            Debug.Log("ブロックが赤線に触れました");

            // 1個目がエリアに入った瞬間だけコルーチンを開始
            if (LineCoroutine == null)
            {
                LineCoroutine = StartCoroutine(LineRoutine());
                GameManager.Instance.lineCount = true;  // 触れていることを知らせる
            }
        }
    }

    // 何かが離れたら
    private void OnTriggerExit(Collider other)
    {
        // ブロックが離れたら
        if (other.CompareTag("Target"))
        {
            // 0未満を防止しながら数を引く
            touchingCount = Mathf.Max(0, touchingCount - 1);
            Debug.Log("ブロックが赤線から離れました");

            // エリア内のTargetが完全に0個になった時だけコルーチンを止める
            if (touchingCount == 0 && LineCoroutine != null)
            {
                StopCoroutine(LineCoroutine);
                LineCoroutine = null;  // リセット
                GameManager.Instance.lineCount = false;  // 離れたことを知らせる
            }
        }
    }

    // 3秒間をカウントするコルーチン
    private IEnumerator LineRoutine()
    {
        Debug.Log("カウントダウン開始");

        // 5秒間待つ
        yield return new WaitForSeconds(5.0f);

        Debug.Log("5秒間経ちました!");

        // ゲーム終了したらブロックが増えないようにする
        GameManager.Instance.pose = true;  
        // GameOverを知らせる
        GameManager.Instance.gameOver = true;
    }
}