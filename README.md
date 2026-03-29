# 🚀 VisionTech Full-Stack Project

This repository contains the **C# .NET API** ecosystem with **DDD (Clean Architecture)** architecture.
and explanation of **Express.js Backend**, **Vue Ant Design Frontend**

---
## 🛠️ WSL Internal IP Set
<details>
<summary>Click to view code snippet</summary>


  **replace func server.listen()**
  ```javascript
  server.listen(API_PORT, '0.0.0.0', () => {
    console.info(`[(${process.env.NODE_ENV}) ${process.env.APP_VERSION}] listening on port ${API_PORT}, https=${Boolean(HTTPS_CERTIFICATE)}`)
  })
  pada file express-template/index.js
  ```
  
  **replace CORS_OPTION in the env used**
  ```config
  CORS_OPTIONS='{
    "methods": "GET,HEAD,PUT,PATCH,POST,DELETE,OPTIONS",
    "preflightContinue": false,
    "optionsSuccessStatus": 204,
    "credentials": true,
    "origin": ["http://127.0.0.1:8080", "http://localhost:8080", "https://127.0.0.1:8080", "https://localhost:8080"]
  }'
  ```

</details>

## 🛠️ Crucial Fix (OTP Debug)

There is an update to the OTP logic to handle the *type mismatch* (Integer vs String) that often causes crashes in the Keyv/Auth Library during ID validation.

**File Path:** `express_template/base/controller/auth/own.js`

Replace the old `otp` function with the following code:

<details>
<summary>Click to view code snippet</summary>

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

      // FIX: Make sure ID is STRING to prevent crash on Keyv storage
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

## 💻 Run Guide on Windows (Native)

Use **PowerShell** to perform the following steps:

1. **Install Dependencies:**
   Run `npm install` in each project root folder and `apps` folder.
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

## 🐧 Run Guide on WSL (Ubuntu)

### 1. Initial Configuration
* Update Linux: `sudo apt update`

### 2. Setup Node.js (V22)
```bash
# Instal NVM
curl -o- [https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.7/install.sh](https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.7/install.sh) | bash
source ~/.bashrc
nvm install 22 && nvm use 22
```

### 3. Run Node.js Stack
**IMPORTANT:** Delete the default Windows `node_modules` folder due to binary differences (*invalid ELF header*).

* **Express Template:**
  ```bash
  cd /mnt/{BASE_DIR}/visiontech/express-template
  rm -rf node_modules package-lock.json
  npm install #[root & apps] 
  npm run local
  ```
* **Vue AntD Template:**
  ```bash
  cd /mnt/{BASE_DIR}/visiontech/vue-antd-template
  rm -rf node_modules package-lock.json
  npm install #[root & apps]
  cd apps
  npm run sample
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
* **Invalid ELF Header:** Remove `node_modules` and then `npm install` again in WSL.
* **Browser Access:** Use `http://localhost:PORT` in Edge Windows.
* **Node Version:** Make sure `node -v` shows version **22.x.x**.

---
*Last Updated: 2026-03-29*