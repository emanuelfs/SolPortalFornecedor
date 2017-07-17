var formulariosObtidos = false;
var camposFormCadastro = {
    idFormulario: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    idStatusOrigem: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    idStatusDestino: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    }
};
var colunasTabelaPrincipal = [
    {
        data: 'statusOrigem.NOME',
        name: 'NOME_STATUS_ORIGEM'
    },
    {
        data: 'statusDestino.NOME',
        name: 'NOME_STATUS_DESTINO'
    },
    {
        data: 'statusOrigem.formulario.NOME',
        name: 'NOME_FORMULARIO'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'FluxoStatus/Get';
metodoInsert = 'FluxoStatus/Insert';

function limparValidacaoCombo(idCombo) {
    $('#' + idCombo)[0].selectize.setValue('');
    $('#formCadastro').data('bootstrapValidator').updateStatus(idCombo, 'NOT_VALIDATED');
}

function inicializarCombosStatus(iniciarSelectize) {
    if (undefined != $('#idStatusOrigem')[0].selectize) {
        limparValidacaoCombo('idStatusOrigem');
        limparValidacaoCombo('idStatusDestino');
        $('#idStatusOrigem')[0].selectize.destroy();
        $('#idStatusDestino')[0].selectize.destroy();
    }
    $('#idStatusOrigem').html('<option value="">Informe o status de origem</option>');
    $('#idStatusDestino').html('<option value="">Informe o status de destino</option>');
    if (iniciarSelectize) {
        $('#idStatusOrigem').selectize();
        $('#idStatusDestino').selectize();
    }
}

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');

    if (formulariosObtidos) {
        limparValidacaoCombo('idFormulario');
        inicializarCombosStatus(true);
    }

    var listaSelects =
    [
        {
            registrosObtidos: formulariosObtidos,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarFormularios
        }
    ];
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

//Setar para habilitar botao de excluir
metodoDelete = 'FluxoStatus/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'ID_STATUS_ORIGEM\': ' + objetoTabela.statusOrigem.ID + ', \'ID_STATUS_DESTINO\': ' + objetoTabela.statusDestino.ID + ' }';
}
//Setar para habilitar botao de excluir

function obterStatusFormulario(idFormulario) {
    mostrarPopup();
    fecharMsgErro();
    $.ajax({
        url: 'StatusFormulario/GetPorformulario',
        type: 'POST',
        data: { ID_FORMULARIO: idFormulario },
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            if (dados.length > 0) {
                var auxOptionOrigem;
                var auxOptionDestino;
                for (var i = 0; i < dados.length; i++) {
                    auxOptionOrigem = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                    auxOptionDestino = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                    $('#idStatusOrigem').append(auxOptionOrigem);
                    $('#idStatusDestino').append(auxOptionDestino);
                }
            } else {
                mostrarMsgErro('Nenhum status foi encontrado');
            }
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os status, favor tente novamente');
        },
        complete: function () {
            $('#idStatusOrigem').selectize();
            $('#idStatusDestino').selectize();
        }
    });
}

function inicializarFormularios(ID_FORMULARIO) {
    mostrarPopup();

    $.ajax({
        url: 'Formularios/GetParaComponente',
        type: 'POST',
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            var auxOption = $('<option value="">Informe o formulário</option>');
            $('#idFormulario').append(auxOption);
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                $('#idFormulario').append(auxOption);
            }
            $('#idFormulario').selectize().on('change', function () {
                if (!isNullOrEmpty($(this).val())) {
                    $('div.selectize-input > input').blur();
                    inicializarCombosStatus(false);
                    obterStatusFormulario($(this).val());
                }
            });

            limparValidacaoCombo('idFormulario');
            inicializarCombosStatus(true);

            formulariosObtidos = true;
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os formulários, favor tente novamente');
        }
    });
}

function montarDadosCadastro() {
    return {
        ID_FORMULARIO: $('#idFormulario').val(),
        ID_STATUS_ORIGEM: $('#idStatusOrigem').val(),
        ID_STATUS_DESTINO: $('#idStatusDestino').val()
    };
}

function salvarFormulario(continuar) {
    if ($('#idStatusOrigem').val() == $('#idStatusDestino').val()) {
        mostrarMsgErro('Favor informar o status de origem diferente do status de destino');
    } else {
        var auxUrl = metodoInsert;
        var auxData = montarDadosCadastro();
        salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData, undefined, continuar);
    }
}

$(document).ready(function () {
    inicializarTabelaPrincipal($('#tabelaPrincipal'), colunasTabelaPrincipal);

    options.fields = camposFormCadastro;
    $('#formCadastro').bootstrapValidator(options);

    $('#btnNovo').click(function () {
        limparFormCadastro();
    });

    $('#btnExportarExcel').click(function () {
        exportarArquivo('FluxoStatus/GetParaExcel', 'FluxoStatus/ExportarExcel', 'FluxosFormularios');
    });

    $('#btnSalvar').click(function () {
        salvarFormulario(false);
    });

    $('#btnSalvarContinuar').click(function () {
        salvarFormulario(true);
    });

    $('#btnCancelar').click(function () {
        mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), undefined);
    });
});