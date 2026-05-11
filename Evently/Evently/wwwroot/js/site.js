// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// ═══════════════════════════════════════
// NOTIFICATION BELL TOGGLE
// Append this block to the bottom of site.js
// ═══════════════════════════════════════

(function () {
    const toggle = document.getElementById('notifToggle');
    const dropdown = document.getElementById('notifDropdown');

    if (!toggle || !dropdown) return;

    // Open / close on bell click
    // Open / close on bell click
    toggle.addEventListener('click', function (e) {
        e.stopPropagation();
        const isOpen = dropdown.classList.toggle('open');
        toggle.setAttribute('aria-expanded', isOpen);

        // ── Clear badge once opened ──
        if (isOpen) {
            const badge = toggle.querySelector('.notif-badge');
            if (badge) badge.remove();
        }
    });

    // Close when clicking anywhere outside
    document.addEventListener('click', function (e) {
        if (!dropdown.contains(e.target) && e.target !== toggle) {
            dropdown.classList.remove('open');
            toggle.setAttribute('aria-expanded', 'false');
        }
    });

    // Close on Escape key
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            dropdown.classList.remove('open');
            toggle.setAttribute('aria-expanded', 'false');
        }
    });
})();