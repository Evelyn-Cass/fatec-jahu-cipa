document.getElementById('attachment-input').addEventListener('change', function (event) {
    var reader = new FileReader();
    reader.onload = function () {
        var output = document.getElementById('imagePreview');
        output.src = reader.result;
        output.style.display = 'block';
    };
    var fileInput = event.target;
    document.getElementById('attachment-input-text').value = fileInput.files.length > 0 ? fileInput.files[0].name : '';
    if (fileInput.files.length > 0) {
        reader.readAsDataURL(fileInput.files[0]);
    }
});