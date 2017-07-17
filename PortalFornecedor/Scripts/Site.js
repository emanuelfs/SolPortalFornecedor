//Variáveis
var divLoader;
var dvMsgSucesso;
var dvMsgErro;
var tabelaPrincipal;
var options = {
    excluded: [],
    message: 'This value is not valid',
    feedbackIcons: {
        valid: 'glyphicon glyphicon-ok',
        invalid: 'glyphicon glyphicon-remove',
        validating: 'glyphicon glyphicon-refresh'
    },
    fields: {

    }
};
var diferenciaDiasFiltro = 30;
var valorPadraoMeusLancamentos = 'M';
var valorPadraoTodosLancamentos = 'T';
var metodoGet = undefined;
var metodoInsert = undefined;
var metodoUpdate = undefined;
var metodoDelete = undefined;
var metodoTramitacao = undefined;
var metodoDetalhes = false;
var metodoSelectLinha = false;
var metodoClonar = false;
var exibirCadeadoBloqueio = false;
var listaLinhasSelect;
var textoPadraoMarcarLinhaSelect = 'Selecionar';
var textoPadraoDesmarcarLinhaSelect = 'Desmarcar';
var textoPadraoNenhumArquivo = 'Nenhum arquivo foi informado';
var listaArquivosUpload = undefined;
var msgPadraoLimiteUploadsAtingido = 'N&atilde;o &eacute; poss&iacute;vel anexar mais nenhum arquivo, pois o limite j&aacute; foi atingido';
var LIMITE_ARQUIVOS_FORMULARIO = undefined;
var LIMITE_TAMANHO_ARQUIVO_UPLOAD = undefined;
var TIPOS_ARQUIVO_FORMULARIO = undefined;
var LIMITE_NOME_ARQUIVO_TRAMITACAO = undefined;
//Variáveis

//Utils
function mostrarPopup() {
    divLoader.dialog('open');
    $('#divLoader').css('height', 'auto');
}

function fecharPopup() {
    divLoader.dialog('close');
}

function mostrarMsgSucesso(msg) {
    $('#txtMsgSucesso').html(msg);
    dvMsgSucesso.dialog('open');
}

function fecharMsgSucesso() {
    dvMsgSucesso.dialog('close');
}

function mostrarMsgErro(msg) {
    $('#txtMsgErro').html(msg);
    dvMsgErro.dialog('open');
}

function fecharMsgErro() {
    dvMsgErro.dialog('close');
}

function isNullOrEmpty(texto) {
    return undefined == texto
    || null == texto
    || '' == texto.toString().trim();
}

function removerAcentos(textoOriginal) {
    return textoOriginal
    .toLowerCase()

    .replace(new RegExp('\u00e0', 'g'), 'a')
    .replace(new RegExp('\u00e1', 'g'), 'a')
    .replace(new RegExp('\u00e2', 'g'), 'a')
    .replace(new RegExp('\u00e3', 'g'), 'a')
    .replace(new RegExp('\u00e4', 'g'), 'a')

    .replace(new RegExp('\u00e9', 'g'), 'e')
    .replace(new RegExp('\u00e8', 'g'), 'e')
    .replace(new RegExp('\u00ea', 'g'), 'e')
    .replace(new RegExp('\u00eb', 'g'), 'e')

    .replace(new RegExp('\u00ec', 'g'), 'i')
    .replace(new RegExp('\u00ed', 'g'), 'i')
    .replace(new RegExp('\u00ee', 'g'), 'i')
    .replace(new RegExp('\u00ef', 'g'), 'i')

    .replace(new RegExp('\u00f2', 'g'), 'o')
    .replace(new RegExp('\u00f3', 'g'), 'o')
    .replace(new RegExp('\u00f4', 'g'), 'o')
    .replace(new RegExp('\u00f5', 'g'), 'o')
    .replace(new RegExp('\u00f6', 'g'), 'o')

    .replace(new RegExp('\u00f9', 'g'), 'u')
    .replace(new RegExp('\u00fa', 'g'), 'u')
    .replace(new RegExp('\u00fb', 'g'), 'u')
    .replace(new RegExp('\u00fc', 'g'), 'u')

    .replace(new RegExp('\u00e7', 'g'), 'c');
}

function ativarMenuEscolhido() {
    $(".sidebar li ul li ul li a").each(function () {
        if (this.href == window.location.href) {
            $(this).parent().addClass("active");
            $(this).parent().parent().parent("li").addClass("open");
            $(this).parent().parent().css('display', 'block');

            $(this).parent().parent().parent().parent().parent("li").addClass("open");
            $(this).parent().parent().parent().parent().css('display', 'block')

        }
    });

    $(".sidebar li ul li a").each(function () {
        if (this.href == window.location.href) {
            $(this).parent().addClass("active");
            $(this).parent().parent().parent("li").addClass("open");
            $(this).parent().parent().css('display', 'block')
        }
    });
}
//Utils

//Painels
function prepararDvTabelaPrincipalParaExibicao(msgSucesso, tituloPagina) {
    if (undefined != tituloPagina) {
        $('#tituloPagina').html(tituloPagina);
    }
    if (undefined != msgSucesso) {
        if (undefined != tabelaPrincipal) {
            guardarDadosFiltro();
            tabelaPrincipal.ajax.reload(null, false);
        }
        mostrarMsgSucesso(msgSucesso);
    } else {
        fecharMsgSucesso();
        fecharMsgErro();
    }
}

function mostrarDvTabelaPrincipal(auxFormCadastro, auxDvFormCadastro, auxDvTabelaPrincipal, msgSucesso, tituloPagina) {
    if (undefined != auxFormCadastro) {
        auxFormCadastro.bootstrapValidator(options);
        auxFormCadastro.data('bootstrapValidator').resetForm();
    }

    //Limpando algumas variávies
    listaArquivosUpload = undefined;
    //Limpando algumas variávies

    if ('none' == auxDvTabelaPrincipal.css('display')) {
        auxDvFormCadastro.slideToggle(0, function () {
            prepararDvTabelaPrincipalParaExibicao(msgSucesso, tituloPagina);
            auxDvTabelaPrincipal.slideToggle(0);
        });
    } else {
        prepararDvTabelaPrincipalParaExibicao(msgSucesso, tituloPagina);
    }
}

function inicializarSelects(listaSelects) {
    if (undefined != listaSelects) {
        var registrosObtidos;
        var idRegistroEscolhido;
        var funcaoInicializacao;
        for (var i = 0; i < listaSelects.length; i++) {
            registrosObtidos = listaSelects[i].registrosObtidos;
            idRegistroEscolhido = listaSelects[i].idRegistroEscolhido;
            funcaoInicializacao = listaSelects[i].funcaoInicializacao;
            if (undefined != registrosObtidos && registrosObtidos == false) {
                funcaoInicializacao(idRegistroEscolhido);
            }
        }
    }
}

function mostrarDvFormCadastro(auxDvFormCadastro, auxDvTabelaPrincipal, listaSelects) {
    auxDvTabelaPrincipal.slideToggle(0, function () {
        fecharMsgSucesso();
        fecharMsgErro();
        auxDvFormCadastro.slideToggle(0, function () {
            inicializarSelects(listaSelects)
        });
    });
}
//Painels

//TabelaPrincipal
function iniciarListaLinhasSelect() {
    listaLinhasSelect = $('<div style="display: none;"></div>');
}

function limparListaLinhasSelect() {
    if (metodoSelectLinha && undefined != listaLinhasSelect) {
        listaLinhasSelect.find('.idLinhaSelect').each(function () {
            $('#cbxSelectLinha' + $(this).data('idLinha')).removeAttr('checked');
            $('#cbxSelectLinha' + $(this).data('idLinha')).attr('data-original-title', textoPadraoMarcarLinhaSelect);
        });
        listaLinhasSelect.html('');
    }
}

function atualizarLinhaListaLinhasSelect(cbxSelectLinha, idLinha) {
    if ($(cbxSelectLinha).is(':checked')) {
        $(cbxSelectLinha).attr('data-original-title', textoPadraoDesmarcarLinhaSelect);
        adicionarLinhaListaLinhasSelect(idLinha);
    } else {
        $(cbxSelectLinha).attr('data-original-title', textoPadraoMarcarLinhaSelect);
        removerLinhaListaLinhasSelect(idLinha)
    }
}

