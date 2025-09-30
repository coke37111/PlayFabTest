# MochiRollRoll 키스토어 정보

## 키스토어 설정
- **파일명**: MochiRollRoll.keystore
- **위치**: C:\Projects\Classic1\Keystore\MochiRollRoll.keystore
- **별칭(Alias)**: mochirollroll
- **키스토어 비밀번호**: MochiRoll2024!
- **키 비밀번호**: MochiRoll2024!
- **유효기간**: 25,000일 (~2094년)

## SHA-1 지문
- **현재 키스토어 SHA-1**: 6B:96:B6:41:4B:48:9E:0F:9D:2D:5A:A4:DF:25:5E:0D:A2:7E:0F:73
- **Google Play Console 등록 SHA-1**: B2:B8:AD:49:E9:10:69:2E:00:3F:7D:CC:BC:BE:9B:50:57:62:B2:E9

## 주의사항
⚠️ **중요**: 현재 키스토어의 SHA-1 지문이 Google Play Console에 등록된 지문과 다릅니다.

### 해결 방법
1. **Google Play Console에서 새 SHA-1 추가**:
   - Google Play Console → 앱 서명 → 업로드 키 인증서
   - 새 SHA-1 지문 추가: `6B:96:B6:41:4B:48:9E:0F:9D:2D:5A:A4:DF:25:5E:0D:A2:7E:0F:73`

2. **Firebase 프로젝트에도 새 SHA-1 추가**:
   - Firebase Console → 프로젝트 설정 → Android 앱
   - SHA 인증서 지문에 새 SHA-1 추가

## Unity 설정
- **androidUseCustomKeystore**: 1 (활성화)
- **AndroidKeystoreName**: C:/Projects/Classic1/Keystore/MochiRollRoll.keystore
- **AndroidKeyaliasName**: mochirollroll

## 빌드 시 주의사항
- 릴리즈 빌드 시 키스토어 비밀번호 입력 필요
- Unity Cloud Build 사용 시 키스토어 파일을 안전하게 업로드 필요