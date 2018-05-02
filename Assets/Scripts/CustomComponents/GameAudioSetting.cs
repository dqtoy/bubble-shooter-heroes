using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameAudioSetting : MonoBehaviour
{

    public static bool AudioEnabling = true;

    public CustomToggle CustomSoundToggle;
    public Toggle SoundToggle;

    void OnEnable()
    {
        AudioEnabling = Utilities.PlayerPrefs.GetBool("AudioEnable", true);
        if (SoundToggle != null)
        {
            SoundToggle.isOn = AudioEnabling;
        }
        if (CustomSoundToggle != null)
        {
            CustomSoundToggle.SetToggle(AudioEnabling);
        }
    }

    public void SetAudioEnable(bool enable)
    {
        AudioHelper.SetAudioEnable(enable);
        Utilities.PlayerPrefs.SetBool("AudioEnable", enable);
        Utilities.PlayerPrefs.Flush();
    }

}
