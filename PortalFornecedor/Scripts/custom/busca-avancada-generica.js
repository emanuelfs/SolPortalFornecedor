var dvFormulariosFiltro;
var selFormulariosDualList;
var colunasTabelaPrincipal = [
    {
        data: 'ID',
        name: 'ID'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = '../Preenchimentos/GetBuscaAvancadaGenerica';
metodoDetalhes = true;
exibirCadeadoBloqueio = true;

function exibirDetalhes(idFormularioEscolhido, nomeFormularioEscolhido, id, idBusca, dataHora, nomeStatusAtual, lojaCriador, nomeCriador) {
    limparFiltro();
    var auxData = {
        ID_FORMULARIO: idFormularioEscolhido,
        ID_PREENCHIMENTO_FORMULARIO: id
    };
    var camposFixos = {
        ID_BUSCA: idBusca,
        DATA_HORA: dataHora,
        NOME_STATUS_ATUAL: nomeStatusAtual,
        LOJA_CRIADOR: lojaCriador,
        NOME_CRIADOR: nomeCriador,
        NOME_FORMULARIO: nomeFormularioEscolhido
    };
    montarComponentes('../Preenchimentos/GetPorPreenchimento', auxData, camposFixos);
}

//Setar para habilitar o botao de alteracao
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.ID_FORMULARIO + '\', \'' +
    objetoTabela['Formulário'] + '\', \'' +
    objetoTabela.ID + '\', \'' +
    objetoTabela['Identificador'] + '\', \'' +
    objetoTabela['Criação'] + '\', \'' +
    objetoTabela['Status'] + '\', \'' +
    objetoTabela['Loja'] + '\', \'' +
    objetoTabela['Responsável'] + '\'';
}
//Setar para habilitar o botao de alteracao

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
                desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Formulário', camposFixos.NOME_FORMULARIO);
                desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'ID.', camposFixos.ID_BUSCA);
                desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Criação', camposFixos.DATA_HORA);
                desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Status', camposFixos.NOME_STATUS_ATUAL);
                desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Loja', camposFixos.LOJA_CRIADOR);
                desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), 'Responsável', camposFixos.NOME_CRIADOR);
                for (var i = 0; i < dados.length; i++) {
                    desenharComponenteSomenteLeitura($('#dvComponentesDinamicos'), dados[i].componente.NOME, dados[i].VALOR_VARCHAR);
                }
                adicionarHistoricoTramitacoes(
                    $('#dvFormCadastro'),
                    $('#dvTabelaPrincipal'),
                    $('#dvComponentesDinamicos'),
                    '../Tramitacoes/GetPorPreenchimento', {
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

function inicializarFiltro() {
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

    $('#idBuscaPreenchimento').val('');
}

function possuiFormularios() {
    return null != $('#selFormularios').val() && undefined != $('#selFormularios').val();
}

function atualizarListaFormulariosEscolhidos() {
    $('#listaFormularios').html('');
    if (possuiFormularios()) {
        var itensSelect = $($('#selFormularios')[0]).find('option:selected');
        if (undefined != itensSelect && itensSelect.length > 0) {
            $('#listaFormularios').html($(itensSelect[0]).html());
            for (var i = 1; i < itensSelect.length; i++) {
                $('#listaFormularios').html($('#listaFormularios').html() + ', ' + $(itensSelect[i]).html());
            }
        } else {
            $('#listaFormularios').html('Nenhum Formulário foi Informado');
        }
    } else {
        $('#listaFormularios').html('Nenhum Formulário foi Informado');
    }
}

function inicializarModalFormularios() {
    dvFormulariosFiltro = $('#dvFormulariosFiltro').dialog({
        resizable: false,
        height: 'auto',
        width: '80%',
        modal: true,
        autoOpen: false,
        draggable: false,
        show: {
            effect: "blind",
            duration: 400
        },
        hide: {
            effect: "blind",
            duration: 400
        },
        open: function (event, ui) {
            $(this).parent().find('.ui-dialog-titlebar-close').hide();
        },
        buttons: [
            {
                text: 'Confirmar',
                'class': 'btn btn-sm btn-success',
                'style': 'font-weight: bold; font-size: 12px; margin-right: 10px;',
                click: function () {
                    mostrarPopup();
                    atualizarListaFormulariosEscolhidos();
                    atualizarSelectFormularios();
                    fecharPopup();
                    $(this).dialog("close");
                }
            },
            {
                text: 'Cancelar',
                'class': 'btn btn-sm btn-danger',
                'style': 'font-weight: bold; font-size: 12px;',
                click: function () {
                    $(this).dialog("close");
                    reverterSelectFormularios();
                }
            }
        ],
    });
}

function desenharTabelaPrincipal() {
    mostrarPopup();

    colunasTabelaPrincipal = [];

    var dados = [
        {
            nomeColuna: 'ID_FORMULARIO',
            exibirColuna: false
        },
        {
            nomeColuna: 'ID',
            exibirColuna: false
        },
        {
            nomeColuna: 'ID_STATUS_ATUAL',
            exibirColuna: false
        },
        {
            nomeColuna: 'NOME_RESPONSAVEL',
            exibirColuna: false
        },
        {
            nomeColuna: 'Formulário',
            exibirColuna: true
        },
        {
            nomeColuna: 'Criação',
            exibirColuna: true
        },
        {
            nomeColuna: 'Identificador',
            exibirColuna: true
        },
        {
            nomeColuna: 'Status',
            exibirColuna: true
        },
        {
            nomeColuna: 'Loja',
            exibirColuna: true
        },
        {
            nomeColuna: 'Responsável',
            exibirColuna: true
        }
    ];

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
    }
    linhaColunasDinamicas.append('<th style="text-align: center;">Ações</th>');
    theadTabelaPrincipal.append(linhaColunasDinamicas);
    tabelaPrincipal.append(theadTabelaPrincipal);
    $('#dvPaiTabela').append(tabelaPrincipal);

    colunasTabelaPrincipal[colunasTabelaPrincipal.length] = {
        render: renderColunaOpcoes,
        mRender: renderColunaOpcoes
    }

    guardarDadosFiltro();
    inicializarTabelaPrincipalColunasDinamicas($('#tabelaPrincipal'), colunasTabelaPrincipal, '', listaNomeColuna, $('#dataInicialFiltro').val(), $('#dataFinalFiltro').val());
    mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'));

    fecharPopup();
}

function inicializarSelectFormularios() {
    selFormulariosDualList = $('#selFormularios').bootstrapDualListbox({
        nonSelectedListLabel: '<b>Dispon&iacute;veis</b>',
        selectedListLabel: '<b>Escolhidos</b>'
    });
}

function atualizarSelectFormularios() {
    var auxItensSelect = $($('#selFormularios')[0]).find('option');
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
    $('#selFormularios').data('totItensEscolhidos', $($('#selFormularios')[0]).find('option:selected').length);
    $('#selFormularios').data('itensSelect', itensSelect);
}

function reverterSelectFormularios() {
    if ('none' == $('#dvFormulariosFiltro').parent().css('display')) {
        var itensSelect = $('#selFormularios').data('itensSelect');
        if (undefined != itensSelect) {
            $('#selFormularios').empty();
            for (var i = 0; i < itensSelect.length; i++) {
                $('#selFormularios').append($(itensSelect[i]));
            }
            selFormulariosDualList.bootstrapDualListbox('refresh');
        }
        atualizarSelectFormularios();
    } else {
        setTimeout(function () {
            reverterSelectFormularios();
        }, 200);
    }
}

function obterFormularios() {
    mostrarPopup();
    $.ajax({
        url: '../Formularios/GetParaHome',
        type: 'POST',
        data: { somenteAtivos: 'N', filtrarStatus: 'S' },
        success: function (result) {
            var dados = result.data;

            if (undefined != dados && dados.length > 0) {
                if (undefined == selFormulariosDualList) {
                    inicializarSelectFormularios();
                }

                $('#selFormularios').empty();

                var auxOption;
                for (var i = 0; i < dados.length; i++) {
                    auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                    $('#selFormularios').append(auxOption);
                }

                atualizarSelectFormularios();
                selFormulariosDualList.bootstrapDualListbox('refresh');
                mostrarFormulariosFiltro();
                guardarFormulariosFiltro();
            } else {
                mostrarMsgErro('Nenhum formulário foi encontrado');
            }

            fecharPopup();
        },
        error: function (request, status, error) {
            mostrarMsgErro('Falha ao tentar obter os formulários, favor tente novamente');
            fecharPopup();
        }
    });
}

function mostrarFormulariosFiltro() {
    $('#selFormularios').parent().find('input.filter').each(function () {
        $(this).val('');
        $(this).change();
    });
    dvFormulariosFiltro.dialog('open');
    $('#dvFormulariosFiltro').css('height', 'auto');
}

function obterFormulariosFiltro(formulariosFiltro) {
    if (undefined != selFormulariosDualList && undefined != formulariosFiltro && formulariosFiltro.length > 0) {
        $('#selFormularios').empty();
        var idFormulario;
        var nomeFormulario;
        var selecionado;
        for (var i = 0 ; i < formulariosFiltro.length; i++) {
            idFormulario = formulariosFiltro[i].ID;
            nomeFormulario = formulariosFiltro[i].NOME;
            selecionado = formulariosFiltro[i].SELECTED;
            if ('S' == selecionado) {
                $('#selFormularios').append($('<option selected value="' + idFormulario + '">' + nomeFormulario + '</option>'));
            } else {
                $('#selFormularios').append($('<option value="' + idFormulario + '">' + nomeFormulario + '</option>'));
            }
        }
        atualizarListaFormulariosEscolhidos();
        atualizarSelectFormularios();
        selFormulariosDualList.bootstrapDualListbox('refresh');
    }
}

function guardarFormulariosFiltro() {
    var formulariosFiltro = [];
    var indiceFormulariosFiltro = 0;
    var itensSelect = $('#selFormularios').data('itensSelect');
    if (undefined != itensSelect) {
        for (var i = 0; i < itensSelect.length; i++) {
            formulariosFiltro[indiceFormulariosFiltro++] = {
                ID: $(itensSelect[i]).val(),
                NOME: $(itensSelect[i]).html(),
                SELECTED: undefined != $(itensSelect[i]).attr('selected') ? 'S' : 'N'
            };
        }
    }
    tabelaPrincipal.formulariosFiltro = formulariosFiltro;
}

$(document).ready(function () {
    inicializarFiltro();
    inicializarModalFormularios();
    desenharTabelaPrincipal();

    $('#btnEscolherForms').click(function () {
        if (undefined == selFormulariosDualList) {
            obterFormularios();
        } else {
            mostrarFormulariosFiltro();
        }
    });

    $('#btnFiltrar').click(function () {
        if (undefined != tabelaPrincipal &&
            ajustarPeriodoFiltro($('#dataInicialFiltro'),
            $('#dataInicialFiltro'),
            $('#dataFinalFiltro'),
            tabelaPrincipal,
            diferenciaDiasFiltro)) {
            $('#dvDadosTabelaPrincipal').css('display', '');
            guardarDadosFiltro();
            tabelaPrincipal.ajax.reload(null, true);
        }
    });

    $('#btnLimparFiltro').click(function () {
        limparFiltro();
    });

    $('#btnExportarExcel').click(function () {
        var dadosFiltro = {
            formulariosFiltro: obterIdsFormulariosFiltro(tabelaPrincipal.formulariosFiltro),
            dataInicialFiltro: tabelaPrincipal.dataInicialFiltro,
            dataFinalFiltro: tabelaPrincipal.dataFinalFiltro,
            idBuscaPreenchimento: tabelaPrincipal.idBuscaPreenchimento
        };
        exportarArquivo('../Preenchimentos/GetParaExcelGenerica', '../Preenchimentos/ExportarExcel', 'Busca Avançada Genérica', dadosFiltro);
    });

    $('#btnCancelar').click(function () {
        mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), undefined, undefined);
    });
});