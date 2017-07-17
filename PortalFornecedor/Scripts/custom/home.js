var TOT_FORMULARIOS_LINHA = 4;
var guardaTituloPagina = $('#tituloPagina').html();
var idFormularioEscolhido;
var nomeFormularioEscolhido;
var anexoObrigatorioFormEscolhido;
var exibirTodosFormEscolhido;
var camposFormCadastro;
var colunasTabelaPrincipal = [
    {
        data: 'ID',
        name: 'ID'
    },
    {
        render: renderColunaOpcoes
    }
];
var colunasParaExcel;
var dvFormGroupDinamicoFiltro = 'campoDinamicoFiltro_';
metodoGet = 'Preenchimentos/Get';
metodoInsert = 'Preenchimentos/Insert';
metodoDetalhes = true;
metodoClonar = true;
exibirCadeadoBloqueio = true;

function iniciarCamposFormCadastro() {
    var result = {};
    return result;
}

function atualizarFormCadastro(camposForm) {
    if (undefined != $('#formCadastro').data('bootstrapValidator')) {
        $('#formCadastro').removeData('bootstrapValidator');
    }
    options.fields = camposForm;
    $('#formCadastro').bootstrapValidator(options);
}

function ajustarBotoes(somenteLeitura) {
    if (somenteLeitura) {
        $('#btnSalvar').css('display', 'none');
        $('#btnCancelar').removeClass('btn-danger');
        $('#btnCancelar').addClass('btn-primary');
        $('#btnCancelar > label').html('Voltar');
    } else {
        $('#btnSalvar').css('display', '');
        $('#btnCancelar').removeClass('btn-primary');
        $('#btnCancelar').addClass('btn-danger');
        $('#btnCancelar > label').html('Cancelar');
    }
    limparFiltro();
}

function exibirDetalhes(id, idStatusAtual, idBusca, dataHora, nomeStatusAtual, lojaCriador) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').removeProp('idRegistro');
    $('#btnSalvar').removeProp('idStatusAtual');
    ajustarBotoes(true);
    var auxData = {
        ID_FORMULARIO: idFormularioEscolhido,
        ID_PREENCHIMENTO_FORMULARIO: id,
        ID_STATUS_REGRA: idStatusAtual
    };
    var camposFixos = {
        ID_BUSCA: idBusca,
        DATA_HORA: dataHora,
        NOME_STATUS_ATUAL: nomeStatusAtual,
        LOJA_CRIADOR: lojaCriador
    };
    montarComponentes('Preenchimentos/GetPorPreenchimento', auxData, camposFixos, true, true);
}

function carregarFormCadastro(id, idStatusAtual, idBusca, dataHora, nomeStatusAtual, lojaCriador) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').prop('idRegistro', id);
    $('#btnSalvar').prop('idStatusAtual', idStatusAtual);
    ajustarBotoes(false);
    var auxData = {
        ID_FORMULARIO: idFormularioEscolhido,
        ID_PREENCHIMENTO_FORMULARIO: id,
        ID_STATUS_REGRA: idStatusAtual,
        realizarBloqueio: 'S',
        ID_STATUS_ATUAL: idStatusAtual
    };
    var camposFixos = {
        ID_BUSCA: idBusca,
        DATA_HORA: dataHora,
        NOME_STATUS_ATUAL: nomeStatusAtual,
        LOJA_CRIADOR: lojaCriador
    };
    obterParametrosParaUpload(id);
    montarComponentes('Preenchimentos/GetPorPreenchimento', auxData, camposFixos, false, true);
}

