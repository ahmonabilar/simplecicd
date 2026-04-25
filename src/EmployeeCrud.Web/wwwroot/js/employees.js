(function () {
    var deleteForms = document.querySelectorAll(".js-delete-employee");
    deleteForms.forEach(function (form) {
        form.addEventListener("submit", function (event) {
            var _a;
            var employeeName = (_a = form.dataset.employeeName) !== null && _a !== void 0 ? _a : "this employee";
            var confirmed = window.confirm("Delete ".concat(employeeName, "?"));
            if (!confirmed) {
                event.preventDefault();
            }
        });
    });
    var statusMessage = document.querySelector(".js-status-message");
    if (statusMessage) {
        window.setTimeout(function () {
            var closeButton = statusMessage.querySelector(".btn-close");
            closeButton === null || closeButton === void 0 ? void 0 : closeButton.click();
        }, 3500);
    }
})();
