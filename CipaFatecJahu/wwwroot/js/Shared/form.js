checkboxAttachment = document.getElementById('change-attachment');
divAttachment = document.getElementById('div-attachment');

checkboxAttachment.addEventListener('click', function () {
    if (this.checked) {
        divAttachment.style.display = "flex";
    } else {
        divAttachment.style.display = "none";
    }
});