function carregarFormClonagem(id, idStatusAtual, idBusca, dataHora, nomeStatusAtual, lojaCriador) {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeProp('idRegistro');
    $('#btnSalvar').removeProp('idStatusAtual');
    ajustarBotoes(false);
    var idStatusInicial = $('#statusFiltro').data('idStatusInicial');
    var auxData = {
        ID_FORMULARIO: idFormularioEscolhido,
        ID_PREENCHIMENTO_FORMULARIO: id,
        ID_STATUS_REGRA: undefined == idStatusInicial ? -1 : idStatusInicial
    };
    var camposFixos = {
        ID_BUSCA: idBusca,
        DATA_HORA: dataHora,
        NOME_STATUS_ATUAL: nomeStatusAtual,
        LOJA_CRIADOR: lojaCriador
    };
    obterParametrosParaUpload(undefined);
    montarComponentes('Preenchimentos/GetPorPreenchimento', auxData, camposFixos, false, false);
}

//Setar para habilitar botao de excluir
metodoDelete = 'Preenchimentos/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'ID_FORMULARIO\': ' + idFormularioEscolhido + ',\'ID\': ' + objetoTabela.ID + ',\'realizarBloqueio\': \'N\',\'ID_STATUS_ATUAL\': ' + objetoTabela.ID_STATUS_ATUAL + ' }';
}
//Setar para habilitar botao de excluir

//Setar para habilitar o botao de alteracao
metodoUpdate = 'Preenchimentos/Update';
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.ID + '\', \'' + objetoTabela.ID_STATUS_ATUAL + '\', \'' + objetoTabela['Identificador'] + '\', \'' + objetoTabela['Criação'] + '\', \'' + objetoTabela['Status'] + '\', \'' + objetoTabela['Loja'] + '\'';
}
//Setar para habilitar o botao de alteracao

function montarComponente(idCompForm, valorVarchar, itemLista, componente) {
    var objValidador = {};
    var nomeComp = desenharComponente($('#dvComponentesDinamicos'),
                        componente,
                        idCompForm,
                        valorVarchar,
                        itemLista,
                        objValidador);
    if (componente.TAMANHO > 0) {
        objValidador['stringLength'] = {
            max: componente.TAMANHO,
            message: 'M&aacute;ximo de ' + componente.TAMANHO + ' caracteres'
        };
    }
    if (COMPONENTE_OBRIGATORIO == componente.OBRIGATORIEDADE) {
        objValidador['notEmpty'] = {
            message: 'Campo obrigat&oacute;rio'
        };
    }
    camposFormCadastro[nomeComp] = {
        validators: objValidador
    };
}

function obterStatusTramitacao(idFormulario, objFiltro) {
    if (undefined != idFormulario) {
        mostrarPopup();

        var dropDownList = $('#statusFiltro');
        if (undefined != dropDownList[0].selectize) {
            dropDownList[0].selectize.destroy();
            dropDownList.html('<option value="T">Todos</option>');
        }
        var auxOption;

        $.ajax({
            url: 'StatusFormulario/GetPorformulario',
            type: 'POST',
            data: { ID_FORMULARIO: idFormulario, ID_STATUS_ORIGEM: undefined, filtrarStatus: 'S' },
            success: function (result) {
                fecharPopup();

                var dados = result.data;
                if (dados.length > 0) {
                    var idStatusInicial;
                    var idStatusRetorno;
                    for (var i = 0; i < dados.length; i++) {
                        if (1 == dados[i].INICIAL) {
                            idStatusInicial = dados[i].ID;
                            auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                        } else {
                            if (1 == dados[i].RETORNO) {
                                idStatusRetorno = dados[i].ID;
                            }
                            auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                        }
                        dropDownList.append(auxOption);
                    }

                    dropDownList.selectize().on('change', function () {
                        $('div.selectize-input > input').blur();
                    });
                    dropDownList.data('idStatusInicial', idStatusInicial);
                    dropDownList.data('idStatusRetorno', idStatusRetorno);

                    guardarDadosFiltro();
                    colunasParaExcel = objFiltro.dados;
                    inicializarTabelaPrincipalColunasDinamicas($('#tabelaPrincipal'), colunasTabelaPrincipal, idFormulario, objFiltro.dados, $('#dataInicialFiltro').val(), $('#dataFinalFiltro').val(), dropDownList.val(), $('#tipoLancamentosFiltro').val());
                    mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvTabelaFormularios'), $('#dvTabelaPrincipal'), undefined, objFiltro.nome);
                } else {
                    mostrarMsgErro('Nenhum status foi encontrado');
                }
            },
            error: function (request, status, error) {
                fecharPopup();
                mostrarMsgErro('Falha ao tentar obter os status, favor tente novamente');
            },
            complete: function () {
                atualizarFormCadastro(camposFormCadastro);
            }
        });
    } else {
        mostrarMsgErro('Nenhum status foi encontrado');
    }
}

