var TOT_FORMULARIOS_LINHA = 4;
var guardaTituloPagina = $('#tituloPagina').html();
var idFormularioEscolhido;
var nomeFormularioEscolhido;
var exibirTodosFormEscolhido;
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
metodoGet = 'Preenchimentos/GetBuscaAvancada';
metodoDetalhes = true;
exibirCadeadoBloqueio = true;

function exibirDetalhes(id, idBusca, dataHora, nomeStatusAtual, lojaCriador) {
    limparFiltro();
    var auxData = {
        ID_FORMULARIO: idFormularioEscolhido,
        ID_PREENCHIMENTO_FORMULARIO: id
    };
    var camposFixos = {
        ID_BUSCA: idBusca,
        DATA_HORA: dataHora,
        NOME_STATUS_ATUAL: nomeStatusAtual,
        LOJA_CRIADOR: lojaCriador
    };
    montarComponentes('Preenchimentos/GetPorPreenchimento', auxData, camposFixos);
}

//Setar para habilitar o botao de alteracao
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.ID + '\', \'' + objetoTabela['Identificador'] + '\', \'' + objetoTabela['Criação'] + '\', \'' + objetoTabela['Status'] + '\', \'' + objetoTabela['Loja'] + '\'';
}
//Setar para habilitar o botao de alteracao

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
                    for (var i = 0; i < dados.length; i++) {
                        auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                        dropDownList.append(auxOption);
                    }

                    dropDownList.selectize().on('change', function () {
                        $('div.selectize-input > input').blur();
                    });

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
            }
        });
    } else {
        mostrarMsgErro('Nenhum status foi encontrado');
    }
}

function montarComponentes(auxUrl, auxData, camposFixos) {
    mostrarPopup();

    $('.dvFormGroupDinamico').remove();

    $.ajax({
        url: auxUrl,
        type: 'POST',
        data: auxData,
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            if (dados.length > 0) {
                desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'ID.', camposFixos.ID_BUSCA);
                desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Criação', camposFixos.DATA_HORA);
                desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Status', camposFixos.NOME_STATUS_ATUAL);
                desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Loja', camposFixos.LOJA_CRIADOR);
                for (var i = 0; i < dados.length; i++) {
                    desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), dados[i].componente.NOME, dados[i].VALOR_VARCHAR);
                }
                adicionarHistoricoTramitacoes(
                    $('#dvFormCadastro'),
                    $('#dvTabelaPrincipal'),
                    $('#dvComponentesDinamicos'),
                    'Tramitacoes/GetPorPreenchimento', {
                        ID_PREENCHIMENTO_FORMULARIO: auxData.ID_PREENCHIMENTO_FORMULARIO
                    }
                );
            } else {
                mostrarMsgErro('Nenhum componente foi encontrado');
            }
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os componentes, favor tente novamente');
        }
    });
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

function obterColunasTabelaPrincipal(idFormulario, nome, exibirTodos) {
    mostrarPopup();

    $('.' + dvFormGroupDinamicoFiltro).remove();

    $.ajax({
        url: 'Preenchimentos/GetColunasDinamicas',
        type: 'POST',
        data: { ID_FORMULARIO: idFormulario, somenteAtivos: 'N' },
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
                        if (dados[i].exibirEmBuscaAvancada) {
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

                    $('#idBuscaPreenchimento').val('');

                    obterStatusTramitacao(idFormularioEscolhido, { dados: listaNomeColuna, nome: nome });
                } else {
                    $('#tituloPagina').html(guardaTituloPagina);
                    mostrarMsgErro('Nenhum componente foi encontrado para o formulário ' + nome);
                }
            }
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os componentes, favor tente novamente');
        }
    });
}

function desenharFormulario(dvPai, id, codigo, nome, exibirTodos) {
    var result = $('<div class="col-lg-3"></div>');
    var btnResult = $('<button class="btn btn-md btn-primary btn-formulario"><i class="fa fa-search" style="font-size:30px"></i><br /><label>' + nome + '</label></button>');
    btnResult.data('codigo', codigo);
    btnResult.click(function () {
        obterColunasTabelaPrincipal(id, nome, exibirTodos);
    });
    result.append(btnResult);
    dvPai.append(result);
}

function obterFormularios() {
    mostrarPopup();

    $.ajax({
        url: 'Formularios/GetParaHome',
        type: 'POST',
        data: { somenteAtivos: 'N', filtrarStatus: 'S' },
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
                    desenharFormulario(dvPai, dados[indiceFormulario].ID, dados[indiceFormulario].CODIGO, dados[indiceFormulario].NOME, dados[indiceFormulario].EXIBIR_TODOS);
                    indiceFormulario++;
                }

                dvPai = $('<div class="row"></div>');
                $('#dvPrincipal').append(dvPai);
                for (i = 0; i < totFormulariosResto; i++) {
                    desenharFormulario(dvPai, dados[indiceFormulario].ID, dados[indiceFormulario].CODIGO, dados[indiceFormulario].NOME, dados[indiceFormulario].EXIBIR_TODOS);
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
            guardarDadosFiltro();
            tabelaPrincipal.ajax.reload(null, true);
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

    $('#btnCancelar').click(function () {
        mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), undefined, undefined);
    });

    $('#btnVoltar').click(function () {
        mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvTabelaPrincipal'), $('#dvTabelaFormularios'), undefined, guardaTituloPagina);
    });
});