var formulariosObtidos = false;
var camposFormCadastro = {
    nomeStatus: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    idFormulario: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    statusInicial: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    statusRetorno: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    enviarEmail: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    tituloEmail: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    corpoEmail: {
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
        data: 'formulario.NOME',
        name: 'NOME_FORMULARIO'
    },
    {
        data: 'TEXTO_INICIAL',
        name: 'TEXTO_INICIAL'
    },
    {
        data: 'TEXTO_RETORNO',
        name: 'TEXTO_RETORNO'
    },
    {
        data: 'TEXTO_ENVIAR_EMAIL',
        name: 'TEXTO_ENVIAR_EMAIL'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'StatusFormulario/Get';
metodoInsert = 'StatusFormulario/Insert';

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeProp('idRegistro');
    $('#nomeStatus').val('');

    var statusInicial = $('input[type=radio][name=statusInicial]:checked');
    statusInicial.attr("checked", false);
    statusInicial.removeProp('checked');

    var statusRetorno = $('input[type=radio][name=statusRetorno]:checked');
    statusRetorno.attr("checked", false);
    statusRetorno.removeProp('checked');

    var enviarEmail = $('input[type=radio][name=enviarEmail]:checked');
    enviarEmail.attr("checked", false);
    enviarEmail.removeProp('checked');

    $('#dvDadosEmail').css('display', 'none');
    $('#tituloEmail').val('');
    $('#corpoEmail').val('');

    if (formulariosObtidos) {
        $('#idFormulario')[0].selectize.setValue('');
        $('#formCadastro').data('bootstrapValidator').updateStatus('idFormulario', 'NOT_VALIDATED');
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

function carregarDadosStatusParaAlteracao(ID, NOME, INICIAL, RETORNO, ENVIAR_EMAIL, TITULO_EMAIL, CORPO_EMAIL, ID_FORMULARIO) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').prop('idRegistro', ID);
    $('#nomeStatus').val(NOME);

    var statusInicial = $('input[type=radio][name=statusInicial][value=' + INICIAL + ']');
    statusInicial.attr("checked", true);
    statusInicial.prop('checked', 'checked');

    var statusRetorno = $('input[type=radio][name=statusRetorno][value=' + RETORNO + ']');
    statusRetorno.attr("checked", true);
    statusRetorno.prop('checked', 'checked');

    var enviarEmail = $('input[type=radio][name=enviarEmail][value=' + ENVIAR_EMAIL + ']');
    enviarEmail.attr("checked", true);
    enviarEmail.prop('checked', 'checked');

    $('#dvDadosEmail').css('display', 1 == ENVIAR_EMAIL ? '' : 'none');
    $('#tituloEmail').val(TITULO_EMAIL);
    $('#corpoEmail').val(CORPO_EMAIL);

    if (formulariosObtidos) {
        $('#idFormulario')[0].selectize.setValue(ID_FORMULARIO);
        $('#formCadastro').data('bootstrapValidator').updateStatus('idFormulario', 'NOT_VALIDATED');
    }

    var listaSelects =
    [
        {
            registrosObtidos: formulariosObtidos,
            idRegistroEscolhido: ID_FORMULARIO,
            funcaoInicializacao: inicializarFormularios
        }
    ];
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

function obterCorpoEmailParaAlteracao(ID, NOME, INICIAL, RETORNO, ENVIAR_EMAIL, TITULO_EMAIL, ID_FORMULARIO) {
    if (1 == ENVIAR_EMAIL) {
        mostrarPopup();
        $.ajax({
            url: 'StatusFormulario/GetCorpoEmailParaAlteracao',
            type: 'POST',
            data: { idStatus: ID },
            success: function (result) {
                carregarDadosStatusParaAlteracao(ID, NOME, INICIAL, RETORNO, ENVIAR_EMAIL, TITULO_EMAIL, result.corpoEmail, ID_FORMULARIO);
                fecharPopup();
            },
            error: function (request, status, error) {
                fecharPopup();
                mostrarMsgErro('Falha ao tentar obter os dados do status, favor tente novamente');
            }
        });
    } else {
        carregarDadosStatusParaAlteracao(ID, NOME, INICIAL, RETORNO, ENVIAR_EMAIL, TITULO_EMAIL, '', ID_FORMULARIO);
    }
}

function carregarFormCadastro(ID, NOME, INICIAL, RETORNO, ENVIAR_EMAIL, TITULO_EMAIL, ID_FORMULARIO) {
    obterCorpoEmailParaAlteracao(ID, NOME, INICIAL, RETORNO, ENVIAR_EMAIL, TITULO_EMAIL, ID_FORMULARIO);
}

//Setar para habilitar botao de excluir
metodoDelete = 'StatusFormulario/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'ID\': ' + objetoTabela.ID + ' }';
}
//Setar para habilitar botao de excluir

//Setar para habilitar o botao de alteracao
metodoUpdate = 'StatusFormulario/Update';
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.ID + '\', \'' + objetoTabela.NOME + '\', \'' + objetoTabela.INICIAL + '\', \'' + objetoTabela.RETORNO + '\', \'' + objetoTabela.ENVIAR_EMAIL + '\', \'' + objetoTabela.TITULO_EMAIL + '\', \'' + objetoTabela.formulario.ID + '\'';
}
//Setar para habilitar o botao de alteracao

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
            $('#idFormulario').selectize();

            if (undefined == ID_FORMULARIO) {
                $('#idFormulario')[0].selectize.setValue('');
                $('#formCadastro').data('bootstrapValidator').updateStatus('idFormulario', 'NOT_VALIDATED');
            } else {
                $('#idFormulario')[0].selectize.setValue(ID_FORMULARIO);
                $('#formCadastro').data('bootstrapValidator').updateStatus('idFormulario', 'VALID');
            }

            formulariosObtidos = true;
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os formulários, favor tente novamente');
        }
    });
}