function obterTipoPermissaoStatus(obj) {
    var tipoPermissaoStatus = undefined;
    if (undefined != obj.componente) {
        tipoPermissaoStatus = obj.componente.tipoPermissaoStatus;
    } else {
        tipoPermissaoStatus = obj.tipoPermissaoStatus;
    }
    return tipoPermissaoStatus;
}

function ajustarIdCampoDinamicoFiltro(nomeColuna) {
    if (!isNullOrEmpty(nomeColuna)) {
        return nomeColuna.replace(new RegExp(' ', 'g'), '_');
    }
    return nomeColuna;
}

function obterCamposDinamicosFiltro(camposDinamicosFiltro) {
    if (undefined != camposDinamicosFiltro) {
        var auxId;
        var auxNomeValor;
        for (var i = 0 ; i < camposDinamicosFiltro.length; i++) {
            auxNomeValor = camposDinamicosFiltro[i].split(';');
            auxId = '#' + dvFormGroupDinamicoFiltro + ajustarIdCampoDinamicoFiltro(auxNomeValor[0]);
            $(auxId).val(auxNomeValor[1]);
        }
    }
}

function guardarCamposDinamicosFiltro() {
    var camposDinamicosFiltro = [];
    var indiceCamposDinamicosFiltro = 0;
    var nome;
    $('.' + dvFormGroupDinamicoFiltro + ' > input').each(function () {
        nome = $(this).attr('nomeCampoFiltroDinamico');
        camposDinamicosFiltro[indiceCamposDinamicosFiltro++] = nome + ';' + $(this).val();
    });
    tabelaPrincipal.camposDinamicosFiltro = camposDinamicosFiltro;
}

function adicionarCampoDinamicoFiltro(nomeColuna) {
    var campo = $(''
    + ' <div class="form-group col-lg-3 ' + dvFormGroupDinamicoFiltro + '">'
    + '     <label>' + nomeColuna + ':</label>'
    + '     <input id="' + dvFormGroupDinamicoFiltro + ajustarIdCampoDinamicoFiltro(nomeColuna) + '" nomeCampoFiltroDinamico="' + nomeColuna + '" class="form-control">'
    + ' </div>');
    $('#dvCamposDinamicosFiltro').append(campo);
}

