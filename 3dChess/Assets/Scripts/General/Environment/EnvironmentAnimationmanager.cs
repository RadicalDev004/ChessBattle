using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentAnimationmanager : MonoBehaviour
{
    public GameObject Fire;

    private void Start()
    {
        StartCoroutine(FireAnimationCor());
    }

    private IEnumerator FireAnimationCor()
    {
        float minBound = 0.4f;
        float maxBound = 0.8f;
        float mid = (minBound + maxBound) * 0.5f;

        bool goBig = true;

        while (true)
        {
            float rndScale;

            if (goBig)
                rndScale = Random.Range(mid, maxBound);   // big
            else
                rndScale = Random.Range(minBound, mid);   // small

            goBig = !goBig;

            float rndTime = Random.Range(0.5f, 1f);

            Tween.LocalScale(Fire.transform, Vector3.one * rndScale, rndTime, 0, Tween.EaseInOut);

            yield return new WaitForSeconds(rndTime);
        }
    }
}
