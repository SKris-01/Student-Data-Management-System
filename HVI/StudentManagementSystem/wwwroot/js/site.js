// Student Management System - Enhanced JavaScript

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    initializeAnimations();
    initializeFormValidation();
    initializeTooltips();
    initializeSearchFeatures();
});

// Initialize animations
function initializeAnimations() {
    // Add fade-in animation to cards
    const cards = document.querySelectorAll('.card');
    cards.forEach((card, index) => {
        card.style.animationDelay = `${index * 0.1}s`;
        card.classList.add('fade-in-up');
    });

    // Add hover effects to buttons
    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(button => {
        button.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-2px)';
        });

        button.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0)';
        });
    });
}

// Enhanced form validation
function initializeFormValidation() {
    const forms = document.querySelectorAll('form');

    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            const submitBtn = form.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Processing...';
                submitBtn.disabled = true;

                // Re-enable after 3 seconds if form doesn't submit
                setTimeout(() => {
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = submitBtn.getAttribute('data-original-text') || 'Submit';
                }, 3000);
            }
        });

        // Store original button text
        const submitBtn = form.querySelector('button[type="submit"]');
        if (submitBtn) {
            submitBtn.setAttribute('data-original-text', submitBtn.innerHTML);
        }
    });

    // Real-time validation for specific fields
    const emailInputs = document.querySelectorAll('input[type="email"]');
    emailInputs.forEach(input => {
        input.addEventListener('blur', validateEmail);
    });

    const passwordInputs = document.querySelectorAll('input[type="password"]');
    passwordInputs.forEach(input => {
        input.addEventListener('input', validatePassword);
    });
}

// Email validation
function validateEmail(e) {
    const email = e.target.value;
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const isValid = emailRegex.test(email);

    toggleFieldValidation(e.target, isValid, 'Please enter a valid email address');
}

// Password validation
function validatePassword(e) {
    const password = e.target.value;
    const isValid = password.length >= 6;

    toggleFieldValidation(e.target, isValid, 'Password must be at least 6 characters long');
}

// Toggle field validation styling
function toggleFieldValidation(field, isValid, message) {
    const feedback = field.parentNode.querySelector('.validation-feedback') ||
                    createValidationFeedback(field.parentNode);

    if (isValid) {
        field.classList.remove('is-invalid');
        field.classList.add('is-valid');
        feedback.style.display = 'none';
    } else {
        field.classList.remove('is-valid');
        field.classList.add('is-invalid');
        feedback.textContent = message;
        feedback.style.display = 'block';
    }
}

// Create validation feedback element
function createValidationFeedback(parent) {
    const feedback = document.createElement('div');
    feedback.className = 'validation-feedback text-danger small mt-1';
    feedback.style.display = 'none';
    parent.appendChild(feedback);
    return feedback;
}

// Initialize tooltips
function initializeTooltips() {
    // Initialize Bootstrap tooltips if available
    if (typeof bootstrap !== 'undefined') {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
}

// Enhanced search features
function initializeSearchFeatures() {
    const searchInputs = document.querySelectorAll('input[name="departmentName"]');

    searchInputs.forEach(input => {
        // Add autocomplete suggestions
        const suggestions = [
            'Computer Science',
            'Information Technology',
            'Electronics Engineering',
            'Mechanical Engineering',
            'Civil Engineering'
        ];

        input.addEventListener('input', function() {
            showSearchSuggestions(this, suggestions);
        });

        // Clear suggestions when clicking outside
        document.addEventListener('click', function(e) {
            if (!input.contains(e.target)) {
                clearSearchSuggestions(input);
            }
        });
    });
}

// Show search suggestions
function showSearchSuggestions(input, suggestions) {
    const value = input.value.toLowerCase();
    if (value.length < 2) {
        clearSearchSuggestions(input);
        return;
    }

    const filtered = suggestions.filter(s =>
        s.toLowerCase().includes(value)
    );

    if (filtered.length === 0) {
        clearSearchSuggestions(input);
        return;
    }

    let suggestionBox = input.parentNode.querySelector('.search-suggestions');
    if (!suggestionBox) {
        suggestionBox = createSuggestionBox(input.parentNode);
    }

    suggestionBox.innerHTML = '';
    filtered.forEach(suggestion => {
        const item = document.createElement('div');
        item.className = 'suggestion-item p-2 border-bottom';
        item.textContent = suggestion;
        item.style.cursor = 'pointer';

        item.addEventListener('click', function() {
            input.value = suggestion;
            clearSearchSuggestions(input);
        });

        suggestionBox.appendChild(item);
    });

    suggestionBox.style.display = 'block';
}

// Create suggestion box
function createSuggestionBox(parent) {
    const box = document.createElement('div');
    box.className = 'search-suggestions position-absolute bg-white border rounded shadow-sm';
    box.style.cssText = `
        top: 100%;
        left: 0;
        right: 0;
        z-index: 1000;
        max-height: 200px;
        overflow-y: auto;
        display: none;
    `;

    parent.style.position = 'relative';
    parent.appendChild(box);
    return box;
}

// Clear search suggestions
function clearSearchSuggestions(input) {
    const suggestionBox = input.parentNode.querySelector('.search-suggestions');
    if (suggestionBox) {
        suggestionBox.style.display = 'none';
    }
}

// Utility function for showing notifications
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
    notification.style.cssText = `
        top: 20px;
        right: 20px;
        z-index: 9999;
        min-width: 300px;
    `;

    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    document.body.appendChild(notification);

    // Auto-remove after 5 seconds
    setTimeout(() => {
        if (notification.parentNode) {
            notification.remove();
        }
    }, 5000);
}

// Smooth scrolling for anchor links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// Add loading state to navigation links
document.querySelectorAll('.nav-link').forEach(link => {
    link.addEventListener('click', function() {
        if (this.href && !this.href.includes('#')) {
            this.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Loading...';
        }
    });
});
