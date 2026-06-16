// Pawsitive Rescue – Global Site JavaScript (site.js)

// Globally accessible self-contained Toast notification function
window.showToast = function(message, type = "success") {
    let container = document.getElementById("pawsitive-toast-container");
    if (!container) {
        container = document.createElement("div");
        container.id = "pawsitive-toast-container";
        container.style.position = "fixed";
        container.style.bottom = "20px";
        container.style.right = "20px";
        container.style.zIndex = "9999";
        container.style.display = "flex";
        container.style.flexDirection = "column";
        container.style.gap = "10px";
        document.body.appendChild(container);
    }
    
    const toast = document.createElement("div");
    toast.className = `pawsitive-toast pawsitive-toast-${type} animate-bounce-in`;
    toast.style.padding = "12px 24px";
    toast.style.borderRadius = "12px";
    toast.style.border = "2px solid #000";
    
    // Brand Color Palette matching styles.css (Neobrutalism)
    if (type === "success") {
        toast.style.background = "#d1e7dd";
        toast.style.color = "#0f5132";
    } else if (type === "error" || type === "danger") {
        toast.style.background = "#f8d7da";
        toast.style.color = "#842029";
    } else if (type === "warning") {
        toast.style.background = "#fff3cd";
        toast.style.color = "#664d03";
    } else { // info
        toast.style.background = "#cff4fc";
        toast.style.color = "#055160";
    }
    
    toast.style.fontWeight = "700";
    toast.style.fontFamily = "var(--font-body), sans-serif";
    toast.style.fontSize = "0.9rem";
    toast.style.boxShadow = "4px 4px 0px #000";
    toast.style.transition = "all 0.3s cubic-bezier(0.25, 1, 0.5, 1)";
    toast.style.opacity = "0";
    toast.style.transform = "translateY(20px)";
    toast.textContent = message;
    
    container.appendChild(toast);
    
    // Micro-animation trigger
    requestAnimationFrame(() => {
        toast.style.opacity = "1";
        toast.style.transform = "translateY(0)";
    });
    
    // Automatically dismiss after 3.5 seconds
    setTimeout(() => {
        toast.style.opacity = "0";
        toast.style.transform = "translateY(-20px)";
        setTimeout(() => toast.remove(), 300);
    }, 3500);
};

// Dynamic navbar updates based on role
document.addEventListener("DOMContentLoaded", () => {
    // Sync client-side authentication representation with server session state
    fetch("/api/Auth/me")
      .then(res => res.json())
      .then(res => {
        const localUser = typeof pawsitiveCurrentUser === "function" ? pawsitiveCurrentUser() : null;
        if (res.success) {
          const serverUser = res.data;
          // If no local representation, or email/role has changed, sync and refresh
          const localEmail = (localUser && localUser.email) ? localUser.email.toLowerCase().trim() : "";
          const serverEmail = (serverUser && serverUser.email) ? serverUser.email.toLowerCase().trim() : "";
          
          if (!localUser || localEmail !== serverEmail || localUser.role !== serverUser.role) {
            localStorage.setItem("pawsitive_user", JSON.stringify(serverUser));
            window.location.reload();
          }
        } else {
          // Server session is lost/unauthenticated. Clear local state.
          if (localUser) {
            localStorage.removeItem("pawsitive_user");
            window.location.reload();
          }
        }
      })
      .catch(err => {
        console.error("Auth sync check failed:", err);
      });

    const user = typeof pawsitiveCurrentUser === "function" ? pawsitiveCurrentUser() : null;
    if (user) {
        if (user.role === "volunteer") {
            const links = document.querySelectorAll("a.pawsitive-nav-link, a.nav-link");
            links.forEach(link => {
                const text = link.textContent.trim();
                const href = link.getAttribute("href");
                if (text === "Tình nguyện" || href === "/Home/Volunteer" || href === "/Home/Volunteer/") {
                    link.setAttribute("href", "/Account/Profile");
                    link.addEventListener("click", () => {
                        sessionStorage.setItem("active_profile_tab", "volunteer-tasks");
                    });
                }
            });
        } else if (user.role === "admin") {
            // Ensure Admin link is present in the header navbar
            const navbarNav = document.querySelector(".navbar-nav");
            if (navbarNav && !navbarNav.querySelector('a[href*="/Admin"]')) {
                const profileLink = navbarNav.querySelector('a[href*="/Account/Profile"]');
                const adminLi = document.createElement("li");
                adminLi.className = "nav-item";
                adminLi.innerHTML = `
                    <a class="pawsitive-nav-link text-primary font-bold" href="/Admin/Index">
                        <i class="bi bi-speedometer2"></i> Quản trị
                    </a>
                `;
                if (profileLink && profileLink.parentElement) {
                    navbarNav.insertBefore(adminLi, profileLink.parentElement);
                } else {
                    navbarNav.appendChild(adminLi);
                }
            }
        }
    }
});
