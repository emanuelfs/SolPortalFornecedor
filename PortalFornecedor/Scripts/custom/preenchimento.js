var TOT_FORMULARIOS_LINHA = 4;
var guardaTituloPagina = $('#tituloPagina').html();
var idFormularioEscolhido;
var nomeFormularioEscolhido;
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
metodoInsert = undefined;
metodoUpdate = undefined;
metodoDelete = undefined;
metodoTramitacao = undefined;
metodoSelectLinha = true;
metodoDetalhes = true;
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

function obterIdParaCheckBoxDataTable(objetoTabela) {
    return '\'' + objetoTabela.ID + '_' + objetoTabela.Identificador + '\'';
}

function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.ID + '\', \'' + objetoTabela.ID_STATUS_ATUAL + '\', \'' + objetoTabela['Identificador'] + '\', \'' + objetoTabela['Criação'] + '\', \'' + objetoTabela['Status'] + '\', \'' + objetoTabela['Loja'] + '\'';
}

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
    limparListaLinhasSelect();
}

function exibirDetalhes(id, idStatusAtual, idBusca, dataHora, nomeStatusAtual, lojaCriador) {
    $('#btnSalvar').removeProp('idRegistro');
    $('#btnSalvar').removeProp('idStatusAtual');
    var auxData = {
        ID_FORMULARIO: idFormularioEscolhido,
        ID_PREENCHIMENTO_FORMULARIO: id,
        ID_STATUS_REGRA: idStatusAtual
    };
    ajustarBotoes(true);
    var camposFixos = {
        ID_BUSCA: idBusca,
        DATA_HORA: dataHora,
        NOME_STATUS_ATUAL: nomeStatusAtual,
        LOJA_CRIADOR: lojaCriador
    };
    montarComponentes('Preenchimentos/GetPorPreenchimento', auxData, camposFixos, undefined);
}

function tramitar(id, idStatusAtual, idBusca, dataHora, nomeStatusAtual, lojaCriador) {
    $('#btnSalvar').prop('idRegistro', id);
    $('#btnSalvar').prop('idStatusAtual', idStatusAtual);
    var auxData = {
        ID_FORMULARIO: idFormularioEscolhido,
        ID_PREENCHIMENTO_FORMULARIO: id,
        ID_STATUS_REGRA: idStatusAtual,
        realizarBloqueio: 'S',
        ID_STATUS_ATUAL: idStatusAtual
    };
    ajustarBotoes(false);
    var camposFixos = {
        ID_BUSCA: idBusca,
        DATA_HORA: dataHora,
        NOME_STATUS_ATUAL: nomeStatusAtual,
        LOJA_CRIADOR: lojaCriador
    };
    obterParametrosParaUpload(id);
    montarComponentes('Preenchimentos/GetPorPreenchimento', auxData, camposFixos, idStatusAtual);
}

function atualizarBotaoAtendimento(atenderMultiplos) {
    metodoSelectLinha = atenderMultiplos;
    if (atenderMultiplos) {
        $('#btnAtender').css('display', '');
    } else {
        $('#btnAtender').css('display', 'none');
    }
}

