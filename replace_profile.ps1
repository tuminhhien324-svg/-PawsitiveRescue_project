$filePath = "d:\HIENLTWEB\Views\Account\Profile.cshtml"
$content = [System.IO.File]::ReadAllText($filePath, [System.Text.Encoding]::UTF8)

# Normalize line endings to \n to make matching easy
$content = $content -replace "`r`n", "`n"

# 1. Replace menu
$targetMenu = @'
              <a asp-controller="Account" asp-action="Profile" class="profile-menu-item active">
                <i class="bi bi-file-earmark-text-fill"></i> Đơn nhận nuôi
              </a>
              <a
                href="#"
                class="profile-menu-item"
                onclick="
                  showToast('🐾 Chuyển sang danh sách yêu thích...', 'info')
                "
              >
                <i class="bi bi-heart-fill"></i> Thú cưng yêu thích
              </a>
              <a
                href="#"
                class="profile-menu-item"
                onclick="showToast('💳 Xem lịch sử quyên góp...', 'info')"
              >
                <i class="bi bi-calendar-check-fill"></i> Lịch sử quyên góp
              </a>
              <a
                href="#"
                class="profile-menu-item"
                onclick="showToast('⚙️ Mở cài đặt tài khoản...', 'info')"
              >
                <i class="bi bi-gear-fill"></i> Cài đặt tài khoản
              </a>
              <hr class="my-2 border-secondary border-opacity-20" />
              <a asp-controller="Account" asp-action="Login" class="profile-menu-item text-danger">
                <i class="bi bi-box-arrow-right"></i> Đăng xuất
              </a>
'@
$targetMenu = $targetMenu -replace "`r`n", "`n"

$replacementMenu = @'
              <a href="javascript:void(0)" onclick="switchTab('adoptions')" class="profile-menu-item active" id="tab-adoptions">
                <i class="bi bi-file-earmark-text-fill"></i> Đơn nhận nuôi
              </a>
              <a href="javascript:void(0)" onclick="switchTab('favorites')" class="profile-menu-item" id="tab-favorites">
                <i class="bi bi-heart-fill"></i> Thú cưng yêu thích
              </a>
              <a href="javascript:void(0)" onclick="switchTab('donations')" class="profile-menu-item" id="tab-donations">
                <i class="bi bi-calendar-check-fill"></i> Lịch sử quyên góp
              </a>
              <a href="javascript:void(0)" onclick="openSettingsModal()" class="profile-menu-item" id="tab-settings">
                <i class="bi bi-gear-fill"></i> Cài đặt tài khoản
              </a>
              <hr class="my-2 border-secondary border-opacity-20" />
              <a href="javascript:void(0)" onclick="pawsitiveLogout()" class="profile-menu-item text-danger">
                <i class="bi bi-box-arrow-right"></i> Đăng xuất
              </a>
'@
$replacementMenu = $replacementMenu -replace "`r`n", "`n"

if ($content.Contains($targetMenu)) {
    $content = $content.Replace($targetMenu, $replacementMenu)
    Write-Host "Menu replaced."
} else {
    Write-Host "Warning: targetMenu not found."
}

# 2. Replace Bio
$targetBio = @'
                  <div>
                    <div class="d-flex align-items-center gap-2 mb-1">
                      <h2 class="brand-font h4 m-0 text-dark">Minh Quân</h2>
                      <span
                        class="badge badge-pawsitive badge-pawsitive-primary py-1 px-2 text-xs"
                        >Thành viên tích cực</span
                      >
                    </div>
'@
$targetBio = $targetBio -replace "`r`n", "`n"

$replacementBio = @'
                  <div>
                    <div class="d-flex align-items-center gap-2 mb-1">
                      <h2 class="brand-font h4 m-0 text-dark" id="profile-display-name">Minh Quân</h2>
                      <span
                        class="badge badge-pawsitive badge-pawsitive-primary py-1 px-2 text-xs"
                        id="profile-display-rank"
                        >Thành viên tích cực</span
                      >
                    </div>