function montarDadosCadastro(statusInicial, statusRetorno, enviarEmail) {
    var tituloEmail = undefined;
    var corpoEmail = undefined;
    if (1 == enviarEmail.val()) {
        tituloEmail = $('#tituloEmail').val();
        corpoEmail = $('#corpoEmail').val();
    } else {
        $('#formCadastro').data('bootstrapValidator').updateStatus('tituloEmail', 'VALID');
        $('#formCadastro').data('bootstrapValidator').updateStatus('corpoEmail', 'VALID');
    }
    return {
        NOME: $('#nomeStatus').val(),
        ID_FORMULARIO: $('#idFormulario').val(),
        INICIAL: statusInicial.val(),
        RETORNO: statusRetorno.val(),
        ENVIAR_EMAIL: enviarEmail.val(),
        TITULO_EMAIL: tituloEmail,
        CORPO_EMAIL: corpoEmail
    };
}

$(document).ready(function () {
    inicializarTabelaPrincipal($('#tabelaPrincipal'), colunasTabelaPrincipal);

    options.fields = camposFormCadastro;
    $('#formCadastro').bootstrapValidator(options);

    $('input[type=radio][name=enviarEmail]').change(function () {
        var enviarEmail = $(this).val();
        if (1 == enviarEmail) {
            if ('none' == $('#dvDadosEmail').css('display')) {
                $('#dvDadosEmail').slideToggle();
            }
        } else {
            if ('none' != $('#dvDadosEmail').css('display')) {
                $('#dvDadosEmail').slideToggle();
            }
        }
    });

    $('#btnNovo').click(function () {
        limparFormCadastro();
    });

    $('#btnExportarExcel').click(function () {
        exportarArquivo('StatusFormulario/GetParaExcel', 'StatusFormulario/ExportarExcel', 'StatusFormularios');
    });

    $('#btnSalvar').click(function () {
        var statusInicial = $('input[type=radio][name=statusInicial]:checked');
        var statusRetorno = $('input[type=radio][name=statusRetorno]:checked');
        var enviarEmail = $('input[type=radio][name=enviarEmail]:checked');

        if (1 == statusInicial.val() && statusInicial.val() == statusRetorno.val()) {
            mostrarMsgErro('Um status não pode ser inicial e de retorno ao mesmo tempo');
        } else {
            var auxUrl;
            var auxData = montarDadosCadastro(statusInicial, statusRetorno, enviarEmail);

            var auxId = $(this).prop('idRegistro');
            if (undefined == auxId) {
                auxUrl = metodoInsert;
            } else {
                auxUrl = metodoUpdate;
                auxData.ID = auxId;
            }

            salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData);
        }
    });

    $('#btnSalvarContinuar').click(function () {
        var statusInicial = $('input[type=radio][name=statusInicial]:checked');
        var statusRetorno = $('input[type=radio][name=statusRetorno]:checked');
        var enviarEmail = $('input[type=radio][name=enviarEmail]:checked');

        if (1 == statusInicial.val() && statusInicial.val() == statusRetorno.val()) {
            mostrarMsgErro('Um status não pode ser inicial e de retorno ao mesmo tempo');
        } else {
            var auxUrl = metodoInsert;
            var auxData = montarDadosCadastro(statusInicial, statusRetorno, enviarEmail);
            salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData, undefined, true);
        }
    });

    $('#btnCancelar').click(function () {
        mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), undefined);
    });
});