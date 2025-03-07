const search = document.getElementById('search-submit');
const material = document.getElementById('search-material');
const management = document.getElementById('search-management');
const searchAlert = document.getElementById('search-alert');

search.addEventListener('click', function (event) {
    if (material.value === '0' || management.value === '0') {
        alertTimer();
        event.preventDefault();
        return;
    }
    // submit the form
});

function alertTimer() {
    searchAlert.style.display = 'block';
    setTimeout(function () {
        searchAlert.style.display = 'none';
    }, 3000);
}