function adicionarLinhaListaLinhasSelect(idLinha) {
    var idLinhaSelect = $('<input id="idLinhaSelect' + idLinha + '" class="idLinhaSelect" type="hidden" value="' + idLinha + '"/>');
    idLinhaSelect.data('idLinha', idLinha);
    listaLinhasSelect.append(idLinhaSelect);
}

function removerLinhaListaLinhasSelect(idLinha) {
    listaLinhasSelect.find('#idLinhaSelect' + idLinha).remove();
}

function obterLinhasSelect() {
    var result = '';
    listaLinhasSelect.find('.idLinhaSelect').each(function () {
        if ('' == result) {
            result += $(this).data('idLinha');
        } else {
            result += ';' + $(this).data('idLinha');
        }
    });
    return result;
}

function renderColunaOpcoes(data, type, full) {
    var result = $('<div></div>');
    var dvPrincipal = $('<div style="min-width: 185px; text-align: center;"></div>');

    if (metodoSelectLinha) {
        var cbxSelectLinha;
        if (undefined != full['Identificador']) {
            cbxSelectLinha = $('<input id="cbxSelectLinha' + full.ID + '_' + full['Identificador'] + '" type="checkbox" style="margin-right: 10px; cursor: pointer;" data-toggle="tooltip" data-original-title="' + textoPadraoMarcarLinhaSelect + '" onclick="atualizarLinhaListaLinhasSelect(this, ' + obterIdParaCheckBoxDataTable(full) + ')" />');
        } else {
            cbxSelectLinha = $('<input id="cbxSelectLinha' + full.ID + '" type="checkbox" style="margin-right: 10px; cursor: pointer;" data-toggle="tooltip" data-original-title="' + textoPadraoMarcarLinhaSelect + '" onclick="atualizarLinhaListaLinhasSelect(this, ' + obterIdParaCheckBoxDataTable(full) + ')" />');
        }
        dvPrincipal.append(cbxSelectLinha);
    }

    if (exibirCadeadoBloqueio && !isNullOrEmpty(full.NOME_RESPONSAVEL)) {
        var iconeBloqueio = $('' +
        '<div style="display: inline-block; margin-right: 10px; color: #f39c12;" data-toggle="tooltip" data-original-title="' + full.NOME_RESPONSAVEL + '">' +
        	'<i class="fa fa-lock"></i>' +
        '</div>');
        dvPrincipal.append(iconeBloqueio);
    }

    if (metodoDetalhes) {
        var btnVerDetalhes = $('' +
        '<button onclick="exibirDetalhes(' + colunasTabelaAlteracao(full) + ');" class="btn btn-sm btn-success" style="margin-right: 10px;" type="button" data-toggle="tooltip" data-original-title="Ver Detalhes">' +
	    	'<i class="fa fa-search"></i>' +
	    '</button>');
        dvPrincipal.append(btnVerDetalhes);
    }

    if (undefined != metodoTramitacao) {
        var btnTramitar = $('' +
        '<button onclick="tramitar(' + colunasTabelaAlteracao(full) + ');" class="btn btn-sm btn-success" style="margin-right: 10px;" type="button" data-toggle="tooltip" data-original-title="Tramitar">' +
	    	'<i class="fa fa-cog"></i>' +
	    '</button>');
        dvPrincipal.append(btnTramitar);
    }

    if (metodoClonar) {
        var btnClonar = $('' +
        '<button onclick="carregarFormClonagem(' + colunasTabelaAlteracao(full) + ');" class="btn btn-sm btn-success" style="margin-right: 10px;" type="button" data-toggle="tooltip" data-original-title="Clonar">' +
	    	'<i class="fa fa-files-o"></i>' +
	    '</button>');
        dvPrincipal.append(btnClonar);
    }

    if (undefined != metodoUpdate) {
        var btnAlterar = $('' +
        '<button onclick="carregarFormCadastro(' + colunasTabelaAlteracao(full) + ');" class="btn btn-sm btn-success" style="margin-right: 10px;" type="button" data-toggle="tooltip" data-original-title="Alterar">' +
	    	'<i class="glyphicon glyphicon-edit"></i>' +
	    '</button>');
        dvPrincipal.append(btnAlterar);
    }

    if (undefined != metodoDelete) {
        var permitirExclusao = !isNullOrEmpty(colunasTabelaRemocao(full));
        var btnExcluir;
        if (permitirExclusao) {
            btnExcluir = $('' +
            '<button onclick="excluirRegistro(' + colunasTabelaRemocao(full) + ');" class="btn btn-sm btn-danger" type="button" data-toggle="tooltip" data-original-title="Excluir">' +
                '<i class="glyphicon glyphicon-trash"></i>' +
            '</button>');
        } else {
            btnExcluir = $('' +
            '<button class="btn btn-sm btn-danger" style="cursor: not-allowed; opacity: 0.65;" readonly="readonly" type="button">' +
                '<i class="glyphicon glyphicon-trash"></i>' +
            '</button>');
        }
        dvPrincipal.append(btnExcluir);
    }

    result.append(dvPrincipal);
    return result.html();
}

function voltarPaginaTabelaPrincipal() {
    var auxBtnPrevious = document.getElementById('tabelaPrincipal_previous');
    if (undefined != auxBtnPrevious) {
        $('#tabelaPrincipal_previous').click();
    }
}

function inicializarTabelaPrincipal(auxTabelaPrincipal, listaColunas) {
    if (undefined != metodoGet) {
        tabelaPrincipal = auxTabelaPrincipal
        .on('preXhr.dt', function (e, settings, data) {
            fecharMsgErro();
        })
        .DataTable({
            aaSorting: [],
            serverSide: true,
            processing: true,
            scrollX: true,
            oLanguage: {
                sProcessing: '<img style="width:30px; height:30px;" src="/Resources/loader.gif"/>'
            },
            ajax: {
                url: metodoGet,
                type: 'POST',
                error: function (request, status, error) {
                    $('.dataTables_processing').css('display', 'none');
                    mostrarMsgErro('Falha na conex&atilde;o, favor tente novamente');
                }
            },
            columns: listaColunas
        });
    }
}

function limparFiltro() {
    fecharMsgErro();
    if (undefined != tabelaPrincipal) {
        if (undefined != tabelaPrincipal.dataInicialFiltro) {
            $('#dataInicialFiltro').val(tabelaPrincipal.dataInicialFiltro);
            $('#dataInicialFiltro').datepicker('update');
        }
        if (undefined != tabelaPrincipal.dataFinalFiltro) {
            $('#dataFinalFiltro').val(tabelaPrincipal.dataFinalFiltro);
            $('#dataFinalFiltro').datepicker('update');
        }
        if (undefined != tabelaPrincipal.statusForm) {
            if (undefined != $('#statusFiltro')[0] && undefined != $('#statusFiltro')[0].selectize) {
                $('#statusFiltro')[0].selectize.setValue(tabelaPrincipal.statusForm);
            }
        }
        if (undefined != tabelaPrincipal.tipoLancamentosFiltro) {
            if (undefined != $('#tipoLancamentosFiltro')[0] && undefined != $('#tipoLancamentosFiltro')[0].selectize) {
                $('#tipoLancamentosFiltro')[0].selectize.setValue(tabelaPrincipal.tipoLancamentosFiltro);
            }
        }
        if (undefined != tabelaPrincipal.camposDinamicosFiltro) {
            try {
                obterCamposDinamicosFiltro(tabelaPrincipal.camposDinamicosFiltro);
            } catch (ex) {
                //Função só existe na busca avançada
            }
        }
        if (undefined != tabelaPrincipal.formulariosFiltro) {
            try {
                obterFormulariosFiltro(tabelaPrincipal.formulariosFiltro);
            } catch (ex) {
                //Função só existe na busca avançada genérica
            }
        }
        if (undefined != tabelaPrincipal.idBuscaPreenchimento) {
            $('#idBuscaPreenchimento').val(tabelaPrincipal.idBuscaPreenchimento);
        } else if (!isNullOrEmpty($('#idBuscaPreenchimento')[0])) {
            $('#idBuscaPreenchimento').val('');
        }
    }
}

