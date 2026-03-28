# 🚀 VisionTech Full-Stack Project

Repository ini berisi ekosistem aplikasi yang terdiri dari **Express.js Backend**, **Vue Ant Design Frontend**, dan **C# .NET API** dengan arsitektur **DDD (Clean Architecture)**.

---

## 🛠️ Perbaikan Krusial (Debug OTP)

Terdapat update pada logic OTP untuk menangani *type mismatch* (Integer vs String) yang sering menyebabkan crash pada Library Keyv/Auth saat validasi ID.

**File Path:** `express_template/base/controller/auth/own.js`

Ganti fungsi `otp` yang lama dengan kode berikut:

<details>
<summary>Klik untuk melihat snippet kode OTP</summary>

```javascript
const otp = async (req, res) => {
  try {
    const { id, pin } = req.body

    // --- STEP 1: LOG INPUT ---
    console.log('--- DEBUG OTP START ---')
    console.log('Step 1 - Input Body:', { id, pin })

    const user = await authFns.findUser({ [AUTH_USER_FIELD_ID_FOR_JWT]: id })

    // --- STEP 2: LOG USER FROM DB ---
    if (!user) {
      console.log('Step 2 - User not found in DB')
      return res.status(401).json({ message: 'User not found' })
    }
    console.log('Step 2 - User Found:', { id: user.id, username: user.username })

    // --- STEP 3: LOG GAKEY & SECRET ---
    const rawGaKey = user[AUTH_USER_FIELD_GAKEY]
    const secretKey = getSecret('verify', 'access')

    console.log('Step 3 - Data Types Check:', {
      gaKeyType: typeof rawGaKey,
      gaKeyValue: rawGaKey,
      secretType: typeof secretKey,
      secretValue: secretKey ? 'EXISTS' : 'MISSING/NULL'
    })

    // --- STEP 4: PRE-CHECK OTP ---
    const gaKey = String(rawGaKey || '').trim()
    console.log('Step 4 - Sanitized gaKey:', gaKey)

    const isTestMode = (USE_OTP === 'TEST')
    console.log('Step 5 - Mode Check:', { USE_OTP, isTestMode })

    let isOtpValid = false
    if (isTestMode) {
      isOtpValid = (String(pin) === '111111')
    } else {
      console.log('Step 6 - Calling otplib.check...')
      try {
        isOtpValid = otplib.authenticator.check(String(pin), gaKey)
      } catch (otperr) {
        console.log('CRASH INSIDE OTPLIB:', otperr.message)
        throw otperr
      }
    }

    if (isOtpValid) {
      console.log('Step 7 - OTP Valid, creating token...')

      // FIX: Pastikan ID adalah STRING untuk mencegah crash pada Keyv storage
      const userForToken = {
        ...user,
        id: String(user.id),
        username: String(user.username)
      }

      const tokens = await createToken(userForToken)
      setTokensToHeader(res, tokens)
      console.log('--- DEBUG OTP SUCCESS ---')
      return res.status(200).json(tokens)
    } else {
      console.log('Step 7 - OTP Invalid')
      return res.status(401).json({ message: 'Invalid PIN' })
    }

  } catch (e) {
    console.log('--- DEBUG OTP ERROR ---')
    console.log('Error Type:', e.constructor.name)
    console.log('Error Message:', e.message)
    console.log('Stack Trace Snippet:', e.stack.split('\n').slice(0, 3).join('\n'))
    return res.status(500).json()
  }
}
```

</details>

---

## 💻 Panduan Run di Windows (Native)

Gunakan **PowerShell** untuk menjalankan langkah-langkah berikut:

1. **Install Dependencies:**
   Jalankan `npm install` di setiap root folder project dan folder `apps`.
2. **Run Backend (Express):**
   ```powershell
   cd express-template
   npm run local:win
   ```
3. **Run Frontend (Vue AntD):**
   ```powershell
   cd vue-antd-template\apps
   npm run sample
   ```

---

## 🐧 Panduan Run di WSL (Ubuntu)

### 1. Konfigurasi Awal
* Ganti semua `localhost` menjadi `0.0.0.0` pada config express & vue agar bisa diakses dari browser Windows.
* Update Linux: `sudo apt update`

### 2. Setup Node.js (V22)
```bash
# Instal NVM
curl -o- [https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.7/install.sh](https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.7/install.sh) | bash
source ~/.bashrc
nvm install 22 && nvm use 22
```

### 3. Jalankan Node.js Stack
**PENTING:** Hapus folder `node_modules` bawaan Windows karena perbedaan binary (*invalid ELF header*).

* **Express Template:**
  ```bash
  cd /mnt/{BASE_DIR}/visiontech/express-template
  rm -rf node_modules package-lock.json
  npm install && npm run local
  ```
* **Vue AntD Template:**
  ```bash
  cd /mnt/{BASE_DIR}/visiontech/vue-antd-template
  rm -rf node_modules package-lock.json
  npm install && npm run sample
  ```

---

## 🏗️ C# API & WebSocket (WSL)

1. **Instal .NET SDK 8:**
   ```bash
   sudo apt install -y dotnet-sdk-8.0
   ```
2. **Run Project:**
   ```bash
   cd /mnt/{BASE_DIR}/visiontech/MyLibraryApp/
   dotnet run --project MyLibrary.Api --launch-profile "http"
   ```

---

## 📝 Troubleshooting & Tips
* **Invalid ELF Header:** Hapus `node_modules` lalu `npm install` ulang di dalam WSL.
* **Akses Browser:** Gunakan `http://localhost:PORT` di Edge Windows.
* **Node Version:** Pastikan `node -v` menunjukkan versi **22.x.x**.

---
*Last Updated: 2026-03-29*