function montarComponentes(auxUrl, auxData, camposFixos, somenteLeitura, exibirHistorico) {
    mostrarPopup();

    $('.dvFormGroupDinamico').remove();
    camposFormCadastro = iniciarCamposFormCadastro();

    $.ajax({
        url: auxUrl,
        type: 'POST',
        data: auxData,
        success: function (result) {
            fecharPopup();

            var msgErro = result.msgErro;
            if (!isNullOrEmpty(msgErro)) {
                mostrarMsgErro(msgErro);
            } else {
                var dados = result.data;
                if (dados.length > 0) {
                    if (undefined != camposFixos) {
                        desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'ID.', camposFixos.ID_BUSCA);
                        desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Criação', camposFixos.DATA_HORA);
                        desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Status', camposFixos.NOME_STATUS_ATUAL);
                        desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Loja', camposFixos.LOJA_CRIADOR);
                    }
                    iniciarControleComponentes(dados.length);
                    var tipoPermissaoStatus;
                    for (var i = 0; i < dados.length; i++) {
                        tipoPermissaoStatus = obterTipoPermissaoStatus(dados[i]);
                        if (undefined == tipoPermissaoStatus) {
                            if (undefined == dados[i].componente) {
                                desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), dados[i].NOME, '');
                            } else {
                                desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), dados[i].componente.NOME, dados[i].VALOR_VARCHAR);
                            }
                        } else {
                            switch (tipoPermissaoStatus) {
                                case PERMISSAO_COMPONENTE_ESCRITA:
                                    if (somenteLeitura) {
                                        desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), dados[i].componente.NOME, dados[i].VALOR_VARCHAR);
                                    } else {
                                        if (undefined == dados[i].componente) {
                                            montarComponente(undefined, undefined, undefined, dados[i]);
                                        } else {
                                            montarComponente(dados[i].ID, dados[i].VALOR_VARCHAR, dados[i].itemLista, dados[i].componente);
                                        }
                                    }
                                    break;
                                case PERMISSAO_COMPONENTE_OCULTO:
                                    continue;
                                default:
                                    if (undefined == dados[i].componente) {
                                        desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), dados[i].NOME, '');
                                    } else {
                                        desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), dados[i].componente.NOME, dados[i].VALOR_VARCHAR);
                                    }
                                    break;
                            }
                        }
                    }
                    verificarDropDownLists();

                    if (!somenteLeitura) {
                        adicionarObsTramitacao('obsTramitacao', $('#dvComponentesDinamicos'));
                        adicionarBotaoUploadArquivos($('#dvComponentesDinamicos'));
                    }

                    if (exibirHistorico) {
                        adicionarHistoricoTramitacoes(
                            $('#dvFormCadastro'),
                            $('#dvTabelaPrincipal'),
                            $('#dvComponentesDinamicos'),
                            'Tramitacoes/GetPorPreenchimento', {
                                ID_PREENCHIMENTO_FORMULARIO: auxData.ID_PREENCHIMENTO_FORMULARIO
                            });
                    } else {
                        mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), undefined);
                    }
                } else {
                    mostrarMsgErro('Nenhum componente foi encontrado');
                }
            }
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os componentes, favor tente novamente');
        },
        complete: function () {
            atualizarFormCadastro(camposFormCadastro);
        }
    });
}