function guardarDadosFiltro() {
    if (undefined != tabelaPrincipal) {
        tabelaPrincipal.dataInicialFiltro = $('#dataInicialFiltro').val();
        tabelaPrincipal.dataFinalFiltro = $('#dataFinalFiltro').val();
        tabelaPrincipal.statusForm = $('#statusFiltro').val();
        tabelaPrincipal.tipoLancamentosFiltro = $('#tipoLancamentosFiltro').val();
        try {
            guardarCamposDinamicosFiltro();
        } catch (ex) {
            //Função só existe na busca avançada
        }
        try {
            guardarFormulariosFiltro();
        } catch (ex) {
            //Função só existe na busca avançada genérica
        }
        tabelaPrincipal.idBuscaPreenchimento = $('#idBuscaPreenchimento').val();
        tabelaPrincipal.podeFiltrar = true;
        tabelaPrincipal.start = undefined;
        tabelaPrincipal.sortColumn = undefined;
        tabelaPrincipal.sortColumnDir = undefined;
        tabelaPrincipal.search = undefined;
    }
}

function podeFiltrarTabelaPrincipalColunasDinamicas(aoData) {
    var podeFiltrar = undefined == tabelaPrincipal || tabelaPrincipal.podeFiltrar == true;
    if (undefined != tabelaPrincipal) {
        if (undefined != aoData) {
            if (undefined != aoData[3]) {
                if (aoData[3].value == tabelaPrincipal.start) {
                    podeFiltrar = false;
                } else {
                    tabelaPrincipal.start = aoData[3].value;
                    podeFiltrar = true;
                }
            }
            if (!podeFiltrar) {
                if (undefined != aoData[2] &&
                    undefined != aoData[2].value[0] &&
                    undefined != aoData[2].value[0].column) {
                    if (aoData[2].value[0].column == tabelaPrincipal.sortColumn) {
                        podeFiltrar = false;
                    } else {
                        tabelaPrincipal.sortColumn = aoData[2].value[0].column;
                        tabelaPrincipal.sortColumnDir = aoData[2].value[0].dir;
                        podeFiltrar = true;
                    }
                    if (!podeFiltrar) {
                        if (aoData[2].value[0].dir == tabelaPrincipal.sortColumnDir) {
                            podeFiltrar = false;
                        } else {
                            tabelaPrincipal.sortColumn = aoData[2].value[0].column;
                            tabelaPrincipal.sortColumnDir = aoData[2].value[0].dir;
                            podeFiltrar = true;
                        }
                    }
                }
            }
            if (!podeFiltrar) {
                if (undefined != aoData[5] &&
                    undefined != aoData[5].value) {
                    if (aoData[5].value.value == tabelaPrincipal.search) {
                        podeFiltrar = false;
                    } else {
                        tabelaPrincipal.search = aoData[5].value.value;
                        podeFiltrar = true;
                    }
                }
            }
        }
        tabelaPrincipal.podeFiltrar == false;
    }
    return podeFiltrar;
}

function obterIdsFormulariosFiltro(auxFormulariosFiltro) {
    var idsFormulariosFiltro = [];
    var indiceIdsFormulariosFiltro = 0;
    for (var i = 0; i < auxFormulariosFiltro.length; i++) {
        if ('S' == auxFormulariosFiltro[i].SELECTED) {
            idsFormulariosFiltro[indiceIdsFormulariosFiltro++] = auxFormulariosFiltro[i].ID;
        }
    }
    return idsFormulariosFiltro;
}

function inicializarTabelaPrincipalColunasDinamicas(auxTabelaPrincipal, listaColunas, auxIdFormulario, auxColunasFormulario, auxDataInicialFiltro, auxDataFinalFiltro, auxStatusForm, auxTipoLancamentosFiltro, auxCamposDinamicosFiltro, auxFormulariosFiltro) {
    var auxIdBuscaPreenchimento;
    if (undefined != metodoGet) {
        tabelaPrincipal = auxTabelaPrincipal
        .on('preXhr.dt', function (e, settings, data) {
            fecharMsgErro();
        })
        .DataTable({
            aaSorting: [],
            serverSide: true,
            processing: true,
            scrollX: true,
            oLanguage: {
                sProcessing: '<img style="width:30px; height:30px;" src="/Resources/loader.gif"/>'
            },
            fnServerData: function (sSource, aoData, fnCallback) {
                if (podeFiltrarTabelaPrincipalColunasDinamicas(aoData)) {
                    if (undefined != tabelaPrincipal) {
                        if (undefined != tabelaPrincipal.dataInicialFiltro) {
                            auxDataInicialFiltro = tabelaPrincipal.dataInicialFiltro;
                            $('#dataInicialFiltro').val(auxDataInicialFiltro);
                            $('#dataInicialFiltro').datepicker('update');
                        }
                        if (undefined != tabelaPrincipal.dataFinalFiltro) {
                            auxDataFinalFiltro = tabelaPrincipal.dataFinalFiltro;
                            $('#dataFinalFiltro').val(auxDataFinalFiltro);
                            $('#dataFinalFiltro').datepicker('update');
                        }
                        if (undefined != tabelaPrincipal.statusForm) {
                            auxStatusForm = tabelaPrincipal.statusForm;
                            if (undefined != $('#statusFiltro')[0] && undefined != $('#statusFiltro')[0].selectize) {
                                $('#statusFiltro')[0].selectize.setValue(auxStatusForm);
                            }
                        }
                        if (undefined != tabelaPrincipal.tipoLancamentosFiltro) {
                            auxTipoLancamentosFiltro = tabelaPrincipal.tipoLancamentosFiltro;
                            if (undefined != $('#tipoLancamentosFiltro')[0] && undefined != $('#tipoLancamentosFiltro')[0].selectize) {
                                $('#tipoLancamentosFiltro')[0].selectize.setValue(auxTipoLancamentosFiltro);
                            }
                        }
                        if (undefined != tabelaPrincipal.camposDinamicosFiltro) {
                            try {
                                auxCamposDinamicosFiltro = tabelaPrincipal.camposDinamicosFiltro;
                                obterCamposDinamicosFiltro(auxCamposDinamicosFiltro);
                            } catch (ex) {
                                //Função só existe na busca avançada
                            }
                        }
                        if (undefined != tabelaPrincipal.formulariosFiltro) {
                            try {
                                auxFormulariosFiltro = tabelaPrincipal.formulariosFiltro;
                                obterFormulariosFiltro(auxFormulariosFiltro);
                            } catch (ex) {
                                //Função só existe na busca avançada genérica
                            }
                        }
                        if (undefined != tabelaPrincipal.idBuscaPreenchimento) {
                            auxIdBuscaPreenchimento = tabelaPrincipal.idBuscaPreenchimento;
                            $('#idBuscaPreenchimento').val(auxIdBuscaPreenchimento);
                        }
                    }
                    var dados = { requestModel: JSON.stringify(aoData), idFormulario: auxIdFormulario, colunasFormulario: auxColunasFormulario, dataInicialFiltro: auxDataInicialFiltro, dataFinalFiltro: auxDataFinalFiltro, statusForm: auxStatusForm };
                    if (undefined != auxTipoLancamentosFiltro) {
                        dados.tipoLancamentosFiltro = auxTipoLancamentosFiltro;
                    }
                    if (undefined != auxCamposDinamicosFiltro) {
                        dados.camposDinamicosFiltro = auxCamposDinamicosFiltro;
                    }
                    if (undefined != auxFormulariosFiltro) {
                        dados.formulariosFiltro = obterIdsFormulariosFiltro(auxFormulariosFiltro);
                    }
                    if (undefined != auxIdBuscaPreenchimento) {
                        dados.idBuscaPreenchimento = auxIdBuscaPreenchimento;
                    }
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        url: metodoGet,
                        data: JSON.stringify(dados),
                        success: function (result) {
                            var auxLinhas = result.data;
                            if (undefined != auxLinhas) {
                                var auxLinha;
                                var linhas = [];
                                var nomeColuna;
                                var valorColuna;
                                var jsonLinha;
                                for (var i = 0; i < auxLinhas.length; i++) {
                                    auxLinha = auxLinhas[i];
                                    jsonLinha = '{';
                                    if (auxLinha.length > 0) {
                                        nomeColuna = auxLinha[0].Key;
                                        valorColuna = auxLinha[0].Value;
                                        jsonLinha += '"' + nomeColuna + '":"' + valorColuna + '"';
                                        for (var j = 1; j < auxLinha.length; j++) {
                                            nomeColuna = auxLinha[j].Key;
                                            valorColuna = auxLinha[j].Value;
                                            jsonLinha += ',"' + nomeColuna + '":"' + valorColuna + '"';
                                        }
                                    }
                                    jsonLinha += '}';
                                    linhas[i] = JSON.parse(jsonLinha);
                                }
                                result.data = linhas;
                                fnCallback(result);
                                if (undefined != result.voltarPagina) {
                                    voltarPaginaTabelaPrincipal();
                                }
                                if (metodoSelectLinha && undefined != listaLinhasSelect) {
                                    listaLinhasSelect.find('.idLinhaSelect').each(function () {
                                        $('#cbxSelectLinha' + $(this).data('idLinha')).attr('checked', 'checked');
                                        $('#cbxSelectLinha' + $(this).data('idLinha')).attr('data-original-title', textoPadraoDesmarcarLinhaSelect);
                                    });
                                }
                            }
                        },
                        error: function (request, status, error) {
                            $('.dataTables_processing').css('display', 'none');
                            mostrarMsgErro('Falha na conex&atilde;o, favor tente novamente');
                        }
                    });
                } else {
                    $('.dataTables_processing').css('display', 'none');
                }
            },
            columns: listaColunas
        });
        tabelaPrincipal.dataInicialFiltro = auxDataInicialFiltro;
        tabelaPrincipal.dataFinalFiltro = auxDataFinalFiltro;
        tabelaPrincipal.statusForm = auxStatusForm;
        tabelaPrincipal.tipoLancamentosFiltro = auxTipoLancamentosFiltro;
        try {
            guardarCamposDinamicosFiltro();
        } catch (ex) {
            //Função só existe na busca avançada
        }
        try {
            guardarFormulariosFiltro();
        } catch (ex) {
            //Função só existe na busca avançada genérica
        }
        tabelaPrincipal.idBuscaPreenchimento = auxIdBuscaPreenchimento;
        tabelaPrincipal.start = 0;
    }
}

