var tiposComponentesObtidos = false;
var tiposValoresComponentesObtidos = false;
var formulariosObtidos = false;
var tiposLitasObtidos = false;
var mascarasObtidas = false;
var validadoresObtidos = false;
var camposFormCadastro = {
    nomeComponente: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    ordemComponente: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    tamanhoComponente: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    obrigaComponente: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    exibirComponenteNoGrid: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    exibirComponenteNoLancamento: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    exibirComponenteNoAtendimento: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    exibirComponenteNaBuscaAvancada: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    caixaAlta: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    idTipoComponente: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    idTipoValorComponente: {
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
        data: 'tipoComponente.NOME',
        name: 'NOME_TIPO_COMPONENTE'
    },
    {
        data: 'formulario.NOME',
        name: 'NOME_FORMULARIO'
    },
    {
        data: 'tipoLista.NOME',
        name: 'NOME_TIPO_LISTA'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'Componentes/Get';
metodoInsert = 'Componentes/Insert';

function limparFormCadastro() {
    var valorPadraoSomenteLeitura = '';
    var dadosSomenteLeitura = {
        nome: valorPadraoSomenteLeitura,
        descricao: valorPadraoSomenteLeitura,
        nomeFormulario: valorPadraoSomenteLeitura,
        nomeTipoComponente: valorPadraoSomenteLeitura,
        nomeTipoLista: valorPadraoSomenteLeitura,
        tamanhoComponente: valorPadraoSomenteLeitura,
        nomeTipoValorComponente: valorPadraoSomenteLeitura,
        nomeMascara: valorPadraoSomenteLeitura,
        nomeValidador: valorPadraoSomenteLeitura
    };
    $('#idTipoComponente').data('somenteLeitura', false);
    $('#idTipoComponente').data('dadosSomenteLeitura', dadosSomenteLeitura);

    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeProp('idRegistro');
    $('#nomeComponente').val('');
    $('#descricaoComponente').val('');
    $('#ordemComponente').val('');
    $('#tamanhoComponente').val('');

    var obrigaComponente = $('input[type=radio][name=obrigaComponente]:checked');
    obrigaComponente.attr("checked", false);
    obrigaComponente.removeProp('checked');

    var exibirComponenteNoGrid = $('input[type=radio][name=exibirComponenteNoGrid]:checked');
    exibirComponenteNoGrid.attr("checked", false);
    exibirComponenteNoGrid.removeProp('checked');

    var exibirComponenteNoLancamento = $('input[type=radio][name=exibirComponenteNoLancamento]:checked');
    exibirComponenteNoLancamento.attr("checked", false);
    exibirComponenteNoLancamento.removeProp('checked');

    var exibirComponenteNoAtendimento = $('input[type=radio][name=exibirComponenteNoAtendimento]:checked');
    exibirComponenteNoAtendimento.attr("checked", false);
    exibirComponenteNoAtendimento.removeProp('checked');

    var exibirComponenteNaBuscaAvancada = $('input[type=radio][name=exibirComponenteNaBuscaAvancada]:checked');
    exibirComponenteNaBuscaAvancada.attr("checked", false);
    exibirComponenteNaBuscaAvancada.removeProp('checked');

    var caixaAlta = $('input[type=radio][name=caixaAlta]:checked');
    caixaAlta.attr("checked", false);
    caixaAlta.removeProp('checked');

    if (tiposComponentesObtidos) {
        $('#idTipoComponente')[0].selectize.setValue('');
        $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoComponente', 'NOT_VALIDATED');
    }
    if (tiposValoresComponentesObtidos) {
        //$('#idTipoValorComponente')[0].selectize.setValue('');
        $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoValorComponente', 'NOT_VALIDATED');
    }
    if (formulariosObtidos) {
        $('#idFormulario')[0].selectize.setValue('');
        $('#formCadastro').data('bootstrapValidator').updateStatus('idFormulario', 'NOT_VALIDATED');
    }
    if (tiposLitasObtidos) {
        $('#idTipoLista')[0].selectize.setValue('');
        $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoLista', 'NOT_VALIDATED');
    }
    if (mascarasObtidas) {
        $('#idMascara')[0].selectize.setValue('');
        //$('#formCadastro').data('bootstrapValidator').updateStatus('idMascara', 'NOT_VALIDATED');
    }
    if (validadoresObtidos) {
        $('#idValidador')[0].selectize.setValue('');
        //$('#formCadastro').data('bootstrapValidator').updateStatus('idValidador', 'NOT_VALIDATED');
    }

    $('#dvTipoLista').css('display', 'none');
    $('#dvNomeTipoLista').css('display', 'none');
    $('#dvTamanhoComponente').css('display', 'none');
    $('#dvTamanhoComponenteLeitura').css('display', 'none');
    $('#dvMascara').css('display', 'none');
    $('#dvNomeMascara').css('display', 'none');
    $('#dvValidador').css('display', 'none');
    $('#dvNomeValidador').css('display', 'none');
    $('#dvTipoValorComponente').css('display', 'none');
    $('#dvNomeTipoValorComponente').css('display', 'none');

    var listaSelects =
    [
        {
            registrosObtidos: tiposLitasObtidos,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarTiposLista
        },
        {
            registrosObtidos: tiposComponentesObtidos,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarTiposComponentes
        },
        {
            registrosObtidos: tiposValoresComponentesObtidos,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarTiposValoresComponentes
        },
        {
            registrosObtidos: formulariosObtidos,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarFormularios
        },
        {
            registrosObtidos: mascarasObtidas,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarMascaras
        },
        {
            registrosObtidos: validadoresObtidos,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarValidadores
        }
    ];

    ajustarCamposDvFormCadastro(false, dadosSomenteLeitura);
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

function ajustarCampo(divCampo, divCampoLeitura, campoLeitura, valor, somenteLeitura) {
    campoLeitura.html(valor);
    if (somenteLeitura) {
        divCampo.css('display', 'none');
        divCampoLeitura.css('display', '');
    } else {
        divCampoLeitura.css('display', 'none');
        divCampo.css('display', '');
    }
}

function ajustarCamposDvFormCadastro(somenteLeitura, dadosSomenteLeitura) {
    ajustarCampo($('#dvNomeComponente'), $('#dvNomeComponenteLeitura'), $('#nomeComponenteLeitura'), dadosSomenteLeitura.nome, somenteLeitura);
    ajustarCampo($('#dvDescricaoComponente'), $('#dvDescricaoComponenteLeitura'), $('#descricaoComponenteLeitura'), dadosSomenteLeitura.descricao, somenteLeitura);
    ajustarCampo($('#dvIdFormulario'), $('#dvNomeFormulario'), $('#nomeFormulario'), dadosSomenteLeitura.nomeFormulario, somenteLeitura);
    ajustarCampo($('#dvIdTipoComponente'), $('#dvNomeTipoComponente'), $('#nomeTipoComponente'), dadosSomenteLeitura.nomeTipoComponente, somenteLeitura);
}

function carregarFormCadastro(ID, NOME, DESCRICAO, ORDEM, TAMANHO, OBRIGATORIEDADE, EXIBIR_NO_GRID, EXIBIR_NO_LANCAMENTO, EXIBIR_NO_ATENDIMENTO, EXIBIR_NA_BUSCA_AVANCADA, CAIXA_ALTA, ID_MASCARA_COMPONENTE, NOME_MASCARA_COMPONENTE, ID_VALIDADOR_COMPONENTE, NOME_VALIDADOR_COMPONENTE, ID_TIPO_COMPONENTE, NOME_TIPO_COMPONENTE, CODIGO_TIPO_COMPONENTE, ID_TIPO_VALOR_COMPONENTE, CODIGO_TIPO_VALOR_COMPONENTE, NOME_TIPO_VALOR_COMPONENTE, ID_FORMULARIO, NOME_FORMULARIO, ID_TIPO_LISTA, NOME_TIPO_LISTA, componentePreenchido) {
    var somenteLeitura = componentePreenchido == 1;
    var dadosSomenteLeitura = {
        nome: NOME,
        descricao: DESCRICAO,
        nomeFormulario: NOME_FORMULARIO,
        nomeTipoComponente: NOME_TIPO_COMPONENTE,
        nomeTipoLista: NOME_TIPO_LISTA,
        tamanhoComponente: TAMANHO,
        nomeTipoValorComponente: NOME_TIPO_VALOR_COMPONENTE,
        nomeMascara: NOME_MASCARA_COMPONENTE,
        nomeValidador: NOME_VALIDADOR_COMPONENTE
    };
    $('#idTipoComponente').data('somenteLeitura', somenteLeitura);
    $('#idTipoComponente').data('dadosSomenteLeitura', dadosSomenteLeitura);

    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').prop('idRegistro', ID);
    $('#nomeComponente').val(NOME);
    $('#descricaoComponente').val(DESCRICAO);
    $('#ordemComponente').val(ORDEM);
    $('#tamanhoComponente').val(TAMANHO);

    var obrigaComponente = $('input[type=radio][name=obrigaComponente][value=' + OBRIGATORIEDADE + ']');
    obrigaComponente.attr("checked", true);
    obrigaComponente.prop('checked', 'checked');

    var exibirComponenteNoGrid = $('input[type=radio][name=exibirComponenteNoGrid][value=' + EXIBIR_NO_GRID + ']');
    exibirComponenteNoGrid.attr("checked", true);
    exibirComponenteNoGrid.prop('checked', 'checked');

    var exibirComponenteNoLancamento = $('input[type=radio][name=exibirComponenteNoLancamento][value=' + EXIBIR_NO_LANCAMENTO + ']');
    exibirComponenteNoLancamento.attr("checked", true);
    exibirComponenteNoLancamento.prop('checked', 'checked');

    var exibirComponenteNoAtendimento = $('input[type=radio][name=exibirComponenteNoAtendimento][value=' + EXIBIR_NO_ATENDIMENTO + ']');
    exibirComponenteNoAtendimento.attr("checked", true);
    exibirComponenteNoAtendimento.prop('checked', 'checked');

    var exibirComponenteNaBuscaAvancada = $('input[type=radio][name=exibirComponenteNaBuscaAvancada][value=' + EXIBIR_NA_BUSCA_AVANCADA + ']');
    exibirComponenteNaBuscaAvancada.attr("checked", true);
    exibirComponenteNaBuscaAvancada.prop('checked', 'checked');

    var caixaAlta = $('input[type=radio][name=caixaAlta][value=' + CAIXA_ALTA + ']');
    caixaAlta.attr("checked", true);
    caixaAlta.prop('checked', 'checked');

    if (tiposComponentesObtidos) {
        $('#idTipoComponente')[0].selectize.setValue(ID_TIPO_COMPONENTE + ';' + CODIGO_TIPO_COMPONENTE);
        $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoComponente', 'NOT_VALIDATED');
    }
    if (tiposValoresComponentesObtidos) {
        $('#idTipoValorComponente')[0].selectize.setValue(ID_TIPO_VALOR_COMPONENTE + ';' + CODIGO_TIPO_VALOR_COMPONENTE);
        $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoValorComponente', 'NOT_VALIDATED');
    }
    if (formulariosObtidos) {
        $('#idFormulario')[0].selectize.setValue(ID_FORMULARIO);
        $('#formCadastro').data('bootstrapValidator').updateStatus('idFormulario', 'NOT_VALIDATED');
    }
    if (tiposLitasObtidos) {
        if (undefined != ID_TIPO_LISTA && 0 != ID_TIPO_LISTA) {
            $('#idTipoLista')[0].selectize.setValue(ID_TIPO_LISTA);
            $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoLista', 'NOT_VALIDATED');
        } else {
            $('#idTipoLista')[0].selectize.setValue('');
            $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoLista', 'VALID');
        }
    }
    if (mascarasObtidas) {
        if (undefined != ID_MASCARA_COMPONENTE && 0 != ID_MASCARA_COMPONENTE) {
            $('#idMascara')[0].selectize.setValue(ID_MASCARA_COMPONENTE);
            //$('#formCadastro').data('bootstrapValidator').updateStatus('idMascara', 'NOT_VALIDATED');
        } else {
            $('#idMascara')[0].selectize.setValue('');
            //$('#formCadastro').data('bootstrapValidator').updateStatus('idMascara', 'VALID');
        }
    }
    if (validadoresObtidos) {
        if (undefined != ID_VALIDADOR_COMPONENTE && 0 != ID_VALIDADOR_COMPONENTE) {
            $('#idValidador')[0].selectize.setValue(ID_VALIDADOR_COMPONENTE);
            //$('#formCadastro').data('bootstrapValidator').updateStatus('idValidador', 'NOT_VALIDATED');
        } else {
            $('#idValidador')[0].selectize.setValue('');
            //$('#formCadastro').data('bootstrapValidator').updateStatus('idValidador', 'VALID');
        }
    }

    var listaSelects =
    [
        {
            registrosObtidos: tiposLitasObtidos,
            idRegistroEscolhido: ID_TIPO_LISTA,
            funcaoInicializacao: inicializarTiposLista
        },
        {
            registrosObtidos: tiposComponentesObtidos,
            idRegistroEscolhido: ID_TIPO_COMPONENTE + ';' + CODIGO_TIPO_COMPONENTE,
            funcaoInicializacao: inicializarTiposComponentes
        },
        {
            registrosObtidos: tiposValoresComponentesObtidos,
            idRegistroEscolhido: ID_TIPO_VALOR_COMPONENTE + ';' + CODIGO_TIPO_VALOR_COMPONENTE,
            funcaoInicializacao: inicializarTiposValoresComponentes
        },
        {
            registrosObtidos: formulariosObtidos,
            idRegistroEscolhido: ID_FORMULARIO,
            funcaoInicializacao: inicializarFormularios
        },
        {
            registrosObtidos: mascarasObtidas,
            idRegistroEscolhido: ID_MASCARA_COMPONENTE,
            funcaoInicializacao: inicializarMascaras
        },
        {
            registrosObtidos: validadoresObtidos,
            idRegistroEscolhido: ID_VALIDADOR_COMPONENTE,
            funcaoInicializacao: inicializarValidadores
        }
    ];

    ajustarCamposDvFormCadastro(somenteLeitura, dadosSomenteLeitura);
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

//Setar para habilitar botao de excluir
metodoDelete = 'Componentes/Delete';
function colunasTabelaRemocao(objetoTabela) {
    var somenteLeitura = objetoTabela.componentePreenchido == 1;
    if (somenteLeitura) {
        return '';
    }
    return '{ \'ID\': ' + objetoTabela.ID + ' }';
}
//Setar para habilitar botao de excluir

//Setar para habilitar o botao de alteracao
metodoUpdate = 'Componentes/Update';
function colunasTabelaAlteracao(objetoTabela) {
    var auxTamanhoComponente =
        (null == objetoTabela.TAMANHO || undefined == objetoTabela.TAMANHO) ?
        '' : objetoTabela.TAMANHO
    return '\'' + objetoTabela.ID + '\', \'' + objetoTabela.NOME + '\', \'' + objetoTabela.DESCRICAO + '\', \'' + objetoTabela.ORDEM + '\', \'' + auxTamanhoComponente + '\', \'' + objetoTabela.OBRIGATORIEDADE + '\', \'' + objetoTabela.EXIBIR_NO_GRID + '\', \'' + objetoTabela.EXIBIR_NO_LANCAMENTO + '\', \'' + objetoTabela.EXIBIR_NO_ATENDIMENTO + '\', \'' + objetoTabela.EXIBIR_NA_BUSCA_AVANCADA + '\', \'' + objetoTabela.CAIXA_ALTA + '\', \'' + objetoTabela.mascaraComponente.ID + '\', \'' + objetoTabela.mascaraComponente.NOME + '\', \'' + objetoTabela.validadorComponente.ID + '\', \'' + objetoTabela.validadorComponente.NOME + '\', \'' + objetoTabela.tipoComponente.ID + '\', \'' + objetoTabela.tipoComponente.NOME + '\', \'' + objetoTabela.tipoComponente.CODIGO + '\', \'' + objetoTabela.tipoValorComponente.ID + '\', \'' + objetoTabela.tipoValorComponente.CODIGO + '\', \'' + objetoTabela.tipoValorComponente.NOME + '\', \'' + objetoTabela.formulario.ID + '\', \'' + objetoTabela.formulario.NOME + '\', \'' + objetoTabela.tipoLista.ID + '\', \'' + objetoTabela.tipoLista.NOME + '\', \'' + objetoTabela.componentePreenchido + '\'';
}
//Setar para habilitar o botao de alteracao

function checarTipoComponenteTipoLista(codigoTipoComponente, somenteLeitura) {
    if (undefined == $('#idTipoLista')[0].selectize ||
        undefined == $('#idTipoValorComponente')[0].selectize ||
        undefined == $('#idMascara')[0].selectize ||
        undefined == $('#idValidador')[0].selectize) {
        setTimeout(function () {
            checarTipoComponenteTipoLista(codigoTipoComponente, somenteLeitura);
        }, 200);
    } else {
        var dadosSomenteLeitura = $('#idTipoComponente').data('dadosSomenteLeitura');
        switch (codigoTipoComponente) {
            case TIPO_TEXT_BOX:
                $('#dvTipoLista').css('display', 'none');
                $('#dvNomeTipoLista').css('display', 'none');
                $('#idTipoLista')[0].selectize.setValue('');
                $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoLista', 'VALID');

                $('#formCadastro').data('bootstrapValidator').updateStatus('tamanhoComponente', 'NOT_VALIDATED');
                //$('#formCadastro').data('bootstrapValidator').updateStatus('idMascara', 'NOT_VALIDATED');
                //$('#formCadastro').data('bootstrapValidator').updateStatus('idValidador', 'NOT_VALIDATED');
                $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoValorComponente', 'NOT_VALIDATED');
                ajustarCampo($('#dvTamanhoComponente'), $('#dvTamanhoComponenteLeitura'), $('#tamanhoComponenteLeitura'), dadosSomenteLeitura.tamanhoComponente, somenteLeitura);
                ajustarCampo($('#dvMascara'), $('#dvNomeMascara'), $('#nomeMascara'), dadosSomenteLeitura.nomeMascara, somenteLeitura);
                ajustarCampo($('#dvValidador'), $('#dvNomeValidador'), $('#nomeValidador'), dadosSomenteLeitura.nomeValidador, somenteLeitura);
                //ajustarCampo($('#dvTipoValorComponente'), $('#dvNomeTipoValorComponente'), $('#nomeTipoValorComponente'), dadosSomenteLeitura.nomeTipoValorComponente, somenteLeitura);
                break;
            case TIPO_TEXT_AREA:
                $('#dvTipoLista').css('display', 'none');
                $('#dvNomeTipoLista').css('display', 'none');
                $('#idTipoLista')[0].selectize.setValue('');
                $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoLista', 'VALID');

                $('#formCadastro').data('bootstrapValidator').updateStatus('tamanhoComponente', 'NOT_VALIDATED');
                ajustarCampo($('#dvTamanhoComponente'), $('#dvTamanhoComponenteLeitura'), $('#tamanhoComponenteLeitura'), dadosSomenteLeitura.tamanhoComponente, somenteLeitura);

                $('#dvMascara').css('display', 'none');
                $('#dvNomeMascara').css('display', 'none');
                $('#idMascara')[0].selectize.setValue('');
                //$('#formCadastro').data('bootstrapValidator').updateStatus('idMascara', 'VALID');

                $('#dvValidador').css('display', 'none');
                $('#dvNomeValidador').css('display', 'none');
                $('#idValidador')[0].selectize.setValue('');
                //$('#formCadastro').data('bootstrapValidator').updateStatus('idValidador', 'VALID');

                $('#dvTipoValorComponente').css('display', 'none');
                $('#dvNomeTipoValorComponente').css('display', 'none');
                //$('#idTipoValorComponente')[0].selectize.setValue('');
                $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoValorComponente', 'VALID');
                break;
            case TIPO_DROP_DOWN_LIST:
                $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoLista', 'NOT_VALIDATED');
                ajustarCampo($('#dvTipoLista'), $('#dvNomeTipoLista'), $('#nomeTipoLista'), dadosSomenteLeitura.nomeTipoLista, somenteLeitura);

                $('#dvTamanhoComponente').css('display', 'none');
                $('#dvTamanhoComponenteLeitura').css('display', 'none');
                $('#tamanhoComponente').val('');
                $('#formCadastro').data('bootstrapValidator').updateStatus('tamanhoComponente', 'VALID');

                $('#dvMascara').css('display', 'none');
                $('#dvNomeMascara').css('display', 'none');
                $('#idMascara')[0].selectize.setValue('');
                //$('#formCadastro').data('bootstrapValidator').updateStatus('idMascara', 'VALID');

                $('#dvValidador').css('display', 'none');
                $('#dvNomeValidador').css('display', 'none');
                $('#idValidador')[0].selectize.setValue('');
                //$('#formCadastro').data('bootstrapValidator').updateStatus('idValidador', 'VALID');

                $('#dvTipoValorComponente').css('display', 'none');
                $('#dvNomeTipoValorComponente').css('display', 'none');
                //$('#idTipoValorComponente')[0].selectize.setValue('');
                $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoValorComponente', 'VALID');
                break;
        }
    }
}

function inicializarTiposComponentes(idCodigoTipoComponente) {
    mostrarPopup();

    $.ajax({
        url: 'TiposComponentes/GetParaComponente',
        type: 'POST',
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            var auxOption = $('<option value="">Informe o tipo</option>');
            $('#idTipoComponente').append(auxOption);
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].ID + ';' + dados[i].CODIGO + '">' + dados[i].NOME + '</option>');
                $('#idTipoComponente').append(auxOption);
            }
            $('#idTipoComponente').selectize();
            $('#idTipoComponente').change(function () {
                var codigoTipoComponente = $(this).val().toString().split(';')[1];
                var somenteLeitura = $(this).data('somenteLeitura');
                checarTipoComponenteTipoLista(parseInt(codigoTipoComponente), somenteLeitura);
            });

            if (undefined == idCodigoTipoComponente) {
                $('#idTipoComponente')[0].selectize.setValue('');
                $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoComponente', 'NOT_VALIDATED');
            } else {
                $('#idTipoComponente')[0].selectize.setValue(idCodigoTipoComponente);
                $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoComponente', 'VALID');
            }

            tiposComponentesObtidos = true;
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os tipos de componente, favor tente novamente');
        }
    });
}

