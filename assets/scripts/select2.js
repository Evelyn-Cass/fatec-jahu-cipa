$(function () {

    Array.from(document.getElementsByClassName('nav-select')).forEach(element => {
        $(element).select2();
    });
});