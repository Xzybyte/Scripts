using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class SubtitleMixer : PlayableBehaviour
{

    // Called each frame the mixer is active, after inputs are processed
    public override void ProcessFrame(Playable handle, FrameData info, object playerData)
    {
        Text textObject = playerData as Text;
        if (textObject == null)
            return;

        string text = string.Empty;

        int count = handle.GetInputCount();
        for (var i = 0; i < count; i++)
        {

            Playable inputHandle = handle.GetInput(i);
            float weight = handle.GetInputWeight(i);

            if (inputHandle.IsValid() && inputHandle.GetPlayState() == PlayState.Playing && weight > 0)
            {
                var data = ((ScriptPlayable<SubtitleDataPlayable>)inputHandle).GetBehaviour();
                if (data != null)
                {
                    // custom blend that is more suited for crossblends
                    if (weight > 0.5)
                    {

                        float progress = (float)(inputHandle.GetTime() / inputHandle.GetDuration());
                        progress *= data.progressSpeed;
                        int subStringLength = Mathf.RoundToInt(Mathf.Clamp01(progress) * data.text.Length);
                        text = data.text.Substring(0, subStringLength);
                    }
                }

            }
        }

        textObject.text = text;
    }
}
