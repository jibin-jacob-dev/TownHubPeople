document.addEventListener('DOMContentLoaded', function () {
    var wrapper = document.getElementById('wrapper');
    var sidebarToggle = document.getElementById('sidebarToggle');
    var overlay = document.getElementById('sidebar-overlay');

    var sidebar = document.getElementById('sidebar-wrapper');

    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function (e) {
            e.preventDefault();
            var isMobile = window.innerWidth < 992;
            if (isMobile) {
                wrapper.classList.toggle('toggled');
            } else {
                var content = document.getElementById('page-content-wrapper');
                if (content) {
                    content.style.opacity = '0.97';
                    setTimeout(function () { content.style.opacity = '1'; }, 150);
                }
                var isCollapsing = !sidebar.classList.contains('collapsed');
                sidebar.classList.toggle('collapsed', isCollapsing);
                localStorage.setItem('sidebarCollapsed', isCollapsing ? 'true' : 'false');
            }
        });
    }

    if (overlay) {
        overlay.addEventListener('click', function () {
            wrapper.classList.remove('toggled');
        });
    }

    // Profile dropdown
    var profileDropdown = document.querySelector('.profile-dropdown');
    var profileToggle = document.querySelector('.profile-toggle');
    if (profileToggle) {
        profileToggle.addEventListener('click', function (e) {
            e.stopPropagation();
            profileDropdown.classList.toggle('open');
        });
        document.addEventListener('click', function (e) {
            if (!profileDropdown.contains(e.target)) {
                profileDropdown.classList.remove('open');
            }
        });
    }

    // Theme toggle
    var themeToggle = document.getElementById('themeToggle');
    var themeToggleRow = document.getElementById('themeToggleRow');
    var themeLabel = document.getElementById('themeLabel');

    function setTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        document.documentElement.setAttribute('data-bs-theme', theme);
        localStorage.setItem('theme', theme);
        if (themeToggle) themeToggle.checked = (theme === 'dark');
        if (themeLabel) themeLabel.textContent = theme === 'dark' ? 'Light Mode' : 'Dark Mode';
    }

    if (themeToggle) {
        var currentTheme = document.documentElement.getAttribute('data-theme') || 'light';
        themeToggle.checked = (currentTheme === 'dark');
        if (themeLabel) themeLabel.textContent = currentTheme === 'dark' ? 'Light Mode' : 'Dark Mode';

        themeToggle.addEventListener('change', function () {
            setTheme(this.checked ? 'dark' : 'light');
        });
    }

    if (themeToggleRow) {
        themeToggleRow.addEventListener('click', function (e) {
            if (e.target === themeToggle || e.target.closest('.theme-switch')) return;
            e.preventDefault();
            e.stopPropagation();
            themeToggle.checked = !themeToggle.checked;
            setTheme(themeToggle.checked ? 'dark' : 'light');
        });
    }

    // Toast notifications from TempData
    var notificationEl = document.getElementById('notification-data');
    if (notificationEl) {
        var data = JSON.parse(notificationEl.dataset.notification);
        toastr.options = {
            closeButton: true,
            progressBar: true,
            positionClass: 'toast-top-right',
            timeOut: 5000
        };
        toastr[data.type](data.message);
    }

    // Global search palette
    var searchOverlay = document.getElementById('search-overlay');
    var searchInput = document.getElementById('searchInput');
    var searchResults = document.getElementById('searchResults');
    var searchToggle = document.getElementById('searchToggle');
    var activeIndex = -1;

    var searchItems = [
        { title: 'Dashboard', path: 'Home', url: '/', icon: 'fa-tachometer-alt', color: '#60a5fa', group: 'Modules' },
        { title: 'Inventory', path: 'Module', url: '/Inventory/Items', icon: 'fa-warehouse', color: '#34d399', group: 'Modules' },
        { title: 'Finance', path: 'Module', url: '/Finance/ChartOfAccounts', icon: 'fa-coins', color: '#fbbf24', group: 'Modules' },
        { title: 'Purchasing', path: 'Module', url: '/Purchasing/Vendors', icon: 'fa-shopping-cart', color: '#fb923c', group: 'Modules' },
        { title: 'Sales', path: 'Module', url: '/Sales/Customers', icon: 'fa-chart-line', color: '#a78bfa', group: 'Modules' },
        { title: 'Manufacturing', path: 'Module', url: '/Manufacturing/WorkCenters', icon: 'fa-cogs', color: '#fb7185', group: 'Modules' },
        { title: 'HR & Payroll', path: 'Module', url: '/HR/Employees', icon: 'fa-users', color: '#38bdf8', group: 'Modules' },
        { title: 'Chart of Accounts', path: 'Finance › Chart of Accounts', url: '/Finance/ChartOfAccounts', icon: 'fa-sitemap', color: '#3b82f6', group: 'Finance' },
        { title: 'Fiscal Years', path: 'Finance › Fiscal Years', url: '/Finance/FiscalYears', icon: 'fa-calendar-alt', color: '#8b5cf6', group: 'Finance' },
        { title: 'Journal Entries', path: 'Finance › Journal Entries', url: '/Finance/JournalEntries', icon: 'fa-book', color: '#10b981', group: 'Finance' },
        { title: 'Reports', path: 'Finance › Reports', url: '/Finance/Reports', icon: 'fa-chart-bar', color: '#f59e0b', group: 'Finance' },
        { title: 'Trial Balance', path: 'Finance › Reports › Trial Balance', url: '/Finance/Reports/TrialBalance', icon: 'fa-balance-scale', color: '#06b6d4', group: 'Finance' },
        { title: 'General Ledger', path: 'Finance › Reports › General Ledger', url: '/Finance/Reports/GeneralLedger', icon: 'fa-book-open', color: '#8b5cf6', group: 'Finance' },
        { title: 'Balance Sheet', path: 'Finance › Reports › Balance Sheet', url: '/Finance/Reports/BalanceSheet', icon: 'fa-file-invoice-dollar', color: '#10b981', group: 'Finance' },
        { title: 'Income Statement', path: 'Finance › Reports › Income Statement', url: '/Finance/Reports/IncomeStatement', icon: 'fa-chart-pie', color: '#ef4444', group: 'Finance' },
        { title: 'Items', path: 'Inventory › Items', url: '/Inventory/Items', icon: 'fa-box', color: '#3b82f6', group: 'Inventory' },
        { title: 'Categories', path: 'Inventory › Categories', url: '/Inventory/ItemCategories', icon: 'fa-tags', color: '#8b5cf6', group: 'Inventory' },
        { title: 'Units of Measure', path: 'Inventory › Units of Measure', url: '/Inventory/UnitsOfMeasure', icon: 'fa-ruler', color: '#06b6d4', group: 'Inventory' },
        { title: 'Warehouses', path: 'Inventory › Warehouses', url: '/Inventory/Warehouses', icon: 'fa-building', color: '#f59e0b', group: 'Inventory' },
        { title: 'Stock Adjustments', path: 'Inventory › Stock Adjustments', url: '/Inventory/StockAdjustments', icon: 'fa-sliders-h', color: '#10b981', group: 'Inventory' },
        { title: 'Stock Transfers', path: 'Inventory › Stock Transfers', url: '/Inventory/StockTransfers', icon: 'fa-exchange-alt', color: '#ef4444', group: 'Inventory' },
        { title: 'Vendors', path: 'Purchasing › Vendors', url: '/Purchasing/Vendors', icon: 'fa-address-book', color: '#fb923c', group: 'Purchasing' },
        { title: 'Purchase Orders', path: 'Purchasing › Purchase Orders', url: '/Purchasing/PurchaseOrders', icon: 'fa-file-alt', color: '#3b82f6', group: 'Purchasing' },
        { title: 'Goods Receipts', path: 'Purchasing › Goods Receipts', url: '/Purchasing/GoodsReceiptNotes', icon: 'fa-truck-loading', color: '#10b981', group: 'Purchasing' },
        { title: 'Purchase Returns', path: 'Purchasing › Purchase Returns', url: '/Purchasing/PurchaseReturns', icon: 'fa-undo-alt', color: '#ef4444', group: 'Purchasing' },
        { title: 'Customers', path: 'Sales › Customers', url: '/Sales/Customers', icon: 'fa-users', color: '#fb923c', group: 'Sales' },
        { title: 'Quotations', path: 'Sales › Quotations', url: '/Sales/Quotations', icon: 'fa-file-invoice', color: '#8b5cf6', group: 'Sales' },
        { title: 'Sales Orders', path: 'Sales › Sales Orders', url: '/Sales/SalesOrders', icon: 'fa-file-alt', color: '#3b82f6', group: 'Sales' },
        { title: 'Delivery Notes', path: 'Sales › Delivery Notes', url: '/Sales/DeliveryNotes', icon: 'fa-truck', color: '#10b981', group: 'Sales' },
        { title: 'Sales Invoices', path: 'Sales › Sales Invoices', url: '/Sales/SalesInvoices', icon: 'fa-file-invoice-dollar', color: '#f59e0b', group: 'Sales' },
        { title: 'Sales Returns', path: 'Sales › Sales Returns', url: '/Sales/SalesReturns', icon: 'fa-undo-alt', color: '#ef4444', group: 'Sales' },
        { title: 'Credit Notes', path: 'Sales › Credit Notes', url: '/Sales/CreditNotes', icon: 'fa-receipt', color: '#06b6d4', group: 'Sales' },
        { title: 'New Account', path: 'Finance › Chart of Accounts › New', url: '/Finance/ChartOfAccounts/Create', icon: 'fa-plus', color: '#3b82f6', group: 'Actions' },
        { title: 'New Fiscal Year', path: 'Finance › Fiscal Years › New', url: '/Finance/FiscalYears/Create', icon: 'fa-plus', color: '#8b5cf6', group: 'Actions' },
        { title: 'New Journal Entry', path: 'Finance › Journal Entries › New', url: '/Finance/JournalEntries/Create', icon: 'fa-plus', color: '#10b981', group: 'Actions' },
        { title: 'Create Item', path: 'Inventory › Items › Create', url: '/Inventory/Items/Create', icon: 'fa-plus', color: '#3b82f6', group: 'Actions' },
        { title: 'Create Category', path: 'Inventory › Categories › Create', url: '/Inventory/ItemCategories/Create', icon: 'fa-plus', color: '#8b5cf6', group: 'Actions' },
        { title: 'Create Unit of Measure', path: 'Inventory › Units of Measure › Create', url: '/Inventory/UnitsOfMeasure/Create', icon: 'fa-plus', color: '#06b6d4', group: 'Actions' },
        { title: 'Create Warehouse', path: 'Inventory › Warehouses › Create', url: '/Inventory/Warehouses/Create', icon: 'fa-plus', color: '#f59e0b', group: 'Actions' },
        { title: 'New Stock Adjustment', path: 'Inventory › Stock Adjustments › New', url: '/Inventory/StockAdjustments/Create', icon: 'fa-plus', color: '#10b981', group: 'Actions' },
        { title: 'New Stock Transfer', path: 'Inventory › Stock Transfers › New', url: '/Inventory/StockTransfers/Create', icon: 'fa-plus', color: '#ef4444', group: 'Actions' },
        { title: 'New Vendor', path: 'Purchasing › Vendors › New', url: '/Purchasing/Vendors/Create', icon: 'fa-plus', color: '#fb923c', group: 'Actions' },
        { title: 'New Purchase Order', path: 'Purchasing › Purchase Orders › New', url: '/Purchasing/PurchaseOrders/Create', icon: 'fa-plus', color: '#3b82f6', group: 'Actions' },
        { title: 'New Goods Receipt', path: 'Purchasing › Goods Receipts › New', url: '/Purchasing/GoodsReceiptNotes/Create', icon: 'fa-plus', color: '#10b981', group: 'Actions' },
        { title: 'New Purchase Return', path: 'Purchasing › Purchase Returns › New', url: '/Purchasing/PurchaseReturns/Create', icon: 'fa-plus', color: '#ef4444', group: 'Actions' },
        { title: 'New Customer', path: 'Sales › Customers › New', url: '/Sales/Customers/Create', icon: 'fa-plus', color: '#fb923c', group: 'Actions' },
        { title: 'New Quotation', path: 'Sales › Quotations › New', url: '/Sales/Quotations/Create', icon: 'fa-plus', color: '#8b5cf6', group: 'Actions' },
        { title: 'New Sales Order', path: 'Sales › Sales Orders › New', url: '/Sales/SalesOrders/Create', icon: 'fa-plus', color: '#3b82f6', group: 'Actions' },
        { title: 'New Delivery Note', path: 'Sales › Delivery Notes › New', url: '/Sales/DeliveryNotes/Create', icon: 'fa-plus', color: '#10b981', group: 'Actions' },
        { title: 'New Sales Invoice', path: 'Sales › Sales Invoices › New', url: '/Sales/SalesInvoices/Create', icon: 'fa-plus', color: '#f59e0b', group: 'Actions' },
        { title: 'New Sales Return', path: 'Sales › Sales Returns › New', url: '/Sales/SalesReturns/Create', icon: 'fa-plus', color: '#ef4444', group: 'Actions' },
        { title: 'New Credit Note', path: 'Sales › Credit Notes › New', url: '/Sales/CreditNotes/Create', icon: 'fa-plus', color: '#06b6d4', group: 'Actions' },
        { title: 'Work Centers', path: 'Manufacturing › Work Centers', url: '/Manufacturing/WorkCenters', icon: 'fa-industry', color: '#f59e0b', group: 'Manufacturing' },
        { title: 'Bills of Materials', path: 'Manufacturing › Bills of Materials', url: '/Manufacturing/BillOfMaterials', icon: 'fa-sitemap', color: '#8b5cf6', group: 'Manufacturing' },
        { title: 'Work Orders', path: 'Manufacturing › Work Orders', url: '/Manufacturing/WorkOrders', icon: 'fa-clipboard-list', color: '#3b82f6', group: 'Manufacturing' },
        { title: 'Material Requisitions', path: 'Manufacturing › Material Requisitions', url: '/Manufacturing/MaterialRequisitions', icon: 'fa-dolly', color: '#10b981', group: 'Manufacturing' },
        { title: 'Production Entries', path: 'Manufacturing › Production Entries', url: '/Manufacturing/ProductionEntries', icon: 'fa-box-open', color: '#ef4444', group: 'Manufacturing' },
        { title: 'New Work Center', path: 'Manufacturing › Work Centers › New', url: '/Manufacturing/WorkCenters/Create', icon: 'fa-plus', color: '#f59e0b', group: 'Actions' },
        { title: 'New Bill of Materials', path: 'Manufacturing › Bills of Materials › New', url: '/Manufacturing/BillOfMaterials/Create', icon: 'fa-plus', color: '#8b5cf6', group: 'Actions' },
        { title: 'New Work Order', path: 'Manufacturing › Work Orders › New', url: '/Manufacturing/WorkOrders/Create', icon: 'fa-plus', color: '#3b82f6', group: 'Actions' },
        { title: 'New Material Requisition', path: 'Manufacturing › Material Requisitions › New', url: '/Manufacturing/MaterialRequisitions/Create', icon: 'fa-plus', color: '#10b981', group: 'Actions' },
        { title: 'New Production Entry', path: 'Manufacturing › Production Entries › New', url: '/Manufacturing/ProductionEntries/Create', icon: 'fa-plus', color: '#ef4444', group: 'Actions' },
        { title: 'Employees', path: 'HR & Payroll › Employees', url: '/HR/Employees', icon: 'fa-id-badge', color: '#0ea5e9', group: 'HR & Payroll' },
        { title: 'Departments', path: 'HR & Payroll › Departments', url: '/HR/Departments', icon: 'fa-sitemap', color: '#8b5cf6', group: 'HR & Payroll' },
        { title: 'Leave Types', path: 'HR & Payroll › Leave Types', url: '/HR/LeaveTypes', icon: 'fa-list', color: '#f59e0b', group: 'HR & Payroll' },
        { title: 'Leave Allocations', path: 'HR & Payroll › Leave Allocations', url: '/HR/LeaveAllocations', icon: 'fa-calendar-check', color: '#10b981', group: 'HR & Payroll' },
        { title: 'Leave Requests', path: 'HR & Payroll › Leave Requests', url: '/HR/LeaveRequests', icon: 'fa-calendar-minus', color: '#ef4444', group: 'HR & Payroll' },
        { title: 'Create Employee', path: 'HR & Payroll › Employees › Create', url: '/HR/Employees/Create', icon: 'fa-plus', color: '#0ea5e9', group: 'Actions' },
        { title: 'Create Department', path: 'HR & Payroll › Departments › Create', url: '/HR/Departments/Create', icon: 'fa-plus', color: '#8b5cf6', group: 'Actions' },
        { title: 'New Leave Request', path: 'HR & Payroll › Leave Requests › New', url: '/HR/LeaveRequests/Create', icon: 'fa-plus', color: '#ef4444', group: 'Actions' },
        { title: 'Salary Components', path: 'HR & Payroll › Salary Components', url: '/HR/SalaryComponents', icon: 'fa-puzzle-piece', color: '#06b6d4', group: 'HR & Payroll' },
        { title: 'Employee Salaries', path: 'HR & Payroll › Employee Salaries', url: '/HR/EmployeeSalaries', icon: 'fa-coins', color: '#f59e0b', group: 'HR & Payroll' },
        { title: 'Payroll Runs', path: 'HR & Payroll › Payroll Runs', url: '/HR/PayrollRuns', icon: 'fa-file-invoice-dollar', color: '#10b981', group: 'HR & Payroll' },
        { title: 'New Salary Component', path: 'HR & Payroll › Salary Components › New', url: '/HR/SalaryComponents/Create', icon: 'fa-plus', color: '#06b6d4', group: 'Actions' },
        { title: 'Assign Employee Salary', path: 'HR & Payroll › Employee Salaries › Assign', url: '/HR/EmployeeSalaries/Create', icon: 'fa-plus', color: '#f59e0b', group: 'Actions' },
        { title: 'New Payroll Run', path: 'HR & Payroll › Payroll Runs › New', url: '/HR/PayrollRuns/Create', icon: 'fa-plus', color: '#10b981', group: 'Actions' },
        { title: 'Users', path: 'Administration › Users', url: '/Admin/Users', icon: 'fa-users', color: '#0ea5e9', group: 'Administration' },
        { title: 'Roles', path: 'Administration › Roles', url: '/Admin/Roles', icon: 'fa-user-shield', color: '#8b5cf6', group: 'Administration' },
        { title: 'Audit Trail', path: 'Administration › Audit Trail', url: '/Admin/AuditTrail', icon: 'fa-history', color: '#f59e0b', group: 'Administration' },
        { title: 'Menu Configuration', path: 'Administration › Menu Config', url: '/Admin/MenuConfig', icon: 'fa-bars', color: '#10b981', group: 'Administration' },
        { title: 'Create User', path: 'Administration › Users › Create', url: '/Admin/Users/Create', icon: 'fa-plus', color: '#0ea5e9', group: 'Actions' },
        { title: 'Create Role', path: 'Administration › Roles › Create', url: '/Admin/Roles/Create', icon: 'fa-plus', color: '#8b5cf6', group: 'Actions' }
    ];

    function openSearch() {
        if (!searchOverlay) return;
        searchOverlay.classList.add('open');
        searchInput.value = '';
        activeIndex = -1;
        renderResults('');
        setTimeout(function () { searchInput.focus(); }, 50);
    }

    function closeSearch() {
        if (!searchOverlay) return;
        searchOverlay.classList.remove('open');
    }

    function renderResults(query) {
        var q = query.toLowerCase().trim();
        var filtered = q === '' ? searchItems : searchItems.filter(function (item) {
            return item.title.toLowerCase().indexOf(q) > -1 ||
                   item.path.toLowerCase().indexOf(q) > -1 ||
                   item.group.toLowerCase().indexOf(q) > -1;
        });

        if (filtered.length === 0) {
            searchResults.innerHTML = '<div class="search-empty"><i class="fas fa-search me-2"></i>No results found</div>';
            return;
        }

        var groups = {};
        filtered.forEach(function (item) {
            if (!groups[item.group]) groups[item.group] = [];
            groups[item.group].push(item);
        });

        var html = '';
        var idx = 0;
        Object.keys(groups).forEach(function (group) {
            html += '<div class="search-group-label">' + group + '</div>';
            groups[group].forEach(function (item) {
                var cls = 'search-item' + (idx === activeIndex ? ' active' : '') + (item.disabled ? ' disabled' : '');
                html += '<a href="' + item.url + '" class="' + cls + '" data-index="' + idx + '">';
                html += '<div class="search-item-icon" style="background:' + item.color + '20; color:' + item.color + ';">';
                html += '<i class="fas ' + item.icon + '"></i></div>';
                html += '<div class="search-item-text"><div class="search-item-title">' + highlight(item.title, q) + '</div>';
                html += '<div class="search-item-path">' + item.path + '</div></div></a>';
                idx++;
            });
        });
        searchResults.innerHTML = html;
    }

    function highlight(text, query) {
        if (!query) return text;
        var i = text.toLowerCase().indexOf(query);
        if (i === -1) return text;
        return text.substring(0, i) + '<mark>' + text.substring(i, i + query.length) + '</mark>' + text.substring(i + query.length);
    }

    if (searchToggle) {
        searchToggle.addEventListener('click', function (e) {
            e.preventDefault();
            openSearch();
        });
    }

    if (searchOverlay) {
        searchOverlay.addEventListener('click', function (e) {
            if (e.target === searchOverlay) closeSearch();
        });

        searchInput.addEventListener('input', function () {
            activeIndex = -1;
            renderResults(this.value);
        });

        searchInput.addEventListener('keydown', function (e) {
            var items = searchResults.querySelectorAll('.search-item:not(.disabled)');
            if (e.key === 'ArrowDown') {
                e.preventDefault();
                activeIndex = Math.min(activeIndex + 1, items.length - 1);
                renderResults(this.value);
            } else if (e.key === 'ArrowUp') {
                e.preventDefault();
                activeIndex = Math.max(activeIndex - 1, 0);
                renderResults(this.value);
            } else if (e.key === 'Enter' && activeIndex >= 0) {
                e.preventDefault();
                var active = searchResults.querySelector('.search-item.active');
                if (active && !active.classList.contains('disabled')) {
                    window.location.href = active.href;
                }
            } else if (e.key === 'Escape') {
                closeSearch();
            }
        });
    }

    document.addEventListener('keydown', function (e) {
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            openSearch();
        }
    });

    // Module tabs: single-row horizontal scroller with Netflix-style arrows
    document.querySelectorAll('.module-tabs').forEach(function (container) {
        var scroller = container.querySelector('.module-tabs-scroll');
        var leftBtn = container.querySelector('.module-tabs-arrow-left');
        var rightBtn = container.querySelector('.module-tabs-arrow-right');
        if (!scroller) return;

        function update() {
            var scrollable = scroller.scrollWidth > scroller.clientWidth + 1;
            container.classList.toggle('is-scrollable', scrollable);
            if (leftBtn) leftBtn.classList.toggle('is-visible', scrollable && scroller.scrollLeft > 1);
            if (rightBtn) rightBtn.classList.toggle('is-visible', scrollable && scroller.scrollLeft < scroller.scrollWidth - scroller.clientWidth - 1);
        }

        function step(dir) {
            scroller.scrollBy({ left: dir * Math.max(160, scroller.clientWidth * 0.6), behavior: 'smooth' });
        }

        if (leftBtn) leftBtn.addEventListener('click', function () { step(-1); });
        if (rightBtn) rightBtn.addEventListener('click', function () { step(1); });
        scroller.addEventListener('scroll', update);
        window.addEventListener('resize', update);

        // Center the active tab on load.
        var active = scroller.querySelector('.nav-link.active');
        if (active) {
            scroller.scrollLeft = Math.max(0, active.offsetLeft - (scroller.clientWidth / 2) + (active.offsetWidth / 2));
        }
        update();
    });

    // Flatpickr date pickers — auto-upgrade all date inputs
    if (typeof flatpickr !== 'undefined') {
        document.querySelectorAll('input[type="date"]').forEach(function (input) {
            input.type = 'text';
            input.classList.add('flatpickr-input');
            flatpickr(input, {
                dateFormat: 'Y-m-d',
                altInput: true,
                altFormat: 'd M Y',
                allowInput: true,
                animate: true,
                monthSelectorType: 'dropdown',
                prevArrow: '<i class="fas fa-chevron-left"></i>',
                nextArrow: '<i class="fas fa-chevron-right"></i>',
                onReady: function (selectedDates, dateStr, instance) {
                    var wrapper = document.createElement('div');
                    wrapper.className = 'flatpickr-footer';

                    var todayBtn = document.createElement('button');
                    todayBtn.type = 'button';
                    todayBtn.className = 'flatpickr-footer-btn flatpickr-today-btn';
                    todayBtn.textContent = 'Today';
                    todayBtn.addEventListener('click', function () {
                        instance.setDate(new Date(), true);
                        instance.close();
                    });

                    var clearBtn = document.createElement('button');
                    clearBtn.type = 'button';
                    clearBtn.className = 'flatpickr-footer-btn flatpickr-clear-btn';
                    clearBtn.textContent = 'Clear';
                    clearBtn.addEventListener('click', function () {
                        instance.clear();
                        instance.close();
                    });

                    wrapper.appendChild(todayBtn);
                    wrapper.appendChild(clearBtn);
                    instance.calendarContainer.appendChild(wrapper);
                }
            });
        });
    }

    // Delete confirmation pattern (delegated for dynamically rendered DataTables rows)
    document.addEventListener('click', function (e) {
        var btn = e.target.closest('[data-delete-url]');
        if (!btn) return;
        e.preventDefault();
        var url = btn.dataset.deleteUrl;
        Swal.fire({
            title: 'Delete Record?',
            text: 'This action cannot be undone.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#dc3545',
            cancelButtonColor: '#6c757d',
            confirmButtonText: 'Yes, delete it'
        }).then(function (result) {
            if (result.isConfirmed) {
                var token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                fetch(url, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': token,
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                }).then(function (response) {
                    return response.json();
                }).then(function (data) {
                    if (data.success) {
                        toastr.success(data.message || 'Record deleted successfully.');
                        location.reload();
                    } else {
                        toastr.error(data.message || 'Failed to delete record.');
                    }
                }).catch(function () {
                    toastr.error('An error occurred.');
                });
            }
        });
    });

    // Popup Create/Edit forms (delegated, mirrors the delete handler above).
    // A [data-form-url] trigger opens the form (fetched as a partial) inside SweetAlert2.
    document.addEventListener('click', function (e) {
        var btn = e.target.closest('[data-form-url]');
        if (!btn) return;
        e.preventDefault();
        openFormModal({
            url: btn.dataset.formUrl,
            title: btn.dataset.formTitle || 'Form'
        });
    });
});

