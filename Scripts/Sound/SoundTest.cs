using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundTest : MonoBehaviour
{
    public SFXType stest;
    public BGMType btest;


    public void BGMTest()
    {
        SoundManager.Instance.PlayBGM(btest, true);
    }

    public void BGMStop()
    {
        SoundManager.Instance.StopBGM();
    }

    public void SFXTest()
    {
        SoundManager.Instance.PlaySFX(stest);
    }
}
