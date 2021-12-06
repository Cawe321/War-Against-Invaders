using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountUIManager : MonoBehaviour
{
    #region LOGIN
    [Header("References - Login")]
    public RectTransform LoginMenu;
    public InputField LoginEmailInput;
    public InputField LoginPasswordInput;
    public Text LoginErrorMessageText;

    public void LoginSubmit()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = LoginEmailInput.text,
            Password = LoginPasswordInput.text,
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginError);
    }

    private void OnLoginError(PlayFabError error)
    {
        LoginErrorMessageText.text = error.ErrorMessage;
        LoginErrorMessageText.gameObject.SetActive(true);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        DataManager.instance.userPlayFabID = result.PlayFabId;
        OpenNoticeMenu("Login success!", NOTICE_MENU_TYPE.LOGIN_SUCCESS);
    }
    #endregion

    #region REGISTER
    [Header("References - Register")]
    public RectTransform RegisterMenu;
    public InputField RegisterEmailInput;
    public InputField RegisterNameInput;
    public InputField RegisterPasswordInput;
    public Text RegisterErrorMessageText;

    public void RegisterSubmit()
    {
        if (RegisterPasswordInput.text.Length < 6)
        {
            RegisterErrorMessageText.text = "Password is too short! It requires at least 6 characters.";
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Email = RegisterEmailInput.text,
            Password = RegisterPasswordInput.text,
            Username = RegisterNameInput.text,
            RequireBothUsernameAndEmail = true
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterError);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        DataManager.instance.userPlayFabID = result.PlayFabId;
        OpenNoticeMenu("Registration complete! You are now logged in.", NOTICE_MENU_TYPE.REGISTER_SUCCESS);
    }

    void OnRegisterError(PlayFabError error)
    {
        RegisterErrorMessageText.text = error.ErrorMessage;
        RegisterErrorMessageText.gameObject.SetActive(true);
    }
    #endregion

    #region FORGOT
    [Header("References - Forgot")]
    public RectTransform ForgotMenu;
    public InputField ForgotEmailInput;
    public Text ForgotErrorMessageText;

    public void ForgotSubmit()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = ForgotEmailInput.text,
            TitleId = "4B1EA"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnForgotReset, OnForgotError);
    }

    private void OnForgotError(PlayFabError obj)
    {
        ForgotErrorMessageText.text = obj.ErrorMessage;
        ForgotErrorMessageText.gameObject.SetActive(true);
    }

    private void OnForgotReset(SendAccountRecoveryEmailResult obj)
    {
        OpenNoticeMenu("An email with instructions has been sent to you.", NOTICE_MENU_TYPE.FORGOTPASSWORD_SUCCESS);
    }
    #endregion

    [Header("References - Misc")]
    public RectTransform NoticeMenu;
    public Text NoticeMenuText;
    public enum NOTICE_MENU_TYPE
    {
        LOGIN_SUCCESS,
        REGISTER_SUCCESS,
        FORGOTPASSWORD_SUCCESS,
    }
    NOTICE_MENU_TYPE noticeMenuType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenRegister()
    {
        RegisterMenu.gameObject.SetActive(true);

        LoginMenu.gameObject.SetActive(false);
        ForgotMenu.gameObject.SetActive(false);
    }

    public void OpenLogin()
    {
        LoginMenu.gameObject.SetActive(true);

        RegisterMenu.gameObject.SetActive(false);
        ForgotMenu.gameObject.SetActive(false);
    }

    public void OpenForgotPassword()
    {
        ForgotMenu.gameObject.SetActive(true);

        LoginMenu.gameObject.SetActive(false);
        RegisterMenu.gameObject.SetActive(false);
    }
    
    public void OpenNoticeMenu(string text, NOTICE_MENU_TYPE noticeType)
    {
        noticeMenuType = noticeType;
        NoticeMenu.gameObject.SetActive(true);
        NoticeMenuText.text = text;
    }

    public void CloseNoticeMenu()
    {
        switch (noticeMenuType)
        {
            case NOTICE_MENU_TYPE.LOGIN_SUCCESS:
                {
                    FindObjectOfType<IntroSceneManager>().LoginSuccessful();
                    break;
                }
            case NOTICE_MENU_TYPE.REGISTER_SUCCESS:
                {
                    FindObjectOfType<IntroSceneManager>().LoginSuccessful();
                    break;
                }
            case NOTICE_MENU_TYPE.FORGOTPASSWORD_SUCCESS:
                {
                    NoticeMenu.gameObject.SetActive(false);
                    OpenLogin();
                    break;
                }

        }
    }
}
