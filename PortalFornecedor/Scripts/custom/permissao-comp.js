var textoVazioStatus = 'Informe o status';
var textoVazioComponente = 'Informe o componente';
var gruposObtidos = false;
var formulariosObtidos = false;
var camposFormCadastro = {
    nomeGrupo: {
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
    idStatus: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    idComponente: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    idTipo: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    }
};
var colunasTabelaPrincipal = [
    {
        data: 'grupo.NOME',
        name: 'NOME_GRUPO'
    },
    {
        data: 'status.formulario.NOME',
        name: 'NOME_FORMULARIO'
    },
    {
        data: 'status.NOME',
        name: 'NOME_STATUS'
    },
    {
        data: 'componente.NOME',
        name: 'NOME_COMPONENTE'
    },
    {
        data: 'TEXTO_TIPO',
        name: 'TEXTO_TIPO'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'PermissaoComp/Get';
metodoInsert = 'PermissaoComp/Insert';

function limparValidacaoCombo(idCombo) {
    $('#' + idCombo)[0].selectize.setValue('');
    $('#formCadastro').data('bootstrapValidator').updateStatus(idCombo, 'NOT_VALIDATED');
}

function inicializarComboFilho(idCampo, textoVazio, iniciarSelectize) {
    if (undefined != $('#' + idCampo)[0].selectize) {
        limparValidacaoCombo(idCampo);
        $('#' + idCampo)[0].selectize.destroy();
    }
    $('#' + idCampo).html('<option value="">' + textoVazio + '</option>');
    if (iniciarSelectize) {
        $('#' + idCampo).selectize();
    }
}

function atualizarVisibilidadeCampos(somenteLeitura) {
    if (somenteLeitura) {
        $('#dvNomeGrupo').css('display', 'none');
        $('#dvFormulario').css('display', 'none');
        $('#dvStatus').css('display', 'none');
        $('#dvComponente').css('display', 'none');

        $('#dvNomeGrupoLeitura').css('display', '');
        $('#dvFormularioLeitura').css('display', '');
        $('#dvStatusLeitura').css('display', '');
        $('#dvComponenteLeitura').css('display', '');
    } else {
        $('#dvNomeGrupo').css('display', '');
        $('#dvFormulario').css('display', '');
        $('#dvStatus').css('display', '');
        $('#dvComponente').css('display', '');

        $('#dvNomeGrupoLeitura').css('display', 'none');
        $('#dvFormularioLeitura').css('display', 'none');
        $('#dvStatusLeitura').css('display', 'none');
        $('#dvComponenteLeitura').css('display', 'none');
    }
}

function limparFormCadastro() {
    atualizarVisibilidadeCampos(false);

    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeData('idRegistro');

    if (gruposObtidos) {
        $('#nomeGrupo')[0].selectize.setValue('');
        $('#formCadastro').data('bootstrapValidator').updateStatus('nomeGrupo', 'NOT_VALIDATED');
        $('#nomeGrupoLeitura').html('');
    }

    $('#idStatus').removeData('idStatusAlteracao');
    $('#idComponente').removeData('idComponenteAlteracao');

    if (formulariosObtidos) {
        limparValidacaoCombo('idFormulario');
        inicializarComboFilho('idStatus', textoVazioStatus, true);
        inicializarComboFilho('idComponente', textoVazioComponente, true);
        $('#idFormularioLeitura').html('');
    }

    $('#idTipo')[0].selectize.setValue('');
    $('#formCadastro').data('bootstrapValidator').updateStatus('idTipo', 'NOT_VALIDATED');

    var listaSelects =
    [
        {
            registrosObtidos: gruposObtidos,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarGrupos
        },
        {
            registrosObtidos: formulariosObtidos,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarFormularios
        }
    ];
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

function carregarFormCadastro(NOME_GRUPO, ID_FORMULARIO, ID_STATUS, ID_COMPONENTE, TIPO) {
    atualizarVisibilidadeCampos(true);

    $('#btnSalvarContinuar').css('display', 'none');

    $('#btnSalvar').data('idRegistro', {
        nomeGrupo: NOME_GRUPO,
        idStatus: ID_STATUS,
        idComponente: ID_COMPONENTE
    });

    if (gruposObtidos) {
        $('#nomeGrupo')[0].selectize.setValue(NOME_GRUPO);
        $('#formCadastro').data('bootstrapValidator').updateStatus('nomeGrupo', 'NOT_VALIDATED');
        $('#nomeGrupoLeitura').html(NOME_GRUPO);
    }

    $('#idStatus').data('idStatusAlteracao', ID_STATUS);
    $('#idComponente').data('idComponenteAlteracao', ID_COMPONENTE);

    if (formulariosObtidos) {
        $('#idFormulario')[0].selectize.setValue(ID_FORMULARIO);
        $('#formCadastro').data('bootstrapValidator').updateStatus('idFormulario', 'NOT_VALIDATED');
        $('#idFormularioLeitura').html($('#idFormulario').text());
    }

    $('#idTipo')[0].selectize.setValue(TIPO);
    $('#formCadastro').data('bootstrapValidator').updateStatus('idTipo', 'NOT_VALIDATED');

    var listaSelects =
    [
        {
            registrosObtidos: gruposObtidos,
            idRegistroEscolhido: NOME_GRUPO,
            funcaoInicializacao: inicializarGrupos
        },
        {
            registrosObtidos: formulariosObtidos,
            idRegistroEscolhido: ID_FORMULARIO,
            funcaoInicializacao: inicializarFormularios
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

//Setar para habilitar botao de excluir
metodoDelete = 'PermissaoComp/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'NOME_GRUPO\': \'' + objetoTabela.grupo.NOME + '\', \'ID_STATUS\': ' + objetoTabela.status.ID + ', \'ID_COMPONENTE\': ' + objetoTabela.componente.ID + ' }';
}
//Setar para habilitar botao de excluir


//Setar para habilitar o botao de alteracao
metodoUpdate = 'PermissaoComp/Update';
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.grupo.NOME + '\', \'' + objetoTabela.status.formulario.ID + '\', \'' + objetoTabela.status.ID + '\', \'' + objetoTabela.componente.ID + '\', \'' + objetoTabela.TIPO + '\'';
}
//Setar para habilitar o botao de alteracao

function inicializarGrupos(NOME_GRUPO) {
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

            $('#nomeGrupo').selectize();

            if (undefined == NOME_GRUPO) {
                $('#nomeGrupo')[0].selectize.setValue('');
                $('#formCadastro').data('bootstrapValidator').updateStatus('nomeGrupo', 'NOT_VALIDATED');
                $('#nomeGrupoLeitura').html('');
            } else {
                $('#nomeGrupo')[0].selectize.setValue(NOME_GRUPO);
                $('#formCadastro').data('bootstrapValidator').updateStatus('nomeGrupo', 'VALID');
                $('#nomeGrupoLeitura').html(NOME_GRUPO);
            }

            gruposObtidos = true;

            fecharPopup();
        },
        error: function (request, status, error) {
            $('#nomeGrupo').selectize();
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os grupos, favor tente novamente');
        }
    });
}

function obterComponentesFormulario(idFormulario) {
    mostrarPopup();

    $.ajax({
        url: 'Componentes/GetPorformulario',
        type: 'POST',
        data: { ID_FORMULARIO: idFormulario },
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            if (dados.length > 0) {
                var auxOption;
                var idComponenteAlteracao = $('#idComponente').data('idComponenteAlteracao');
                for (var i = 0; i < dados.length; i++) {
                    auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                    if (idComponenteAlteracao == dados[i].ID) {
                        auxOption.attr('selected', 'selected');
                    }
                    $('#idComponente').append(auxOption);
                }
            } else {
                mostrarMsgErro('Nenhum componente foi encontrado');
            }
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os componentes, favor tente novamente');
        },
        complete: function () {
            $('#idComponente').selectize();
            $('#idComponenteLeitura').html($('#idComponente').text());
        }
    });
}

