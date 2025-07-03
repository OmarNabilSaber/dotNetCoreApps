document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.increase-btn').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const productId = this.getAttribute('data-product-id');
            const input = document.getElementById('qty-input-' + productId);
            const errorSpan = document.getElementById('qty-error-' + productId);
            const amount = parseInt(input.value);
            // Clear error
            errorSpan.textContent = '';
            errorSpan.className = '';
            if (isNaN(amount) || amount <= 0) {
                errorSpan.textContent = 'Please enter a valid quantity to increase.';
                errorSpan.className = 'd-block text-danger text-end small ms-2';
                return;
            }
            fetch('./Product/IncreaseQuantity', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: `productId=${productId}&amount=${amount}`
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        document.getElementById('quantity-' + productId).textContent = data.newQuantity;
                        input.value = '';
                        errorSpan.textContent = '';
                        errorSpan.className = '';
                    } else {
                        errorSpan.textContent = data.message;
                        errorSpan.className = 'd-block mt-2 text-danger text-end small ms-2';
                    }
                });
        });
    });
    document.querySelectorAll('.decrease-btn').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const productId = this.getAttribute('data-product-id');
            const input = document.getElementById('qty-input-' + productId);
            const errorSpan = document.getElementById('qty-error-' + productId);
            const amount = parseInt(input.value);
            // Clear error
            errorSpan.textContent = '';
            errorSpan.className = '';
            if (isNaN(amount) || amount <= 0) {
                errorSpan.textContent = 'Please enter a valid quantity to decrease.';
                errorSpan.className = 'd-block text-danger text-end small ms-2';
                return;
            }
            fetch('./Product/DecreaseQuantity', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: `productId=${productId}&amount=${amount}`
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        document.getElementById('quantity-' + productId).textContent = data.newQuantity;
                        input.value = '';
                        errorSpan.textContent = '';
                        errorSpan.className = '';
                    } else {
                        errorSpan.textContent = data.message;
                        errorSpan.className = 'd-block text-danger text-end small ms-2';
                    }
                });
        });
    });
    const searchInput = document.getElementById('product-search');
    if (searchInput) {
        searchInput.addEventListener('input', function () {
            const search = this.value.trim().toLowerCase();
            const rows = document.querySelectorAll('table tbody tr');
            let anyCodeMatch = false;
            rows.forEach(row => {
                const codeCell = row.querySelector('td:nth-child(1)');
                const nameCell = row.querySelector('td:nth-child(2)');
                if (!codeCell || !nameCell) return;
                const code = codeCell.textContent.trim().toLowerCase();
                const name = nameCell.textContent.trim().toLowerCase();
                if (search === "" || code.includes(search)) {
                    row.style.display = '';
                    anyCodeMatch = true;
                } else {
                    row.style.display = 'none';
                }
            });
            if (!anyCodeMatch && search !== "") {
                rows.forEach(row => {
                    const codeCell = row.querySelector('td:nth-child(1)');
                    const nameCell = row.querySelector('td:nth-child(2)');
                    if (!codeCell || !nameCell) return;
                    const name = nameCell.textContent.trim().toLowerCase();
                    if (name.includes(search)) {
                        row.style.display = '';
                    }
                });
            }
        });
    }
    // Hide all qty-error spans when clicking outside the input or error span
    document.addEventListener('click', function (event) {
        // If the click is not on a quantity input or an error span
        if (!event.target.closest('.quantity-input') && !event.target.closest('[id^="qty-error-"]')) {
            document.querySelectorAll('span.text-danger').forEach(function (span) {
                span.textContent = '';
                span.className = '';
            });
        }
    });
});