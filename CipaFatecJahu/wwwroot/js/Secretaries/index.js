function onStatusChange(checkbox) {
    var email = checkbox.getAttribute('data-email');
    var currentStatus = checkbox.getAttribute('data-status');
    var newStatus = checkbox.checked ? 'Ativo' : 'Inativo';
    if (currentStatus !== newStatus) {
        var url = '/Secretaries/ChangeStatus?email=' + encodeURIComponent(email) + '&status=' + encodeURIComponent(newStatus);
        window.location.href = url;
    }
}