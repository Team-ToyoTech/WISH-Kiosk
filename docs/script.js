// simple tabs
document.querySelectorAll(".tabs button").forEach((btn) => {
    btn.addEventListener("click", () => {
        document
            .querySelectorAll(".tabs button")
            .forEach((b) => b.classList.remove("active"));
        document
            .querySelectorAll(".tabpanel")
            .forEach((p) => p.classList.remove("active"));
        btn.classList.add("active");
        const id = btn.dataset.tab;
        const panel = document.getElementById("tab-" + id);
        if (panel) panel.classList.add("active");
    });
});

// fetch README and render via marked
window.addEventListener("DOMContentLoaded", async () => {
    const box = document.getElementById("readmeContent");
    try {
        const url =
            "https://raw.githubusercontent.com/Team-ToyoTech/WISH-Kiosk/master/README.md";
        const res = await fetch(url, { cache: "no-store" });
        if (!res.ok) throw new Error("HTTP " + res.status);
        const md = await res.text();
        box.innerHTML = window.marked.parse(md);
    } catch (err) {
        box.innerHTML =
            '<div class="warn">README를 불러오지 못했습니다. 네트워크 또는 CORS 정책을 확인하세요.</div>';
    }
});