function salvarRegistro(auxFormCadastro, auxDvFormCadastro, auxDvTabelaPrincipal, auxUrl, auxData, tituloPagina, continuar, limparParaContinuar) {
    auxFormCadastro.data('bootstrapValidator').validate();
    if (auxFormCadastro.data('bootstrapValidator').isValid()) {
        mostrarPopup();

        $.ajax({
            url: auxUrl,
            type: 'POST',
            data: auxData,
            success: function (result) {
                fecharPopup();

                var msgErro = result.msgErro;
                if (undefined != msgErro && '' != msgErro.toString().trim()) {
                    mostrarMsgErro(msgErro);
                } else {
                    if (undefined == continuar || false == continuar) {
                        mostrarDvTabelaPrincipal(auxFormCadastro, auxDvFormCadastro, auxDvTabelaPrincipal, result.msgSucesso, tituloPagina);
                    } else if (undefined != result.msgSucesso) {
                        if (undefined != tabelaPrincipal) {
                            guardarDadosFiltro();
                            tabelaPrincipal.ajax.reload(null, false);
                        }
                        limparListaLinhasSelect();
                        if (limparParaContinuar) {
                            $('[name*=' + NAME_PADRAO_COMPONENTES + ']').each(function () {
                                if (undefined != $(this)[0].selectize) {
                                    $(this)[0].selectize.setValue('');
                                } else {
                                    $(this).val('');
                                }
                                if (undefined != auxFormCadastro.data('bootstrapValidator')) {
                                    auxFormCadastro.data('bootstrapValidator').updateStatus($(this).attr('name'), 'NOT_VALIDATED');
                                }
                            });
                            $('#obsTramitacao').val('');
                            listaArquivosUpload = undefined;
                            var dvListaArquivos = $('#dvListaArquivos');
                            if (undefined != dvListaArquivos.attr('id')) {
                                dvListaArquivos.html(textoPadraoNenhumArquivo);
                            }
                        }
                        mostrarMsgSucesso(result.msgSucesso);
                    }
                }
            },
            error: function (request, status, error) {
                fecharPopup();
                mostrarMsgErro('Falha na conex&atilde;o, favor tente novamente');
            }
        });
    }
}

function excluirRegistro(auxData) {
    fecharMsgSucesso();
    fecharMsgErro();

    $('#dialog-confirm').dialog({
        resizable: false,
        height: 'auto',
        width: 400,
        modal: true,
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
                text: 'Sim',
                'class': 'btn btn-sm btn-secunday',
                click: function () {
                    $(this).dialog("close");

                    mostrarPopup();

                    $.ajax({
                        url: metodoDelete,
                        type: 'POST',
                        data: auxData,
                        success: function (result) {
                            fecharPopup();

                            var msgErro = result.msgErro;
                            if (undefined != msgErro && '' != msgErro.toString().trim()) {
                                mostrarMsgErro(msgErro);
                            } else {
                                guardarDadosFiltro();
                                tabelaPrincipal.ajax.reload(function (json) {
                                    if (undefined != json && undefined != json.voltarPagina) {
                                        voltarPaginaTabelaPrincipal();
                                    }
                                }, false);
                                mostrarMsgSucesso(result.msgSucesso);
                            }
                        },
                        error: function (request, status, error) {
                            fecharPopup();
                            mostrarMsgErro('Falha na conex&atilde;o, favor tente novamente');
                        }
                    });
                }
            },
            {
                text: 'N\u00e3o',
                'class': 'btn btn-sm btn-secunday',
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
    });
}
//TabelaPrincipal

//ResponsividadeTabelaPrincipal
function mostrarAcoes(idRegistroTabela) {
    $('#menuAcoes_' + idRegistroTabela).css('display', '');
    $('#menuAcoes_' + idRegistroTabela).attr('tabindex', '0');
    $('#menuAcoes_' + idRegistroTabela).animate({ width: '11.5em' }, 250);
    $('#menuAcoes_' + idRegistroTabela).focus();
}

function ocultarAcoes(menuAcoes) {
    menuAcoes.css('width', '1em');
    menuAcoes.css('display', 'none');
}
//ResponsividadeTabelaPrincipal

function ajustarPeriodoFiltro(txtDataAlterada, txtDataInicialFiltro, txtDataFinalFiltro, tabelaPrincipal, diferenciaDiasFiltro) {
    var periodoValido = false;
    if (undefined != txtDataAlterada.data('fezCargaInicial') &&
            undefined != tabelaPrincipal &&
            !isNullOrEmpty(txtDataInicialFiltro.val()) &&
            !isNullOrEmpty(txtDataFinalFiltro.val())) {
        var auxDataInicialFiltro = txtDataInicialFiltro.val().split('/');
        var auxDataFinalFiltro = txtDataFinalFiltro.val().split('/');
        if (undefined != auxDataInicialFiltro &&
            3 == auxDataInicialFiltro.length &&
            undefined != auxDataFinalFiltro &&
            3 == auxDataFinalFiltro.length) {
            var dataInicialFiltro = new Date(auxDataInicialFiltro[2], auxDataInicialFiltro[1] - 1, auxDataInicialFiltro[0], 0, 0, 0, 0);
            var dataFinalFiltro = new Date(auxDataFinalFiltro[2], auxDataFinalFiltro[1] - 1, auxDataFinalFiltro[0], 0, 0, 0, 0);
            dataInicialFiltro = dataInicialFiltro.getTime();
            dataFinalFiltro = dataFinalFiltro.getTime();
            var diferenca = parseInt((dataFinalFiltro - dataInicialFiltro) / (24 * 3600 * 1000));
            if (diferenca < 0) {
                mostrarMsgErro('Favor informar a data final maior ou igual a data inicial');
            } else {
                if (diferenca >= 0 && diferenca <= diferenciaDiasFiltro) {
                    fecharMsgErro();
                    periodoValido = true;
                } else {
                    mostrarMsgErro('Favor informar um intervalo de at&eacute; ' + diferenciaDiasFiltro + ' dias');
                }
            }
        }
    }
    return periodoValido;
}

function adicionarObsTramitacao(idObsTramitacao, dvComponentesDinamicos) {
    var dvFormGroup = $('<div class="form-group col-lg-12 dvFormGroupDinamico"></div>');
    var labelNomeComp = $('<label>Observa&ccedil;&atilde;o:</label>');
    var textArea = $('<textarea id="' + idObsTramitacao + '" class="form-control" style="height: 80px; resize: none;" placeholder="Informe a observa&ccedil;&atilde;o" maxlength="500"/>');

    dvFormGroup.append(labelNomeComp);
    dvFormGroup.append(textArea);
    dvComponentesDinamicos.append(dvFormGroup);
}