function obterColunasTabelaPrincipal(idFormulario, nome, anexoObrigatorio, exibirTodos) {
    mostrarPopup();

    $('.' + dvFormGroupDinamicoFiltro).remove();

    $.ajax({
        url: 'Preenchimentos/GetColunasDinamicas',
        type: 'POST',
        data: { ID_FORMULARIO: idFormulario, somenteAtivos: 'S' },
        success: function (result) {
            fecharPopup();

            var msgErro = result.msgErro;
            if (!isNullOrEmpty(msgErro)) {
                mostrarMsgErro(msgErro);
            } else {
                colunasTabelaPrincipal = [];

                var dados = result.data;
                if (dados.length > 0) {
                    $('#dvPaiTabela').html('');
                    var tabelaPrincipal = $('<table class="table table-striped table-bordered table-hover" id="tabelaPrincipal"></table>');
                    var theadTabelaPrincipal = $('<thead></thead>');
                    var linhaColunasDinamicas = $('<tr></tr>');
                    var nomeColuna;
                    var indiceLinhaTabela = 0;
                    var indiceListaNomeColuna = 0;
                    var listaNomeColuna = []
                    for (var i = 0; i < dados.length; i++) {
                        nomeColuna = dados[i].nomeColuna;
                        listaNomeColuna[indiceListaNomeColuna++] = nomeColuna;
                        if (dados[i].exibirColuna) {
                            linhaColunasDinamicas.append('<th>' + ('Identificador' == nomeColuna ? 'ID.' : nomeColuna) + '</th>');
                            colunasTabelaPrincipal[indiceLinhaTabela++] = {
                                data: nomeColuna,
                                mData: nomeColuna,
                                name: nomeColuna,
                                sName: nomeColuna,
                            }
                        }
                        if (dados[i].exibirEmLancamento) {
                            adicionarCampoDinamicoFiltro(nomeColuna);
                        }
                    }
                    linhaColunasDinamicas.append('<th style="text-align: center;">Ações</th>');
                    theadTabelaPrincipal.append(linhaColunasDinamicas);
                    tabelaPrincipal.append(theadTabelaPrincipal);
                    $('#dvPaiTabela').append(tabelaPrincipal);

                    colunasTabelaPrincipal[colunasTabelaPrincipal.length] = {
                        render: renderColunaOpcoes,
                        mRender: renderColunaOpcoes
                    }

                    idFormularioEscolhido = idFormulario;
                    nomeFormularioEscolhido = nome;
                    anexoObrigatorioFormEscolhido = anexoObrigatorio == 1 ? true : false;
                    exibirTodosFormEscolhido = exibirTodos == 1 ? true : false;

                    var dataInicialFiltro = new Date();
                    dataInicialFiltro.setDate(dataInicialFiltro.getDate() - diferenciaDiasFiltro);
                    var dataFinalFiltro = new Date();

                    inicializarCampoData('dataInicialFiltro');
                    $('#dataInicialFiltro').datepicker('setDate', dataInicialFiltro);
                    $('#dataInicialFiltro').datepicker('update');
                    $('#dataInicialFiltro').data('fezCargaInicial', 'S');

                    inicializarCampoData('dataFinalFiltro');
                    $('#dataFinalFiltro').datepicker('setDate', dataFinalFiltro);
                    $('#dataFinalFiltro').datepicker('update');
                    $('#dataFinalFiltro').data('fezCargaInicial', 'S');

                    if (undefined != $('#tipoLancamentosFiltro')[0].selectize) {
                        $('#tipoLancamentosFiltro')[0].selectize.destroy();
                    }
                    $('#tipoLancamentosFiltro').html('');
                    var auxOption = $('<option value="' + valorPadraoMeusLancamentos + '" selected>Meus Lançamentos</option>');
                    $('#tipoLancamentosFiltro').append(auxOption);
                    if (exibirTodosFormEscolhido) {
                        auxOption = $('<option value="' + valorPadraoTodosLancamentos + '">Todos os Lançamentos</option>');
                        $('#tipoLancamentosFiltro').append(auxOption);
                    }
                    $('#tipoLancamentosFiltro').selectize();
                    $('#tipoLancamentosFiltro')[0].selectize.setValue(valorPadraoMeusLancamentos);

                    obterStatusTramitacao(idFormularioEscolhido, {
                        dados: listaNomeColuna, nome: nome
                    });
                } else {
                    $('#tituloPagina').html(guardaTituloPagina);
                    mostrarMsgErro('Nenhum componente foi encontrado para o formulário ' + nome);
                }
            }
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os componentes, favor tente novamente');
        },
        complete: function () {
            atualizarFormCadastro(camposFormCadastro);
        }
    });
}

function desenharFormulario(dvPai, id, codigo, nome, anexoObrigatorio, exibirTodos) {
    var result = $('<div class="col-lg-3"></div>');
    var btnResult = $('<button class="btn btn-md btn-primary btn-formulario"><i class="fa fa-file-text-o" style="font-size:30px"></i><br /><label>' + nome + '</label></button>');
    btnResult.data('codigo', codigo);
    btnResult.click(function () {
        metodoUpdate = undefined;
        metodoDelete = undefined;
        obterColunasTabelaPrincipal(id, nome, anexoObrigatorio, exibirTodos);
    });
    result.append(btnResult);
    dvPai.append(result);
}

