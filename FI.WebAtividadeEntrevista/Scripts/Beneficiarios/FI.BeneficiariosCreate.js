// Array to store Beneficiarios locally
var beneficiariosArray = [];
var editMode = false;
var editId = null;

$(document).ready(function () {
    $('#formBenef').on('submit', function (e) {
        let $form = $('#formBenef')
        e.preventDefault();
        var nome = $form.find("#Nome").val();
        var cpf = $form.find("#CPF").val();
        var idCliente = $form.find("#IdCliente").val();
        var cpfExists = beneficiariosArray.some(function (b) {
            if (editMode) {
                return b.CPF === cpf && b.Id !== editId;
            } else {
                return b.CPF === cpf;
            }
        });
        if (cpfExists) {
            ModalDialog("Erro!", "Já existe um beneficiário com este CPF.");
            return;
        }
        if (editMode) {
            // Edit mode: Alterar
            var idx = beneficiariosArray.findIndex(function (b) { return b.Id === editId; });
            if (idx !== -1) {
                beneficiariosArray[idx].Nome = nome;
                beneficiariosArray[idx].CPF = cpf;
                beneficiariosArray[idx].IdCliente = idCliente;
            }
            ModalDialog("Sucesso!", "Beneficiário alterado com sucesso!");
            editMode = false;
            editId = null;
            $('#SalvaBenef').text('Salvar');
        } else {
            // Create mode: Incluir
            var newId = beneficiariosArray.length > 0 ? Math.max.apply(null, beneficiariosArray.map(function(b){return b.Id;})) + 1 : 1;
            beneficiariosArray.push({
                Id: newId,
                Nome: nome,
                CPF: cpf,
                IdCliente: idCliente
            });
            ModalDialog("Sucesso!", "Beneficiário incluído com sucesso!");
        }
        if ($('#gridBeneficiarios').length) {
            $('#gridBeneficiarios').jtable('load');
        }
        $('#formBenef')[0].reset();
    })

    if (document.getElementById("gridBeneficiarios"))
        $('#gridBeneficiarios').jtable({
            title: 'Beneficiarios',
            paging: false, //Enable paging
            sorting: true, //Enable sorting
            defaultSorting: 'Nome ASC', //Set default sorting
            actions: {
                listAction: function (postData, jtParams) {
                    // Return local array as jTable expects
                    return $.Deferred(function ($dfd) {
                        $dfd.resolve({ Result: 'OK', Records: beneficiariosArray });
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
    var idx = beneficiariosArray.findIndex(function (b) { return b.Id === id; });
    if (idx !== -1) {
        beneficiariosArray.splice(idx, 1);
        ModalDialog("Item excluído com Sucesso!", "Beneficiário removido da lista.");
        if ($('#gridBeneficiarios').length) {
            $('#gridBeneficiarios').jtable('load');
        }
    }
}

function AlterarItemBenef(id) {
    var benef = beneficiariosArray.find(function (b) { return b.Id === id; });
    if (benef) {
        $('#formBenef #Nome').val(benef.Nome);
        $('#formBenef #CPF').val(benef.CPF);
        $('#formBenef #IdCliente').val(benef.IdCliente);
        editMode = true;
        editId = id;
        $('#SalvaBenef').text('Alterar');
    } else {
        ModalDialog("Ocorreu um erro", "Item não encontrado");
    }
}