function adicionarArquivoListaArquivosUpload(idArquivo, nomeArquivo, bytesArquivo) {
    if (undefined != listaArquivosUpload) {
        listaArquivosUpload.find('#' + idArquivo).data('objArquivo', '{"nomeArquivo":"' + nomeArquivo + '","bytesArquivo":"' + bytesArquivo + '"}');
        var totArquivosCarregados = listaArquivosUpload.data('totArquivosCarregados');
        if (undefined == totArquivosCarregados) {
            totArquivosCarregados = 1;
        } else {
            totArquivosCarregados++;
        }
        listaArquivosUpload.data('totArquivosCarregados', totArquivosCarregados);
    }
}

function excluirArquivoListaArquivosUpload(idArquivo) {
    if (undefined != listaArquivosUpload) {
        var atualizarTotArquivosCarregados = false;
        if (undefined != listaArquivosUpload.find('#' + idArquivo).data('objArquivo')) {
            atualizarTotArquivosCarregados = true;
        }
        listaArquivosUpload.find('#' + idArquivo).remove();
        if (atualizarTotArquivosCarregados) {
            var totArquivosCarregados = listaArquivosUpload.data('totArquivosCarregados');
            if (undefined != totArquivosCarregados) {
                totArquivosCarregados--;
            }
            listaArquivosUpload.data('totArquivosCarregados', totArquivosCarregados);
        }
        var totArquivos = listaArquivosUpload.data('totArquivos');
        if (undefined != totArquivos) {
            totArquivos--;
        }
        listaArquivosUpload.data('totArquivos', totArquivos);
    }
}

function adicionarBotaoCargaArquivo(idArquivo, arquivo) {
    var btnCarregar = $('' +
    '<button class="btn btn-sm btn-success" style="margin-right: 10px;" type="button" data-toggle="tooltip" data-original-title="Carregar">' +
		'<i class="fa fa-upload"></i>' +
	'</button>');
    btnCarregar.click(function () {
        $(this).removeAttr('class');
        $(this).removeAttr('type');
        $(this).removeAttr('data-toggle');
        $(this).removeAttr('data-original-title');
        $(this).attr('style', 'margin-right: 10px !important; cursor: context-menu !important; border: 0px !important; background-color: transparent !important; outline:0 !important;');
        $(this).html('<img style="width:30px; height:30px;" src="/Resources/loader.gif"/>');
        var guardaBtnCarregar = $(this);
        var reader = new FileReader();
        reader.onload = function (e) {
            adicionarArquivoListaArquivosUpload(idArquivo, arquivo.name, e.target.result);
            $('.tooltip').remove();
            guardaBtnCarregar.remove();
        };
        reader.readAsDataURL(arquivo);
    });
    return btnCarregar;
}

function adicionarBotaoExclusaoArquivo(tabelaArquivos, idArquivo, linhaArquivo, textoPadraoNenhumArquivo) {
    var btnExcluir = $('' +
    '<button class="btn btn-sm btn-danger" type="button" data-toggle="tooltip" data-original-title="Excluir">' +
        '<i class="glyphicon glyphicon-trash"></i>' +
    '</button>');
    btnExcluir.click(function () {
        excluirArquivoListaArquivosUpload(idArquivo);
        if (undefined != tabelaArquivos.data('DataTable')) {
            linhaArquivo.addClass('selected');
            tabelaArquivos.data('DataTable').row('.selected').remove().draw(false);
            if (tabelaArquivos.data('DataTable').rows().data().length == 0) {
                $('#dvListaArquivos').html(textoPadraoNenhumArquivo);
            }
        }
    });
    return btnExcluir;
}

function montarLinkArquivo(dvListaArquivos, arquivo) {
    if (undefined != dvListaArquivos && undefined != arquivo) {
        var linkArquivo = $('<a style="cursor: pointer;">' + arquivo.NOME + '</a>');
        linkArquivo.click(function () {
            var idForm = 'frmDownloadArquivo_' + arquivo.ID;
            $('#' + idForm).remove();
            var formDownloadArquivo = $('<form id="' + idForm + '" action="../ArquivoTramitacao/Visualizar" target="_blank" method="POST"></form>');
            var idArquivo = $('<input name="idArquivo" type="hidden" value="' + arquivo.ID + '">');
            var nomeArquivo = $('<input name="nomeArquivo" type="hidden" value="' + arquivo.NOME + '">');
            formDownloadArquivo.append(idArquivo);
            formDownloadArquivo.append(nomeArquivo);
            $('body').append(formDownloadArquivo);
            formDownloadArquivo.submit();
            $('#' + idForm).remove();
        });
        dvListaArquivos.append(linkArquivo);
    }
}

function adicionarDivListaArquivos(arquivos) {
    var dvListaArquivos = $('<div></div>');
    if (undefined != arquivos && arquivos.length > 0) {
        montarLinkArquivo(dvListaArquivos, arquivos[0]);
        for (var i = 1; i < arquivos.length; i++) {
            dvListaArquivos.append('<br/>');
            montarLinkArquivo(dvListaArquivos, arquivos[i]);
        }
    }
    return dvListaArquivos;
}

function obterArquivosUpload() {
    var result = '[';
    if (undefined != listaArquivosUpload) {
        var objArquivo;
        listaArquivosUpload.find('div').each(function () {
            objArquivo = $(this).data('objArquivo');
            if (undefined != objArquivo) {
                if ('[' == result) {
                    result += objArquivo;
                } else {
                    result += ',' + objArquivo;
                }
            }
        });
    }
    return result + ']';
}

function arquivosCarregados() {
    var result = true;
    if (undefined != listaArquivosUpload) {
        var totArquivos = listaArquivosUpload.data('totArquivos');
        var totArquivosCarregados = listaArquivosUpload.data('totArquivosCarregados');
        return totArquivos == totArquivosCarregados;
    }
    return result;
}

function formularioSemArquivos() {
    var result = true;
    if (undefined != listaArquivosUpload
        && undefined != listaArquivosUpload.data('totArquivosCarregados')
        && 0 != listaArquivosUpload.data('totArquivosCarregados')) {
        result = false;
    }
    return result;
}

function adicionarHeaderTabelaArquivos(tabelaArquivos) {
    var headerTabelaArquivos = $('<thead></thead>');
    var linhaArquivo = $('<tr></tr>');
    linhaArquivo.append('<th>Arquivo</th>');
    linhaArquivo.append('<th>Tamanho</th>');
    linhaArquivo.append('<th style="text-align:center;">A&ccedil;&otilde;es</th>');
    headerTabelaArquivos.append(linhaArquivo);
    tabelaArquivos.append(headerTabelaArquivos);
}

function adicionarHeaderTabelaHistorico(tabelaHistorico) {
    var headerTabelaHistorico = $('<thead></thead>');
    var linhaHistorico = $('<tr></tr>');
    linhaHistorico.append('<th>Status</th>');
    linhaHistorico.append('<th>Data/Hora</th>');
    linhaHistorico.append('<th>Respons&aacute;vel</th>');
    linhaHistorico.append('<th>Observa&ccedil;&atilde;o</th>');
    linhaHistorico.append('<th>Altera&ccedil;&otilde;es</th>');
    linhaHistorico.append('<th style="text-align:center;">Arquivos</th>');
    headerTabelaHistorico.append(linhaHistorico);
    tabelaHistorico.append(headerTabelaHistorico);
}

function adicionarLinhaArquivo(tabelaArquivos, idArquivo, arquivo, textoPadraoNenhumArquivo, desenharDataTable) {
    var linhaArquivo = $('<tr></tr>');
    linhaArquivo.append('<td>' + arquivo.name + '</td>');
    linhaArquivo.append('<td>' + parseFloat((parseFloat(arquivo.size) * 0.001)).toFixed(2).toString().replace('.', ',') + ' KB</td>');

    var tdAcoesArquivo = $('<td style="text-align:center;"></td>');

    var btnCarregar = adicionarBotaoCargaArquivo(idArquivo, arquivo);
    tdAcoesArquivo.append(btnCarregar);
    btnCarregar.click();

    var btnExcluir = adicionarBotaoExclusaoArquivo(tabelaArquivos, idArquivo, linhaArquivo, textoPadraoNenhumArquivo);
    tdAcoesArquivo.append(btnExcluir);

    linhaArquivo.append(tdAcoesArquivo);

    if (desenharDataTable) {
        tabelaArquivos.append(linhaArquivo);
    } else {
        tabelaArquivos.data('DataTable').row.add(linhaArquivo).draw().node();
    }

    var dvIdArquivo = $('<div id="' + idArquivo + '"></div>');
    listaArquivosUpload.append(dvIdArquivo);
}