function obterFormularios() {
    mostrarPopup();

    $.ajax({
        url: 'Formularios/GetParaHome',
        type: 'POST',
        data: {
            somenteAtivos: 'S', filtrarStatus: 'S'
        },
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            var totFormularios = dados.length;
            if (totFormularios > 0) {
                var totFormulariosLinha = (parseInt(totFormularios / TOT_FORMULARIOS_LINHA)) * TOT_FORMULARIOS_LINHA;
                var totFormulariosResto = parseInt(totFormularios % TOT_FORMULARIOS_LINHA);
                var indiceFormulario = 0;
                var i;
                var novaLinha;

                var dvPai;
                for (i = 0; i < totFormulariosLinha; i++) {
                    novaLinha = i % TOT_FORMULARIOS_LINHA == 0 ? true : false;
                    if (novaLinha) {
                        dvPai = $('<div class="row"></div>');
                        $('#dvPrincipal').append(dvPai);
                    }
                    desenharFormulario(dvPai, dados[indiceFormulario].ID, dados[indiceFormulario].CODIGO, dados[indiceFormulario].NOME, dados[indiceFormulario].OBRIGATORIEDADE_ANEXO, dados[indiceFormulario].EXIBIR_TODOS);
                    indiceFormulario++;
                }

                dvPai = $('<div class="row"></div>');
                $('#dvPrincipal').append(dvPai);
                for (i = 0; i < totFormulariosResto; i++) {
                    desenharFormulario(dvPai, dados[indiceFormulario].ID, dados[indiceFormulario].CODIGO, dados[indiceFormulario].NOME, dados[indiceFormulario].OBRIGATORIEDADE_ANEXO, dados[indiceFormulario].EXIBIR_TODOS);
                    indiceFormulario++;
                }
            }
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os formulários, favor tente novamente');
        }
    });
}

function inicializarCampoData(id) {
    $('#' + id).datepicker({
        format: 'dd/mm/yyyy',
        todayBtn: 'linked',
        todayHighlight: true,
        autoclose: true,
        orientation: 'top right',
        language: 'pt-BR'
    })
                    .on('changeDate', function (e) {
                        ajustarPeriodoFiltro($(this), $('#dataInicialFiltro'), $('#dataFinalFiltro'), tabelaPrincipal, diferenciaDiasFiltro);
                    });
}

function montarDadosCadastro() {
    var auxListaComponentes = montarListaComponentes();
    return {
        dadosCadastro: {
            ID_FORMULARIO: idFormularioEscolhido,
            LISTA_COMPONENTES: auxListaComponentes.componentesFormulario,
            OBSERVACAO: $('#obsTramitacao').val(),
            arquivosTramitacao: obterArquivosUpload()
        },
        logAlteracaoComponentesForm: auxListaComponentes.mensagemAlteracao
    };
}

