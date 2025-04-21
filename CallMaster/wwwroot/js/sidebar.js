document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebar');
    const toggleBtn = document.getElementById('toggleSidebar');
    let isExpanded = localStorage.getItem('sidebarExpanded') === 'true';

    // Initialize state
    if (isExpanded) {
        sidebar.classList.remove('collapsed');
    } else {
        sidebar.classList.add('collapsed');
    }

    // Toggle sidebar
    toggleBtn.addEventListener('click', () => {
        isExpanded = !isExpanded;
        sidebar.classList.toggle('collapsed', !isExpanded);
        localStorage.setItem('sidebarExpanded', isExpanded);
    });

    // Hover effect for collapsed state
    sidebar.addEventListener('mouseenter', () => {
        if (!isExpanded) {
            sidebar.style.width = '200px';
        }
    });

    sidebar.addEventListener('mouseleave', () => {
        if (!isExpanded) {
            sidebar.style.width = 'var(--sidebar-collapsed)';
        }
    });

    // Responsive handling
    function handleResponsive() {
        if (window.innerWidth < 768) {
            sidebar.classList.add('collapsed');
            isExpanded = false;
        } else {
            sidebar.classList.toggle('collapsed', !isExpanded);
        }
    }

    window.addEventListener('resize', handleResponsive);
    handleResponsive();
});