'@
$replacementBio = $replacementBio -replace "`r`n", "`n"

if ($content.Contains($targetBio)) {
    $content = $content.Replace($targetBio, $replacementBio)
    Write-Host "Bio replaced."
} else {
    Write-Host "Warning: targetBio not found."
}

# 3. Replace Points
$targetPoints = @'
                  <span class="small opacity-80 d-block font-bold"
                    >Điểm Tích Cực</span
                  >
                  <h3
                    class="brand-font display-6 m-0 text-white"
                    style="font-size: 2rem"
                  >
                    1,250
                  </h3>
'@
$targetPoints = $targetPoints -replace "`r`n", "`n"

$replacementPoints = @'
                  <span class="small opacity-80 d-block font-bold"
                    >Điểm Tích Cực</span
                  >
                  <h3
                    class="brand-font display-6 m-0 text-white"
                    style="font-size: 2rem"
                    id="profile-points"
                  >
                    1,250
                  </h3>
'@
$replacementPoints = $replacementPoints -replace "`r`n", "`n"

if ($content.Contains($targetPoints)) {
    $content = $content.Replace($targetPoints, $replacementPoints)
    Write-Host "Points replaced."
} else {
    Write-Host "Warning: targetPoints not found."
}

# 4. Replace lists
$targetLists = @'
          <!-- Adoption application status list -->
          <div class="card-pawsitive p-4 bg-white mb-4">
            <div
              class="d-flex justify-content-between align-items-center mb-4 pb-2 border-bottom border-secondary border-opacity-20"
            >
              <h3 class="brand-font h5 m-0 text-danger">
                Đơn nhận nuôi của tôi
              </h3>
              <a
                href="#"
                class="small text-muted font-bold text-decoration-none"
                onclick="
                  showToast(
                    '🐾 Đang mở danh sách toàn bộ hồ sơ nhận nuôi...',
                    'info',
                  )
                "
                >Xem tất cả</a>
            </div>

            <div class="d-flex flex-column gap-3">
              <!-- App 1 -->
              <div
                class="p-3 border border-2 border-dark rounded-md d-flex align-items-center justify-content-between flex-column flex-sm-row gap-3"
              >
                <div class="d-flex align-items-center gap-3">
                  <div
                    class="border border-2 border-dark rounded-md overflow-hidden d-none d-sm-block"
                    style="width: 54px; height: 54px; flex-shrink: 0"
                  >
                    <img
                      src="https://images.unsplash.com/photo-1587300003388-59208cc962cb?auto=format&fit=crop&q=80&w=100"
                      class="w-100 h-100"
                      style="object-fit: cover"
                    />
                  </div>
                  <div>
                    <h4 class="brand-font fs-6 mb-1">Bánh Mì (Corgi)</h4>
                    <p class="small text-muted m-0">Đang xét duyệt hồ sơ...</p>
                  </div>
                </div>
                <div class="text-sm-end">
                  <span
                    class="badge bg-danger rounded-pill px-3 py-1 text-white d-inline-block font-bold mb-1"
                    >Đang xử lý</span
                  >
                  <span class="d-block small text-muted text-xs"
                    >Cập nhật: 2 ngày trước</span
                  >
                </div>
              </div>

              <!-- App 2 -->
              <div
                class="p-3 border border-2 border-dark rounded-md d-flex align-items-center justify-content-between flex-column flex-sm-row gap-3"
              >
                <div class="d-flex align-items-center gap-3">
                  <div
                    class="border border-2 border-dark rounded-md overflow-hidden d-none d-sm-block"
                    style="width: 54px; height: 54px; flex-shrink: 0"
                  >
                    <img
                      src="https://images.unsplash.com/photo-1514888286974-6c03e2ca1dba?auto=format&fit=crop&q=80&w=100"
                      class="w-100 h-100"
                      style="object-fit: cover"
                    />
                  </div>
                  <div>
                    <h4 class="brand-font fs-6 mb-1">Luna (Mèo Tam Thể)</h4>
                    <p class="small text-muted m-0">
                      Đã đón bé về nhà ngày 15/01/2024
                    </p>
                  </div>
                </div>
                <div class="text-sm-end">
                  <span
                    class="badge bg-primary rounded-pill px-3 py-1 text-white d-inline-block font-bold mb-1"
                    style="
                      background-color: var(--pawsitive-secondary) !important;
                    "
                    >Thành công</span
                  >
                  <a
                    asp-controller="Home" asp-action="Blogs"
                    class="d-block small text-primary fw-bold text-decoration-none text-xs text-danger"
                    >Viết blog về bé</a>
                </div>
              </div>
            </div>
          </div>

          <!-- Liked pets -->
          <div class="mb-4">
            <h3 class="brand-font h5 text-dark mb-3">Thú cưng yêu thích (3)</h3>
            <div class="row g-3">
              <!-- Pet 1 -->
              <div class="col-md-4 col-6">
                <div
                  class="card-pawsitive p-2 bg-white d-flex flex-column text-center"
                >
                  <div
                    class="pet-frame ratio ratio-1x1 border border-2 border-dark rounded-md overflow-hidden mb-2 position-relative"
                  >
                    <img
                      src="https://images.unsplash.com/photo-1537151608828-ea2b117b62e4?auto=format&fit=crop&q=80&w=200"
                    />
                    <span
                      class="position-absolute top-0 end-0 m-1 bg-white text-danger border border-1 border-dark rounded-full d-flex align-items-center justify-content-center"
                      style="width: 28px; height: 28px"
                    >
                      <i class="bi bi-heart-fill" style="font-size: 0.8rem"></i>
                    </span>
                  </div>
                  <h4 class="brand-font fs-6 m-0 text-danger">Khoai Tây</h4>
                  <span class="small text-muted text-xs">Beagle • 2 tuổi</span>
                </div>
              </div>
              <!-- Pet 2 -->
              <div class="col-md-4 col-6">
                <div
                  class="card-pawsitive p-2 bg-white d-flex flex-column text-center"
                >
                  <div
                    class="pet-frame ratio ratio-1x1 border border-2 border-dark rounded-md overflow-hidden mb-2 position-relative"
                  >
                    <img
                      src="https://images.unsplash.com/photo-1533738363-b7f9aef128ce?auto=format&fit=crop&q=80&w=200"
                    />
                    <span
                      class="position-absolute top-0 end-0 m-1 bg-white text-danger border border-1 border-dark rounded-full d-flex align-items-center justify-content-center"
                      style="width: 28px; height: 28px"
                    >
                      <i class="bi bi-heart-fill" style="font-size: 0.8rem"></i>
                    </span>
                  </div>
                  <h4 class="brand-font fs-6 m-0 text-danger">Bông Gòn</h4>
                  <span class="small text-muted text-xs"
                    >Mèo Anh • 6 tháng</span
                  >
                </div>
              </div>
              <!-- Pet 3 -->
              <div class="col-md-4 col-12 d-md-block d-none">
                <div
                  class="card-pawsitive p-2 bg-white d-flex flex-column text-center"
                >
                  <div
                    class="pet-frame ratio ratio-1x1 border border-2 border-dark rounded-md overflow-hidden mb-2 position-relative"
                  >
                    <img
                      src="https://images.unsplash.com/photo-1552053831-71594a27632d?auto=format&fit=crop&q=80&w=200"
                    />
                    <span
                      class="position-absolute top-0 end-0 m-1 bg-white text-danger border border-1 border-dark rounded-full d-flex align-items-center justify-content-center"
                      style="width: 28px; height: 28px"
                    >
                      <i class="bi bi-heart-fill" style="font-size: 0.8rem"></i>
                    </span>
                  </div>
                  <h4 class="brand-font fs-6 m-0 text-danger">Lucky</h4>
                  <span class="small text-muted text-xs">Golden • 1 tuổi</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Recent Activities list -->
          <div class="card-pawsitive p-4 bg-white">
            <h3
              class="brand-font h5 text-dark mb-4 pb-2 border-bottom border-secondary border-opacity-20"
            >
              Hoạt động gần đây
            </h3>

            <div class="d-flex flex-column gap-4 py-2">
              <!-- Act 1 -->
              <div class="activity-pipe">
                <div class="activity-pipe-icon bg-danger border-danger">
                  <i class="bi bi-heart-fill" style="font-size: 0.8rem"></i>
                </div>
                <p class="small text-muted m-0">
                  Bạn đã quyên góp
                  <strong class="text-dark">500.000đ</strong> cho chiến dịch quỹ
                  "Mùa đông không lạnh".
                  <span class="d-block text-xs mt-1">3 ngày trước</span>
                </p>
              </div>

              <!-- Act 2 -->
              <div class="activity-pipe">
                <div
                  class="activity-pipe-icon bg-primary border-primary"
                  style="
                    background-color: var(--pawsitive-secondary) !important;
                    border-color: var(--pawsitive-secondary) !important;
                  "
                >
                  <i class="bi bi-book-half" style="font-size: 0.8rem"></i>
                </div>
                <p class="small text-muted m-0">
                  Đã đọc bài viết tài liệu:
                  <a
                    asp-controller="Home" asp-action="Blogs"
                    class="text-danger fw-bold text-decoration-none"
                    >"Cách chăm sóc chó con khi mới về nhà"</a>.
                  <span class="d-block text-xs mt-1">1 tuần trước</span>
                </p>
              </div>

              <!-- Act 3 -->
              <div class="activity-pipe">
                <div
                  class="activity-pipe-icon bg-warning border-warning"
                  style="
                    background-color: var(--pawsitive-primary) !important;
                    border-color: var(--pawsitive-primary) !important;
                  "
                >
                  <i
                    class="bi bi-chat-left-dots-fill"
                    style="font-size: 0.75rem"
                  ></i>
                </div>
                <p class="small text-muted m-0">
                  Bình luận trên hồ sơ nhận nuôi bé Corgi Bánh Mì:
                  <span class="italic text-dark"
                    >"Bé dễ thương quá, hy vọng sớm được gặp bé!"</span
                  >.
                  <span class="d-block text-xs mt-1">2 tuần trước</span>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>


    <!-- Bootstrap 5 JS Bundle -->