// Standard Edit/Delete actions column for DataTables. Replaces the copy-pasted render string.
//   actionsColumn({ edit: '/Module/Feature/Edit', del: '/Module/Feature/Delete', extra: fn(id,row) })
// Pass edit/del as the base path (the row id is appended); omit either to hide that button.
function actionsColumn(opts) {
    opts = opts || {};
    return {
        data: 'id',
        orderable: false,
        render: function (data, type, row) {
            var html = '';
            if (opts.edit) {
                if (opts.editModal) {
                    html += '<button type="button" class="btn btn-sm btn-outline-primary me-1" data-form-url="' + opts.edit + '/' + data + '" data-form-title="' + (opts.editTitle || 'Edit') + '"><i class="fas fa-edit"></i></button>';
                } else {
                    html += '<a href="' + opts.edit + '/' + data + '" class="btn btn-sm btn-outline-primary me-1"><i class="fas fa-edit"></i></a>';
                }
            }
            if (opts.del) {
                html += '<button class="btn btn-sm btn-outline-danger" data-delete-url="' + opts.del + '/' + data + '"><i class="fas fa-trash"></i></button>';
            }
            if (typeof opts.extra === 'function') {
                html += opts.extra(data, row) || '';
            }
            return html;
        }
    };
}

// Reloads whatever list the current page shows after a popup form saves.
// DataTables pages expose a global `table`; the Login Slides page exposes `window.refreshGrid`.
function refreshCurrentGrid() {
    if (typeof window.refreshGrid === 'function') { window.refreshGrid(); return; }
    if (window.table && window.table.ajax) { window.table.ajax.reload(null, false); return; }
    location.reload();
}

