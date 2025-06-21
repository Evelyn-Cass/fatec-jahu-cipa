function onStatusChange(checkbox) {
    var email = checkbox.getAttribute('data-email');
    var currentStatus = checkbox.getAttribute('data-status');
    var newStatus = checkbox.checked ? 'Ativo' : 'Inativo';
    if (currentStatus !== newStatus) {
        var mensagem = `Tem certeza que deseja alterar o status para "${newStatus}"?`;
        if (window.confirm(mensagem)) {
            var url = '/Secretaries/ChangeStatus?email=' + encodeURIComponent(email) + '&status=' + encodeURIComponent(newStatus);
            window.location.href = url;
        } else {
            // Reverte o checkbox para o estado anterior
            checkbox.checked = !checkbox.checked;
        }
    }
}