function adicionarLinhaHistorico(tabelaHistorico, colunasLinha, arquivos) {
    var linhaHistorico = $('<tr></tr>');
    for (var i = 0; i < colunasLinha.length; i++) {
        linhaHistorico.append('<td>' + colunasLinha[i] + '</td>');
    }

    var tdAcoesHistorico = $('<td style="text-align:center;"></td>');

    var dvListaArquivos = adicionarDivListaArquivos(arquivos);
    tdAcoesHistorico.append(dvListaArquivos);

    linhaHistorico.append(tdAcoesHistorico);

    tabelaHistorico.append(linhaHistorico);
}

function inverterString(stringOriginal) {
    if (!isNullOrEmpty(stringOriginal)) {
        return stringOriginal.split('').reverse().join('');
    }
    return stringOriginal;
}

function obterParametrosParaUpload(idPreenchimentoFormulario) {
    LIMITE_ARQUIVOS_FORMULARIO = undefined
    LIMITE_TAMANHO_ARQUIVO_UPLOAD = undefined;
    TIPOS_ARQUIVO_FORMULARIO = undefined;
    LIMITE_NOME_ARQUIVO_TRAMITACAO = undefined;
    $.ajax({
        url: 'Parametro/GetParametrosParaUpload',
        type: 'POST',
        data: { ID_PREENCHIMENTO_FORMULARIO: idPreenchimentoFormulario },
        success: function (result) {
            LIMITE_ARQUIVOS_FORMULARIO = result.limiteArquivosFormulario;
            LIMITE_TAMANHO_ARQUIVO_UPLOAD = result.limiteTamanhoArquivoUpload;
            TIPOS_ARQUIVO_FORMULARIO = result.tiposArquivoFormulario;
            LIMITE_NOME_ARQUIVO_TRAMITACAO = result.limiteNomeArquivoUpload;
        },
        error: function (request, status, error) {
            console.log('Falha ao tentar obter as configura&ccedil;&otilde;es para o upload, favor tente novamente');
        }
    });
}

function parametrosValidosParaUpload() {
    var msgNenhumLimiteUpload = 'Nenhum limite de upload foi configurado, ou o valor est&aacute; incorreto';
    var msgNenhumLimiteTamanho = 'Nenhum limite de tamanho foi configurado, ou o valor est&aacute; incorreto';
    var msgNenhumLimiteTipo = 'Nenhum tipo de extens&atilde;o foi configurado';
    var msgNenhumLimiteNome = 'Nenhum limite de caracteres para o nome do(s) anexo(s) foi configurado';

    try {
        var valor = parseInt(LIMITE_ARQUIVOS_FORMULARIO);
        if (isNullOrEmpty(valor) || isNaN(valor) || valor < 0) {
            mostrarMsgErro(msgNenhumLimiteUpload);
            return false;
        }
    } catch (ex) {
        mostrarMsgErro(msgNenhumLimiteUpload);
        return false;
    }

    if (LIMITE_ARQUIVOS_FORMULARIO == 0) {
        mostrarMsgErro(msgPadraoLimiteUploadsAtingido);
        return false;
    }

    try {
        var valor = parseInt(LIMITE_TAMANHO_ARQUIVO_UPLOAD);
        if (isNullOrEmpty(valor) || isNaN(valor) || valor <= 0) {
            mostrarMsgErro(msgNenhumLimiteTamanho);
            return false;
        }
    } catch (ex) {
        mostrarMsgErro(msgNenhumLimiteTamanho);
        return false;
    }

    try {
        if (isNullOrEmpty(TIPOS_ARQUIVO_FORMULARIO)) {
            mostrarMsgErro(msgNenhumLimiteTipo);
            return false;
        }
        var valor = TIPOS_ARQUIVO_FORMULARIO.toString().split(';').length;
        if (isNullOrEmpty(valor) || isNaN(valor) || valor <= 0) {
            mostrarMsgErro(msgNenhumLimiteTipo);
            return false;
        }
    } catch (ex) {
        mostrarMsgErro(msgNenhumLimiteTipo);
        return false;
    }

    try {
        if (isNullOrEmpty(LIMITE_NOME_ARQUIVO_TRAMITACAO)) {
            mostrarMsgErro(msgNenhumLimiteNome);
            return false;
        }
        var valor = parseInt(LIMITE_NOME_ARQUIVO_TRAMITACAO.toString());
        if (valor <= 0) {
            mostrarMsgErro(msgNenhumLimiteNome);
            return false;
        }
    } catch (ex) {
        mostrarMsgErro(msgNenhumLimiteNome);
        return false;
    }

    return true;
}

function formatarMsgErroArquivoNomeCaractereInvalido(nomeArquivo) {
    return 'O nome do arquivo <b>' + nomeArquivo + '</b> n&atilde;o pode conter o(s) seguinte(s) caractere(s): <b>v&iacute;rgula (,)</b>';
}

function formatarMsgErroArquivoNomeInvalido(nomeArquivo, limite) {
    return 'O nome do arquivo <b>' + nomeArquivo + '</b> juntamente com a sua extens&atilde;o n&atilde;o pode ser maior do que <b>' + limite + '</b> caracteres';
}

function formatarMsgErroArquivoExtensaoInvalida(nomeArquivo, extensoes) {
    var msgPadraoExtensao = 'A extens&atilde;o do arquivo <b>' + nomeArquivo + '</b> n&atilde;o &eacute; permitida, favor informar arquivos somente com a extens&atilde;o ';
    var auxExtensoes = extensoes.split(';');
    if (auxExtensoes.length == 1) {
        return msgPadraoExtensao + auxExtensoes[0];
    }
    var extensoesConcatenadas = extensoes.replace(new RegExp(';', 'g'), ', ');
    extensoesConcatenadas = inverterString(extensoesConcatenadas);
    extensoesConcatenadas = extensoesConcatenadas.replace(' ,', ' uo ');
    extensoesConcatenadas = inverterString(extensoesConcatenadas);
    return msgPadraoExtensao + extensoesConcatenadas;
}

function formatarMsgErroArquivoForaLimite(nomeArquivo, limite) {
    var limiteMB = 1000000;
    var limiteKB = 1000;
    if (limite >= limiteMB) {
        return 'O tamanho do arquivo <b>' + nomeArquivo + '</b> ultrapassou o limite de ' + (limite / limiteMB) + ' MB';
    }
    if (limite >= limiteKB) {
        return 'O tamanho do arquivo <b>' + nomeArquivo + '</b> ultrapassou o limite de ' + (limite / limiteKB) + ' KB';
    }
    if (limite > 1) {
        return 'O tamanho do arquivo <b>' + nomeArquivo + '</b> ultrapassou o limite de ' + limite + ' bytes';
    }
    return 'O tamanho do arquivo <b>' + nomeArquivo + '</b> ultrapassou o limite de ' + limite + ' byte';
}

