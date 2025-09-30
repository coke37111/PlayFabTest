using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    public bool IsLoggedIn { get; private set; }
    public string PlayFabId { get; private set; }

    public async void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            throw new Exception("TitleId Is Null");
        }

        // PlayFab에서 기존에 저장된 UserID 확인 (중복 방지)
        string existingUserId = await LoadFirebaseUserIdAsync();

        if (string.IsNullOrEmpty(existingUserId))
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }
        else
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = existingUserId,
                CreateAccount = false
            };

            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }
        //#if UNITY_EDITOR

        
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



    public async Task<bool> LoginWithGoogleAsync()
    {
        try
        {
            Debug.Log("[SocialAuthenticationService] Google 계정 연동 시작...");

            var tcs = new TaskCompletionSource<bool>();

            // Google 계정으로 연동하지만 실제로는 Guest 로그인으로 처리
            // 소셜 타입만 Google로 설정하여 상태 관리
            var guestResult = await LoginAsGuestAsync();
            if (guestResult)
            {
                Debug.Log("[SocialAuthenticationService] Google 계정 연동 완료 (Guest 기반)");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SocialAuthenticationService] Google 계정 연동 예외: {ex.Message}");
            return false;
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
            var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            if (auth?.CurrentUser == null)
            {
                Debug.LogWarning("⚠️ Firebase 인증 상태가 아님 - Firebase UserID 동기화 중단");
                return;
            }

            string currentFirebaseUserId = auth.CurrentUser.UserId;
            Debug.Log($"📋 현재 Firebase UserID: {currentFirebaseUserId}");

            // PlayFab에서 기존에 저장된 UserID 확인 (중복 방지)
            string existingUserId = await LoadFirebaseUserIdAsync();

            if (!string.IsNullOrEmpty(existingUserId) && existingUserId == currentFirebaseUserId)
            {
                Debug.Log($"🔄 동일한 Firebase UserID가 이미 저장됨: {existingUserId} - 백업 저장 생략");
                Debug.Log("✅ 계정 연동 시 Firebase UserID 동기화 완료 (중복 저장 생략)");
                return;
            }

            // 새로운 UserID이므로 PlayFab에 저장
            Debug.Log($"💾 새로운 Firebase UserID 백업 저장: {currentFirebaseUserId}");
            bool saveSuccess = await SaveFirebaseUserIdAsync(currentFirebaseUserId);

            if (saveSuccess)
            {
                Debug.Log($"✅ Firebase UserID PlayFab 백업 저장 완료: {currentFirebaseUserId}");
                Debug.Log("✅ 계정 연동 시 Firebase UserID 동기화 완료");
            }
            else
            {
                Debug.LogWarning("⚠️ Firebase UserID PlayFab 백업 저장 실패");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ 계정 연동 시 Firebase UserID 동기화 실패: {ex.Message}");
        }
    }

    public async Task<string> LoadFirebaseUserIdAsync()
    {
        try
        {
            Debug.Log("[SocialDataService] Firebase UserID 로드 중...");

            if (!IsLoggedIn)
            {
                Debug.LogWarning("[SocialDataService] PlayFab 로그인이 필요합니다");
                return null;
            }

            var tcs = new TaskCompletionSource<string>();

            var request = new GetUserDataRequest
            {
                Keys = new List<string> { "FirebaseUserId" }
            };

            PlayFabClientAPI.GetUserData(request,
                result => {
                    if (result.Data?.ContainsKey("FirebaseUserId") == true)
                    {
                        string firebaseUserId = result.Data["FirebaseUserId"].Value;
                        Debug.Log($"[SocialDataService] Firebase UserID 로드 완료: {firebaseUserId}");
                        tcs.SetResult(firebaseUserId);
                    }
                    else
                    {
                        Debug.Log("[SocialDataService] 저장된 Firebase UserID가 없습니다");
                        tcs.SetResult(null);
                    }
                },
                error => {
                    Debug.LogError($"[SocialDataService] Firebase UserID 로드 실패: {error.GenerateErrorReport()}");
                    tcs.SetResult(null);
                });

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SocialDataService] Firebase UserID 로드 예외: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> SaveFirebaseUserIdAsync(string firebaseUserId)
    {
        try
        {
            Debug.Log($"[SocialDataService] Firebase UserID 저장 중: {firebaseUserId}");

            if (!IsLoggedIn)
            {
                Debug.LogError("[SocialDataService] PlayFab 로그인이 필요합니다");
                return false;
            }

            if (string.IsNullOrEmpty(firebaseUserId))
            {
                Debug.LogWarning("[SocialDataService] Firebase UserID가 비어있습니다");
                return false;
            }

            var tcs = new TaskCompletionSource<bool>();

            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                    {
                        { "FirebaseUserId", firebaseUserId }
                    }
            };

            PlayFabClientAPI.UpdateUserData(request,
                result => {
                    Debug.Log($"[SocialDataService] Firebase UserID 저장 완료: {firebaseUserId}");
                    tcs.SetResult(true);
                },
                error => {
                    Debug.LogError($"[SocialDataService] Firebase UserID 저장 실패: {error.GenerateErrorReport()}");
                    tcs.SetResult(false);
                });

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SocialDataService] Firebase UserID 저장 예외: {ex.Message}");
            return false;
        }
    }
}