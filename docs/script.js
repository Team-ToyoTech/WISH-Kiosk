// Footer year
document.getElementById("yearNow").textContent = new Date().getFullYear();

// Scoped tabs per container
function initScopedTabs(root = document) {
    root.querySelectorAll(".tabs").forEach((tabs) => {
        const buttons = tabs.querySelectorAll("button[data-tab]");
        buttons.forEach((btn) => {
            btn.addEventListener("click", () => {
                const container = tabs.parentElement || document;
                buttons.forEach((b) => b.classList.remove("active"));
                btn.classList.add("active");
                container
                    .querySelectorAll(".tabpanel")
                    .forEach((p) => p.classList.remove("active"));
                const panel = container.querySelector(
                    "#tab-" + btn.dataset.tab
                );
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
        topButtons.forEach((b) =>
            b.dataset.pane === "server"
                ? b.classList.add("active")
                : b.classList.remove("active")
        );
    } else {
        paneS.classList.remove("active");
        paneK.classList.add("active");
        topButtons.forEach((b) =>
            b.dataset.pane === "kiosk"
                ? b.classList.add("active")
                : b.classList.remove("active")
        );
    }
    updateHeaderLinks(name);
}

function initTopTabs() {
    document.querySelectorAll(".top-tabs button[data-pane]").forEach((btn) => {
        btn.addEventListener("click", () => {
            activatePane(btn.dataset.pane);
            const pane = document.getElementById("pane-" + btn.dataset.pane);
            if (pane)
                pane.scrollIntoView({ behavior: "smooth", block: "start" });
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

function detectInitialPaneFromHash() {
    const hash = window.location.hash;
    if (!hash) return "kiosk";
    const id = hash.slice(1);
    const target = document.getElementById(id);
    const paneS = document.getElementById("pane-server");
    const paneK = document.getElementById("pane-kiosk");
    if (target && paneS && paneS.contains(target)) return "server";
    if (target && paneK && paneK.contains(target)) return "kiosk";
    return "kiosk";
}

window.addEventListener("DOMContentLoaded", () => {
    initScopedTabs();
    initTopTabs();
    initSmartNav();

    // Set initial pane
    const initial = detectInitialPaneFromHash();
    activatePane(initial);

    // README 원문을 그대로 주입하던 로직은 제거(요구사항: “내용에 녹여서”)
    // 이전 버전: loadReadme("readmeContentKiosk", kioskRaw); loadReadme("readmeContentServer", serverRaw);
});