function adicionarBotaoUploadArquivos(dvComponentesDinamicos) {
    var dvFormGroup = $('<div class="form-group col-lg-12 dvFormGroupDinamico"></div>');
    var dvInputGroup = $('<div class="input-group"></div>');
    var dvBtn = $('<div class="btn btn-primary btn-file"><i class="glyphicon glyphicon-paperclip"></i>&nbsp;&nbsp;Anexar Arquivos</div>');
    var inputFile = $('<input type="file" multiple="multiple" />');

    var dvBoxArquivos = $('<div class="box box-default" style="margin: 0px; margin-top: 15px;"></div>');
    var dvBoxBodyArquivos = $('<div class="box-body"></div>');
    var dvListaArquivos = $('<div id="dvListaArquivos">' + textoPadraoNenhumArquivo + '</div>');
    inputFile.change(function () {
        fecharMsgErro();
        if (undefined == listaArquivosUpload) {
            listaArquivosUpload = $('<div></div>');
        }
        var arquivos = $(this).prop('files');
        if (undefined != arquivos && arquivos.length > 0 && parametrosValidosParaUpload()) {
            var totArquivos = listaArquivosUpload.data('totArquivos');
            if (undefined == totArquivos) {
                totArquivos = 0;
            }
            var totArquivosPodeAnexar = LIMITE_ARQUIVOS_FORMULARIO - totArquivos;
            if (totArquivosPodeAnexar <= 0) {
                mostrarMsgErro(msgPadraoLimiteUploadsAtingido);
            } else {
                if (arquivos.length > totArquivosPodeAnexar) {
                    if (totArquivosPodeAnexar > 1) {
                        mostrarMsgErro('Favor escolher at&eacute; ' + totArquivosPodeAnexar + ' arquivos');
                    } else {
                        mostrarMsgErro('Favor escolher somente 1 arquivo');
                    }
                } else {
                    var tabelaArquivos = $('#tabelaArquivos');
                    var desenharDataTable = undefined == tabelaArquivos.data('DataTable');
                    if (desenharDataTable) {
                        tabelaArquivos = $('<table id="tabelaArquivos" class="table table-striped table-bordered table-hover dataTable no-footer"></table>');
                        adicionarHeaderTabelaArquivos(tabelaArquivos);
                    }

                    var numeroEscolha = $(this).data('numeroEscolha');
                    if (undefined == numeroEscolha) {
                        numeroEscolha = 0;
                    } else {
                        numeroEscolha++;
                    }
                    $(this).data('numeroEscolha', numeroEscolha);

                    var idArquivo;
                    var auxExtensaoArquivo;
                    var extensaoArquivo;
                    var msgErroUpload = undefined;
                    var totArquivosAdicionados = 0;
                    for (var i = 0; i < arquivos.length; i++) {
                        if (arquivos[i].name.toString().indexOf(',') >= 0) {
                            if (undefined == msgErroUpload) {
                                msgErroUpload = formatarMsgErroArquivoNomeCaractereInvalido(arquivos[i].name);
                            } else {
                                msgErroUpload += '<br/>' + formatarMsgErroArquivoNomeCaractereInvalido(arquivos[i].name);
                            }
                            continue;
                        }
                        if (isNullOrEmpty(LIMITE_NOME_ARQUIVO_TRAMITACAO) || parseInt(arquivos[i].name.toString().length) > parseInt(LIMITE_NOME_ARQUIVO_TRAMITACAO)) {
                            if (undefined == msgErroUpload) {
                                msgErroUpload = formatarMsgErroArquivoNomeInvalido(arquivos[i].name, LIMITE_NOME_ARQUIVO_TRAMITACAO);
                            } else {
                                msgErroUpload += '<br/>' + formatarMsgErroArquivoNomeInvalido(arquivos[i].name, LIMITE_NOME_ARQUIVO_TRAMITACAO);
                            }
                            continue;
                        }
                        auxExtensaoArquivo = arquivos[i].name.split('.');
                        extensaoArquivo = auxExtensaoArquivo[auxExtensaoArquivo.length - 1].toUpperCase();
                        if (isNullOrEmpty(TIPOS_ARQUIVO_FORMULARIO) || TIPOS_ARQUIVO_FORMULARIO.indexOf(extensaoArquivo) < 0) {
                            if (undefined == msgErroUpload) {
                                msgErroUpload = formatarMsgErroArquivoExtensaoInvalida(arquivos[i].name, TIPOS_ARQUIVO_FORMULARIO);
                            } else {
                                msgErroUpload += '<br/>' + formatarMsgErroArquivoExtensaoInvalida(arquivos[i].name, TIPOS_ARQUIVO_FORMULARIO);
                            }
                            continue;
                        }
                        if (arquivos[i].size > LIMITE_TAMANHO_ARQUIVO_UPLOAD) {
                            if (undefined == msgErroUpload) {
                                msgErroUpload = formatarMsgErroArquivoForaLimite(arquivos[i].name, LIMITE_TAMANHO_ARQUIVO_UPLOAD);
                            } else {
                                msgErroUpload += '<br/>' + formatarMsgErroArquivoForaLimite(arquivos[i].name, LIMITE_TAMANHO_ARQUIVO_UPLOAD);
                            }
                            continue;
                        }
                        idArquivo = 'arquivo_' + numeroEscolha + '_' + i;
                        adicionarLinhaArquivo(tabelaArquivos, idArquivo, arquivos[i], textoPadraoNenhumArquivo, desenharDataTable);
                        totArquivosAdicionados++;
                    }
                    totArquivos += totArquivosAdicionados;
                    listaArquivosUpload.data('totArquivos', totArquivos);

                    if (!isNullOrEmpty(msgErroUpload)) {
                        mostrarMsgErro(msgErroUpload);
                    }

                    if (desenharDataTable && totArquivosAdicionados > 0) {
                        dvListaArquivos.html('');
                        dvListaArquivos.append(tabelaArquivos);
                        var auxDataTable = tabelaArquivos.DataTable({
                            responsive: true,
                            scrollX: true,
                            aaSorting: []
                        });
                        tabelaArquivos.data('DataTable', auxDataTable);
                    }
                }
            }
        }
        $(this).val('');
    });
    dvBtn.append(inputFile);
    dvInputGroup.append(dvBtn);
    dvFormGroup.append(dvInputGroup);
    dvBoxArquivos.append(dvBoxBodyArquivos);
    dvBoxBodyArquivos.append(dvListaArquivos);
    dvFormGroup.append(dvBoxArquivos);
    dvComponentesDinamicos.append(dvFormGroup);
}

function montarTabelaHistoricoTramitacoes(dvFormGroup, dados) {
    var dvBoxHistorico = $('<div id="dvBoxHistorico" class="box box-default" style="margin: 0px; margin-top: 15px;"></div>');
    var dvBoxBodyHistorico = $('<div class="box-body"></div>');

    var tabelaHistorico = $('<table id="tabelaHistorico" class="table table-striped table-bordered table-hover dataTable no-footer"></table>');
    adicionarHeaderTabelaHistorico(tabelaHistorico);

    if (dados.length > 0) {
        var colunasLinha;
        for (var i = 0; i < dados.length; i++) {
            colunasLinha = [
                dados[i].statusDestino.NOME,
                dados[i].DATA_HORA,
                dados[i].NOME_RESPONSAVEL + ' (' + dados[i].USER_NAME_RESPONSAVEL + ')',
                dados[i].OBSERVACAO,
                dados[i].LOG_ALTERACAO_COMPONENTES
            ];
            adicionarLinhaHistorico(tabelaHistorico, colunasLinha, dados[i].arquivos);
        }
    }

    dvBoxBodyHistorico.append(tabelaHistorico);
    dvBoxHistorico.append(dvBoxBodyHistorico);
    dvFormGroup.append(dvBoxHistorico);

    tabelaHistorico.DataTable({
        responsive: true,
        scrollX: true,
        aaSorting: []
    });
}

function adicionarHistoricoTramitacoes(auxDvFormCadastro, auxDvTabelaPrincipal, dvComponentesDinamicos, auxUrl, auxData) {
    var dvFormGroup = $('<div class="form-group col-lg-12 dvFormGroupDinamico"></div>');
    var dvInputGroup = $('<div class="input-group"></div>');
    var htmlVerHistorico = '<i class="fa fa-history"></i>&nbsp;&nbsp;Ver Hist&oacute;rico';
    var htmlOcultarHistorico = '<i class="fa fa-history"></i>&nbsp;&nbsp;Ocultar Hist&oacute;rico';
    var dvBtn = $('<div class="btn btn-primary btn-file"></div>');
    dvBtn.append(htmlVerHistorico);
    dvBtn.data('verHistorico', 'S');
    dvBtn.click(function () {
        if ('S' == $(this).data('verHistorico')) {
            var dados = dvBtn.data('dadosHistorico');
            if (undefined != dados) {
                montarTabelaHistoricoTramitacoes(dvFormGroup, dados);
                dvBtn.html(htmlOcultarHistorico);
                dvBtn.data('verHistorico', 'N');
            } else {
                mostrarPopup();
                $.ajax({
                    url: auxUrl,
                    type: 'POST',
                    data: auxData,
                    success: function (result) {
                        dados = result.data;
                        montarTabelaHistoricoTramitacoes(dvFormGroup, dados);
                        dvBtn.data('dadosHistorico', dados);
                        dvBtn.html(htmlOcultarHistorico);
                        dvBtn.data('verHistorico', 'N');
                        fecharPopup();
                    },
                    error: function (request, status, error) {
                        fecharPopup();
                        mostrarMsgErro('Falha ao tentar obter o hist&oacute;rico, favor tente novamente');
                    }
                });
            }
        } else {
            $('#dvBoxHistorico').remove();
            dvBtn.html(htmlVerHistorico);
            dvBtn.data('verHistorico', 'S');
        }
    });
    dvInputGroup.append(dvBtn);
    dvFormGroup.append(dvInputGroup);
    dvComponentesDinamicos.append(dvFormGroup);
    mostrarDvFormCadastro(auxDvFormCadastro, auxDvTabelaPrincipal, undefined);
}