// Initializes the plugins/behaviours a form partial may need. Called both for popup
// forms (scope = the Swal popup, with dropdownParent set so select2 isn't clipped) and
// for the full-page fallback views (scope = document). Everything is scoped + guarded so
// it's safe to run regardless of which fields the form actually contains.
function initFormModalPlugins(scope, opts) {
    scope = scope || document;
    opts = opts || {};

    // select2 multi/single selects
    if (window.jQuery && jQuery.fn.select2) {
        jQuery(scope).find('.js-select2').each(function () {
            var cfg = {
                width: '100%',
                theme: 'bootstrap-5',
                allowClear: true,
                placeholder: jQuery(this).data('placeholder') || 'Select…'
            };
            if (opts.dropdownParent) cfg.dropdownParent = opts.dropdownParent;
            jQuery(this).select2(cfg);
        });
    }

    // Role create/edit: "copy from role" vs "apply preset" mutual exclusion + hint
    var copySelect = scope.querySelector('#copyFromSelect');
    var presetSelect = scope.querySelector('#presetSelect');
    if (copySelect && presetSelect) {
        var hint = scope.querySelector('#permissionHint');
        var updateHint = function () {
            if (hint) hint.classList.toggle('d-none', !(copySelect.value || presetSelect.value));
            presetSelect.disabled = !!copySelect.value;
            copySelect.disabled = !!(presetSelect.value && !copySelect.value);
            if (!copySelect.value && !presetSelect.value) {
                copySelect.disabled = false;
                presetSelect.disabled = false;
            }
        };
        copySelect.addEventListener('change', updateHint);
        presetSelect.addEventListener('change', updateHint);
    }

    // Login slide: live FontAwesome icon preview
    var iconInput = scope.querySelector('#iconClassInput');
    var iconPreview = scope.querySelector('#iconPreview');
    if (iconInput && iconPreview) {
        iconInput.addEventListener('input', function () {
            iconPreview.className = 'fas ' + this.value;
        });
    }

    // Login slide: image dropzone (initDropzone is document-scoped via element IDs)
    if (scope.querySelector('#dropzone') && typeof initDropzone === 'function') {
        initDropzone('dropzone', 'imageInput', 'dropzoneIdle', 'dropzonePreview', 'previewImg', 'fileName', 'fileSize', 'removeFile');
    }
}

