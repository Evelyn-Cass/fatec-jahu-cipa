function onStatusChange(checkbox) {
    var id = checkbox.getAttribute('data-id');
    var currentStatus = checkbox.getAttribute('data-status');
    var newStatus = checkbox.checked ? 'Ativo' : 'Inativo';
    if (currentStatus !== newStatus) {
        var url = '/Documents/ChangeStatus?id=' + encodeURIComponent(id) + '&status=' + encodeURIComponent(newStatus);
        window.location.href = url;
    }
}