function verificarProximoStatus(idFormulario, idStatusOrigem, objFiltro) {
    if (undefined != idFormulario && undefined != idStatusOrigem) {
        mostrarPopup();

        $.ajax({
            url: 'StatusFormulario/GetPorformulario',
            type: 'POST',
            data: { ID_FORMULARIO: idFormulario, ID_STATUS_ORIGEM: idStatusOrigem, filtrarStatus: 'S' },
            success: function (result) {
                var dados = result.data;
                if (dados.length > 0) {
                    metodoTramitacao = 'Preenchimentos/Tramitar';
                } else {
                    metodoTramitacao = undefined;
                }
                atualizarBotaoAtendimento(false);
                if (undefined != objFiltro) {
                    guardarDadosFiltro();
                    iniciarListaLinhasSelect();
                    colunasParaExcel = objFiltro.dados;
                    inicializarTabelaPrincipalColunasDinamicas($('#tabelaPrincipal'), colunasTabelaPrincipal, idFormulario, objFiltro.dados, $('#dataInicialFiltro').val(), $('#dataFinalFiltro').val(), $('#statusFiltro').val());
                    mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvTabelaFormularios'), $('#dvTabelaPrincipal'), undefined, objFiltro.nome);
                } else {
                    limparListaLinhasSelect();
                    guardarDadosFiltro();
                    tabelaPrincipal.ajax.reload(null, true);
                }
                fecharPopup();
            },
            error: function (request, status, error) {
                mostrarMsgErro('Falha ao tentar filtrar, favor tente novamente');
                fecharPopup();
            }
        });
    } else {
        mostrarMsgErro('Falha ao tentar filtrar: nenhum status foi informado');
        fecharPopup();
    }
}

