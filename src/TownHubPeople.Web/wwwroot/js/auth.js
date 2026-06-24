(function () {
    // Slide rotation
    var slides = document.querySelectorAll('.showcase-slide');
    var dots = document.querySelectorAll('.showcase-dot');
    var current = 0;
    var timer;

    function showSlide(index) {
        slides.forEach(function (s) { s.classList.remove('active'); });
        dots.forEach(function (d) { d.classList.remove('active'); });
        slides[index].classList.add('active');
        if (dots[index]) dots[index].classList.add('active');
        current = index;
    }

    function nextSlide() {
        showSlide((current + 1) % slides.length);
    }

    function startTimer() {
        timer = setInterval(nextSlide, 6000);
    }

    dots.forEach(function (dot) {
        dot.addEventListener('click', function () {
            clearInterval(timer);
            showSlide(parseInt(this.dataset.index));
            startTimer();
        });
    });

    if (slides.length > 1) startTimer();

    // Password toggle
    var pwInput = document.getElementById('passwordInput');
    var pwToggle = document.getElementById('passwordToggle');
    if (pwToggle && pwInput) {
        pwToggle.addEventListener('click', function () {
            var isPassword = pwInput.type === 'password';
            pwInput.type = isPassword ? 'text' : 'password';
            this.querySelector('i').className = isPassword ? 'fas fa-eye-slash' : 'fas fa-eye';
        });
    }

    // Login/Register loader
    var loginForm = document.getElementById('loginForm');
    var loginBtn = document.getElementById('loginBtn');
    if (loginForm && loginBtn) {
        loginForm.addEventListener('submit', function () {
            var validator = $(loginForm).validate();
            if (validator && !validator.form()) return;
            loginBtn.classList.add('loading');
            loginBtn.disabled = true;
        });
    }

    var registerForm = document.getElementById('registerForm');
    var registerBtn = document.getElementById('registerBtn');
    if (registerForm && registerBtn) {
        registerForm.addEventListener('submit', function () {
            var validator = $(registerForm).validate();
            if (validator && !validator.form()) return;
            registerBtn.classList.add('loading');
            registerBtn.disabled = true;
        });
    }

    // Theme toggle
    var themeBtn = document.getElementById('loginThemeToggle');
    if (themeBtn) {
        var darkIcon = themeBtn.querySelector('.theme-icon-dark');
        var lightIcon = themeBtn.querySelector('.theme-icon-light');

        function applyThemeIcons() {
            var isDark = document.documentElement.getAttribute('data-theme') === 'dark';
            darkIcon.style.display = isDark ? 'none' : '';
            lightIcon.style.display = isDark ? '' : 'none';
        }

        applyThemeIcons();

        themeBtn.addEventListener('click', function () {
            var isDark = document.documentElement.getAttribute('data-theme') === 'dark';
            var newTheme = isDark ? 'light' : 'dark';
            document.documentElement.setAttribute('data-theme', newTheme);
            document.documentElement.setAttribute('data-bs-theme', newTheme);
            localStorage.setItem('theme', newTheme);
            applyThemeIcons();
        });
    }
})();
