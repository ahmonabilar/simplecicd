(() => {
    const deleteForms = document.querySelectorAll<HTMLFormElement>(".js-delete-employee");

    deleteForms.forEach((form) => {
        form.addEventListener("submit", (event) => {
            const employeeName = form.dataset.employeeName ?? "this employee";
            const confirmed = window.confirm(`Delete ${employeeName}?`);

            if (!confirmed) {
                event.preventDefault();
            }
        });
    });

    const statusMessage = document.querySelector<HTMLElement>(".js-status-message");
    if (statusMessage) {
        window.setTimeout(() => {
            const closeButton = statusMessage.querySelector<HTMLButtonElement>(".btn-close");
            closeButton?.click();
        }, 3500);
    }
})();
