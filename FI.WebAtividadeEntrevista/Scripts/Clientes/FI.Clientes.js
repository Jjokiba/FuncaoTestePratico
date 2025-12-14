$(document).ready(function () {
    $('#formCadastro').submit(function (e) {
        e.preventDefault();
        // Only send beneficiariosArray if it exists (i.e., on create, not edit)
        var data = {
            "NOME": $(this).find("#Nome").val(),
            "CEP": $(this).find("#CEP").val(),
            "Email": $(this).find("#Email").val(),
            "Sobrenome": $(this).find("#Sobrenome").val(),
            "Nacionalidade": $(this).find("#Nacionalidade").val(),
            "Estado": $(this).find("#Estado").val(),
            "Cidade": $(this).find("#Cidade").val(),
            "Logradouro": $(this).find("#Logradouro").val(),
            "Telefone": $(this).find("#Telefone").val(),
            "CPF": $(this).find("#CPF").val()
        };
        if (typeof beneficiariosArray !== 'undefined' && Array.isArray(beneficiariosArray) && beneficiariosArray.length > 0) {
            data.Beneficiarios = beneficiariosArray;
        }
        $.ajax({
            url: urlPost,
            method: "POST",
            data: data,
            error:
            function (r) {
                if (r.status == 400)
                    ModalDialog("Ocorreu um erro", r.responseJSON);
                else if (r.status == 500)
                    ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
            },
            success:
            function (r) {
                try {
                    if (r && r.Result === "OK") {
                        if (typeof r.Id !== 'undefined') {
                            $('#Id').val(r.Id);
                            $('#IdCliente').val(r.Id);
                        }
                        ModalDialog("Sucesso!", r.Message || "Cadastro efetuado com sucesso");
                        if ($('#gridBeneficiarios').length) {
                            $('#gridBeneficiarios').jtable('load');
                        }

                        $('#formCadastro')[0].reset();
                    } else {
                        ModalDialog("Sucesso!", r);
                        $('#formCadastro')[0].reset();
                    }
                } catch (ex) {
                    ModalDialog("Sucesso!", r);
                    $('#formCadastro')[0].reset();
                }
            }
        });
    })

})

function ModalDialog(titulo, texto) {
    var random = Math.random().toString().replace('.', '');
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">' + titulo + '</h4>                                                    ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <p>' + texto + '</p>                                                                           ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-footer">                                                                         ' +
        '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
        '                                                                                                                   ' +
        '                </div>                                                                                             ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
}
