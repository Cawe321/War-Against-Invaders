using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AccountUIManager : MonoBehaviour
{
    bool isBusy = false;

    #region LOGIN
    [Header("References - Login")]
    public RectTransform LoginMenu;
    public InputField LoginEmailInput;
    public InputField LoginPasswordInput;
    public Text LoginErrorMessageText;

    public void LoginSubmit()
    {
        if (isBusy)
            return;
        ProcessingMenu.SetActive(true);
        isBusy = true;
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
        ProcessingMenu.SetActive(false);
        isBusy = false;
    }

    private void OnLoginSuccess(LoginResult result)
    {
        DataManager.instance.userPlayFabID = result.PlayFabId;
        PlayerPrefs.SetString("LastEmail", LoginEmailInput.text);
        PlayerPrefs.Save();
        ProcessingMenu.SetActive(false);
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
        if (isBusy)
            return;
        if (RegisterPasswordInput.text.Length < 6)
        {
            RegisterErrorMessageText.text = "Password is too short! It requires at least 6 characters.";
            return;
        }
        ProcessingMenu.SetActive(true);
        isBusy = true;
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
        ProcessingMenu.SetActive(false);
        OpenNoticeMenu("Registration complete! You are now logged in.", NOTICE_MENU_TYPE.REGISTER_SUCCESS);
    }

    void OnRegisterError(PlayFabError error)
    {
        RegisterErrorMessageText.text = error.ErrorMessage;
        RegisterErrorMessageText.gameObject.SetActive(true);
        ProcessingMenu.SetActive(false);
        isBusy = false;
    }
    #endregion

    #region FORGOT
    [Header("References - Forgot")]
    public RectTransform ForgotMenu;
    public InputField ForgotEmailInput;
    public Text ForgotErrorMessageText;

    public void ForgotSubmit()
    {
        ProcessingMenu.SetActive(true);
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
        ProcessingMenu.SetActive(false);
    }

    private void OnForgotReset(SendAccountRecoveryEmailResult obj)
    {
        ProcessingMenu.SetActive(false);
        OpenNoticeMenu("An email with instructions has been sent to you.", NOTICE_MENU_TYPE.FORGOTPASSWORD_SUCCESS);
    }
    #endregion

    [Header("References - Misc")]
    public RectTransform NoticeMenu;
    public Text NoticeMenuText;
    public GameObject ProcessingMenu;
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
        isBusy = false;
        ProcessingMenu.SetActive(false);

        if (PlayerPrefs.HasKey("LastEmail"))
        {
            LoginEmailInput.text = PlayerPrefs.GetString("LastEmail");
        }
        LoginEmailInput.Select();
    }

    // Update is called once per frame
    void Update()
    {
        // Login
        {
            if (NoticeMenu.gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                    CloseNoticeMenu();
            }
            else if (EventSystem.current.currentSelectedGameObject == LoginEmailInput.gameObject)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    LoginPasswordInput.Select();
                }
            }
            else if (EventSystem.current.currentSelectedGameObject == LoginPasswordInput.gameObject)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    LoginSubmit();
                }
            }
        }

        // Register
        {
            if (NoticeMenu.gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                    CloseNoticeMenu();
            }
            else if (EventSystem.current.currentSelectedGameObject == RegisterEmailInput.gameObject)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    RegisterNameInput.Select();
                }
            }
            if (EventSystem.current.currentSelectedGameObject == RegisterNameInput.gameObject)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    RegisterPasswordInput.Select();
                }
            }
            if (EventSystem.current.currentSelectedGameObject == RegisterPasswordInput.gameObject)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    RegisterSubmit();
                }
            }
        }

        // Forgot Password
        {
            if (NoticeMenu.gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                    CloseNoticeMenu();
            }
            else if (EventSystem.current.currentSelectedGameObject == ForgotEmailInput.gameObject)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    ForgotSubmit();
                }
            }
        }
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
                    //NoticeMenu.gameObject.SetActive(false);
                    FindObjectOfType<IntroSceneManager>().LoginSuccessful();
                    break;
                }
            case NOTICE_MENU_TYPE.REGISTER_SUCCESS:
                {
                    //NoticeMenu.gameObject.SetActive(false);
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
