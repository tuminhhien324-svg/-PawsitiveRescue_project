document.addEventListener("DOMContentLoaded", () => {
  const showToast = window.showToast || function(msg, type) { console.log("[" + type + "] " + msg); };
  // --- Sections and Toggles ---
  const authSplitSection = document.getElementById("auth-split-section");
  const authOtpSection = document.getElementById("auth-otp-section");
  const authProfileSection = document.getElementById("auth-profile-section");
  
  const signinBox = document.getElementById("signin-box-container");
  const signupStep1Box = document.getElementById("signup-step1-container");
  
  const linkToSignup = document.getElementById("link-to-signup");
  const linkToSignin = document.getElementById("link-to-signin");
  const authLeftPanel = document.getElementById("auth-left-panel");

  // State
  let signupData = {
    fullname: "",
    email: "",
    phone: "",
    password: ""
  };
  let countdownSeconds = 58;
  let timerInterval = null;

  // Toggle Login/Register Step 1
  linkToSignup?.addEventListener("click", (e) => {
    e.preventDefault();
    signinBox.classList.add("d-none");
    signupStep1Box.classList.remove("d-none");
    authLeftPanel?.classList.add("signup-mode");
    
    document.getElementById("brand-signin-content").classList.add("d-none");
    document.getElementById("brand-signup-content").classList.remove("d-none");
  });

  linkToSignin?.addEventListener("click", (e) => {
    e.preventDefault();
    signupStep1Box.classList.add("d-none");
    signinBox.classList.remove("d-none");
    authLeftPanel?.classList.remove("signup-mode");
    
    document.getElementById("brand-signup-content").classList.add("d-none");
    document.getElementById("brand-signin-content").classList.remove("d-none");
  });

  // --- Sign In Action ---
  const signinForm = document.getElementById("signin-form");
  signinForm?.addEventListener("submit", (e) => {
    e.preventDefault();
    const identity = document.getElementById("signin-identity").value.trim();
    const password = document.getElementById("signin-password").value;
    const errorBox = document.getElementById("signin-error");
    const errorText = document.getElementById("signin-error-text");

    errorBox.classList.add("d-none");
    fetch("/api/Auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email: identity, password: password })
    })
    .then(res => res.json())
    .then(res => {
      if (res.success) {
        localStorage.setItem(PAWSITIVE_AUTH_KEY, JSON.stringify(res.data));
        showToast("🐾 Đăng nhập thành công!", "success");
        
        const urlParams = new URLSearchParams(window.location.search);
        const returnUrl = urlParams.get('ReturnUrl');
        const destination = returnUrl || (res.data.role === "admin" ? "/Admin/Index" : (res.data.role === "volunteer" ? "/Account/Profile" : "/Home/Index"));
        
        setTimeout(() => {
          window.location.href = destination;
        }, 600);
      } else {
        errorText.textContent = res.message || "Email hoặc mật khẩu không đúng.";
        errorBox.classList.remove("d-none");
      }
    })
    .catch(err => {
      console.error(err);
      errorText.textContent = "Không thể kết nối đến máy chủ.";
      errorBox.classList.remove("d-none");
    });
  });

  // --- Sign Up Step 1 Submit ---
  const signupStep1Form = document.getElementById("signup-step1-form");
  signupStep1Form?.addEventListener("submit", (e) => {
    e.preventDefault();
    
    const fullname = document.getElementById("signup-fullname").value.trim();
    const email = document.getElementById("signup-email").value.trim();
    const phone = document.getElementById("signup-phone").value.trim();
    const password = document.getElementById("signup-password").value;
    const confirmPassword = document.getElementById("signup-confirm-password").value;

    if (!fullname || !email || !phone || !password || !confirmPassword) {
      showToast("⚠️ Vui lòng nhập đầy đủ thông tin!", "warning");
      return;
    }

    if (password.length < 6) {
      showToast("⚠️ Mật khẩu phải có tối thiểu 6 ký tự!", "warning");
      return;
    }

    if (password !== confirmPassword) {
      showToast("⚠️ Mật khẩu xác nhận không khớp!", "warning");
      return;
    }

    // Save state
    signupData.fullname = fullname;
    signupData.email = email;
    signupData.phone = phone;
    signupData.password = password;

    // Call send-otp API first!
    showToast("✉️ Đang gửi yêu cầu mã xác thực...", "info");
    fetch("/api/Auth/send-otp", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email: email })
    })
    .then(res => res.json())
    .then(res => {
      if (res.success) {
        transitionToStep2(res.data.otp);
      } else {
        showToast("❌ " + res.message, "error");
      }
    })
    .catch(err => {
      console.error(err);
      showToast("❌ Lỗi kết nối đến máy chủ", "error");
    });
  });

  // --- Step 2: OTP Handling ---
  function maskEmail(email) {
    if (!email) return "";
    const parts = email.split('@');
    if (parts.length !== 2) return email;
    const name = parts[0];
    const domain = parts[1];
    if (name.length <= 3) {
      return name[0] + "***@" + domain;
    }
    return name.slice(0, 3) + "***@" + domain;
  }

  function startTimer() {
    clearInterval(timerInterval);
    countdownSeconds = 58;
    
    const timerText = document.getElementById("otp-timer-count");
    const resendLink = document.getElementById("otp-resend-link");
    
    if (timerText) timerText.textContent = `${countdownSeconds}s`;
    if (resendLink) {
      resendLink.style.pointerEvents = "none";
      resendLink.style.opacity = "0.6";
      resendLink.classList.add("disabled");
    }

    timerInterval = setInterval(() => {
      countdownSeconds--;
      if (timerText) timerText.textContent = `${countdownSeconds}s`;

      if (countdownSeconds <= 0) {
        clearInterval(timerInterval);
        if (resendLink) {
          resendLink.style.pointerEvents = "auto";
          resendLink.style.opacity = "1";
          resendLink.classList.remove("disabled");
          resendLink.innerHTML = "Gửi lại mã";
        }
      }
    }, 1000);
  }

  function transitionToStep2(otpCode) {
    // Hide Split Card
    authSplitSection.classList.add("d-none");
    authSplitSection.classList.remove("d-flex");
    
    // Show OTP Card
    authOtpSection.classList.remove("d-none");
    authOtpSection.classList.add("d-flex");
    
    // Update Masked Email
    const masked = maskEmail(signupData.email);
    document.getElementById("otp-sent-email").textContent = masked;
    
    // Clear inputs
    const otpInputs = document.querySelectorAll("#otp-inputs-row .otp-input-box");
    otpInputs.forEach(input => input.value = "");
    if (otpInputs.length) otpInputs[0].focus();
    
    // Start countdown timer
    startTimer();
    showToast("✉️ Mã OTP đã được gửi tới email của bạn (Mã: " + (otpCode || "123456") + ")", "info");
  }

  // OTP inputs keyboard auto-tabbing
  const otpInputs = document.querySelectorAll("#otp-inputs-row .otp-input-box");
  otpInputs.forEach((input, index) => {
    input.addEventListener("input", (e) => {
      const val = e.target.value;
      if (!/^[0-9]$/.test(val)) {
        e.target.value = "";
        return;
      }
      if (index < otpInputs.length - 1) {
        otpInputs[index + 1].focus();
      }
    });

    input.addEventListener("keydown", (e) => {
      if (e.key === "Backspace") {
        if (e.target.value === "") {
          if (index > 0) {
            otpInputs[index - 1].focus();
            otpInputs[index - 1].value = "";
          }
        } else {
          e.target.value = "";
        }
      }
    });
  });

  // Resend OTP trigger
  const resendLink = document.getElementById("otp-resend-link");
  resendLink?.addEventListener("click", (e) => {
    e.preventDefault();
    if (countdownSeconds > 0) return;
    
    showToast("✉️ Đang gửi lại mã xác thực...", "info");
    fetch("/api/Auth/send-otp", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email: signupData.email })
    })
    .then(res => res.json())
    .then(res => {
      if (res.success) {
        showToast("✉️ Một mã OTP mới đã được gửi (Mã: " + res.data.otp + ")", "info");
        startTimer();
      } else {
        showToast("❌ " + res.message, "error");
      }
    })
    .catch(err => {
      console.error(err);
      showToast("❌ Lỗi kết nối đến máy chủ", "error");
    });
  });

  // Verify OTP button click
  const verifyBtn = document.getElementById("signup-next-2");
  verifyBtn?.addEventListener("click", () => {
    let otpCode = "";
    otpInputs.forEach(input => otpCode += input.value);

    if (otpCode.length < 6) {
      showToast("⚠️ Vui lòng nhập đủ 6 chữ số OTP!", "warning");
      return;
    }

    fetch("/api/Auth/verify-otp", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email: signupData.email, otp: otpCode })
    })
    .then(res => res.json())
    .then(res => {
      if (res.success) {
        clearInterval(timerInterval);
        transitionToStep3();
      } else {
        showToast("❌ " + (res.message || "Mã OTP không chính xác!"), "error");
      }
    })
    .catch(err => {
      console.error(err);
      showToast("❌ Lỗi kết nối đến máy chủ", "error");
    });
  });

  // Exit OTP Button
  const otpExitBtn = document.getElementById("otp-exit-btn");
  otpExitBtn?.addEventListener("click", (e) => {
    e.preventDefault();
    clearInterval(timerInterval);
    authOtpSection.classList.add("d-none");
    authOtpSection.classList.remove("d-flex");
    authSplitSection.classList.remove("d-none");
    authSplitSection.classList.add("d-flex");
  });

  // --- Step 3: Profile Completion Handling ---
  function transitionToStep3() {
    authOtpSection.classList.add("d-none");
    authOtpSection.classList.remove("d-flex");
    
    authProfileSection.classList.remove("d-none");
    authProfileSection.classList.add("d-flex");
  }

  // Avatar Select
  const avatarItems = document.querySelectorAll(".avatar-select-item:not(#custom-avatar-btn)");
  avatarItems.forEach(item => {
    item.addEventListener("click", () => {
      document.querySelectorAll(".avatar-select-item").forEach(i => i.classList.remove("active-avatar"));
      item.classList.add("active-avatar");
    });
  });

  // Custom Avatar upload
  const customAvatarBtn = document.getElementById("custom-avatar-btn");
  const avatarFileInput = document.getElementById("signup-avatar-file");

  customAvatarBtn?.addEventListener("click", () => {
    avatarFileInput.click();
  });

  avatarFileInput?.addEventListener("change", (e) => {
    const file = e.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = function(evt) {
        const base64 = evt.target.result;
        customAvatarBtn.innerHTML = `<img src="${base64}" style="width:100%; height:100%; object-fit:cover; border-radius:50%;" />`;
        
        document.querySelectorAll(".avatar-select-item").forEach(i => i.classList.remove("active-avatar"));
        customAvatarBtn.classList.add("active-avatar");
        customAvatarBtn.dataset.avatar = base64;
      };
      reader.readAsDataURL(file);
    }
  });

  // Animal Preference click
  const prefCards = document.querySelectorAll("#animal-pref-row .pref-card");
  prefCards.forEach(card => {
    card.addEventListener("click", () => {
      prefCards.forEach(c => c.classList.remove("active-pref"));
      card.classList.add("active-pref");
    });
  });

  // Step 3 Submit
  const signupSubmitBtn = document.getElementById("signup-submit-btn");
  signupSubmitBtn?.addEventListener("click", () => {
    // Get chosen avatar url or base64
    const activeAvatar = document.querySelector(".avatar-select-item.active-avatar");
    let avatarUrl = "https://cdn-icons-png.flaticon.com/512/149/149071.png";
    if (activeAvatar) {
      avatarUrl = activeAvatar.dataset.avatar || activeAvatar.querySelector("img")?.src || avatarUrl;
    }

    // Save avatar in localStorage temporarily for user display customization
    localStorage.setItem("pawsitive_user_custom_avatar", avatarUrl);

    // Call Register API
    fetch("/api/Auth/register", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        hoTen: signupData.fullname,
        email: signupData.email,
        password: signupData.password,
        soDienThoai: signupData.phone,
        diaChi: "",
        namSinh: null
      })
    })
    .then(res => res.json())
    .then(res => {
      if (res.success) {
        // Automatically Log user in
        showToast("🎉 Đăng ký thành công! Đang tự động đăng nhập...", "success");
        autoLogin(signupData.email, signupData.password, avatarUrl);
      } else {
        showToast("❌ Lỗi: " + (res.message || "Không thể đăng ký."), "error");
      }
    })
    .catch(err => {
      console.error(err);
      showToast("❌ Lỗi kết nối đến máy chủ", "error");
    });
  });

  function autoLogin(email, password, avatarUrl) {
    fetch("/api/Auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email: email, password: password })
    })
    .then(res => res.json())
    .then(res => {
      if (res.success) {
        // Inject custom avatar if user chose one
        if (avatarUrl) {
          res.data.avatar = avatarUrl;
        }
        localStorage.setItem(PAWSITIVE_AUTH_KEY, JSON.stringify(res.data));
        
        setTimeout(() => {
          window.location.href = "/Home/Index";
        }, 800);
      } else {
        // Fallback if login fails
        showToast("⚠️ Vui lòng đăng nhập bằng tài khoản vừa tạo.", "warning");
        setTimeout(() => {
          window.location.href = "/Account/Login";
        }, 1200);
      }
    })
    .catch(err => {
      console.error(err);
      window.location.href = "/Account/Login";
    });
  }

  // --- Password Eye Toggle ---
  document.querySelectorAll(".password-eye-toggle").forEach(toggle => {
    toggle.addEventListener("click", () => {
      const targetId = toggle.dataset.target;
      const input = document.getElementById(targetId);
      if (input) {
        if (input.type === "password") {
          input.type = "text";
          toggle.classList.remove("bi-eye");
          toggle.classList.add("bi-eye-slash");
        } else {
          input.type = "password";
          toggle.classList.remove("bi-eye-slash");
          toggle.classList.add("bi-eye");
        }
      }
    });
  });
});