// Opens a Create/Edit form (fetched as a partial) inside a SweetAlert2 popup, posts it via
// fetch, surfaces server-side validation inline, and reloads the list on success.
function openFormModal(o) {
    var url = o.url;
    Swal.fire({
        title: o.title,
        html: '<div class="swal-form-loading"><span class="spinner-border spinner-border-sm text-primary"></span> Loading…</div>',
        width: o.width || 760,
        showCancelButton: true,
        confirmButtonText: '<i class="fas fa-save me-1"></i>Save',
        cancelButtonText: 'Cancel',
        focusConfirm: false,
        showLoaderOnConfirm: true,
        allowOutsideClick: function () { return !Swal.isLoading(); },
        customClass: { popup: 'swal-form-popup', htmlContainer: 'swal-form-container' },
        didOpen: function () {
            fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
                .then(function (r) { return r.text(); })
                .then(function (html) {
                    var c = Swal.getHtmlContainer();
                    c.innerHTML = html;
                    initFormModalPlugins(c, { dropdownParent: jQuery(Swal.getPopup()) });
                })
                .catch(function () {
                    Swal.getHtmlContainer().innerHTML = '<div class="text-danger">Failed to load the form.</div>';
                });
        },
        preConfirm: function () {
            var form = Swal.getPopup().querySelector('form');
            if (!form) { Swal.showValidationMessage('Form not loaded.'); return false; }
            return fetch(form.getAttribute('action'), {
                method: 'POST',
                headers: { 'X-Requested-With': 'XMLHttpRequest' },
                body: new FormData(form)
            }).then(function (resp) {
                if (resp.ok) {
                    return resp.json().then(function (data) {
                        if (data && data.success === false) {
                            Swal.showValidationMessage(data.message || 'Could not save.');
                            return { ok: false };
                        }
                        return { ok: true, data: data };
                    });
                }
                // Validation / business error: server returns the form partial with messages
                return resp.text().then(function (html) {
                    var c = Swal.getHtmlContainer();
                    c.innerHTML = html;
                    initFormModalPlugins(c, { dropdownParent: jQuery(Swal.getPopup()) });
                    Swal.showValidationMessage('Please correct the highlighted fields.');
                    return { ok: false };
                });
            }).catch(function () {
                Swal.showValidationMessage('An error occurred. Please try again.');
                return { ok: false };
            });
        }
    }).then(function (result) {
        if (result.isConfirmed && result.value && result.value.ok) {
            var data = result.value.data || {};
            toastr.success(data.message || 'Saved successfully.');
            refreshCurrentGrid();
        }
    });
}