function obterStatusTramitacao(idFormulario, objFiltro) {
    if (undefined != idFormulario) {
        mostrarPopup();

        var dropDownList;
        var auxOption;
        if (objFiltro.filtrar) {
            dropDownList = $('#statusFiltro');
            if (undefined != dropDownList[0].selectize) {
                dropDownList[0].selectize.destroy();
                dropDownList.html('<option value="T">Todos</option>');
            }
        } else {
            var dvFormGroup = $('<div class="form-group col-lg-6 dvFormGroupDinamico"></div>');
            var labelNomeComp = $('<label>Status:</label>');
            dropDownList = $('<select id="statusTramitacao" name="statusTramitacao" class="form-control"></select>');
            auxOption = $('<option value="">Informe o status</option>');

            dropDownList.append(auxOption);

            dvFormGroup.append(labelNomeComp);
            dvFormGroup.append(dropDownList);
            $('#dvComponentesDinamicos').append(dvFormGroup);

            var objValidador = {
                notEmpty: {
                    message: 'Campo obrigat&oacute;rio'
                }
            };
            camposFormCadastro['statusTramitacao'] = {
                validators: objValidador
            };
        }

        $.ajax({
            url: 'StatusFormulario/GetPorformulario',
            type: 'POST',
            data: { ID_FORMULARIO: idFormulario, ID_STATUS_ORIGEM: objFiltro.idStatusOrigem, filtrarStatus: 'S' },
            success: function (result) {
                fecharPopup();

                var dados = result.data;
                if (dados.length > 0) {
                    var idStatusInicial;
                    for (var i = 0; i < dados.length; i++) {
                        if (objFiltro.filtrar && 1 == dados[i].INICIAL) {
                            idStatusInicial = dados[i].ID;
                            auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                        } else {
                            auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                        }
                        dropDownList.append(auxOption);
                    }

                    if (objFiltro.filtrar) {
                        dropDownList.selectize().on('change', function () {
                            $('div.selectize-input > input').blur();
                        });
                        dropDownList.data('idStatusInicial', idStatusInicial);

                        if (isNullOrEmpty(dropDownList.val())) {
                            var valorPrimeiraOpcao = dropDownList.val();
                            var listaOptions = dropDownList[0].selectize.options;
                            if (undefined != listaOptions) {
                                for (var propName in listaOptions) {
                                    valorPrimeiraOpcao = propName;
                                    break;
                                }
                                dropDownList[0].selectize.setValue(valorPrimeiraOpcao);
                            }
                            verificarProximoStatus(idFormularioEscolhido, valorPrimeiraOpcao, objFiltro);
                        } else {
                            guardarDadosFiltro();
                            iniciarListaLinhasSelect();
                            metodoTramitacao = undefined;
                            atualizarBotaoAtendimento(false);
                            colunasParaExcel = objFiltro.dados;
                            inicializarTabelaPrincipalColunasDinamicas($('#tabelaPrincipal'), colunasTabelaPrincipal, idFormulario, objFiltro.dados, $('#dataInicialFiltro').val(), $('#dataFinalFiltro').val(), dropDownList.val());
                            mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvTabelaFormularios'), $('#dvTabelaPrincipal'), undefined, objFiltro.nome);
                        }
                    } else {
                        var auxFindOption = dropDownList.find('option:not([value=""])');
                        if (undefined != auxFindOption && 1 == auxFindOption.length) {
                            $(auxFindOption[0]).attr('selected', 'selected');
                        }

                        dropDownList.selectize();

                        adicionarObsTramitacao('obsTramitacao', $('#dvComponentesDinamicos'));
                        adicionarBotaoUploadArquivos($('#dvComponentesDinamicos'));
                        adicionarHistoricoTramitacoes(
                        $('#dvFormCadastro'),
                        $('#dvTabelaPrincipal'),
                        $('#dvComponentesDinamicos'),
                        'Tramitacoes/GetPorPreenchimento', {
                            ID_PREENCHIMENTO_FORMULARIO: objFiltro.ID_PREENCHIMENTO_FORMULARIO
                        });
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
                atualizarFormCadastro(camposFormCadastro);
            }
        });
    } else {
        mostrarMsgErro('Nenhum status foi encontrado');
    }
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

function montarComponentes(auxUrl, auxData, camposFixos, idStatusAtual) {
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
                    desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'ID.', camposFixos.ID_BUSCA);
                    desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Criação', camposFixos.DATA_HORA);
                    desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Status', camposFixos.NOME_STATUS_ATUAL);
                    desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Loja', camposFixos.LOJA_CRIADOR);
					iniciarControleComponentes(dados.length);
                    var tipoPermissaoStatus;
                    for (var i = 0; i < dados.length; i++) {
                        tipoPermissaoStatus = dados[i].componente.tipoPermissaoStatus;
                        if (undefined == tipoPermissaoStatus) {
                            desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), dados[i].componente.NOME, dados[i].VALOR_VARCHAR);
                        } else {
                            switch (tipoPermissaoStatus) {
                                case PERMISSAO_COMPONENTE_ESCRITA:
                                    if (undefined == idStatusAtual) {
                                        desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), dados[i].componente.NOME, dados[i].VALOR_VARCHAR);
                                    } else {
                                        montarComponente(dados[i].ID, dados[i].VALOR_VARCHAR, dados[i].itemLista, dados[i].componente);
                                    }
                                    break;
                                case PERMISSAO_COMPONENTE_OCULTO:
                                    continue;
                                    break;
                                default:
                                    desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), dados[i].componente.NOME, dados[i].VALOR_VARCHAR);
                                    break;
                            }
                        }
                    }
					verificarDropDownLists();
					
                    if (undefined != idStatusAtual) {
                        obterStatusTramitacao(auxData.ID_FORMULARIO, { filtrar: false, idStatusOrigem: idStatusAtual, ID_PREENCHIMENTO_FORMULARIO: auxData.ID_PREENCHIMENTO_FORMULARIO });
                    } else {
                        adicionarHistoricoTramitacoes(
                            $('#dvFormCadastro'),
                            $('#dvTabelaPrincipal'),
                            $('#dvComponentesDinamicos'),
                            'Tramitacoes/GetPorPreenchimento', {
                                ID_PREENCHIMENTO_FORMULARIO: auxData.ID_PREENCHIMENTO_FORMULARIO
                            });
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

function obterColunasTabelaPrincipal(idFormulario, nome) {
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
                        if (dados[i].exibirEmAtendimento) {
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

                    obterStatusTramitacao(idFormularioEscolhido, {
                        filtrar: true, dados: listaNomeColuna, nome: nome
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

function desenharFormulario(dvPai, id, codigo, nome) {
    var result = $('<div class="col-lg-3"></div>');
    var btnResult = $('<button class="btn btn-md btn-primary btn-formulario"><i class="fa fa-check-square-o" style="font-size:30px"></i><br /><label>' + nome + '</label></button>');
    btnResult.data('codigo', codigo);
    btnResult.click(function () {
        obterColunasTabelaPrincipal(id, nome);
    });
    result.append(btnResult);
    dvPai.append(result);
}

function obterFormularios() {
    mostrarPopup();

    $.ajax({
        url: 'Formularios/GetParaHome',
        type: 'POST',
        data: { somenteAtivos: 'S', filtrarStatus: 'S' },
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
                    desenharFormulario(dvPai, dados[indiceFormulario].ID, dados[indiceFormulario].CODIGO, dados[indiceFormulario].NOME);
                    indiceFormulario++;
                }

                dvPai = $('<div class="row"></div>');
                $('#dvPrincipal').append(dvPai);
                for (i = 0; i < totFormulariosResto; i++) {
                    desenharFormulario(dvPai, dados[indiceFormulario].ID, dados[indiceFormulario].CODIGO, dados[indiceFormulario].NOME);
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

$(document).ready(function () {
    obterFormularios();

    $('#btnFiltrar').click(function () {
        if (undefined != tabelaPrincipal &&
            ajustarPeriodoFiltro($('#dataInicialFiltro'),
            $('#dataInicialFiltro'),
            $('#dataFinalFiltro'),
            tabelaPrincipal,
            diferenciaDiasFiltro)) {
            var idStatusInicial = $('#statusFiltro').data('idStatusInicial');
            if (idStatusInicial != $('#statusFiltro').val()) {
                if ('T' == $('#statusFiltro').val()) {
                    metodoTramitacao = undefined;
                    atualizarBotaoAtendimento(false);

                    guardarDadosFiltro();
                    tabelaPrincipal.ajax.reload(null, true);
                } else {
                    verificarProximoStatus(idFormularioEscolhido, $('#statusFiltro').val());
                }
            } else {
                metodoTramitacao = undefined;
                atualizarBotaoAtendimento(true);

                limparListaLinhasSelect();
                guardarDadosFiltro();
                tabelaPrincipal.ajax.reload(null, true);
            }
        }
    });

    $('#btnLimparFiltro').click(function () {
        limparFiltro();
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

    $('#btnAtender').click(function () {
        var auxLinhasSelect = obterLinhasSelect();
        if (isNullOrEmpty(auxLinhasSelect)) {
            mostrarMsgErro('Nenhum registro foi selecionado');
        } else {
            limparFiltro();
            var auxUrl = 'Preenchimentos/Atender';
            var auxData = {
                ID_FORMULARIO: idFormularioEscolhido,
                idsPreenchimentos: auxLinhasSelect.split(';')
            };
            camposFormCadastro = iniciarCamposFormCadastro();
            atualizarFormCadastro(camposFormCadastro);
            salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData, undefined, true);
        }
    });

    $('#btnSalvar').click(function () {
        if (arquivosCarregados()) {
            fecharMsgErro();
            var auxId = $(this).prop('idRegistro');
            var idStatusAtual = $(this).prop('idStatusAtual');
            var auxUrl = metodoTramitacao;
            var auxListaComponentes = montarListaComponentes();
            var auxData = {
                ID_FORMULARIO: idFormularioEscolhido,
                LISTA_COMPONENTES: auxListaComponentes.componentesFormulario,
                ID_PREENCHIMENTO_FORMULARIO: auxId,
                ID_STATUS_ORIGEM: idStatusAtual,
                ID_STATUS_DESTINO: $('#statusTramitacao').val(),
                OBSERVACAO: $('#obsTramitacao').val(),
                arquivosTramitacao: obterArquivosUpload(),
                realizarBloqueio: 'N',
                logAlteracaoComponentes: auxListaComponentes.mensagemAlteracao
            };
            salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData, undefined);
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