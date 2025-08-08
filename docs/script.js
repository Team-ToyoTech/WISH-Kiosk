// Scoped tabs per container
function initScopedTabs(root=document) {
  root.querySelectorAll(".tabs").forEach((tabs) => {
    const buttons = tabs.querySelectorAll("button[data-tab]");
    buttons.forEach((btn) => {
      btn.addEventListener("click", () => {
        const container = tabs.parentElement || document;
        buttons.forEach((b) => b.classList.remove("active"));
        btn.classList.add("active");
        container.querySelectorAll(".tabpanel").forEach((p) => p.classList.remove("active"));
        const panel = container.querySelector("#tab-" + btn.dataset.tab);
        if (panel) panel.classList.add("active");
      });
    });
  });
}

// Toggle header links based on active pane
function updateHeaderLinks(name) {
  const kioskNav = document.querySelector("header .nav-links.kiosk-only");
  const serverNav = document.querySelector("header .nav-links.server-only");
  if (!kioskNav || !serverNav) return;
  if (name === "server") {
    kioskNav.classList.add("hidden");
    serverNav.classList.remove("hidden");
  } else {
    serverNav.classList.add("hidden");
    kioskNav.classList.remove("hidden");
  }
}

// Top-level Kiosk/Server pane switching
function activatePane(name) {
  const paneK = document.getElementById("pane-kiosk");
  const paneS = document.getElementById("pane-server");
  const topButtons = document.querySelectorAll(".top-tabs button[data-pane]");
  if (!paneK || !paneS) return;

  if (name === "server") {
    paneK.classList.remove("active");
    paneS.classList.add("active");
    topButtons.forEach(b => b.dataset.pane === "server" ? b.classList.add("active") : b.classList.remove("active"));
  } else {
    paneS.classList.remove("active");
    paneK.classList.add("active");
    topButtons.forEach(b => b.dataset.pane === "kiosk" ? b.classList.add("active") : b.classList.remove("active"));
  }
  updateHeaderLinks(name);
}

function initTopTabs() {
  document.querySelectorAll(".top-tabs button[data-pane]").forEach((btn) => {
    btn.addEventListener("click", () => {
      activatePane(btn.dataset.pane);
      const pane = document.getElementById("pane-" + btn.dataset.pane);
      if (pane) pane.scrollIntoView({ behavior: "smooth", block: "start" });
    });
  });
}

// Intercept header nav to switch pane if needed
function initSmartNav() {
  document.querySelectorAll("header nav a[href^='#']").forEach((a) => {
    a.addEventListener("click", (e) => {
      const id = a.getAttribute("href").slice(1);
      const target = document.getElementById(id);
      if (!target) return;
      const paneS = document.getElementById("pane-server");
      const paneK = document.getElementById("pane-kiosk");
      const inServer = paneS && paneS.contains(target);
      const inKiosk = paneK && paneK.contains(target);
      if (inServer) {
        activatePane("server");
      } else if (inKiosk) {
        activatePane("kiosk");
      }
      setTimeout(() => {
        target.scrollIntoView({ behavior: "smooth", block: "start" });
      }, 50);
    });
  });
}

// README loaders
async function loadReadme(targetId, url) {
  const box = document.getElementById(targetId);
  if (!box) return;
  try {
    const res = await fetch(url, { cache: "no-store" });
    if (!res.ok) throw new Error("HTTP " + res.status);
    const md = await res.text();
    box.innerHTML = window.marked.parse(md);
  } catch (err) {
    box.innerHTML = '<div class="warn">README를 불러오지 못했습니다. 네트워크 또는 CORS 정책을 확인하세요.</div>';
  }
}

function detectInitialPaneFromHash() {
  const hash = window.location.hash;
  if (!hash) return "kiosk";
  const id = hash.slice(1);
  const target = document.getElementById(id);
  const paneS = document.getElementById("pane-server");
  const paneK = document.getElementById("pane-kiosk");
  if (target && paneS && paneS.contains(target)) return "server";
  if (target && paneK && paneK.contains(target)) return "kiosk";
  // If hash is unknown, default kiosk
  return "kiosk";
}

window.addEventListener("DOMContentLoaded", async () => {
  initScopedTabs();
  initTopTabs();
  initSmartNav();

  // Set initial pane
  const initial = detectInitialPaneFromHash();
  activatePane(initial);

  // Load READMEs
  const kioskRaw = "https://raw.githubusercontent.com/Team-ToyoTech/WISH-Kiosk/master/README.md";
  const serverRaw = "https://raw.githubusercontent.com/Team-ToyoTech/WISH-Server/master/README.md";
  await loadReadme("readmeContentKiosk", kioskRaw);
  await loadReadme("readmeContentServer", serverRaw);
});
