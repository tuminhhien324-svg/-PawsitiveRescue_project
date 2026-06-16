// ==========================================================================
// Pawsitive Rescue – Auth module (Real client-side authentication)
// Stores a lightweight user representation in localStorage for UI convenience.
// ==========================================================================

const PAWSITIVE_AUTH_KEY = "pawsitive_user";

function pawsitiveCurrentUser() {
  try {
    return JSON.parse(localStorage.getItem(PAWSITIVE_AUTH_KEY) || "null");
  } catch {
    return null;
  }
}

function pawsitiveLogout() {
  // Clear local storage representation
  localStorage.removeItem(PAWSITIVE_AUTH_KEY);
  
  // Call backend cookie sign-out endpoint via API
  fetch("/api/Auth/logout", {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    }
  })
  .then(() => {
    // Redirect to login page
    window.location.href = "/Account/Login";
  })
  .catch(err => {
    console.error("Logout request failed:", err);
    window.location.href = "/Account/Login";
  });
}

function pawsitiveGetRedirectUrl(role) {
  if (role === "admin") {
    return "/Admin/Index";
  }
  return "/Home/Index";
}
