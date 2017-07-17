var selPermissoesDualList;

function inicializarSelectPermissoes() {
    selPermissoesDualList = $('#selPermissoes').bootstrapDualListbox({
        nonSelectedListLabel: '<b>Dispon&iacute;veis</b>',
        selectedListLabel: '<b>Escolhidos</b>'
    });

    $('#selPermissoes').change(function () {
        var totSelecionadosAntes = $('#selPermissoes').data('totItensEscolhidos');
        var totSelecionadosAgora = $($('#selPermissoes')[0]).find('option:selected').length;
        var operacao = (totSelecionadosAgora > totSelecionadosAntes) ? 'INSERIR' : 'EXCLUIR';

        var listaStatus = [];
        if (permissoesValidas()) {
            listaStatus = $('#selPermissoes').val();
        }

        atualizarPermissoes(listaStatus);
    });
}

function atualizarSelectPermissoes() {
    var auxItensSelect = $($('#selPermissoes')[0]).find('option');
    var itensSelect = [];
    var indice = 0;
    var valueOption;
    var textOption;
    var optionSelected;
    for (var i = 0; i < auxItensSelect.length; i++) {
        valueOption = $(auxItensSelect[i]).val();
        textOption = $(auxItensSelect[i]).html();
        optionSelected = $(auxItensSelect[i]).prop('selected');
        if (optionSelected == true) {
            itensSelect[indice++] = $('<option selected value="' + valueOption + '" >' + textOption + '</option>');
        } else {
            itensSelect[indice++] = $('<option value="' + valueOption + '" >' + textOption + '</option>');
        }
    }
    $('#selPermissoes').data('totItensEscolhidos', $($('#selPermissoes')[0]).find('option:selected').length);
    $('#selPermissoes').data('itensSelect', itensSelect);
}

function permissoesValidas() {
    return null != $('#selPermissoes').val() && undefined != $('#selPermissoes').val();
}

function reverterSelectPermissoes() {
    var itensSelect = $('#selPermissoes').data('itensSelect');
    if (undefined != itensSelect) {
        $('#selPermissoes').empty();
        for (var i = 0; i < itensSelect.length; i++) {
            $('#selPermissoes').append($(itensSelect[i]));
        }
        selPermissoesDualList.bootstrapDualListbox('refresh');
    }
    atualizarSelectPermissoes();
}

function atualizarPermissoes(listaStatusForm) {
    mostrarPopup();

    $.ajax({
        url: 'PermissaoStatus/AtualizarPermissoes',
        type: 'POST',
        data: { nomeGrupo: $('#nomeGrupo').val(), idFormulario: $('#idFormulario').val(), listaStatus: listaStatusForm },
        success: function (result) {
            var msgErro = result.msgErro;
            if (undefined != msgErro && '' != msgErro.toString().trim()) {
                reverterSelectPermissoes();
                mostrarMsgErro(msgErro);
            } else {
                var msgSucesso = result.msgSucesso;
                if (undefined != msgSucesso && '' != msgSucesso.toString().trim()) {
                    mostrarMsgSucesso(msgSucesso);
                }
            }
            fecharPopup();
        },
        error: function (request, status, error) {
            reverterSelectPermissoes();
            mostrarMsgErro('Falha ao tentar atualizar as permissões, favor tente novamente');
            fecharPopup();
        }
    });
}

function obterPermissoes() {
    if (isNullOrEmpty($('#nomeGrupo').val())) {
        $('#dvSelPermissoes').css('display', 'none');
        $('#lblNenhumaPermissao').css('display', '');
    } else {
        mostrarPopup();

        $.ajax({
            url: 'PermissaoStatus/GetPorGrupoFormulario',
            type: 'POST',
            data: { nomeGrupo: $('#nomeGrupo').val(), idFormulario: $('#idFormulario').val() },
            success: function (result) {
                var dados = result.data;

                if (undefined != dados && dados.length > 0) {
                    if (undefined == selPermissoesDualList) {
                        inicializarSelectPermissoes();
                    }

                    $('#selPermissoes').empty();

                    var auxOption;
                    for (var i = 0; i < dados.length; i++) {
                        if (1 == dados[i].POSSUI_PERMISSAO) {
                            auxOption = $('<option value="' + dados[i].ID_STATUS + '" selected>' + dados[i].NOME_STATUS + '</option>');
                        } else {
                            auxOption = $('<option value="' + dados[i].ID_STATUS + '">' + dados[i].NOME_STATUS + '</option>');
                        }
                        $('#selPermissoes').append(auxOption);
                    }

                    atualizarSelectPermissoes();
                    selPermissoesDualList.bootstrapDualListbox('refresh');

                    $('#dvSelPermissoes').css('display', '');
                    $('#lblNenhumaPermissao').css('display', 'none');
                } else {
                    $('#dvSelPermissoes').css('display', 'none');
                    $('#lblNenhumaPermissao').css('display', '');
                }

                fecharPopup();
            },
            error: function (request, status, error) {
                mostrarMsgErro('Falha ao tentar obter as permissões, favor tente novamente');
                fecharPopup();
            }
        });
    }
}

function carregarGrupos() {
    mostrarPopup();

    var auxOption = $('<option value="">Informe o grupo</option>');
    $('#nomeGrupo').append(auxOption);

    $.ajax({
        url: 'Grupo/GetParaPermissao',
        type: 'POST',
        success: function (result) {
            var dados = result.data;
            $('#nomeGrupo').append(auxOption);
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].NOME + '">' + dados[i].NOME + '</option>');
                $('#nomeGrupo').append(auxOption);
            }

            $('#nomeGrupo').selectize().on('change', function () {
                $('div.selectize-input > input').blur();
                obterPermissoes();
            });

            fecharPopup();
        },
        error: function (request, status, error) {
            $('#nomeGrupo').selectize();
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os grupos, favor tente novamente');
        }
    });
}

function carregarFormularios() {
    mostrarPopup();

    var auxOption = $('<option value="">Informe o formulário</option>');
    $('#idFormulario').append(auxOption);

    $.ajax({
        url: 'Formularios/GetParaComponente',
        type: 'POST',
        success: function (result) {
            var dados = result.data;
            $('#idFormulario').append(auxOption);
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                $('#idFormulario').append(auxOption);
            }

            $('#idFormulario').selectize().on('change', function () {
                $('div.selectize-input > input').blur();
                obterPermissoes();
            });

            fecharPopup();
        },
        error: function (request, status, error) {
            $('#idFormulario').selectize();
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os formulários, favor tente novamente');
        }
    });
}

$(document).ready(function () {
    $('#btnExportarExcel').click(function () {
        exportarArquivo('PermissaoStatus/GetParaExcel', 'PermissaoStatus/ExportarExcel', 'Permissoes');
    });

    carregarGrupos();
    carregarFormularios();
});