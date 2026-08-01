using UnityEngine;
using System.Collections;

public class WoodManager : MonoBehaviour
{
    private Coroutine burnCoroutine;  // 3秒間炎に触れているかを監視するコルーチンの状態

    // SE用
    [SerializeField] private AudioClip destroySE;

    // 何かに触れたら
    private void OnTriggerEnter(Collider other)
    {
        // 炎が触れたら
        if (other.CompareTag("Fire") && burnCoroutine == null)
        {
            Debug.Log("ブロックに炎が触れました");

            // 3秒カウントするコルーチンを開始
            burnCoroutine = StartCoroutine(BurnRoutine());
            GameManager.Instance.fireCount = true;  // 触れていることを知らせる
        }
    }

    // 何かが離れたら
    private void OnTriggerExit(Collider other)
    {
        // 炎が離れたら
        if (other.CompareTag("Fire") && burnCoroutine != null)
        {
            Debug.Log("ブロックから炎が離れました");

            // 3秒カウントするコルーチンを中断
            StopCoroutine(burnCoroutine);
            burnCoroutine = null;  // リセット
            GameManager.Instance.fireCount = false;  // 離れたことを知らせる
        }
    }

    // 3秒間をカウントするコルーチン
    private IEnumerator BurnRoutine()
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
        burnCoroutine = null;
        GameManager.Instance.fireCount = false;  // 離れたことを知らせる
    }
}
