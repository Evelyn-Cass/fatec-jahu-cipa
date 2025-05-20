document.getElementById('password-field').addEventListener('keyup', function(event) {
    var input = event.target;

    if (event.getModifierState('CapsLock')) {
        input.setCustomValidity("Capslock ativado!");
        validade = false;
    } else {
        input.setCustomValidity("");
        validade = true;
    }
    input.reportValidity();
});
