// recover.js – Password Recovery Flow Handler
document.addEventListener("DOMContentLoaded", () => {
    const showToast = window.showToast || function(msg, type) { console.log("[" + type + "] " + msg); };
    
    // Steps Containers
    const step1 = document.getElementById("recover-step-1");
    const step2 = document.getElementById("recover-step-2");
    const step3 = document.getElementById("recover-step-3");

    // Inputs & Buttons
    const emailInput = document.getElementById("recover-email");
    const btnStep1 = document.getElementById("btn-recover-1");

    const codeInput = document.getElementById("recover-code");
    const btnStep2 = document.getElementById("btn-recover-2");
    const timerLabel = document.getElementById("mock-countdown");

    const passInput1 = document.getElementById("recover-pass-1");
    const passInput2 = document.getElementById("recover-pass-2");
    const btnStep3 = document.getElementById("btn-recover-3");

    // Local state
    let recoverEmail = "";
    let countdownSeconds = 58;
    let timerInterval = null;

    // Start countdown timer for resending OTP
    function startTimer() {
        clearInterval(timerInterval);
        countdownSeconds = 58;
        if (timerLabel) {
            timerLabel.textContent = `Gửi lại mã sau ${countdownSeconds}s`;
            timerLabel.style.opacity = "0.7";
            timerLabel.style.pointerEvents = "none";
        }

        timerInterval = setInterval(() => {
            countdownSeconds--;
            if (timerLabel) {
                timerLabel.textContent = `Gửi lại mã sau ${countdownSeconds}s`;
            }

            if (countdownSeconds <= 0) {
                clearInterval(timerInterval);
                if (timerLabel) {
                    timerLabel.innerHTML = '<a href="#" id="resend-recover-otp" class="text-danger fw-bold text-decoration-none">Gửi lại mã OTP</a>';
                    timerLabel.style.opacity = "1";
                    timerLabel.style.pointerEvents = "auto";
                    
                    // Attach event listener for resend
                    const resendLink = document.getElementById("resend-recover-otp");
                    resendLink?.addEventListener("click", (e) => {
                        e.preventDefault();
                        resendOtp();
                    });
                }
            }
        }, 1000);
    }

    function resendOtp() {
        showToast("✉️ Đang gửi lại mã xác thực...", "info");
        fetch("/api/auth/forgot-password", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email: recoverEmail })
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
    }

    // Step 1: Send recovery email OTP request
    btnStep1?.addEventListener("click", (e) => {
        e.preventDefault();
        const emailVal = emailInput.value.trim();
        if (!emailVal) {
            showToast("⚠️ Vui lòng nhập địa chỉ email của bạn!", "warning");
            return;
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(emailVal)) {
            showToast("⚠️ Địa chỉ Email không đúng định dạng!", "warning");
            return;
        }

        recoverEmail = emailVal;
        showToast("✉️ Đang gửi yêu cầu khôi phục...", "info");

        fetch("/api/auth/forgot-password", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email: recoverEmail })
        })
        .then(res => res.json())
        .then(res => {
            if (res.success) {
                showToast("✉️ Mã OTP đã được gửi về email của bạn (Mã: " + res.data.otp + ")", "info");
                
                // Transition to Step 2
                step1.classList.add("d-none");
                step2.classList.remove("d-none");
                codeInput.value = "";
                codeInput.focus();
                
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

    // Step 2: Verify OTP
    btnStep2?.addEventListener("click", (e) => {
        e.preventDefault();
        const otpVal = codeInput.value.trim();
        if (otpVal.length < 6) {
            showToast("⚠️ Vui lòng nhập đủ 6 chữ số OTP!", "warning");
            return;
        }

        showToast("🔑 Đang xác minh mã OTP...", "info");

        fetch("/api/auth/verify-otp", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email: recoverEmail, otp: otpVal })
        })
        .then(res => res.json())
        .then(res => {
            if (res.success) {
                showToast("✔️ Xác minh OTP thành công!", "success");
                clearInterval(timerInterval);
                
                // Transition to Step 3
                step2.classList.add("d-none");
                step3.classList.remove("d-none");
                passInput1.value = "";
                passInput2.value = "";
                passInput1.focus();
            } else {
                showToast("❌ " + (res.message || "Mã OTP không chính xác!"), "error");
            }
        })
        .catch(err => {
            console.error(err);
            showToast("❌ Lỗi kết nối đến máy chủ", "error");
        });
    });

    // Step 3: Reset Password
    btnStep3?.addEventListener("click", (e) => {
        e.preventDefault();
        const pass1 = passInput1.value;
        const pass2 = passInput2.value;

        if (!pass1 || !pass2) {
            showToast("⚠️ Vui lòng nhập đầy đủ mật khẩu mới!", "warning");
            return;
        }

        if (pass1.length < 6) {
            showToast("⚠️ Mật khẩu mới phải có độ dài tối thiểu 6 ký tự!", "warning");
            return;
        }

        if (pass1 !== pass2) {
            showToast("⚠️ Mật khẩu xác nhận không trùng khớp!", "warning");
            return;
        }

        showToast("💾 Đang cập nhật mật khẩu mới...", "info");

        fetch("/api/auth/reset-password", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email: recoverEmail, password: pass1 })
        })
        .then(res => res.json())
        .then(res => {
            if (res.success) {
                showToast("🎉 Đặt lại mật khẩu mới thành công!", "success");
                
                setTimeout(() => {
                    window.location.href = "/Account/Login";
                }, 1000);
            } else {
                showToast("❌ Lỗi: " + res.message, "error");
            }
        })
        .catch(err => {
            console.error(err);
            showToast("❌ Lỗi kết nối đến máy chủ", "error");
        });
    });
});