// Active/Inactive badge — use as a column `render`: { data: 'isActive', render: activeBadge }
function activeBadge(data) {
    return data
        ? '<span class="badge bg-success">Active</span>'
        : '<span class="badge bg-secondary">Inactive</span>';
}

// DocumentStatus badge (Draft/Submitted/Approved/Rejected/Cancelled/Closed) by ordinal or name.
function statusBadge(data) {
    var map = {
        0: ['Draft', 'bg-secondary'], 1: ['Submitted', 'bg-info'], 2: ['Approved', 'bg-success'],
        3: ['Rejected', 'bg-danger'], 4: ['Cancelled', 'bg-warning'], 5: ['Closed', 'bg-dark']
    };
    var byName = {
        'Draft': 'bg-secondary', 'Submitted': 'bg-info', 'Approved': 'bg-success',
        'Rejected': 'bg-danger', 'Cancelled': 'bg-warning', 'Closed': 'bg-dark'
    };
    var label, cls;
    if (typeof data === 'number' && map[data]) { label = map[data][0]; cls = map[data][1]; }
    else { label = data; cls = byName[data] || 'bg-secondary'; }
    return '<span class="badge ' + cls + '">' + label + '</span>';
}

// DataTables helper with column resize, reorder, visibility, group-by, and localStorage persistence
function initDataTable(tableId, ajaxUrl, columns, options) {
    var tableEl = document.getElementById(tableId);
    if (!tableEl) return null;

    var headerCells = tableEl.querySelectorAll('thead th');
    var colNames = [];
    for (var i = 0; i < headerCells.length; i++) colNames.push(headerCells[i].textContent.trim());

    var saved = _dtLoadState(tableId, columns.length);
    var viewOrder = (options && options.order) || [[0, 'asc']];
    var effectiveOrder = (saved && saved.order) ? saved.order : viewOrder;

    var toolbar = document.createElement('div');
    toolbar.className = 'dt-toolbar d-flex align-items-center gap-2 flex-wrap mb-2';
    toolbar.innerHTML = _dtBuildToolbar(tableId, columns, colNames, saved);
    tableEl.parentNode.insertBefore(toolbar, tableEl);

    if (saved && saved.colVis) {
        for (var i = 0; i < columns.length; i++) {
            if (saved.colVis[i] === false) columns[i].visible = false;
        }
    }

    var hasSavedWidths = saved && saved.colWidths && Object.keys(saved.colWidths).length > 0;
    var groupByCol = (saved && saved.groupBy != null && saved.groupBy >= 0) ? saved.groupBy : null;

    var defaults = {
        processing: true,
        serverSide: true,
        scrollY: 'calc(100vh - 390px)',
        scrollCollapse: true,
        autoWidth: !hasSavedWidths,
        ajax: {
            url: ajaxUrl,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
            },
            data: function (d) { return JSON.stringify(d); }
        },
        columns: columns,
        order: effectiveOrder,
        pageLength: (saved && saved.pageLength) || 25,
        language: {
            processing: '<div class="spinner-border spinner-border-sm text-primary" role="status"><span class="visually-hidden">Loading...</span></div> Loading...'
        }
    };

    if (typeof DataTable !== 'undefined' && DataTable.ColReorder) {
        defaults.colReorder = (saved && saved.colOrder) ? { order: saved.colOrder, enable: true } : true;
    }

    if (typeof DataTable !== 'undefined' && DataTable.RowGroup) {
        defaults.rowGroup = {
            enable: groupByCol !== null,
            dataSrc: groupByCol !== null ? columns[groupByCol].data : (columns[0] ? columns[0].data || 0 : 0),
            startRender: function (rows, group) {
                var gbIdx = table ? table._dtGroupBy : groupByCol;
                return _dtGroupRender(rows, group, columns, gbIdx);
            }
        };
    }

    if (groupByCol !== null) {
        defaults.order = [[groupByCol, 'asc']].concat(
            effectiveOrder.filter(function (o) { return o[0] !== groupByCol; })
        );
    }

    var config = {};
    for (var k in defaults) config[k] = defaults[k];
    if (options) {
        for (var k in options) {
            if (k !== 'order' && k !== 'colReorder' && k !== 'rowGroup' && k !== 'pageLength' && k !== 'autoWidth') {
                config[k] = options[k];
            }
        }
    }

    var table = new DataTable('#' + tableId, config);

    table._dtGroupBy = groupByCol;
    table._dtColumns = columns;
    table._dtColNames = colNames;
    table._dtDefaultOrder = viewOrder;
    table._dtWidths = (saved && saved.colWidths) ? Object.assign({}, saved.colWidths) : {};

    if (hasSavedWidths) {
        table.one('draw.dt', function () { _dtApplyWidths(table, saved.colWidths); });
    }
    table.on('draw.dt', function () {
        if (table._dtWidths && Object.keys(table._dtWidths).length > 0) {
            _dtApplyWidths(table, table._dtWidths);
        }
    });

    table.one('init.dt', function () { _dtInitResize(tableId, table); });

    _dtWireToolbar(tableId, table, columns, colNames, toolbar);

    table.on('order.dt', function () { _dtSaveState(tableId, table); });
    table.on('length.dt', function () { _dtSaveState(tableId, table); });
    if (typeof DataTable !== 'undefined' && DataTable.ColReorder) {
        table.on('column-reorder', function () {
            _dtSaveState(tableId, table);
            setTimeout(function () { _dtInitResize(tableId, table); }, 100);
        });
    }

    return table;
}

