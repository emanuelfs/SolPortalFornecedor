var tiposLitasObtidos = false;
var camposFormCadastro = {
    nomeItemLista: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    idTipoLista: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    }
};
var colunasTabelaPrincipal = [
    {
        data: 'NOME',
        name: 'NOME'
    },
    {
        data: 'tipoLista.NOME',
        name: 'NOME_TIPO_LISTA'
    },
    {
        data: 'itemPai.NOME',
        name: 'NOME_PAI'
    },
    {
        data: 'tipoLista.listaPai.NOME',
        name: 'NOME_TIPO_LISTA_PAI'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'ItensListas/Get';
metodoInsert = 'ItensListas/Insert';

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeProp('idRegistro');
    $('#nomeItemLista').val('');

    if (tiposLitasObtidos) {
        $('#idTipoLista')[0].selectize.setValue('');
        $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoLista', 'NOT_VALIDATED');
    }

    $('#idPai').removeData('ID_PAI');

    var listaSelects =
    [
        {
            registrosObtidos: tiposLitasObtidos,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarTiposLista
        }
    ];
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

function carregarFormCadastro(ID, NOME, ID_TIPO_LISTA, ID_PAI) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').prop('idRegistro', ID);
    $('#nomeItemLista').val(NOME);

    if (tiposLitasObtidos) {
        $('#idTipoLista')[0].selectize.setValue(ID_TIPO_LISTA);
        $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoLista', 'NOT_VALIDATED');
    }

    if (!isNullOrEmpty(ID_PAI) && 0 != ID_PAI) {
        $('#idPai').data('ID_PAI', ID_PAI);
    } else {
        $('#idPai').removeData('ID_PAI');
    }

    var listaSelects =
    [
        {
            registrosObtidos: tiposLitasObtidos,
            idRegistroEscolhido: ID_TIPO_LISTA,
            funcaoInicializacao: inicializarTiposLista
        }
    ];
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

//Setar para habilitar botao de excluir
metodoDelete = 'ItensListas/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'ID\': ' + objetoTabela.ID + ' }';
}
//Setar para habilitar botao de excluir

//Setar para habilitar o botao de alteracao
metodoUpdate = 'ItensListas/Update';
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.ID + '\', \'' + objetoTabela.NOME + '\', \'' + objetoTabela.tipoLista.ID + '\', \'' + objetoTabela.itemPai.ID + '\'';
}
//Setar para habilitar o botao de alteracao

function atualizarItensVinculacao(idTipoLista) {
    mostrarPopup();

    if (undefined != $('#idPai')[0].selectize) {
        $('#idPai')[0].selectize.destroy();
    }
    $('#idPai').html('');

    var auxOption = $('<option value="">Informe o item vinculado</option>');
    $('#idPai').append(auxOption);

    if (!isNullOrEmpty(idTipoLista)) {
        $.ajax({
            url: 'ItensListas/GetPorTipoListaPai',
            data: { ID_TIPO_LISTA: idTipoLista, ID_ITEM_LISTA: $('#btnSalvar').prop('idRegistro') },
            type: 'POST',
            success: function (result) {
                fecharPopup();

                var dados = result.data;
                $('#idPai').append(auxOption);
                for (var i = 0; i < dados.length; i++) {
                    auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                    $('#idPai').append(auxOption);
                }
                $('#idPai').selectize();

                var idItemPai = $('#idPai').data('ID_PAI');
                if (undefined == idItemPai) {
                    $('#idPai')[0].selectize.setValue('');
                } else {
                    $('#idPai')[0].selectize.setValue(idItemPai);
                }
            },
            error: function (request, status, error) {
                fecharPopup();
                $('#idPai').selectize();
                mostrarMsgErro('Falha ao tentar obter os itens para vincular, favor tente novamente');
            }
        });
    } else {
        fecharPopup();
        $('#idPai').selectize();
    }
}

function inicializarTiposLista(ID_TIPO_LISTA) {
    mostrarPopup();

    $.ajax({
        url: 'TiposListas/GetParaItem',
        type: 'POST',
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            var auxOption = $('<option value="">Informe a lista</option>');
            $('#idTipoLista').append(auxOption);
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                $('#idTipoLista').append(auxOption);
            }
            $('#idTipoLista').selectize();

            $('#idTipoLista').selectize().on('change', function () {
                $('div.selectize-input > input').blur();
                atualizarItensVinculacao($(this).val());
            });

            if (undefined == ID_TIPO_LISTA) {
                $('#idTipoLista')[0].selectize.setValue('');
                $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoLista', 'NOT_VALIDATED');
            } else {
                $('#idTipoLista')[0].selectize.setValue(ID_TIPO_LISTA);
                $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoLista', 'VALID');
            }

            tiposLitasObtidos = true;
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter as listas, favor tente novamente');
        }
    });
}

function montarDadosCadastro() {
    return {
        NOME: $('#nomeItemLista').val(),
        ID_TIPO_LISTA: $('#idTipoLista').val(),
        ID_PAI: $('#idPai').val()
    };
}

$(document).ready(function () {
    inicializarTabelaPrincipal($('#tabelaPrincipal'), colunasTabelaPrincipal);

    $('#idPai').selectize();

    options.fields = camposFormCadastro;
    $('#formCadastro').bootstrapValidator(options);

    $('#btnNovo').click(function () {
        limparFormCadastro();
    });

    $('#btnExportarExcel').click(function () {
        exportarArquivo('ItensListas/GetParaExcel', 'ItensListas/ExportarExcel', 'ItensListas');
    });

    $('#btnSalvar').click(function () {
        var auxUrl;
        var auxData = montarDadosCadastro();

        var auxId = $(this).prop('idRegistro');
        if (undefined == auxId) {
            auxUrl = metodoInsert;
        } else {
            auxUrl = metodoUpdate;
            auxData.ID = auxId;
        }

        salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData);
    });

    $('#btnSalvarContinuar').click(function () {
        var auxUrl = metodoInsert;
        var auxData = montarDadosCadastro();
        salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData, undefined, true);
    });

    $('#btnCancelar').click(function () {
        mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), undefined);
    });
});