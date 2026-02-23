// Inventory Manager Pro - Main JavaScript

$(document).ready(function() {
    // Update current time in footer
    function updateTime() {
        const now = new Date();
        $('#currentTime').text(now.toLocaleString());
    }
    updateTime();
    setInterval(updateTime, 1000);

    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Focus search on / key
    $(document).on('keydown', function(e) {
        if (e.key === '/' && !$(e.target).is('input, textarea')) {
            e.preventDefault();
            $('#quickSearch').focus();
        }
    });

    // Confirm before delete
    $('form[data-confirm]').on('submit', function(e) {
        if (!confirm($(this).data('confirm') || 'Are you sure?')) {
            e.preventDefault();
        }
    });

    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Format currency inputs
    $('input[data-currency]').on('blur', function() {
        const value = parseFloat($(this).val()) || 0;
        $(this).val(value.toFixed(2));
    });

    // Prevent double form submission
    $('form').on('submit', function() {
        const $form = $(this);
        if ($form.data('submitted')) {
            return false;
        }
        $form.data('submitted', true);
        $form.find('button[type="submit"]').prop('disabled', true);
    });

    // Auto-select text on focus for quantity/price inputs
    $('input[type="number"]').on('focus', function() {
        $(this).select();
    });

    // Keyboard navigation in tables
    $('table tbody').on('keydown', 'tr', function(e) {
        const $current = $(this);
        if (e.key === 'ArrowDown') {
            e.preventDefault();
            $current.next('tr').focus();
        } else if (e.key === 'ArrowUp') {
            e.preventDefault();
            $current.prev('tr').focus();
        } else if (e.key === 'Enter') {
            $current.find('a').first().click();
        }
    });
});

// Utility functions
function formatCurrency(amount) {
    return '$' + parseFloat(amount).toLocaleString('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

function formatNumber(num) {
    return parseInt(num).toLocaleString('en-US');
}

function showLoading(message = 'Loading...') {
    const overlay = $('<div class="spinner-overlay"><div class="text-center"><div class="spinner-border text-primary" role="status"></div><p class="mt-2">' + message + '</p></div></div>');
    $('body').append(overlay);
}

function hideLoading() {
    $('.spinner-overlay').remove();
}

function showToast(message, type = 'info') {
    const toast = $(`
        <div class="position-fixed bottom-0 end-0 p-3" style="z-index: 9999;">
            <div class="toast show bg-${type} text-white" role="alert">
                <div class="toast-body">
                    ${message}
                    <button type="button" class="btn-close btn-close-white ms-2" data-bs-dismiss="toast"></button>
                </div>
            </div>
        </div>
    `);
    $('body').append(toast);
    setTimeout(() => toast.remove(), 3000);
}

// API helper
async function apiCall(url, method = 'GET', data = null) {
    try {
        const options = {
            method,
            headers: { 'Content-Type': 'application/json' }
        };
        if (data) {
            options.body = JSON.stringify(data);
        }
        const response = await fetch(url, options);
        return await response.json();
    } catch (error) {
        console.error('API Error:', error);
        throw error;
    }
}

// Print table
function printTable(tableId, title) {
    const printWindow = window.open('', '', 'height=600,width=800');
    printWindow.document.write('<html><head><title>' + title + '</title>');
    printWindow.document.write('<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">');
    printWindow.document.write('</head><body class="p-4">');
    printWindow.document.write('<h2>' + title + '</h2>');
    printWindow.document.write('<p>Generated: ' + new Date().toLocaleString() + '</p>');
    printWindow.document.write(document.getElementById(tableId).outerHTML);
    printWindow.document.write('</body></html>');
    printWindow.document.close();
    printWindow.print();
}