function obterStatusFormulario(idFormulario) {
    mostrarPopup();

    $.ajax({
        url: 'StatusFormulario/GetPorformulario',
        type: 'POST',
        data: { ID_FORMULARIO: idFormulario },
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            if (dados.length > 0) {
                var auxOption;
                var idStatusAlteracao = $('#idStatus').data('idStatusAlteracao');
                for (var i = 0; i < dados.length; i++) {
                    auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                    if (idStatusAlteracao == dados[i].ID) {
                        auxOption.attr('selected', 'selected');
                    }
                    $('#idStatus').append(auxOption);
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
            $('#idStatus').selectize();
            $('#idStatusLeitura').html($('#idStatus').text());
            obterComponentesFormulario(idFormulario);
        }
    });
}

function inicializarFormularios(ID_FORMULARIO) {
    mostrarPopup();

    var auxOption = $('<option value="">Informe o formulário</option>');
    $('#idFormulario').append(auxOption);

    inicializarComboFilho('idStatus', textoVazioStatus, true);
    inicializarComboFilho('idComponente', textoVazioComponente, true);

    $.ajax({
        url: 'Formularios/GetParaComponente',
        type: 'POST',
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                $('#idFormulario').append(auxOption);
            }

            $('#idFormulario').selectize().on('change', function () {
                if (!isNullOrEmpty($(this).val())) {
                    $('div.selectize-input > input').blur();
                    inicializarComboFilho('idStatus', textoVazioStatus, false);
                    inicializarComboFilho('idComponente', textoVazioComponente, false);
                    obterStatusFormulario($(this).val());
                }
            });

            if (undefined == ID_FORMULARIO) {
                limparValidacaoCombo('idFormulario');
                $('#idFormularioLeitura').html('');
            } else {
                $('#idFormulario')[0].selectize.setValue(ID_FORMULARIO);
                $('#formCadastro').data('bootstrapValidator').updateStatus('idFormulario', 'VALID');
                $('#idFormularioLeitura').html($('#idFormulario').text());
            }

            formulariosObtidos = true;
        },
        error: function (request, status, error) {
            $('#idFormulario').selectize();
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os formulários, favor tente novamente');
        }
    });
}

function montarDadosCadastro() {
    return {
        NOME_GRUPO: $('#nomeGrupo').val(),
        ID_STATUS: $('#idStatus').val(),
        ID_COMPONENTE: $('#idComponente').val(),
        TIPO: $('#idTipo').val()
    };
}

function salvarFormulario(continuar) {
    var auxUrl;
    var auxData = montarDadosCadastro();

    var auxId = $('#btnSalvar').data('idRegistro');
    if (undefined == auxId) {
        auxUrl = metodoInsert;
    } else {
        auxUrl = metodoUpdate;
    }

    salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData, undefined, continuar);
}

$(document).ready(function () {
    inicializarTabelaPrincipal($('#tabelaPrincipal'), colunasTabelaPrincipal);

    options.fields = camposFormCadastro;
    $('#formCadastro').bootstrapValidator(options);

    $('#idTipo').selectize();
    $('#idTipo')[0].selectize.setValue('');
    $('#formCadastro').data('bootstrapValidator').updateStatus('idTipo', 'NOT_VALIDATED');

    $('#btnNovo').click(function () {
        limparFormCadastro();
    });

    $('#btnExportarExcel').click(function () {
        exportarArquivo('PermissaoComp/GetParaExcel', 'PermissaoComp/ExportarExcel', 'Permissoes');
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