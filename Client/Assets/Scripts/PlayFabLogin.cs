using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    public bool IsLoggedIn { get; private set; }
    public string PlayFabId { get; private set; }

    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            throw new Exception("TitleId Is Null");
        }

//#if UNITY_EDITOR

        var request = new LoginWithCustomIDRequest 
        { 
            CustomId = "GettingStartedGuide", 
            CreateAccount = true 
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
//#else
//        PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest
//        {
//            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
//            CreateAccount = true
//        }, result =>
//        {
//            Debug.Log("Logged in with Android device ID");
//        }, error =>
//        {
//            Debug.LogError("Error logging in with Android device ID: " + error.GenerateErrorReport());
//        });
//#endif
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }



    public async Task<bool> LoginWithGoogleAsync(string googleToken = null)
    {
        try
        {
            Debug.Log("[SocialAuthenticationService] Google 로그인 시작...");

            if (string.IsNullOrEmpty(googleToken))
            {
                Debug.LogWarning("[SocialAuthenticationService] Google 토큰이 제공되지 않아 Guest 로그인으로 대체");
                return await LoginAsGuestAsync();
            }

            var tcs = new TaskCompletionSource<bool>();

            var request = new LoginWithGoogleAccountRequest
            {
                ServerAuthCode = googleToken,
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }
            };

            PlayFabClientAPI.LoginWithGoogleAccount(request,
                result => {
                    HandleLoginSuccess(result);
                    tcs.SetResult(true);
                },
                error => {
                    Debug.LogWarning($"[SocialAuthenticationService] Google 로그인 실패, Guest로 대체: {error.GenerateErrorReport()}");
                    // Google 로그인 실패 시 Guest 로그인으로 fallback
                    Task.Run(async () => {
                        var guestResult = await LoginAsGuestAsync();
                        tcs.SetResult(guestResult);
                    });
                });

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SocialAuthenticationService] Google 로그인 예외: {ex.Message}");
            return await LoginAsGuestAsync(); // 예외 시 Guest 로그인으로 fallback
        }
    }


    public async Task<bool> LoginAsGuestAsync()
    {
        try
        {
            Debug.Log("[SocialAuthenticationService] Guest 로그인 시작...");

            var deviceId = SystemInfo.deviceUniqueIdentifier;
            var tcs = new TaskCompletionSource<bool>();

            var request = new LoginWithCustomIDRequest
            {
                CustomId = deviceId,
                CreateAccount = true,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }
            };

            PlayFabClientAPI.LoginWithCustomID(request,
                result => {
                    HandleLoginSuccess(result);
                    tcs.SetResult(true);
                },
                error => {
                    HandleLoginFailure(error);
                    tcs.SetResult(false);
                });

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SocialAuthenticationService] Guest 로그인 예외: {ex.Message}");
            return false;
        }
    }

    private void HandleLoginSuccess(LoginResult result)
    {
        IsLoggedIn = true;
        PlayFabId = result.PlayFabId;

        Debug.Log($"[SocialAuthenticationService] 로그인 성공: {PlayFabId}");

        // Firebase 연동
        SyncFirebaseUserIdOnAccountLink();
    }

    private void HandleLoginFailure(PlayFabError error)
    {
        IsLoggedIn = false;
        PlayFabId = null;
        //DisplayName = "Guest";
        //CurrentSocialType = SocialLoginType.None;

        Debug.LogError($"[SocialAuthenticationService] 로그인 실패: {error.GenerateErrorReport()}");
        //OnLoginFailed?.Invoke(error);
    }

    public async Task SyncFirebaseUserIdOnAccountLink()
    {
        try
        {
            Debug.Log("🔄 계정 연동 시 Firebase UserID 동기화 시작...");

            if (IsLoggedIn != true)
            {
                Debug.LogWarning("⚠️ PlayFab 로그인 상태가 아님 - Firebase UserID 동기화 중단");
                return;
            }

            //// Firebase 초기화 상태 확인
            //if (FirebaseManager.Instance?.IsInitialized != true)
            //{
            //    Debug.LogWarning("⚠️ Firebase가 초기화되지 않음 - Firebase UserID 동기화 중단");
            //    return;
            //}

            //// Firebase Auth 확인 및 현재 UserID 가져오기
            //var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            //if (auth?.CurrentUser == null)
            //{
            //    Debug.LogWarning("⚠️ Firebase 인증 상태가 아님 - Firebase UserID 동기화 중단");
            //    return;
            //}

            //string currentFirebaseUserId = auth.CurrentUser.UserId;
            //Debug.Log($"📋 현재 Firebase UserID: {currentFirebaseUserId}");

            //// PlayFab에서 기존에 저장된 UserID 확인 (중복 방지)
            //var socialDataService = socialManager.DataService;
            //string existingUserId = await socialDataService.LoadFirebaseUserIdAsync();

            //if (!string.IsNullOrEmpty(existingUserId) && existingUserId == currentFirebaseUserId)
            //{
            //    Debug.Log($"🔄 동일한 Firebase UserID가 이미 저장됨: {existingUserId} - 백업 저장 생략");

            //    // 계정 연동 상태 업데이트만 수행
            //    EnsurePlayerProgressLoaded();
            //    _localPlayerProgress.IsLinkedAccount = true;
            //    SaveLocalPlayerProgress();

            //    // 클라우드와 동기화
            //    await LoadPlayerProgressFromCloud();

            //    Debug.Log("✅ 계정 연동 시 Firebase UserID 동기화 완료 (중복 저장 생략)");
            //    return;
            //}

            //// 새로운 UserID이므로 PlayFab에 저장
            //Debug.Log($"💾 새로운 Firebase UserID 백업 저장: {currentFirebaseUserId}");
            //bool saveSuccess = await socialDataService.SaveFirebaseUserIdAsync(currentFirebaseUserId);

            //if (saveSuccess)
            //{
            //    Debug.Log($"✅ Firebase UserID PlayFab 백업 저장 완료: {currentFirebaseUserId}");

            //    // 계정 연동 상태 업데이트
            //    EnsurePlayerProgressLoaded();
            //    _localPlayerProgress.IsLinkedAccount = true;
            //    SaveLocalPlayerProgress();

            //    // 클라우드와 동기화
            //    await LoadPlayerProgressFromCloud();

            //    Debug.Log("✅ 계정 연동 시 Firebase UserID 동기화 완료");
            //}
            //else
            //{
            //    Debug.LogWarning("⚠️ Firebase UserID PlayFab 백업 저장 실패");
            //}
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ 계정 연동 시 Firebase UserID 동기화 실패: {ex.Message}");
        }
    }
}