function _dtLoadState(tableId, colCount) {
    try {
        var s = JSON.parse(localStorage.getItem('dt_' + tableId));
        if (s && s.colCount === colCount) return s;
        if (s) localStorage.removeItem('dt_' + tableId);
    } catch (e) {}
    return null;
}

function _dtSaveState(tableId, table) {
    var colCount = table.columns()[0].length;
    var vis = {};
    for (var i = 0; i < colCount; i++) vis[i] = table.column(i).visible();

    var colOrder = null;
    try { colOrder = table.colReorder.order(); } catch (e) {}

    var state = {
        colCount: colCount,
        order: table.order(),
        pageLength: table.page.len(),
        groupBy: table._dtGroupBy != null ? table._dtGroupBy : null,
        colVis: vis,
        colWidths: table._dtWidths || {},
        colOrder: colOrder
    };
    localStorage.setItem('dt_' + tableId, JSON.stringify(state));
}

function _dtBuildToolbar(tableId, columns, colNames, saved) {
    var html = '';

    html += '<div class="dropdown">';
    html += '<button class="btn btn-outline-secondary btn-sm dropdown-toggle" type="button" data-bs-toggle="dropdown" data-bs-auto-close="outside">';
    html += '<i class="fas fa-columns me-1"></i>Columns</button>';
    html += '<ul class="dropdown-menu dt-col-menu">';
    for (var i = 0; i < columns.length; i++) {
        if (columns[i].data === null) continue;
        var checked = !(saved && saved.colVis && saved.colVis[i] === false);
        html += '<li><label class="dropdown-item d-flex align-items-center gap-2 py-1">';
        html += '<input type="checkbox" class="form-check-input m-0" data-dt-col="' + i + '"' + (checked ? ' checked' : '') + '> ';
        html += colNames[i] + '</label></li>';
    }
    html += '</ul></div>';

    html += '<div class="dropdown">';
    html += '<button class="btn btn-outline-secondary btn-sm dropdown-toggle" type="button" data-bs-toggle="dropdown">';
    html += '<i class="fas fa-layer-group me-1"></i>Group By</button>';
    html += '<ul class="dropdown-menu dt-group-menu">';
    var activeGroup = (saved && saved.groupBy != null) ? saved.groupBy : -1;
    html += '<li><a class="dropdown-item' + (activeGroup === -1 ? ' active' : '') + '" href="#" data-dt-group="-1">None</a></li>';
    html += '<li><hr class="dropdown-divider"></li>';
    for (var i = 0; i < columns.length; i++) {
        if (columns[i].data === null || columns[i].orderable === false) continue;
        html += '<li><a class="dropdown-item' + (activeGroup === i ? ' active' : '') + '" href="#" data-dt-group="' + i + '">' + colNames[i] + '</a></li>';
    }
    html += '</ul></div>';

    html += '<button class="btn btn-outline-secondary btn-sm dt-reset-btn" title="Reset table to defaults">';
    html += '<i class="fas fa-undo me-1"></i>Reset</button>';

    return html;
}