function inicializarModalMsg(idModal) {
    var result = $('#' + idModal).dialog({
        resizable: false,
        height: 'auto',
        width: 400,
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
                text: 'Ok',
                'class': 'btn btn-sm btn-secunday',
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
    });
    return result;
}

function valorComponenteAlterado(valorAnterior, novoValor) {
    return valorAnterior != novoValor;
}

function montarListaComponentes() {
    var msgComponentesAlterados = '';
    var listaComponentes = [];
    var indiceListaComponentes = 0;
    var idItemLista;
    var valor;
    var idCompForm;
    var nomeComponente;
    var textoAnterior;
    var msgComponenteAlterado;
    $('[name*=' + NAME_PADRAO_COMPONENTES + ']').each(function () {
        idCompForm = $(this).data('idCompForm');
        idItemLista = $(this).data('idItemLista');
        if (undefined == idItemLista) {
            valor = $(this).val();
        } else {
            valor = $(this).text();
        }
        nomeComponente = $(this).data('nomeComponente');
        textoAnterior = $(this).data('textoAnterior');
        if (valorComponenteAlterado(textoAnterior, valor)) {
            if (isNullOrEmpty(textoAnterior)) {
                msgComponenteAlterado = 'Foi informado o valor "' + valor + '" para o campo "' + nomeComponente + '"';
            } else {
                msgComponenteAlterado = 'O valor do campo "' + nomeComponente + '" foi alterado de "' + textoAnterior + '" para "' + valor + '"';
            }
            if (isNullOrEmpty(msgComponentesAlterados)) {
                msgComponentesAlterados = msgComponenteAlterado;
            } else {
                msgComponentesAlterados += '. ' + msgComponenteAlterado;
            }
        }
        listaComponentes[indiceListaComponentes++] = {
            ID: idCompForm,
            VALOR_VARCHAR: valor,
            componente: {
                ID: $(this).data('idComponente')
            },
            itemLista: {
                ID: idItemLista
            }
        };
    });
    return {
        mensagemAlteracao: msgComponentesAlterados,
        componentesFormulario: JSON.stringify(listaComponentes)
    };
}

function exportarArquivo(auxUrlVerificacao, auxUrlExportacao, nomeTabela, dadosFiltro) {
    mostrarPopup();
    $.ajax({
        url: auxUrlVerificacao,
        type: 'POST',
        data: undefined != dadosFiltro ? dadosFiltro : '',
        success: function (result) {
            fecharPopup();
            if (!isNullOrEmpty(result.msgErro)) {
                mostrarMsgErro(result.msgErro);
            } else {
                try {
                    var formDownloadArquivo = $('<form action="' + auxUrlExportacao + '" method="POST"></form>');

                    var dados = $('<input name="dados" type="hidden">');
                    if (undefined != dadosFiltro) {
                        var auxLinhas = result.dados;
                        var auxLinha;
                        var linhas = [];
                        var nomeColuna;
                        var valorColuna;
                        var jsonLinha;
                        for (var i = 0; i < auxLinhas.length; i++) {
                            auxLinha = auxLinhas[i];
                            jsonLinha = '{';
                            if (auxLinha.length > 0) {
                                nomeColuna = auxLinha[0].Key;
                                valorColuna = auxLinha[0].Value;
                                jsonLinha += '"' + nomeColuna + '":"' + valorColuna + '"';
                                for (var j = 1; j < auxLinha.length; j++) {
                                    nomeColuna = auxLinha[j].Key;
                                    valorColuna = auxLinha[j].Value;
                                    jsonLinha += ',"' + nomeColuna + '":"' + valorColuna + '"';
                                }
                            }
                            jsonLinha += '}';
                            linhas[i] = JSON.parse(jsonLinha);
                        }
                        dados.val(JSON.stringify(linhas));
                    } else {
                        dados.val(JSON.stringify(result.dados));
                    }
                    formDownloadArquivo.append(dados);

                    var auxNomeTabela = $('<input name="nomeTabela" value="' + nomeTabela + '" type="hidden">');
                    formDownloadArquivo.append(auxNomeTabela);

                    $('body').append(formDownloadArquivo);

                    formDownloadArquivo.submit();
                    formDownloadArquivo.remove();
                } catch (ex) {
                    mostrarMsgErro('Falha na conex&atilde;o, favor entrar em contato com o suporte');
                }
            }
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha na conex&atilde;o, favor tente novamente');
        }
    });
}

$(document).ready(function () {
    divLoader = $('#divLoader').dialog({
        resizable: false,
        modal: true,
        autoOpen: false,
        height: 95,
        width: 95,
        open: function (event, ui) {
            $(this).parent().find('.ui-dialog-titlebar-close').hide();
            $(this).parent().find('.ui-dialog-titlebar').css('display', 'none');
        }
    });

    dvMsgSucesso = inicializarModalMsg('dvMsgSucesso');
    dvMsgErro = inicializarModalMsg('dvMsgErro');

    ativarMenuEscolhido();

    $('#inputPesquisaMenu').keydown(function () {
        $('#imgCarregandoPesquisaMenu').css('display', '');
        $('#linkPesquisarMenu').css('display', 'none');
    });

    $('#inputPesquisaMenu').keyup(function () {
        var textoPesquisa = removerAcentos($(this).val());
        if ('' == textoPesquisa.trim()) {
            $('#menuPrincipal').find('.active').each(function () {
                $(this).removeClass('active');
            });
            $('#menuPrincipal').find('a').each(function () {
                $(this).parent().css('display', '');
            });
        } else {
            $('#menuPrincipal').find('a').each(function () {
                $(this).parent().css('display', 'none');
            });

            var textoSpan;
            var itemMenu;
            //$('#menuPrincipal').find('a').each(function () {
            $('#menuPrincipal').find('a:not([href=#])').each(function () {
                textoSpan = removerAcentos($(this).find('span').html().toString());
                if (textoSpan.indexOf(textoPesquisa) >= 0) {
                    itemMenu = $(this).parent();
                    while (itemMenu != null && 'menuPrincipal' != itemMenu.attr('id')) {
                        itemMenu.addClass('active');
                        itemMenu.css('display', '');
                        itemMenu = itemMenu.parent();
                    }
                }
            });
        }
        $('#imgCarregandoPesquisaMenu').css('display', 'none');
        $('#linkPesquisarMenu').css('display', '');
    });

    $('input').each(function () {
        var possuiRegexValidator = $(this).attr('data-bv-regexp-regexp');
        if (undefined == possuiRegexValidator) {
            $(this).attr('data-bv-regexp', 'true');
            $(this).attr('data-bv-regexp-regexp', '^[^<>\';"]*$');
            $(this).attr('data-bv-regexp-message', 'Favor n&atilde;o utilizar nenhum destes 6 caracteres ^<>";\'');
        }
    });

    /*$('.form-control').focus(function () {
	    $(this).attr('style', 'border-color: rgb(116, 179, 229) !important; box-shadow: rgba(0, 0, 0, 0.0745098) 0px 1px 1px 0px inset, rgba(102, 173, 231, 0.517647) 0px 0px 6.88427782058716px 0px; !important');
	});

	$('.form-control').focusout(function () {
	    $(this).attr('style', 'border-color: rgb(204, 204, 204) !important;');
	});*/

    $('.dropdown-menu').css('border', '0px');
    $('.dropdown-toggle').click(function () {
        $('li.dropdown').removeAttr('style');
    });

    $('ul.dropdown-menu [data-toggle=dropdown]').on('click', function (event) {
        event.preventDefault();
        event.stopPropagation();

        $(this).removeAttr('style');
        $(this).parent().css('background-color', '#337ab7');

        $(this).mouseover(function () {
            var ariaExpanded = $(this).attr('aria-expanded');
            if (false == ariaExpanded
            || 'false' == ariaExpanded) {
                $(this).css('color', '#333');
                $(this).css('background-color', '#e1e3e9');
            }
        });

        $(this).mouseout(function () {
            var ariaExpanded = $(this).attr('aria-expanded');
            if (false == ariaExpanded
            || 'false' == ariaExpanded) {
                $(this).css('color', '#777');
                $(this).css('background-color', '#fff');
            }
        });

        var ariaExpanded = $(this).attr('aria-expanded');
        if (false == ariaExpanded
            || 'false' == ariaExpanded) {
            $(this).parent().addClass('open');
            ariaExpanded = true;
        } else {
            $(this).parent().removeClass('open');
            ariaExpanded = false;
        }
        $(this).attr('aria-expanded', ariaExpanded);
    });
});