using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WarningSimpleUi : UiBase
{
    [SerializeField] private TextMeshProUGUI text_Warning;
    private CancellationTokenSource _cts;

    public async UniTaskVoid ShowWarning(string message, float duration = 2.0f)
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        if (text_Warning != null)
        {
            text_Warning.text = message;
        }

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: _cts.Token);
            UiManager.Instance.CloseUi<WarningSimpleUi>();
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }
}