function _dtGroupRender(rows, group, columns, groupByCol) {
    if (groupByCol === null || groupByCol === undefined) return group;
    var col = columns[groupByCol];
    var display = group;
    if (col && col.render && typeof col.render === 'function') {
        try { display = col.render(group, 'display', rows.data()[0], { row: 0, col: groupByCol }); }
        catch (e) { display = group; }
    }
    if (display === null || display === undefined || display === '') display = '(empty)';
    return display + ' <span class="badge bg-secondary rounded-pill ms-1">' + rows.count() + '</span>';
}

function _dtApplyWidths(table, widths) {
    var container = table.table().container();
    var scrollHead = container.querySelector('.dataTables_scrollHead');
    if (!scrollHead) return;
    var ths = scrollHead.querySelectorAll('thead th');
    var colOrder = null;
    try { colOrder = table.colReorder.order(); } catch (e) {}

    for (var origIdx in widths) {
        var displayPos = parseInt(origIdx);
        if (colOrder) {
            for (var j = 0; j < colOrder.length; j++) {
                if (colOrder[j] === parseInt(origIdx)) { displayPos = j; break; }
            }
        }
        if (ths[displayPos]) {
            ths[displayPos].style.width = widths[origIdx];
            ths[displayPos].style.minWidth = widths[origIdx];
        }
    }
}

function _dtInitResize(tableId, table) {
    var container = table.table().container();
    var scrollHead = container.querySelector('.dataTables_scrollHead');
    if (!scrollHead) return;

    scrollHead.querySelectorAll('.dt-resize-handle').forEach(function (h) { h.remove(); });
    var ths = scrollHead.querySelectorAll('thead th');

    for (var i = 0; i < ths.length; i++) {
        (function (th, displayIdx) {
            th.style.position = 'relative';
            var handle = document.createElement('div');
            handle.className = 'dt-resize-handle';
            th.appendChild(handle);

            handle.addEventListener('mousedown', function (e) {
                e.preventDefault();
                e.stopPropagation();
                var startX = e.pageX;
                var startW = th.offsetWidth;
                handle.classList.add('active');
                document.body.style.cursor = 'col-resize';
                document.body.style.userSelect = 'none';

                function onMove(ev) {
                    var w = Math.max(50, startW + (ev.pageX - startX));
                    th.style.width = w + 'px';
                    th.style.minWidth = w + 'px';
                }
                function onUp() {
                    document.removeEventListener('mousemove', onMove);
                    document.removeEventListener('mouseup', onUp);
                    handle.classList.remove('active');
                    document.body.style.cursor = '';
                    document.body.style.userSelect = '';

                    var colOrder = null;
                    try { colOrder = table.colReorder.order(); } catch (ex) {}
                    var origIdx = colOrder ? colOrder[displayIdx] : displayIdx;
                    table._dtWidths[origIdx] = th.style.width;
                    _dtSaveState(tableId, table);
                }

                document.addEventListener('mousemove', onMove);
                document.addEventListener('mouseup', onUp);
            });
        })(ths[i], i);
    }
}

