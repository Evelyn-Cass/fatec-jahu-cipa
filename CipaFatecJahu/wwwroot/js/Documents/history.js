function onStatusChange(checkbox) {
    var id = checkbox.getAttribute('data-id');
    var currentStatus = checkbox.getAttribute('data-status');
    var newStatus = checkbox.checked ? 'Ativo' : 'Inativo';
    if (currentStatus !== newStatus) {
        var mensagem = `Tem certeza que deseja mudar o status para "${newStatus}"?`;
        if (window.confirm(mensagem)) {
            var url = '/Documents/ChangeStatus?id=' + encodeURIComponent(id) + '&status=' + encodeURIComponent(newStatus);
            window.location.href = url;
        } else {
            // Reverte o checkbox para o estado anterior
            checkbox.checked = !checkbox.checked;
        }
    }
}
