using UnityEngine;

public class WoodManager : BlockBase
{
    private const string TARGET_TAG = "Fire";

    // 何かに触れたら
    private void OnTriggerEnter(Collider other)
    {
        // 炎が触れたら
        HandleTriggerEnter(other, TARGET_TAG);
    }

    // 何かが離れたら
    private void OnTriggerExit(Collider other)
    {
        HandleTriggerExit(other, TARGET_TAG);
    }
}
