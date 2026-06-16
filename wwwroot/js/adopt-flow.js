// Pawsitive Rescue – Adoption Application Flow Wizard (adopt-flow.js)
console.log("adopt-flow.js initialized");

document.addEventListener("DOMContentLoaded", () => {
    // Determine active pet from URL query parameters
    const urlParams = new URLSearchParams(window.location.search);
    const petId = parseInt(urlParams.get("petId") || "1");

    // Fetch pet details to personalize wizard
    fetchPetDetails(petId);

    // Multi-step Wizard Navigation
    const steps = Array.from(document.querySelectorAll(".wizard-step"));
    let currentStep = 0;

    const btnPrev = document.getElementById("btn-prev");
    const btnNext = document.getElementById("btn-next");
    const btnSubmit = document.getElementById("btn-submit");
    const track = document.getElementById("wizard-progress-track");
    const percentLabel = document.getElementById("wizard-progress-percent");
    const stepTitle = document.getElementById("active-step-title");

    const stepTitles = [
        "Bước 1: Thông tin cá nhân",
        "Bước 2: Kinh nghiệm nuôi dưỡng",
        "Bước 3: Khảo sát điều kiện sống",
        "Bước 4: Gửi đơn đăng ký"
    ];

    function renderStep() {
        steps.forEach((s, idx) => {
            s.classList.toggle("d-none", idx !== currentStep);
        });

        // Update progress bar
        const percent = Math.round(((currentStep) / (steps.length - 1)) * 100);
        track.style.width = percent + "%";
        percentLabel.textContent = `${percent}% hoàn thành`;
        stepTitle.textContent = stepTitles[currentStep];

        // Button visibility
        btnPrev.style.visibility = currentStep === 0 ? "hidden" : "visible";
        btnNext.classList.toggle("d-none", currentStep === steps.length - 1);
        btnSubmit.classList.toggle("d-none", currentStep !== steps.length - 1);

        // If rendering Step 4 (Confirm step), populate confirmation fields
        if (currentStep === 3) {
            populateConfirmationTable();
        }
    }

    // Vietnamese Administrative Divisions Data
    const districtData = {
        "Thành phố Hồ Chí Minh": ["Quận 1", "Quận 3", "Quận 10", "Quận Bình Thạnh", "Quận Phú Nhuận", "Quận Gò Vấp", "Quận Tân Bình", "TP. Thủ Đức"],
        "Thành phố Hà Nội": ["Quận Hoàn Kiếm", "Quận Ba Đình", "Quận Tây Hồ", "Quận Cầu Giấy", "Quận Đống Đa", "Quận Hai Bà Trưng", "Quận Thanh Xuân"],
        "Thành phố Đà Nẵng": ["Quận Hải Châu", "Quận Thanh Khê", "Quận Sơn Trà", "Quận Ngũ Hành Sơn", "Quận Liên Chiểu", "Quận Cẩm Lệ"]
    };

    const selectProvince = document.getElementById("select-province");
    const selectDistrict = document.getElementById("select-district");
    const inputDistrictText = document.getElementById("input-district-text");

    if (selectProvince) {
        selectProvince.addEventListener("change", () => {
            const prov = selectProvince.value;
            if (prov === "Khác") {
                selectDistrict.classList.add("d-none");
                selectDistrict.removeAttribute("required");
                inputDistrictText.classList.remove("d-none");
                inputDistrictText.setAttribute("required", "required");
            } else {
                selectDistrict.classList.remove("d-none");
                selectDistrict.setAttribute("required", "required");
                inputDistrictText.classList.add("d-none");
                inputDistrictText.removeAttribute("required");

                // Populate districts
                selectDistrict.innerHTML = `<option value="" disabled selected>-- Chọn Quận/Huyện --</option>`;
                const list = districtData[prov] || [];
                list.forEach(d => {
                    selectDistrict.innerHTML += `<option value="${d}">${d}</option>`;
                });
            }
        });
    }

    function getCombinedAddress() {
        if (!selectProvince) return "";
        const province = selectProvince.value;
        let district = "";
        if (province === "Khác") {
            district = inputDistrictText.value.trim();
        } else {
            district = selectDistrict.value;
        }
        const ward = document.getElementById("input-ward").value.trim();
        const street = document.getElementById("input-street").value.trim();

        if (!province || !district || !ward || !street) return "";
        return `${street}, Phường/Xã ${ward}, Quận/Huyện ${district}, ${province}`;
    }

    // Step 1 Validation
    function validateStep1() {
        const fullname = document.getElementById("input-fullname").value.trim();
        const phone = document.getElementById("input-phone").value.trim();
        const email = document.getElementById("input-email").value.trim();
        const birthyear = document.getElementById("input-birthyear").value.trim();
        const address = getCombinedAddress();

        if (!fullname || !phone || !email || !birthyear || !address) {
            showToast("⚠️ Vui lòng điền đầy đủ thông tin cá nhân và địa chỉ thường trú!", "error");
            return false;
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            showToast("⚠️ Địa chỉ Email không hợp lệ!", "error");
            return false;
        }

        const year = parseInt(birthyear);
        const currentYear = new Date().getFullYear();
        if (isNaN(year) || year < 1920 || year > currentYear - 10) {
            showToast("⚠️ Năm sinh không hợp lệ!", "error");
            return false;
        }

        return true;
    }

    btnNext.addEventListener("click", () => {
        if (currentStep === 0) {
            if (!validateStep1()) return;
        }
        if (currentStep < steps.length - 1) {
            currentStep++;
            renderStep();
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    });

    btnPrev.addEventListener("click", () => {
        if (currentStep > 0) {
            currentStep--;
            renderStep();
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    });

    // Step 2 Interactivity: History Cards
    const historyCards = document.querySelectorAll(".history-card");
    historyCards.forEach(card => {
        card.addEventListener("click", () => {
            historyCards.forEach(c => c.classList.remove("active-card"));
            card.classList.add("active-card");
        });
    });

    // Step 3 Interactivity: House Cards
    const houseCards = document.querySelectorAll(".house-card");
    houseCards.forEach(card => {
        card.addEventListener("click", () => {
            houseCards.forEach(c => c.classList.remove("active-card"));
            card.classList.add("active-card");
        });
    });

    // Step 3 Interactivity: Garden Buttons
    const btnGardenList = document.querySelectorAll(".btn-garden");
    btnGardenList.forEach(btn => {
        btn.addEventListener("click", () => {
            btnGardenList.forEach(b => {
                b.classList.remove("active");
                b.classList.remove("btn-pawsitive-primary");
                b.classList.add("btn-pawsitive-secondary");
            });
            btn.classList.add("active");
            btn.classList.remove("btn-pawsitive-secondary");
            btn.classList.add("btn-pawsitive-primary");
        });
    });

    // Step 4 Confirmation data population
    function populateConfirmationTable() {
        const fullname = document.getElementById("input-fullname").value.trim();
        const phone = document.getElementById("input-phone").value.trim();
        const address = getCombinedAddress();

        // Get Experience text
        const activeHistoryCard = document.querySelector(".history-card.active-card");
        const historyVal = activeHistoryCard ? activeHistoryCard.dataset.history : "never";
        let historyText = "Chưa bao giờ nuôi";
        if (historyVal === "past") historyText = "Đã từng nuôi";
        else if (historyVal === "current") historyText = "Đang nuôi thú cưng";

        const freeTimeText = document.getElementById("select-free-time").options[document.getElementById("select-free-time").selectedIndex].text;
        const familyConsensus = document.getElementById("consensus-yes").checked ? "Được gia đình đồng ý" : "Gia đình chưa hoàn toàn nhất trí";

        // House info
        const activeHouseCard = document.querySelector(".house-card.active-card");
        const houseVal = activeHouseCard ? activeHouseCard.dataset.house : "apartment";
        const houseText = houseVal === "apartment" ? "Căn hộ/Chung cư" : "Nhà phố/Biệt thự";

        const hasGarden = document.querySelector(".btn-garden.active").textContent === "Có sân vườn" ? "có sân vườn" : "không có sân vườn";

        // Render confirm table cells
        const tbody = document.querySelector("#step-4 table tbody");
        if (tbody) {
            tbody.innerHTML = `
                <tr>
                  <td class="font-bold text-dark ps-0" style="width: 130px;">Người đăng ký:</td>
                  <td>${fullname}</td>
                </tr>
                <tr>
                  <td class="font-bold text-dark ps-0">Số điện thoại:</td>
                  <td>${phone}</td>
                </tr>
                <tr>
                  <td class="font-bold text-dark ps-0">Địa chỉ nhà:</td>
                  <td>${address}</td>
                </tr>
                <tr>
                  <td class="font-bold text-dark ps-0">Điều kiện sống:</td>
                  <td>Sống tại ${houseText} (${hasGarden}), ${familyConsensus}.</td>
                </tr>
                <tr>
                  <td class="font-bold text-dark ps-0">Kinh nghiệm:</td>
                  <td>${historyText}, thời gian rảnh rỗi chăm bé: ${freeTimeText}.</td>
                </tr>
            `;
        }
    }

    // Submit adoption registration
    const adoptForm = document.getElementById("adopt-flow-form");
    adoptForm.addEventListener("submit", async (e) => {
        e.preventDefault();

        // Extra checks for commitments in step 4
        if (!document.getElementById("check-commit-1").checked ||
            !document.getElementById("check-commit-2").checked ||
            !document.getElementById("check-commit-3").checked ||
            !document.getElementById("check-commit-all").checked) {
            showToast("⚠️ Bạn cần đánh dấu đồng ý với tất cả cam kết nhận nuôi!", "error");
            return;
        }

        // Gather Payload properties
        const fullname = document.getElementById("input-fullname").value.trim();
        const phone = document.getElementById("input-phone").value.trim();
        const email = document.getElementById("input-email").value.trim();
        const birthyear = parseInt(document.getElementById("input-birthyear").value);
        const address = getCombinedAddress();

        const activeHistoryCard = document.querySelector(".history-card.active-card");
        const historyVal = activeHistoryCard ? activeHistoryCard.dataset.history : "never";
        let historyText = "Chưa bao giờ nuôi";
        if (historyVal === "past") historyText = "Đã từng nuôi";
        else if (historyVal === "current") historyText = "Đang nuôi";

        const freeTimeText = document.getElementById("select-free-time").value === "low" ? "Dưới 2 tiếng" : (document.getElementById("select-free-time").value === "medium" ? "2-5 tiếng" : "Trên 5 tiếng");
        const consensusText = document.getElementById("consensus-yes").checked ? "Đồng ý" : "Không đồng ý";

        const activeHouseCard = document.querySelector(".house-card.active-card");
        const houseVal = activeHouseCard ? activeHouseCard.dataset.house : "apartment";
        const houseText = houseVal === "apartment" ? "Chung cư" : "Nhà đất";

        const hasGarden = document.querySelector(".btn-garden.active").textContent === "Có sân vườn";
        const hasBalconyFence = document.getElementById("fence-balcony").checked;
        const hasGateFence = document.getElementById("fence-gate").checked;

        const payload = {
            maThuCung: petId,
            hoTen: fullname,
            soDienThoai: phone,
            email: email,
            namSinh: birthyear,
            diaChi: address,
            kinhNghiemNuoi: historyText,
            thoiGianRanhMoiNgay: freeTimeText,
            suDongYCoGiaDinh: consensusText,
            loaiNhaO: houseText,
            coSanVuon: hasGarden,
            coLuoiBanCong: hasBalconyFence,
            coHangRaoCao: hasGateFence
        };

        showToast("📤 Đang gửi hồ sơ xét duyệt nhận nuôi...", "info");

        try {
            const res = await fetch("/api/Adoption", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });
            const json = await res.json();

            if (json.success) {
                // Show modal overlay
                const successOverlay = document.getElementById("success-modal-overlay");
                successOverlay.classList.remove("d-none");
                successOverlay.classList.add("d-flex");
                showToast("🎉 Đã gửi đơn đăng ký thành công!", "success");
            } else {
                showToast("❌ Gửi hồ sơ thất bại: " + json.message, "error");
            }
        } catch (err) {
            console.error(err);
            showToast("❌ Lỗi kết nối đến máy chủ", "error");
        }
    });

    // Helper functions: Fetch pet data
    async function fetchPetDetails(id) {
        try {
            const res = await fetch(`/api/pets/${id}`);
            const json = await res.json();

            if (json.success && json.data) {
                const pet = json.data;
                if (pet.trangThaiNhanNuoi === "Đã nhận nuôi") {
                    showToast("⚠️ Bé này đã được nhận nuôi rồi, vui lòng chọn bé khác nhé!", "error");
                    setTimeout(() => {
                        window.location.href = "/Home/Adopt";
                    }, 2000);
                    return;
                }
                const petNameElements = document.querySelectorAll(".pet-name-placeholder");
                petNameElements.forEach(el => el.textContent = pet.tenThuCung);

                // Update thumbnail image card in Step 4
                const summaryDiv = document.querySelector("#step-4 .col-md-4 .p-3");
                if (summaryDiv) {
                    const avatarUrl = pet.anhChinh || 'https://images.unsplash.com/photo-1543466835-00a7907e9de1?auto=format&fit=crop&q=80&w=200';
                    const ageDisplay = pet.giaTriTuoi ? `${pet.giaTriTuoi} ${pet.donViTuoi || 'Tuổi'}` : 'Chưa rõ';
                    const breedDisplay = pet.giongThuCung || 'Lai';

                    summaryDiv.innerHTML = `
                        <div class="border border-2 border-dark rounded-full overflow-hidden mx-auto mb-3" style="width: 100px; height: 100px;">
                          <img src="${avatarUrl}" alt="${pet.tenThuCung}" class="w-100 h-100" style="object-fit: cover;">
                        </div>
                        <h4 class="brand-font h5 text-danger mb-1">${pet.tenThuCung}</h4>
                        <span class="small text-muted d-block mb-3">${breedDisplay} • ${ageDisplay}</span>
                        <span class="badge btn-pawsitive btn-pawsitive-primary d-block mb-2 py-1 justify-content-center">${pet.gioiTinh || 'Chưa rõ'}</span>
                        <span class="badge btn-pawsitive btn-pawsitive-secondary d-block py-1 justify-content-center">${pet.tinhTrangSucKhoe || 'Khỏe mạnh'}</span>
                    `;
                }
            }
        } catch (err) {
            console.error("Failed to load pet details for adoption flow:", err);
        }
    }

    // Initialize layout
    renderStep();
});