'@
$targetLists = $targetLists -replace "`r`n", "`n"

$replacementLists = @'
          <!-- Adoption application status list -->
          <div class="card-pawsitive p-4 bg-white mb-4 profile-section" id="section-adoptions">
            <div class="d-flex justify-content-between align-items-center mb-4 pb-2 border-bottom border-secondary border-opacity-20">
              <h3 class="brand-font h5 m-0 text-danger">
                Đơn nhận nuôi của tôi
              </h3>
            </div>
            <div class="d-flex flex-column gap-3" id="adoptions-list">
              <!-- Dynamically populated -->
            </div>
          </div>

          <!-- Liked pets -->
          <div class="card-pawsitive p-4 bg-white mb-4 profile-section d-none" id="section-favorites">
            <h3 class="brand-font h5 text-dark mb-3">Thú cưng yêu thích</h3>
            <div class="row g-3" id="favorites-list">
              <!-- Dynamically populated -->
            </div>
          </div>

          <!-- Recent Activities list -->
          <div class="card-pawsitive p-4 bg-white profile-section d-none" id="section-donations">
            <h3 class="brand-font h5 text-dark mb-4 pb-2 border-bottom border-secondary border-opacity-20">
              Lịch sử quyên góp
            </h3>
            <div class="d-flex flex-column gap-4 py-2" id="donations-list">
              <!-- Dynamically populated -->
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Edit Profile Modal -->
    <div class="modal fade" id="editProfileModal" tabindex="-1" aria-labelledby="editProfileModalLabel" aria-hidden="true" style="z-index: 1055;">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content card-pawsitive bg-white p-4" style="border: 3px solid #000; box-shadow: 8px 8px 0px #000;">
          <div class="modal-header border-0 p-0 mb-3">
            <h5 class="modal-title brand-font fs-4 text-danger" id="editProfileModalLabel">⚙️ Cập nhật thông tin</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body p-0">
            <form id="edit-profile-form">
              <div class="form-group-pawsitive mb-3">
                <label for="edit-fullname">Họ và tên</label>
                <input type="text" class="form-control form-control-pawsitive" id="edit-fullname" required />
              </div>
              <div class="form-group-pawsitive mb-3">
                <label for="edit-phone">Số điện thoại</label>
                <input type="text" class="form-control form-control-pawsitive" id="edit-phone" />
              </div>
              <div class="form-group-pawsitive mb-3">
                <label for="edit-birthyear">Năm sinh</label>
                <input type="number" class="form-control form-control-pawsitive" id="edit-birthyear" />
              </div>
              <div class="form-group-pawsitive mb-4">
                <label for="edit-address">Địa chỉ</label>
                <input type="text" class="form-control form-control-pawsitive" id="edit-address" />
              </div>
              <div class="d-flex justify-content-end gap-2">
                <button type="button" class="btn btn-pawsitive btn-pawsitive-secondary rounded-full px-4" data-bs-dismiss="modal">Hủy</button>
                <button type="submit" class="btn btn-pawsitive btn-pawsitive-primary rounded-full px-4 justify-content-center">Lưu thay đổi</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    @section Scripts {
    <script>
      let editModal;

      document.addEventListener("DOMContentLoaded", () => {
        // Auth Guard
        const user = pawsitiveCurrentUser();
        if (!user) {
          window.location.href = "/Account/Login?ReturnUrl=" + encodeURIComponent(window.location.pathname);
          return;
        }

        // Initialize Edit Modal
        editModal = new bootstrap.Modal(document.getElementById('editProfileModal'));

        // Load Dashboard Data
        loadDashboard();

        // Handle settings form submit
        document.getElementById("edit-profile-form").addEventListener("submit", updateProfile);
      });

      function loadDashboard() {
        showToast("🔄 Đang tải thông tin cá nhân...", "info");
        fetch("/api/users/me/dashboard")
          .then(res => res.json())
          .then(res => {
            if (res.success) {
              const data = res.data;
              bindProfile(data.profile);
              bindAdoptions(data.adoptions);
              bindFavorites(data.favorites);
              bindDonations(data.donations);
              showToast("✔️ Tải dữ liệu thành công!", "success");
            } else {
              showToast("❌ Không thể tải dữ liệu: " + res.message, "error");
            }
          })
          .catch(err => {
            console.error(err);
            showToast("❌ Lỗi kết nối đến máy chủ", "error");
          });
      }

      function bindProfile(profile) {
        document.getElementById("profile-display-name").textContent = profile.hoTen;
        document.getElementById("profile-display-rank").textContent = profile.tenHang || "Thành viên mới";
        document.getElementById("profile-points").textContent = profile.diemTichLuy.toLocaleString();
        
        // Save values in edit form inputs
        document.getElementById("edit-fullname").value = profile.hoTen || "";
        document.getElementById("edit-phone").value = profile.soDienThoai || "";
        document.getElementById("edit-birthyear").value = profile.namSinh || "";
        document.getElementById("edit-address").value = profile.diaChi || "";
      }

      function bindAdoptions(adoptions) {
        const container = document.getElementById("adoptions-list");
        container.innerHTML = "";
        
        if (!adoptions || adoptions.length === 0) {
          container.innerHTML = `
            <div class="text-center p-4 border border-2 border-dark rounded-md bg-light">
              <span class="fs-2 mb-2 d-block">🐾</span>
              <p class="m-0 small text-muted">Bạn chưa gửi đơn nhận nuôi nào.</p>
              <a href="/Home/Adopt" class="btn btn-pawsitive btn-pawsitive-primary rounded-full mt-2 py-1 small">Tìm thú cưng</a>
            </div>
          `;
          return;
        }

        adoptions.forEach(app => {
          let statusBadge = "";
          if (app.trangThaiDon === "Đang xử lý") {
            statusBadge = `<span class="badge bg-warning rounded-pill px-3 py-1 text-dark font-bold mb-1">Đang xử lý</span>`;
          } else if (app.trangThaiDon === "Đã duyệt" || app.trangThaiDon === "Thành công" || app.trangThaiDon === "Hoàn tất") {
            statusBadge = `<span class="badge bg-success rounded-pill px-3 py-1 text-white font-bold mb-1">Thành công</span>`;
          } else {
            statusBadge = `<span class="badge bg-danger rounded-pill px-3 py-1 text-white font-bold mb-1">${app.trangThaiDon}</span>`;
          }

          const petImg = app.thuCung.anhChinh || "https://images.unsplash.com/photo-1543466835-00a7907e9de1?auto=format&fit=crop&q=80&w=100";
          const dateStr = new Date(app.ngayTao).toLocaleDateString("vi-VN");

          container.innerHTML += `
            <div class="p-3 border border-2 border-dark rounded-md d-flex align-items-center justify-content-between flex-column flex-sm-row gap-3">
              <div class="d-flex align-items-center gap-3">
                <div class="border border-2 border-dark rounded-md overflow-hidden" style="width: 54px; height: 54px; flex-shrink: 0">
                  <img src="${petImg}" class="w-100 h-100" style="object-fit: cover" />
                </div>
                <div>
                  <h4 class="brand-font fs-6 mb-1">${app.thuCung.tenThuCung} (${app.thuCung.giongThuCung || "Ngoại lai"})</h4>
                  <p class="small text-muted m-0">Đơn gửi ngày: ${dateStr}</p>
                </div>
              </div>
              <div class="text-sm-end">
                ${statusBadge}
                <span class="d-block small text-muted text-xs">Cập nhật: ${new Date(app.ngayCapNhat).toLocaleDateString("vi-VN")}</span>
              </div>
            </div>
          `;
        });
      }

      function bindFavorites(favorites) {
        const container = document.getElementById("favorites-list");
        container.innerHTML = "";

        if (!favorites || favorites.length === 0) {
          container.innerHTML = `
            <div class="col-12 text-center p-4 border border-2 border-dark rounded-md bg-light">
              <span class="fs-2 mb-2 d-block">❤️</span>
              <p class="m-0 small text-muted">Danh sách yêu thích trống.</p>
            </div>
          `;
          return;
        }

        favorites.forEach(pet => {
          const petImg = pet.anhChinh || "https://images.unsplash.com/photo-1543466835-00a7907e9de1?auto=format&fit=crop&q=80&w=200";
          container.innerHTML += `
            <div class="col-md-4 col-6">
              <div class="card-pawsitive p-2 bg-white d-flex flex-column text-center h-100">
                <div class="pet-frame ratio ratio-1x1 border border-2 border-dark rounded-md overflow-hidden mb-2 position-relative">
                  <img src="${petImg}" style="object-fit: cover;" />
                  <span class="position-absolute top-0 end-0 m-1 bg-white text-danger border border-1 border-dark rounded-full d-flex align-items-center justify-content-center cursor-pointer" style="width: 28px; height: 28px;" onclick="removeFavorite(${pet.maThuCung})">
                    <i class="bi bi-heart-fill" style="font-size: 0.8rem"></i>
                  </span>
                </div>
                <h4 class="brand-font fs-6 m-0 text-danger">${pet.tenThuCung}</h4>
                <span class="small text-muted text-xs">${pet.giongThuCung || "Ngoại lai"} • ${pet.giaTriTuoi} ${pet.donViTuoi}</span>
              </div>
            </div>
          `;
        });
      }

      function bindDonations(donations) {
        const container = document.getElementById("donations-list");
        container.innerHTML = "";

        if (!donations || donations.length === 0) {
          container.innerHTML = `
            <div class="text-center p-4 border border-2 border-dark rounded-md bg-light">
              <span class="fs-2 mb-2 d-block">💳</span>
              <p class="m-0 small text-muted">Bạn chưa thực hiện giao dịch quyên góp nào.</p>
            </div>
          `;
          return;
        }

        donations.forEach(don => {
          const dateStr = new Date(don.ngayQuyenGop).toLocaleDateString("vi-VN");
          container.innerHTML += `
            <div class="activity-pipe">
              <div class="activity-pipe-icon bg-danger border-danger">
                <i class="bi bi-heart-fill" style="font-size: 0.8rem"></i>
              </div>
              <p class="small text-muted m-0">
                Bạn đã quyên góp
                <strong class="text-dark">${don.soTien.toLocaleString("vi-VN")}đ</strong> cho quỹ
                <strong>${don.tenQuyQuyenGop}</strong>.
                ${don.loiNhan ? `<span class="d-block italic text-dark mt-1">"${don.loiNhan}"</span>` : ""}
                <span class="d-block text-xs mt-1">Ngày: ${dateStr}</span>
              </p>
            </div>
          `;
        });
      }

      function removeFavorite(petId) {
        showToast("💔 Đang bỏ yêu thích...", "info");
        fetch(`/api/pets/${petId}/favorite`, { method: "POST" })
          .then(res => res.json())
          .then(res => {
            if (res.success) {
              showToast("💔 Đã bỏ bé khỏi mục yêu thích!", "success");
              loadDashboard();
            } else {
              showToast("❌ Lỗi: " + res.message, "error");
            }
          })
          .catch(err => console.error(err));
      }

      function switchTab(tabName) {
        // Switch tab classes
        document.querySelectorAll(".profile-menu-item").forEach(item => item.classList.remove("active"));
        document.getElementById(`tab-${tabName}`).classList.add("active");

        // Toggle sections
        document.querySelectorAll(".profile-section").forEach(sec => sec.classList.add("d-none"));
        document.getElementById(`section-${tabName}`).classList.remove("d-none");
      }

      function openSettingsModal() {
        editModal.show();
      }

      function updateProfile(e) {
        e.preventDefault();
        const fullname = document.getElementById("edit-fullname").value.trim();
        const phone = document.getElementById("edit-phone").value.trim();
        const birthyear = document.getElementById("edit-birthyear").value ? parseInt(document.getElementById("edit-birthyear").value) : null;
        const address = document.getElementById("edit-address").value.trim();

        showToast("💾 Đang cập nhật thông tin cá nhân...", "info");

        fetch("/api/users/me/profile", {
          method: "PUT",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            hoTen: fullname,
            soDienThoai: phone,
            namSinh: birthyear,
            diaChi: address
          })
        })
        .then(res => res.json())
        .then(res => {
          if (res.success) {
            showToast("💾 Cập nhật thông tin thành công!", "success");
            editModal.hide();
            loadDashboard();
          } else {
            showToast("❌ Cập nhật thất bại: " + res.message, "error");
          }
        })
        .catch(err => {
          console.error(err);
          showToast("❌ Lỗi kết nối đến máy chủ", "error");
        });
      }
    </script>
    }
'@
$replacementLists = $replacementLists -replace "`r`n", "`n"

if ($content.Contains($targetLists)) {
    $content = $content.Replace($targetLists, $replacementLists)
    Write-Host "Lists replaced."
} else {
    # Fallback to subparts if some formatting spaces differed slightly
    Write-Host "Warning: targetLists not found."
}

# Write back to file in UTF-8
[System.IO.File]::WriteAllText($filePath, $content, [System.Text.Encoding]::UTF8)
Write-Host "Successfully modified Profile.cshtml."
