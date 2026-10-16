using UnityEngine;

public class PositionFilterUi : UiBase
{
    [SerializeField] private UiButton _buttonClose;
    [SerializeField] private UiButton _buttonConfirm;

    [Header("Total")]
    [SerializeField] private UiButton _buttonTotal;
    [SerializeField] private GameObject _imageCheckTotal;

    [Header("Positions")]
    [SerializeField] private UiButton _buttonFw;
    [SerializeField] private GameObject _imageCheckFw;

    [SerializeField] private UiButton _buttonMf;
    [SerializeField] private GameObject _imageCheckMf;

    [SerializeField] private UiButton _buttonDf;
    [SerializeField] private GameObject _imageCheckDf;

    [SerializeField] private UiButton _buttonGk;
    [SerializeField] private GameObject _imageCheckGk;

    private bool _isTotal = true;
    private bool _isFw = false;
    private bool _isMf = false;
    private bool _isDf = false;
    private bool _isGk = false;

    private ICardFilterable _applyTarget;

    public void Init(ICardFilterable applyTarget, bool total, bool fw, bool mf, bool df, bool gk)
    {
        _applyTarget = applyTarget;
        _isTotal = total;
        _isFw = fw;
        _isMf = mf;
        _isDf = df;
        _isGk = gk;
        UpdateUi();
    }

    private void OnEnable()
    {
        if (_buttonClose != null)
        {
            _buttonClose.BindOnClickButtonEvent(OnClickClose);
        }

        if (_buttonConfirm != null)
        {
            _buttonConfirm.BindOnClickButtonEvent(OnClickConfirm);
        }

        if (_buttonTotal != null)
        {
            _buttonTotal.BindOnClickButtonEvent(OnClickTotal);
        }

        if (_buttonFw != null)
        {
            _buttonFw.BindOnClickButtonEvent(OnClickFw);
        }

        if (_buttonMf != null)
        {
            _buttonMf.BindOnClickButtonEvent(OnClickMf);
        }

        if (_buttonDf != null)
        {
            _buttonDf.BindOnClickButtonEvent(OnClickDf);
        }

        if (_buttonGk != null)
        {
            _buttonGk.BindOnClickButtonEvent(OnClickGk);
        }
    }

    private void OnClickClose()
    {
        UiManager.Instance.CloseUi<PositionFilterUi>();
    }

    private void OnClickConfirm()
    {
        if (_applyTarget != null)
        {
            _applyTarget.ApplyPositionFilter(_isTotal, _isFw, _isMf, _isDf, _isGk);
        }

        UiManager.Instance.CloseUi<PositionFilterUi>();
    }

    private void OnClickTotal()
    {
        ResetToTotal();
    }

    private void ResetToTotal()
    {
        _isTotal = true;
        _isFw = false;
        _isMf = false;
        _isDf = false;
        _isGk = false;
        UpdateUi();
    }

    private void OnClickFw()
    {
        if (_isFw == false)
        {
            _isFw = true;
        }

        else
        {
            _isFw = false;
        }

        CheckPositionState();
    }

    private void OnClickMf()
    {
        if (_isMf == false)
        {
            _isMf = true;
        }

        else
        {
            _isMf = false;
        }

        CheckPositionState();
    }

    private void OnClickDf()
    {
        if (_isDf == false)
        {
            _isDf = true;
        }

        else
        {
            _isDf = false;
        }

        CheckPositionState();
    }

    private void OnClickGk()
    {
        if (_isGk == false)
        {
            _isGk = true;
        }

        else
        {
            _isGk = false;
        }

        CheckPositionState();
    }

    private void CheckPositionState()
    {
        bool allOff = (_isFw == false && _isMf == false && _isDf == false && _isGk == false);
        bool allOn = (_isFw && _isMf && _isDf && _isGk);

        if (allOff)
        {
            _isTotal = true;
        }

        else if (allOn)
        {
            _isTotal = true;
            _isFw = false;
            _isMf = false;
            _isDf = false;
            _isGk = false;
        }

        else
        {
            _isTotal = false;
        }

        UpdateUi();
    }

    private void UpdateUi()
    {
        if (_imageCheckTotal != null)
        {
            _imageCheckTotal.SetActive(_isTotal);
        }

        if (_imageCheckFw != null)
        {
            _imageCheckFw.SetActive(_isFw);
        }

        if (_imageCheckMf != null)
        {
            _imageCheckMf.SetActive(_isMf);
        }

        if (_imageCheckDf != null)
        {
            _imageCheckDf.SetActive(_isDf);
        }

        if (_imageCheckGk != null)
        {
            _imageCheckGk.SetActive(_isGk);
        }
    }
}