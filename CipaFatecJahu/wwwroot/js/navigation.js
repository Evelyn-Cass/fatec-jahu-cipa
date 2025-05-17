const search = document.getElementById('search-submit');
const material = document.getElementById('search-material');
const management = document.getElementById('search-mandate');
const searchAlert = document.getElementById('search-alert');
const navMenu = document.getElementById('nav-menu');
const navContainer = document.getElementById('nav-container')

search.addEventListener('click', function (event) {
    if (material.value === '0' || management.value === '0') {
        alertTimer();
        event.preventDefault();
        return;
    }
    // submit the form
});

function alertTimer() {
    searchAlert.style.display = 'flex';
    setTimeout(function () {
        searchAlert.style.display = 'none';
    }, 3000);
}

navMenu.addEventListener("click", ()=>{
    if (navContainer.style.display === 'flex'){
        navContainer.style.display='none';
        return;
    }
    navContainer.style.display='flex';
});