function inicializarTiposValoresComponentes(idCodigoTipoValorComponente) {
    mostrarPopup();

    $.ajax({
        url: 'TiposValoresComponentes/GetParaComponente',
        type: 'POST',
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            var idVarchar;
            var codigoVarchar;
            var auxOption = $('<option value="">Informe o valor</option>');
            $('#idTipoValorComponente').append(auxOption);
            for (var i = 0; i < dados.length; i++) {
                if (1 == dados[i].CODIGO) {
                    idVarchar = dados[i].ID;
                    codigoVarchar = dados[i].CODIGO;
                }
                auxOption = $('<option value="' + dados[i].ID + ';' + dados[i].CODIGO + '">' + dados[i].NOME + '</option>');
                $('#idTipoValorComponente').append(auxOption);
            }
            $('#idTipoValorComponente').selectize();

            if (undefined == idCodigoTipoValorComponente) {
                //$('#idTipoValorComponente')[0].selectize.setValue('');
                $('#idTipoValorComponente')[0].selectize.setValue(idVarchar + ';' + codigoVarchar);
                $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoValorComponente', 'NOT_VALIDATED');
            } else {
                $('#idTipoValorComponente')[0].selectize.setValue(idCodigoTipoValorComponente);
                $('#formCadastro').data('bootstrapValidator').updateStatus('idTipoValorComponente', 'VALID');
            }

            tiposValoresComponentesObtidos = true;
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os valores de componente, favor tente novamente');
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

function inicializarMascaras(ID_MASCARA_COMPONENTE) {
    mostrarPopup();

    $.ajax({
        url: 'MascaraComponente/GetParaComponente',
        type: 'POST',
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            var auxOption = $('<option value="">Informe a máscara</option>');
            $('#idMascara').append(auxOption);
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                $('#idMascara').append(auxOption);
            }
            $('#idMascara').selectize();

            if (undefined == ID_MASCARA_COMPONENTE) {
                $('#idMascara')[0].selectize.setValue('');
                //$('#formCadastro').data('bootstrapValidator').updateStatus('idMascara', 'NOT_VALIDATED');
            } else {
                $('#idMascara')[0].selectize.setValue(ID_MASCARA_COMPONENTE);
                //$('#formCadastro').data('bootstrapValidator').updateStatus('idMascara', 'VALID');
            }

            mascarasObtidas = true;
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter as máscaras, favor tente novamente');
        }
    });
}

function inicializarValidadores(ID_VALIDADOR_COMPONENTE) {
    mostrarPopup();

    $.ajax({
        url: 'ValidadorComponente/GetParaComponente',
        type: 'POST',
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            var auxOption = $('<option value="">Informe o validador</option>');
            $('#idValidador').append(auxOption);
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                $('#idValidador').append(auxOption);
            }
            $('#idValidador').selectize();

            if (undefined == ID_VALIDADOR_COMPONENTE) {
                $('#idValidador')[0].selectize.setValue('');
                //$('#formCadastro').data('bootstrapValidator').updateStatus('idValidador', 'NOT_VALIDATED');
            } else {
                $('#idValidador')[0].selectize.setValue(ID_VALIDADOR_COMPONENTE);
                //$('#formCadastro').data('bootstrapValidator').updateStatus('idValidador', 'VALID');
            }

            validadoresObtidos = true;
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os validadores, favor tente novamente');
        }
    });
}

