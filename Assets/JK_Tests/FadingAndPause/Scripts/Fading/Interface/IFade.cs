using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

interface IFade
{
    IEnumerator FadeIn();
    IEnumerator FadeOut();
    void InstantFadeIn();
    void InstantFadeOut();
}