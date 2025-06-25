document.getElementById("create-form").addEventListener("submit", function (event) {
    var mensagem = "Os dados fornecidos estão corretos?\nNão será possível alterar posteriormente.";
    if (!window.confirm(mensagem)) {
        event.preventDefault();
    }
});
