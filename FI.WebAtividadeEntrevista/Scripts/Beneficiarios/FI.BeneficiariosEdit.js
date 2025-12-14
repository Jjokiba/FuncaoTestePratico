var editMode = false;
var editId = null;

$(document).ready(function () {
    $('#formBenef').on('submit', function (e) {
        let $form = $('#formBenef')
        e.preventDefault();
        if (editMode) {
            // Edit mode: Alterar
            $.ajax({
                url: urlAlteracaoBeneficiario,
                method: "POST",
                data: {
                    Id: editId,
                    Nome: $form.find("#Nome").val(),
                    CPF: $form.find("#CPF").val(),
                    IdCliente: $form.find("#IdCliente").val()
                },
                error:
                function (r) {
                    if (r.status == 400)
                        ModalDialog("Ocorreu um erro", r.responseJSON);
                    else if (r.status == 500)
                        ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
                },
                success:
                function (r) {
                    ModalDialog("Sucesso!", r)
                    if ($('#gridBeneficiarios').length) {
                        $('#gridBeneficiarios').jtable('load');
                    }
                    $('#formBenef')[0].reset();
                    $('#SalvaBenef').text('Salvar');
                    editMode = false;
                    editId = null;
                }
            });
        } else {
            // Create mode: Incluir
            $.ajax({
                url: urlPostBeneficiario,
                method: "POST",
                data: {
                    Nome: $form.find("#Nome").val(),
                    CPF: $form.find("#CPF").val(),
                    IdCliente: $form.find("#IdCliente").val()
                },
                error:
                function (r) {
                    if (r.status == 400)
                        ModalDialog("Ocorreu um erro", r.responseJSON);
                    else if (r.status == 500)
                        ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
                },
                success:
                function (r) {
                    ModalDialog("Sucesso!", r)
                    if ($('#gridBeneficiarios').length) {
                        $('#gridBeneficiarios').jtable('load');
                    }
                    $('#formBenef')[0].reset();
                }
            });
        }
    })

    if (document.getElementById("gridBeneficiarios"))
        $('#gridBeneficiarios').jtable({
            title: 'Beneficiarios',
            paging: false, //Enable paging
            sorting: true, //Enable sorting
            defaultSorting: 'Nome ASC', //Set default sorting
            actions: {
                listAction: function (postData, jtParams) {
                    // jTable expects a promise or JSON. We'll call the controller and return JSON.
                    return $.Deferred(function ($dfd) {
                        $.ajax({
                            url: urlBeneficiarioList,
                            method: 'POST',
                            data: { idCliente: $("#Id").val() || $("#IdCliente").val() },
                            success: function (data) {
                                $dfd.resolve(data);
                            },
                            error: function (err) {
                                $dfd.reject();
                            }
                        });
                    });
                }
            },
            fields: {
                Nome: {
                    title: 'Nome',
                    width: '50%'
                },
                CPF: {
                    title: 'CPF',
                    width: '20%'
                },
                Ações: {
                    title: 'Ações',
                    width: '30%',
                    display: function (data) {
                        return `<button onclick="ExcluirItemBenef(${data.record.Id})" class="btn btn-primary btn-sm">Excluir</button>
                                <button onclick="AlterarItemBenef(${data.record.Id})" class="btn btn-primary btn-sm">Alterar</button>`;
                    }
                }
            }
        });

    //Load list when modal shown
    $('#exampleModal').on('shown.bs.modal', function () {
        if ($('#gridBeneficiarios').length) {
            $('#gridBeneficiarios').jtable('load');
        }
    });
})

function ExcluirItemBenef(id) {
    $.ajax({
        url: urlExcluirBeneficiario,
        method: "POST",
        data: { id: id },
        error:
            function (r) {
                if (r.status == 400)
                    ModalDialog("Ocorreu um erro", r.responseJSON);
                else if (r.status == 500)
                    ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
            },
        success:
            function (r) {
                ModalDialog("Item excluido com Sucesso!", r)
                if ($('#gridBeneficiarios').length) {
                    $('#gridBeneficiarios').jtable('load');
                }
            }
    });
}

function AlterarItemBenef(id) {
    // Busca o registro pelo id e preenche o form
    $.ajax({
        url: `${urlConsultaBeneficiario}`,
        method: "POST",
        data: { id: id },
        success:
            function (data) {
            if (data) {
                $('#formBenef #Nome').val(data.Nome);
                $('#formBenef #CPF').val(data.CPF);
                $('#formBenef #IdCliente').val(data.IdCliente);
                editMode = true;
                editId = id;
                $('#SalvaBenef').text('Alterar');
            }
        },
        error:
        function (r) {
            if (r.status == 404)
                ModalDialog("Ocorreu um erro", "Item não encontrado");
            else if (r.status == 500)
                ModalDialog("Ocorreu um erro", "Ocorreu um erro interno no servidor.");
        }
    });
}