function _dtWireToolbar(tableId, table, columns, colNames, toolbar) {
    toolbar.querySelectorAll('[data-dt-col]').forEach(function (cb) {
        cb.addEventListener('change', function () {
            var idx = parseInt(this.dataset.dtCol);
            table.column(idx).visible(this.checked);
            _dtSaveState(tableId, table);
            setTimeout(function () { _dtInitResize(tableId, table); }, 100);
        });
    });

    toolbar.querySelectorAll('[data-dt-group]').forEach(function (item) {
        item.addEventListener('click', function (e) {
            e.preventDefault();
            var idx = parseInt(this.dataset.dtGroup);
            toolbar.querySelectorAll('[data-dt-group]').forEach(function (a) { a.classList.remove('active'); });
            this.classList.add('active');

            table._dtGroupBy = idx >= 0 ? idx : null;

            if (idx >= 0) {
                try {
                    table.rowGroup().dataSrc(columns[idx].data);
                    table.rowGroup().enable();
                    var curOrder = table.order();
                    var newOrder = [[idx, 'asc']].concat(curOrder.filter(function (o) { return o[0] !== idx; }));
                    table.order(newOrder).draw();
                } catch (ex) { console.warn('RowGroup:', ex); }
            } else {
                try { table.rowGroup().disable(); table.draw(); } catch (ex) {}
            }
            _dtSaveState(tableId, table);
        });
    });

    toolbar.querySelector('.dt-reset-btn').addEventListener('click', function () {
        localStorage.removeItem('dt_' + tableId);
        location.reload();
    });
}

// Dropzone file upload helper
function initDropzone(wrapperId, inputId, idleId, previewId, imgId, nameId, sizeId, removeId) {
    var wrapper = document.getElementById(wrapperId);
    var input = document.getElementById(inputId);
    var idle = document.getElementById(idleId);
    var preview = document.getElementById(previewId);
    var img = document.getElementById(imgId);
    var nameEl = document.getElementById(nameId);
    var sizeEl = document.getElementById(sizeId);
    var removeBtn = document.getElementById(removeId);
    if (!wrapper || !input) return;

    var maxSize = 5 * 1024 * 1024;
    var allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];

    function formatSize(bytes) {
        if (bytes < 1024) return bytes + ' B';
        if (bytes < 1048576) return (bytes / 1024).toFixed(1) + ' KB';
        return (bytes / 1048576).toFixed(1) + ' MB';
    }

    function showFile(file) {
        if (allowedTypes.indexOf(file.type) === -1) {
            toastr.error('Only JPG, PNG, and WebP images are allowed.');
            return;
        }
        if (file.size > maxSize) {
            toastr.error('File size must be under 5 MB.');
            return;
        }
        var reader = new FileReader();
        reader.onload = function (e) {
            img.src = e.target.result;
            nameEl.textContent = file.name;
            sizeEl.textContent = formatSize(file.size);
            idle.style.display = 'none';
            preview.style.display = '';
        };
        reader.readAsDataURL(file);
    }

    function reset() {
        input.value = '';
        img.src = '';
        nameEl.textContent = '';
        sizeEl.textContent = '';
        preview.style.display = 'none';
        idle.style.display = '';
    }

    wrapper.addEventListener('click', function (e) {
        if (e.target.closest('.dropzone-remove')) return;
        input.click();
    });

    input.addEventListener('change', function () {
        if (this.files && this.files[0]) showFile(this.files[0]);
    });

    wrapper.addEventListener('dragover', function (e) {
        e.preventDefault();
        wrapper.classList.add('dropzone-dragover');
    });

    wrapper.addEventListener('dragleave', function (e) {
        e.preventDefault();
        wrapper.classList.remove('dropzone-dragover');
    });

    wrapper.addEventListener('drop', function (e) {
        e.preventDefault();
        wrapper.classList.remove('dropzone-dragover');
        if (e.dataTransfer.files && e.dataTransfer.files[0]) {
            input.files = e.dataTransfer.files;
            showFile(e.dataTransfer.files[0]);
        }
    });

    if (removeBtn) {
        removeBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            reset();
        });
    }
}

// Generic file dropzone (any type, no image preview) — for documents
function initDocDropzone(wrapperId, inputId, idleId, previewId, nameId, sizeId, removeId, maxBytes) {
    var wrapper = document.getElementById(wrapperId);
    var input = document.getElementById(inputId);
    var idle = document.getElementById(idleId);
    var preview = document.getElementById(previewId);
    var nameEl = document.getElementById(nameId);
    var sizeEl = document.getElementById(sizeId);
    var removeBtn = document.getElementById(removeId);
    if (!wrapper || !input) return;

    var maxSize = maxBytes || (15 * 1024 * 1024);

    function formatSize(bytes) {
        if (bytes < 1024) return bytes + ' B';
        if (bytes < 1048576) return (bytes / 1024).toFixed(1) + ' KB';
        return (bytes / 1048576).toFixed(1) + ' MB';
    }

    function showFile(file) {
        if (file.size > maxSize) {
            toastr.error('File size must be under ' + formatSize(maxSize) + '.');
            input.value = '';
            return;
        }
        nameEl.textContent = file.name;
        sizeEl.textContent = formatSize(file.size);
        idle.style.display = 'none';
        preview.style.display = '';
    }

    function reset() {
        input.value = '';
        nameEl.textContent = '';
        sizeEl.textContent = '';
        preview.style.display = 'none';
        idle.style.display = '';
    }

    wrapper.addEventListener('click', function (e) {
        if (e.target.closest('.dropzone-remove')) return;
        input.click();
    });
    input.addEventListener('change', function () {
        if (this.files && this.files[0]) showFile(this.files[0]);
    });
    wrapper.addEventListener('dragover', function (e) { e.preventDefault(); wrapper.classList.add('dropzone-dragover'); });
    wrapper.addEventListener('dragleave', function (e) { e.preventDefault(); wrapper.classList.remove('dropzone-dragover'); });
    wrapper.addEventListener('drop', function (e) {
        e.preventDefault();
        wrapper.classList.remove('dropzone-dragover');
        if (e.dataTransfer.files && e.dataTransfer.files[0]) {
            input.files = e.dataTransfer.files;
            showFile(e.dataTransfer.files[0]);
        }
    });
    if (removeBtn) removeBtn.addEventListener('click', function (e) { e.stopPropagation(); reset(); });
}

// Select2 helper
function initSelect2(selector, ajaxUrl, placeholder, options) {
    var defaults = {
        theme: 'bootstrap-5',
        placeholder: placeholder || 'Select...',
        allowClear: true,
        ajax: {
            url: ajaxUrl,
            dataType: 'json',
            delay: 300,
            minimumInputLength: 1,
            data: function (params) {
                return { search: params.term };
            },
            processResults: function (data) {
                return { results: data.map(function (item) { return { id: item.id, text: item.text }; }) };
            }
        }
    };
    $(selector).select2(Object.assign({}, defaults, options));
}