function montarDadosCadastro() {
    var obrigaComponente = $('input[type=radio][name=obrigaComponente]:checked');
    var exibirComponenteNoGrid = $('input[type=radio][name=exibirComponenteNoGrid]:checked');
    var exibirComponenteNoLancamento = $('input[type=radio][name=exibirComponenteNoLancamento]:checked');
    var exibirComponenteNoAtendimento = $('input[type=radio][name=exibirComponenteNoAtendimento]:checked');
    var exibirComponenteNaBuscaAvancada = $('input[type=radio][name=exibirComponenteNaBuscaAvancada]:checked');
    var caixaAlta = $('input[type=radio][name=caixaAlta]:checked');
    return {
        NOME: $('#nomeComponente').val(),
        DESCRICAO: $('#descricaoComponente').val(),
        ORDEM: $('#ordemComponente').val(),
        TAMANHO: $('#tamanhoComponente').val(),
        OBRIGATORIEDADE: obrigaComponente.val(),
        EXIBIR_NO_GRID: exibirComponenteNoGrid.val(),
        EXIBIR_NO_LANCAMENTO: exibirComponenteNoLancamento.val(),
        EXIBIR_NO_ATENDIMENTO: exibirComponenteNoAtendimento.val(),
        EXIBIR_NA_BUSCA_AVANCADA: exibirComponenteNaBuscaAvancada.val(),
        CAIXA_ALTA: caixaAlta.val(),
        ID_MASCARA_COMPONENTE: $('#idMascara').val(),
        ID_VALIDADOR_COMPONENTE: $('#idValidador').val(),
        ID_TIPO_COMPONENTE: $('#idTipoComponente').val().toString().split(';')[0],
        ID_TIPO_VALOR_COMPONENTE: $('#idTipoValorComponente').val().toString().split(';')[0],
        ID_FORMULARIO: $('#idFormulario').val(),
        ID_TIPO_LISTA: $('#idTipoLista').val(),
        somenteLeitura: $('#idTipoComponente').data('somenteLeitura') ? 'S' : 'N'
    };
}

$(document).ready(function () {
    inicializarTabelaPrincipal($('#tabelaPrincipal'), colunasTabelaPrincipal);

    options.fields = camposFormCadastro;
    $('#formCadastro').bootstrapValidator(options);

    $('#btnNovo').click(function () {
        limparFormCadastro();
    });

    $('#btnExportarExcel').click(function () {
        exportarArquivo('Componentes/GetParaExcel', 'Componentes/ExportarExcel', 'Componentes');
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