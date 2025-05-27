document.getElementById('attachment-input').addEventListener('change', (event) => {
    document.getElementById('attachment-input-text').value = event.target.files[0].name;
})