$(document).ready(function () {
    obterFormularios();

    $('#btnFiltrar').click(function () {
        if (undefined != tabelaPrincipal &&
            ajustarPeriodoFiltro($('#dataInicialFiltro'),
            $('#dataInicialFiltro'),
            $('#dataFinalFiltro'),
            tabelaPrincipal,
            diferenciaDiasFiltro)) {
            metodoUpdate = undefined;
            metodoDelete = undefined;
            if (valorPadraoMeusLancamentos == $('#tipoLancamentosFiltro').val()) {
                var idStatusInicial = $('#statusFiltro').data('idStatusInicial');
                var idStatusRetorno = $('#statusFiltro').data('idStatusRetorno');
                if (idStatusInicial == $('#statusFiltro').val()) {
                    metodoUpdate = 'Preenchimentos/Update';
                    metodoDelete = 'Preenchimentos/Delete';
                } else if (idStatusRetorno == $('#statusFiltro').val()) {
                    metodoUpdate = 'Preenchimentos/Update';
                }
            }
            guardarDadosFiltro();
            tabelaPrincipal.ajax.reload(null, true);
        }
    });

    $('#btnLimparFiltro').click(function () {
        limparFiltro();
    });

    $('#btnNovo').click(function () {
        $('#btnSalvarContinuar').css('display', '');
        $('#btnSalvar').removeProp('idRegistro');
        $('#btnSalvar').removeProp('idStatusAtual');
        ajustarBotoes(false);
        var idStatusInicial = $('#statusFiltro').data('idStatusInicial');
        var auxData = {
            ID_FORMULARIO: idFormularioEscolhido,
            ID_STATUS_REGRA: undefined == idStatusInicial ? -1 : idStatusInicial
        };
        obterParametrosParaUpload(undefined);
        montarComponentes('Componentes/GetPorformulario', auxData, undefined, false, false);
    });

    $('#btnExportarExcel').click(function () {
        var dadosFiltro = {
            idFormulario: idFormularioEscolhido,
            colunasFormulario: colunasParaExcel,
            dataInicialFiltro: tabelaPrincipal.dataInicialFiltro,
            dataFinalFiltro: tabelaPrincipal.dataFinalFiltro,
            statusForm: tabelaPrincipal.statusForm,
            tipoLancamentosFiltro: tabelaPrincipal.tipoLancamentosFiltro,
            camposDinamicosFiltro: tabelaPrincipal.camposDinamicosFiltro,
            idBuscaPreenchimento: tabelaPrincipal.idBuscaPreenchimento
        };
        exportarArquivo('Preenchimentos/GetParaExcel', 'Preenchimentos/ExportarExcel', nomeFormularioEscolhido, dadosFiltro);
    });

    $('#btnSalvar').click(function () {
        if (arquivosCarregados()) {
            fecharMsgErro();

            var auxId = $(this).prop('idRegistro');

            if (undefined == auxId
            && anexoObrigatorioFormEscolhido
            && formularioSemArquivos()) {
                mostrarMsgErro('Favor anexar pelo menos 1 arquivo');
            } else {
                var auxUrl;
                var auxDadosCadastro = montarDadosCadastro();
                var auxData = auxDadosCadastro.dadosCadastro;

                if (undefined == auxId) {
                    auxUrl = metodoInsert;
                } else {
                    auxUrl = metodoUpdate;
                    auxData.ID_PREENCHIMENTO = auxId;
                    auxData.realizarBloqueio = 'N';
                    auxData.ID_STATUS_ATUAL = $(this).prop('idStatusAtual');
                    auxData.logAlteracaoComponentes = auxDadosCadastro.logAlteracaoComponentesForm;
                }

                salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData, undefined);
            }
        } else {
            mostrarMsgErro('Favor terminar a carga dos arquivos ou remover os que não foram carregados');
        }
    });

    $('#btnSalvarContinuar').click(function () {
        if (arquivosCarregados()) {
            fecharMsgErro();
            if (anexoObrigatorioFormEscolhido
            && formularioSemArquivos()) {
                mostrarMsgErro('Favor anexar pelo menos 1 arquivo');
            } else {
                var auxUrl = metodoInsert;
                var auxData = montarDadosCadastro().dadosCadastro;
                salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData, undefined, true, true);
            }
        } else {
            mostrarMsgErro('Favor terminar a carga dos arquivos ou remover os que não foram carregados');
        }
    });

    $('#btnCancelar').click(function () {
        if (undefined != $('#btnSalvar').prop('idRegistro')) {
            $.ajax({
                url: 'Seguranca/VerificarBloqueioFormulario',
                type: 'POST',
                data: {
                    ID_PREENCHIMENTO_FORMULARIO: $('#btnSalvar').prop('idRegistro'),
                    realizarBloqueio: 'N'
                },
                success: function (result) {
                    var msgErro = result.msgErro;
                    if (!isNullOrEmpty(msgErro)) {
                        console.log(msgErro);
                    }
                },
                error: function (request, status, error) {
                    console.log('Falha ao tentar verificar o bloqueio, favor tente novamente');
                }
            });
        }
        mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), undefined, undefined);
    });

    $('#btnVoltar').click(function () {
        mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvTabelaPrincipal'), $('#dvTabelaFormularios'), undefined, guardaTituloPagina);
    });
});