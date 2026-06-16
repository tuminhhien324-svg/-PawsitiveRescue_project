// donate.js – Donation form and pipeline handler
console.log("donate.js initialized");

let selectedAmount = 100000; // Default preset amount
let selectedPaymentMethod = "Momo";

document.addEventListener("DOMContentLoaded", () => {
    // 1. Fetch campaigns and populate dropdown dynamically
    fetchCampaigns();

    // 2. Preset amount buttons handler
    const presetBtns = document.querySelectorAll(".btn-preset");
    const customAmtBox = document.getElementById("custom-amount-box");
    const customAmtInput = document.getElementById("input-custom-amount");

    presetBtns.forEach(btn => {
        btn.addEventListener("click", () => {
            // Reset active classes
            presetBtns.forEach(b => {
                b.classList.remove("btn-pawsitive-primary");
                b.classList.add("btn-pawsitive-secondary");
            });

            // Set active class on clicked button
            btn.classList.remove("btn-pawsitive-secondary");
            btn.classList.add("btn-pawsitive-primary");

            const amt = btn.dataset.amount;
            if (amt === "custom") {
                selectedAmount = "custom";
                customAmtBox.classList.remove("d-none");
                customAmtInput.focus();
            } else {
                selectedAmount = parseInt(amt);
                customAmtBox.classList.add("d-none");
            }
        });
    });

    // 3. Payment method selection
    const methodCards = document.querySelectorAll(".payment-method-card");
    methodCards.forEach(card => {
        card.addEventListener("click", () => {
            methodCards.forEach(c => c.classList.remove("active-method"));
            card.classList.add("active-method");
            selectedPaymentMethod = card.dataset.method;
        });
    });

    // 4. Clipboard copies for Bank account details
    const copyBtns = document.querySelectorAll("button[data-copy]");
    copyBtns.forEach(btn => {
        btn.addEventListener("click", (e) => {
            e.preventDefault();
            const targetSel = btn.dataset.copy;
            const targetEl = document.querySelector(targetSel);
            if (targetEl) {
                navigator.clipboard.writeText(targetEl.textContent.trim());
                showToast("📋 Đã sao chép vào bộ nhớ tạm!", "success");
            }
        });
    });

    // 5. Submit donation form
    const donationForm = document.getElementById("online-donation-form");
    if (donationForm) {
        donationForm.addEventListener("submit", submitDonation);
    }
});

async function fetchCampaigns() {
    const selectCampaign = document.getElementById("select-campaign");
    const trackerContainer = document.getElementById("campaigns-tracker-container");
    if (!selectCampaign && !trackerContainer) return;

    try {
        const res = await fetch("/api/Donation/campaigns");
        const json = await res.json();

        if (json.success && json.data && json.data.length > 0) {
            if (selectCampaign) {
                // Clear current options except placeholder
                selectCampaign.innerHTML = '<option value="" disabled selected>-- Chọn Quỹ Quyên Góp --</option>';
                json.data.forEach(camp => {
                    const opt = document.createElement("option");
                    opt.value = camp.tenChienDich;
                    opt.textContent = camp.tenChienDich;
                    selectCampaign.appendChild(opt);
                });
            }

            if (trackerContainer) {
                trackerContainer.innerHTML = json.data.map(camp => {
                    const percentage = Math.min(100, Math.round((camp.soTienDaQuyenGop / camp.soTienMucTieu) * 100));
                    const imgUrl = camp.anhChienDich || "https://images.unsplash.com/photo-1543466835-00a7907e9de1?auto=format&fit=crop&q=80&w=200";
                    return `
                        <div class="col-lg-6">
                          <div class="card-pawsitive p-4 bg-white d-flex align-items-center gap-3 flex-column flex-sm-row h-100">
                            <div class="pet-frame rounded-md border border-2 border-dark ratio ratio-1x1 overflow-hidden d-none d-sm-block" style="width: 140px; height: 140px; flex-shrink: 0">
                              <img src="${imgUrl}" alt="${camp.tenChienDich}" style="object-fit: cover; width: 100%; height: 100%;" />
                            </div>
                            <div class="flex-grow-1 w-100">
                              <h3 class="brand-font fs-5 text-danger mb-1">${camp.tenChienDich}</h3>
                              <p class="small text-muted mb-3">${camp.moTa || ''}</p>
                              <div class="d-flex justify-content-between align-items-center mb-1">
                                <span class="small font-bold text-muted">Đã đạt: ${percentage}%</span>
                                <span class="small font-bold text-danger">${camp.soTienDaQuyenGop.toLocaleString("vi-VN")} / ${camp.soTienMucTieu.toLocaleString("vi-VN")}đ</span>
                              </div>
                              <div class="progress-pawsitive">
                                <div class="progress-bar-pawsitive" style="width: ${percentage}%">
                                  <div class="progress-bar-paw-icon">🐾</div>
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>
                    `;
                }).join('');
            }
        } else {
            if (trackerContainer) {
                trackerContainer.innerHTML = `<div class="text-center py-5 text-muted">Không tìm thấy chiến dịch quyên góp nào.</div>`;
            }
        }
    } catch (err) {
        console.error("Failed to fetch campaigns dynamically:", err);
        if (trackerContainer) {
            trackerContainer.innerHTML = `<div class="text-center py-5 text-danger">Lỗi khi tải danh sách chiến dịch.</div>`;
        }
    }
}

async function submitDonation(e) {
    e.preventDefault();

    // Determine final amount
    let finalAmount = 0;
    if (selectedAmount === "custom") {
        const customAmtVal = document.getElementById("input-custom-amount").value;
        if (!customAmtVal || parseInt(customAmtVal) <= 0) {
            showToast("⚠️ Vui lòng nhập số tiền quyên góp!", "error");
            return;
        }
        finalAmount = parseInt(customAmtVal);
    } else {
        finalAmount = selectedAmount;
    }

    if (finalAmount < 1000) {
        showToast("⚠️ Số tiền quyên góp tối thiểu là 1.000 VND!", "error");
        return;
    }

    // Get campaign selection
    const campaignVal = document.getElementById("select-campaign").value;
    if (!campaignVal) {
        showToast("⚠️ Vui lòng chọn quỹ nhận quyên góp!", "error");
        return;
    }

    const messageVal = document.getElementById("input-message").value.trim();
    const isAnonymous = document.getElementById("check-anonymous").checked;

    const payload = {
        soTien: finalAmount,
        tenQuyQuyenGop: campaignVal,
        loiNhan: messageVal || null,
        anDanh: isAnonymous
    };

    showToast("💳 Đang kết nối đến cổng thanh toán...", "info");

    try {
        const res = await fetch("/api/Donation", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });
        const json = await res.json();

        if (json.success) {
            showToast("💖 Quyên góp thành công! Trạm cảm ơn bạn.", "success");
            
            // Show overlay modal
            document.getElementById("donate-success-amount").textContent = finalAmount.toLocaleString("vi-VN") + "đ";
            const overlay = document.getElementById("donate-success-overlay");
            overlay.classList.remove("d-none");
            overlay.classList.add("d-flex");
        } else {
            showToast("❌ Lỗi: " + json.message, "error");
        }
    } catch (err) {
        console.error(err);
        showToast("❌ Lỗi kết nối đến máy chủ", "error");
    }
}

// Global function to close overlay and redirect to profile
window.closeDonateSuccess = function() {
    const overlay = document.getElementById("donate-success-overlay");
    overlay.classList.remove("d-flex");
    overlay.classList.add("d-none");
    
    // Redirect to profile page so user can see it in their activity timeline!
    window.location.href = "/Account/Profile";
};
