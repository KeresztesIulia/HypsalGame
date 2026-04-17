using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class script_ui_FakeLoadingScreen : MonoBehaviour
{
    [System.Serializable]
    struct Timings
    {
        public float savingTime;
        public float exitTime;
        [Range(0,1), Tooltip("How far into the exiting process should Lai interrupt the process?")] public float cancelPercentage;
        public float cancelingTime;
    }

    [System.Serializable]
    struct LoadingTexts
    {
        public string savingText;
        public string[] savingDetailTexts;

        public string exitText;
        public string[] exitDetailTexts;

        public string cancelText;
        public string[] cancelDetailTexts;

        public string endingLogText;

    }

    [SerializeField] Timings _defaultTiming;

    [Header("First time")]
    [SerializeField, Tooltip("-1 to use default; 0 for default percentage")] Timings _firstTimeTiming;
    [SerializeField] LoadingTexts _firstTimeTexts;

    [Header("Second time")]
    [SerializeField, Tooltip("-1 to use default; 0 for default percentage")] Timings _secondTimeTiming;
    [SerializeField] LoadingTexts _secondTimeTexts;

    [Header("Third time?")] //do we have third time? I think not...?
    [SerializeField, Tooltip("-1 to use default; 0 for default percentage")] Timings _thirdTimeTiming;
    [SerializeField] LoadingTexts _thirdTimeTexts;

    [Header("Components")]
    [SerializeField] GameObject _fakeLoadingContainer;
    [SerializeField] TMP_Text _loadingText;
    [SerializeField] Slider _loadingBar;
    [SerializeField] TMP_Text _loadingDetailText;

    public void StartFakeLoading(int loadingIndex)
    {
        Timings timings;
        LoadingTexts loadingTexts;
        switch (loadingIndex)
        {
            case 1:
                timings = _firstTimeTiming;
                loadingTexts = _firstTimeTexts;
                break;
            case 2:
                timings = _secondTimeTiming;
                loadingTexts = _secondTimeTexts;
                break;
            case 3:
                timings = _thirdTimeTiming;
                loadingTexts = _thirdTimeTexts;
                break;
            default:
                timings = _defaultTiming;
                loadingTexts = _firstTimeTexts;
                break;
        }

        _fakeLoadingContainer.SetActive(true);
        // stop player interacting: disable player interaction script, which, on disable, should also disable UI
        StartCoroutine(FakeLoad(timings, loadingTexts));
    }

    IEnumerator FakeLoad(Timings timings, LoadingTexts loadingTexts)
    {
        float fullLoadTime = timings.savingTime != -1 ? timings.savingTime : _defaultTiming.savingTime;
        yield return FakeLoadSection(true, fullLoadTime, fullLoadTime, true, loadingTexts.savingText, loadingTexts.savingDetailTexts);

        fullLoadTime = timings.exitTime != -1 ? timings.exitTime : _defaultTiming.exitTime;
        float cancelPercentage = timings.cancelPercentage != 0 ? timings.cancelPercentage : _defaultTiming.cancelPercentage;
        float interruptionTime = fullLoadTime * cancelPercentage;
        yield return FakeLoadSection(true, interruptionTime, fullLoadTime, true, loadingTexts.exitText, loadingTexts.exitDetailTexts);

        fullLoadTime = timings.cancelingTime != -1 ? timings.cancelingTime : _defaultTiming.cancelingTime;
        yield return FakeLoadSection(false, fullLoadTime, fullLoadTime, true, loadingTexts.cancelText, loadingTexts.cancelDetailTexts);

        // Set new player position
        // activate new room layout
        // reenable interaction
        _fakeLoadingContainer.SetActive(false);

        script_ui_LabelLog.LogDesperateAIText(loadingTexts.endingLogText, true);

        yield return null;
    }

    IEnumerator FakeLoadSection(bool updateLoadingBar, float loadTime, float loadingBarTime, bool updateLoadingDetails, string loadingText, string[] loadingDetailTexts)
    {
        _loadingText.text = loadingText;

        bool hasDetail = !(loadingDetailTexts is null || loadingDetailTexts.Length == 0);
        _loadingDetailText.gameObject.SetActive(hasDetail && updateLoadingDetails);

        float timer = 0;
        while (timer < loadTime)
        {
            if (updateLoadingBar)
                _loadingBar.value = timer / loadingBarTime;

            if (updateLoadingDetails && hasDetail)
            {
                float percent = timer / loadTime;
                int detailIndex = (int)(percent * loadingDetailTexts.Length);
                _loadingDetailText.text = loadingDetailTexts[detailIndex];
            }

            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (updateLoadingBar) _loadingBar.value = loadTime / loadingBarTime;
    }
}
