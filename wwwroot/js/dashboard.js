// dashboard.js - Common Admin Dashboard Utilities
console.log("Pawsitive Rescue dashboard.js initialized");

// Mobile Sidebar Toggle
document.addEventListener('DOMContentLoaded', () => {
    const mobileToggle = document.getElementById('dbMobileToggle');
    const sidebar = document.getElementById('dbSidebar');
    const overlay = document.getElementById('dbOverlay');

    if (mobileToggle && sidebar && overlay) {
        mobileToggle.addEventListener('click', () => {
            sidebar.classList.add('show');
            overlay.classList.add('show');
        });

        overlay.addEventListener('click', () => {
            sidebar.classList.remove('show');
            overlay.classList.remove('show');
        });
    }

    // Topbar Icons Action Bindings
    const bellIcon = document.querySelector('.db-topbar .bi-bell-fill')?.closest('.db-topbar-icon') || document.querySelector('.db-topbar-icon .bi-bell-fill')?.parentElement;
    const gearIcon = document.querySelector('.db-topbar .bi-gear-fill')?.closest('.db-topbar-icon') || document.querySelector('.db-topbar-icon .bi-gear-fill')?.parentElement;
    const avatarIcon = document.querySelector('.db-topbar-avatar');

    if (bellIcon) {
        bellIcon.style.cursor = 'pointer';
        bellIcon.addEventListener('click', (e) => {
            e.preventDefault();
            window.dbToast("🔔 Không có thông báo mới nào.");
        });
    }

    if (gearIcon) {
        gearIcon.style.cursor = 'pointer';
        gearIcon.addEventListener('click', (e) => {
            e.preventDefault();
            window.dbToast("⚙️ Chức năng cấu hình hệ thống đang được phát triển.");
        });
    }

    if (avatarIcon) {
        avatarIcon.style.cursor = 'pointer';
        avatarIcon.addEventListener('click', (e) => {
            e.preventDefault();
            window.location.href = "/Account/Profile";
        });
    }
});

// Toast notification helper
window.dbToast = function(message) {
    const toast = document.getElementById('dbToast');
    if (toast) {
        toast.textContent = message;
        toast.classList.add('show');
        setTimeout(() => {
            toast.classList.remove('show');
        }, 3000);
    } else {
        alert(message);
    }
};
