var TIPO_TEXT_BOX = 1;
var TIPO_DROP_DOWN_LIST = 2;
var TIPO_TEXT_AREA = 3;
var COMPONENTE_OBRIGATORIO = 1;
var NAME_PADRAO_COMPONENTES = 'comp';
var CLASS_PADRAO_COMPONENTE_LISTA = '_lista_';
var CLASS_PADRAO_COMPONENTE_LISTA_FILHO = '_filho';
var CODIGO_MASCARA_CPF = 1;
var CODIGO_MASCARA_CNPJ = 2;
var CODIGO_MASCARA_CPF_CNPJ = 3;
var CODIGO_MASCARA_DATA = 4;
var CODIGO_MASCARA_MONETARIA = 5;
var CODIGO_MASCARA_INTEIRO = 6;
var TAMANHO_CPF = 14;
var VALOR_MASCARA_CPF = '000.000.000-00';
var VALOR_MASCARA_CNPJ = '00.000.000/0000-00';
var VALOR_MASCARA_DATA = '00/00/0000';
var VALOR_MASCARA_MONETARIA = '#.##0,00';
var VALOR_MASCARA_INTEIRO = '#';
var CODIGO_VALIDADOR_CPF = 1;
var CODIGO_VALIDADOR_CNPJ = 2;
var CODIGO_VALIDADOR_CPF_CNPJ = 3;
var MSG_ERRO_CAMPO_INVALIDO = ' inválido';
var PERMISSAO_COMPONENTE_OCULTO = 1;
var PERMISSAO_COMPONENTE_ESCRITA = 2;
var totComponentesParaCarregar = 0;
var totComponentesCarregados = 0;

function verificarDropDownListPai(itemLista) {
    var idListaPai = undefined;
    if (undefined != itemLista
        && undefined != itemLista.tipoLista
        && undefined != itemLista.tipoLista.listaPai) {
        idListaPai = itemLista.tipoLista.listaPai.ID;
    }
    return idListaPai;
}

function iniciarControleComponentes(totComponentes) {
    totComponentesParaCarregar = totComponentes;
    totComponentesCarregados = 0;
}

function atualizarDropDownListFilho(dropDownFilho, idPai) {
    if (undefined != dropDownFilho
        && undefined != dropDownFilho.data('descricaoComponente')
        && undefined != idPai) {
        if (undefined != dropDownFilho[0]
            && undefined != dropDownFilho[0].selectize) {
            dropDownFilho[0].selectize.destroy();
        }
        dropDownFilho.html('<option value="">' + dropDownFilho.data('descricaoComponente') + '</option>');

        var itensLista = dropDownFilho.data('itensLista');
        if (undefined != itensLista && undefined != itensLista[idPai]) {
            for (var i = 0; i < itensLista[idPai].length; i++) {
                dropDownFilho.append(itensLista[idPai][i]);
            }
        }

        dropDownFilho.selectize();
        dropDownFilho.change(function () {
            var idItemLista = $(this).val();
            $(this).data('idItemLista', idItemLista);

            var idTipoLista = $(this).data('idTipoLista');
            if (undefined != idTipoLista) {
                var classCompListaFilho = NAME_PADRAO_COMPONENTES + CLASS_PADRAO_COMPONENTE_LISTA + idTipoLista + CLASS_PADRAO_COMPONENTE_LISTA_FILHO;
                $('select.' + classCompListaFilho).each(function () {
                    atualizarDropDownListFilho($(this), idItemLista);
                });
            }
        });

        if (undefined != dropDownFilho.data('idItemPreenchimento')) {
            dropDownFilho[0].selectize.setValue(dropDownFilho.data('idItemPreenchimento'));
        } else {
            dropDownFilho[0].selectize.setValue('');
        }
        try {
            $('#formCadastro').data('bootstrapValidator').updateStatus(dropDownFilho.attr('name'), 'NOT_VALIDATED');
        } catch (ex) { }//campo sem valdiação
    }
}

function atualizarDropDownListFilhoSemPai(dropDownFilho) {
    if (undefined != dropDownFilho
        && undefined != dropDownFilho.data('descricaoComponente')
        && undefined != dropDownFilho.data('dadosConsulta')) {
        if (undefined != dropDownFilho[0]
            && undefined != dropDownFilho[0].selectize) {
            dropDownFilho[0].selectize.destroy();
        }
        dropDownFilho.html('<option value="">' + dropDownFilho.data('descricaoComponente') + '</option>');

        var dados = dropDownFilho.data('dadosConsulta');
        for (var i = 0; i < dados.length; i++) {
            auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
            dropDownFilho.append(auxOption);
        }

        dropDownFilho.selectize();
        dropDownFilho.change(function () {
            var idItemLista = $(this).val();
            $(this).data('idItemLista', idItemLista);

            var idTipoLista = $(this).data('idTipoLista');
            if (undefined != idTipoLista) {
                var classCompListaFilho = NAME_PADRAO_COMPONENTES + CLASS_PADRAO_COMPONENTE_LISTA + idTipoLista + CLASS_PADRAO_COMPONENTE_LISTA_FILHO;
                $('select.' + classCompListaFilho).each(function () {
                    atualizarDropDownListFilho($(this), idItemLista);
                });
            }
        });

        if (undefined != dropDownFilho.data('idItemPreenchimento')) {
            dropDownFilho[0].selectize.setValue(dropDownFilho.data('idItemPreenchimento'));
        } else {
            dropDownFilho[0].selectize.setValue('');
        }
        try {
            $('#formCadastro').data('bootstrapValidator').updateStatus(dropDownFilho.attr('name'), 'NOT_VALIDATED');
        } catch (ex) { }//campo sem valdiação
    }
}

function verificarDropDownLists() {
    if (totComponentesCarregados < totComponentesParaCarregar) {
        setTimeout(function () {
            verificarDropDownLists();
        }, 200);
    } else {
        var idListaPai;
        var classCompListaPai;
        var dropDownListPai;
        $('select[name*=' + NAME_PADRAO_COMPONENTES + ']').each(function () {
            idListaPai = $(this).data('idListaPai');
            if (!isNullOrEmpty(idListaPai)) {
                classCompListaPai = NAME_PADRAO_COMPONENTES + CLASS_PADRAO_COMPONENTE_LISTA + idListaPai;
                dropDownListPai = $('select.' + classCompListaPai);
                if (undefined != dropDownListPai
                    && undefined != dropDownListPai[0]
                    && undefined != dropDownListPai[0].selectize) {
                    if (!isNullOrEmpty(dropDownListPai[0].selectize.getValue())) {
                        atualizarDropDownListFilho($(this), dropDownListPai[0].selectize.getValue());
                    }
                } else {
                    atualizarDropDownListFilhoSemPai($(this));
                }
            }
        });
    }
}

function obterDescricao(nomeComponente, descricaoComponente) {
    if (isNullOrEmpty(descricaoComponente)) {
        descricaoComponente = nomeComponente;
    }
    return descricaoComponente;
}

function obterTamanho(tamanhoComponente) {
    var result = '';
    if (!isNullOrEmpty(tamanhoComponente)) {
        result = 'maxlength="' + tamanhoComponente + '"';
    }
    return result;
}

function desenharDropDownList(parametros) {
    mostrarPopup();

    var dvPai = parametros.dvPai;
    var idComponente = parametros.idComponente;
    var nomeComponente = parametros.nomeComponente;
    var descricaoComponente = obterDescricao(nomeComponente, parametros.descricaoComponente);
    var idTipoLista = parametros.idTipoLista;
    var idCompForm = parametros.idCompForm;
    var textoPreenchimento = isNullOrEmpty(parametros.textoPreenchimento) ? '' : parametros.textoPreenchimento;

    var nameComp = NAME_PADRAO_COMPONENTES + idComponente;
    var classCompLista = NAME_PADRAO_COMPONENTES + CLASS_PADRAO_COMPONENTE_LISTA + idTipoLista;

    var dvFormGroup = $('<div class="form-group col-lg-6 dvFormGroupDinamico"></div>');
    var labelNomeComp = $('<label>' + nomeComponente + ':</label>');
    var dropDownList = $('<select name="' + nameComp + '" class="form-control ' + NAME_PADRAO_COMPONENTES + ' ' + classCompLista + '"></select>');
    var auxOption = $('<option value="">' + descricaoComponente + '</option>');

    dropDownList.append(auxOption);

    dvFormGroup.append(labelNomeComp);
    dvFormGroup.append(dropDownList);
    dvPai.append(dvFormGroup);

    dropDownList.data('idComponente', idComponente);
    dropDownList.data('idCompForm', idCompForm);
    dropDownList.data('nomeComponente', nomeComponente);
    dropDownList.data('textoAnterior', textoPreenchimento);

    $.ajax({
        url: 'ItensListas/GetPorTipoLista',
        type: 'POST',
        data: { ID_TIPO_LISTA: idTipoLista },
        success: function (result) {
            fecharPopup();
            var dados = result.data;
            var idListaPai;
            var idPai;
            var indiceItensLista;
            var itensLista = [];

            if (undefined != dados && dados.length > 0) {
                idListaPai = verificarDropDownListPai(dados[0]);
                if (undefined == idListaPai) {
                    for (var i = 0; i < dados.length; i++) {
                        auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                        dropDownList.append(auxOption);
                    }
                } else {
                    for (var i = 0; i < dados.length; i++) {
                        if (!isNullOrEmpty(dados[i].itemPai)) {
                            idPai = dados[i].itemPai.ID;
                            auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                            if (undefined == itensLista[idPai]) {
                                itensLista[idPai] = [];
                                indiceItensLista = 0;
                            } else {
                                indiceItensLista = itensLista[idPai].length;
                            }
                            itensLista[idPai][indiceItensLista] = auxOption;
                        }
                    }
                }
            }

            if (undefined != idListaPai) {
                var classCompListaFilho = NAME_PADRAO_COMPONENTES + CLASS_PADRAO_COMPONENTE_LISTA + idListaPai + CLASS_PADRAO_COMPONENTE_LISTA_FILHO;
                dropDownList.addClass(classCompListaFilho);
            }

            dropDownList.data('idTipoLista', idTipoLista);
            dropDownList.data('descricaoComponente', descricaoComponente);
            dropDownList.data('idListaPai', idListaPai);
            dropDownList.data('itensLista', itensLista);
            dropDownList.data('dadosConsulta', dados);

            dropDownList.selectize();
            dropDownList.change(function () {
                var idItemLista = $(this).val();
                $(this).data('idItemLista', idItemLista);

                var idTipoLista = $(this).data('idTipoLista');
                if (undefined != idTipoLista) {
                    var classCompListaFilho = NAME_PADRAO_COMPONENTES + CLASS_PADRAO_COMPONENTE_LISTA + idTipoLista + CLASS_PADRAO_COMPONENTE_LISTA_FILHO;
                    $('select.' + classCompListaFilho).each(function () {
                        atualizarDropDownListFilho($(this), idItemLista);
                    });
                }
            });

            if (undefined != parametros.itemPreenchimento) {
                if (undefined == idListaPai) {
                    dropDownList[0].selectize.setValue(parametros.itemPreenchimento.ID);
                } else {
                    dropDownList.data('idItemPreenchimento', parametros.itemPreenchimento.ID);
                }
            }
        },
        error: function (request, status, error) {
            fecharPopup();
            if (undefined == dropDownList[0].selectize) {
                dropDownList.selectize();
            }
            mostrarMsgErro('Falha ao tentar carregar a lista, favor tente novamente');
        },
        complete: function () {
            totComponentesCarregados++;
        }
    });

    return nameComp;
}

function aplicarMascaraCpfCnpj(textBox, key) {
    var valTextBox = textBox.val();
    var tamanhoValTextBox = valTextBox.length;
    if (tamanhoValTextBox < TAMANHO_CPF) {
        textBox.mask(VALOR_MASCARA_CPF, { reverse: true });
        textBox.data('codigoValidadorCpfCnpj', CODIGO_VALIDADOR_CPF);
    } else {
        if (tamanhoValTextBox == TAMANHO_CPF) {
            if ('1234567890'.indexOf(key) != -1) {
                if (undefined == textBox.data('tamanhoUp')
                    || '0' == undefined == textBox.data('tamanhoUp')
                    || textBox.data('tamanhoDown') == textBox.data('tamanhoUp')) {
                    textBox.mask(VALOR_MASCARA_CNPJ, { reverse: true });
                    textBox.data('codigoValidadorCpfCnpj', CODIGO_VALIDADOR_CNPJ);
                } else {
                    textBox.mask(VALOR_MASCARA_CPF, { reverse: true });
                    textBox.data('codigoValidadorCpfCnpj', CODIGO_VALIDADOR_CPF);
                }
            } else {
                textBox.mask(VALOR_MASCARA_CPF, { reverse: true });
                textBox.data('codigoValidadorCpfCnpj', CODIGO_VALIDADOR_CPF);
            }
        } else {
            textBox.mask(VALOR_MASCARA_CNPJ, { reverse: true });
            textBox.data('codigoValidadorCpfCnpj', CODIGO_VALIDADOR_CNPJ);
        }
    }
    if (tamanhoValTextBox == 0
        && undefined != $('#formCadastro')
        && undefined != $('#formCadastro').data('bootstrapValidator')) {
        $('#formCadastro').data('bootstrapValidator').updateStatus(textBox.attr('name'), 'INVALID', 'notEmpty');
    }
}

function adicionarMascaraTextBox(codigoMascara, textBox) {
    switch (codigoMascara) {
        case CODIGO_MASCARA_CPF:
            textBox.mask(VALOR_MASCARA_CPF, { reverse: true });
            break;
        case CODIGO_MASCARA_CNPJ:
            textBox.mask(VALOR_MASCARA_CNPJ, { reverse: true });
            break;
        case CODIGO_MASCARA_CPF_CNPJ:
            if (!isNullOrEmpty(textBox.val()) && TAMANHO_CPF < textBox.val().length) {
                textBox.mask(VALOR_MASCARA_CNPJ, { reverse: true });
                textBox.data('codigoValidadorCpfCnpj', CODIGO_VALIDADOR_CNPJ);
            } else {
                textBox.mask(VALOR_MASCARA_CPF, { reverse: true });
                textBox.data('codigoValidadorCpfCnpj', CODIGO_VALIDADOR_CPF);
            }
            textBox.on('paste', function (event) {
                var textoColado;
                try {
                    if (window.clipboardData && window.clipboardData.getData) {//Internet Explorer
                        textoColado = window.clipboardData.getData('Text');
                    } else if (event.originalEvent.clipboardData && event.originalEvent.clipboardData.getData) {//Outros Navegadores
                        textoColado = event.originalEvent.clipboardData.getData('text/plain');
                    }
                    textoColado = textoColado.replace(/[^\d]+/g, '');
                } catch (ex) {
                    textoColado = undefined;
                }
                if (!isNullOrEmpty(textoColado)) {
                    if (textoColado.length > TAMANHO_CPF - 3) {
                        textBox.mask(VALOR_MASCARA_CNPJ, { reverse: true });
                        textBox.data('codigoValidadorCpfCnpj', CODIGO_VALIDADOR_CNPJ);
                        textBox.focus().val(textBox.val());
                    }
                }
            });
            textBox.keydown(function (event) {
                textBox.data('tamanhoDown', $(this).val().length);
                aplicarMascaraCpfCnpj($(this), event.key);
            });
            textBox.keyup(function (event) {
                textBox.data('tamanhoUp', $(this).val().length);
                aplicarMascaraCpfCnpj($(this), event.key);
            });
            break;
        case CODIGO_MASCARA_DATA:
            textBox.mask(VALOR_MASCARA_DATA, { reverse: true });
            break;
        case CODIGO_MASCARA_MONETARIA:
            textBox.mask(VALOR_MASCARA_MONETARIA, { reverse: true });
            break;
        case CODIGO_MASCARA_INTEIRO:
            textBox.mask(VALOR_MASCARA_INTEIRO, { reverse: true });
            break;
    }
}

function adicionarValidadorTextBox(codigoValidador, textBox, objValidador, nomeComponente) {
    switch (codigoValidador) {
        case CODIGO_VALIDADOR_CPF:
            objValidador['callback'] = {
                message: nomeComponente + MSG_ERRO_CAMPO_INVALIDO,
                callback: function (value) {
                    return cpfValido(value);
                }
            };
            break;
        case CODIGO_VALIDADOR_CNPJ:
            objValidador['callback'] = {
                message: nomeComponente + MSG_ERRO_CAMPO_INVALIDO,
                callback: function (value) {
                    return cnpjValido(value);
                }
            };
            break;
        case CODIGO_VALIDADOR_CPF_CNPJ:
            objValidador['callback'] = {
                message: nomeComponente + MSG_ERRO_CAMPO_INVALIDO,
                callback: function (value) {
                    if (CODIGO_VALIDADOR_CPF == textBox.data('codigoValidadorCpfCnpj')
                        || (TAMANHO_CPF + 1) == textBox.data('tamanhoDown')
                        || (TAMANHO_CPF + 1) == textBox.data('tamanhoUp')) {
                        return cpfValido(value.replace(/\.|\-|\/|\s/g, ''));
                    } else {
                        return cnpjValido(value);
                    }
                }
            };
            break;
    }
}

function adicionarCaixaAltaTextBox(textBox, caixaAlta) {
    if (!isNullOrEmpty(caixaAlta) && 1 == caixaAlta) {
        textBox.keydown(function (event) {
            $(this).val($(this).val().toString().toUpperCase());
        });
        textBox.keyup(function (event) {
            $(this).val($(this).val().toString().toUpperCase());
        });
    }
}

function desenharTextBox(parametros) {
    var dvPai = parametros.dvPai;
    var idComponente = parametros.idComponente;
    var nomeComponente = parametros.nomeComponente;
    var descricaoComponente = obterDescricao(nomeComponente, parametros.descricaoComponente);
    var tamanhoComponente = obterTamanho(parametros.tamanhoComponente);
    var textoPreenchimento = isNullOrEmpty(parametros.textoPreenchimento) ? '' : parametros.textoPreenchimento;
    var idCompForm = parametros.idCompForm;
    var codigoMascara = parametros.codigoMascara;
    var codigoValidador = parametros.codigoValidador;
    var objValidador = parametros.objValidador;

    var nameComp = NAME_PADRAO_COMPONENTES + idComponente;

    var dvFormGroup = $('<div class="form-group col-lg-6 dvFormGroupDinamico"></div>');
    var labelNomeComp = $('<label>' + nomeComponente + ':</label>');
    var textBox = $('<input name="' + nameComp + '" value="' + textoPreenchimento + '" class="form-control ' + NAME_PADRAO_COMPONENTES + '" placeholder="' + descricaoComponente + '" ' + tamanhoComponente + '/>');

    dvFormGroup.append(labelNomeComp);
    dvFormGroup.append(textBox);
    dvPai.append(dvFormGroup);

    textBox.data('idComponente', idComponente);
    textBox.data('idCompForm', idCompForm);
    textBox.data('nomeComponente', nomeComponente);
    textBox.data('textoAnterior', textoPreenchimento);

    adicionarMascaraTextBox(codigoMascara, textBox);
    adicionarValidadorTextBox(codigoValidador, textBox, objValidador, nomeComponente);
    adicionarCaixaAltaTextBox(textBox, parametros.caixaAlta)

    totComponentesCarregados++;

    return nameComp;
}

function desenharTextArea(parametros) {
    var dvPai = parametros.dvPai;
    var idComponente = parametros.idComponente;
    var nomeComponente = parametros.nomeComponente;
    var descricaoComponente = obterDescricao(nomeComponente, parametros.descricaoComponente);
    var tamanhoComponente = obterTamanho(parametros.tamanhoComponente);
    var textoPreenchimento = isNullOrEmpty(parametros.textoPreenchimento) ? '' : parametros.textoPreenchimento;
    var idCompForm = parametros.idCompForm;

    var nameComp = NAME_PADRAO_COMPONENTES + idComponente;

    var dvFormGroup = $('<div class="form-group col-lg-12 dvFormGroupDinamico"></div>');
    var labelNomeComp = $('<label>' + nomeComponente + ':</label>');
    var textArea = $('<textarea name="' + nameComp + '" value="' + textoPreenchimento + '" class="form-control ' + NAME_PADRAO_COMPONENTES + '" style="height: 80px; resize: none;" placeholder="' + descricaoComponente + '" ' + tamanhoComponente + '/>');

    dvFormGroup.append(labelNomeComp);
    dvFormGroup.append(textArea);
    dvPai.append(dvFormGroup);

    textArea.data('idComponente', idComponente);
    textArea.data('idCompForm', idCompForm);
    textArea.data('nomeComponente', nomeComponente);
    textArea.data('textoAnterior', textoPreenchimento);

    totComponentesCarregados++;

    return nameComp;
}

function desenharComponente(auxDvPai,
                            componente,
                            auxIdCompForm,
                            auxTextoPreenchimento,
                            auxItemPreenchimento,
                            auxObjValidador) {
    var auxParametros = {
        dvPai: auxDvPai,
        idComponente: componente.ID,
        nomeComponente: componente.NOME,
        descricaoComponente: componente.DESCRICAO,
        tamanhoComponente: componente.TAMANHO,
        obrigaComponente: componente.OBRIGATORIEDADE,
        caixaAlta: componente.CAIXA_ALTA,
        idTipoLista: componente.tipoLista.ID,
        idCompForm: auxIdCompForm,
        textoPreenchimento: auxTextoPreenchimento,
        itemPreenchimento: auxItemPreenchimento,
        codigoMascara: isNullOrEmpty(componente.mascaraComponente) ? undefined : componente.mascaraComponente.CODIGO,
        codigoValidador: isNullOrEmpty(componente.validadorComponente) ? undefined : componente.validadorComponente.CODIGO,
        objValidador: auxObjValidador
    }

    var nameComp;
    switch (componente.tipoComponente.CODIGO) {
        case TIPO_TEXT_BOX:
            nameComp = desenharTextBox(auxParametros);
            break;
        case TIPO_DROP_DOWN_LIST:
            nameComp = desenharDropDownList(auxParametros);
            break;
        case TIPO_TEXT_AREA:
            nameComp = desenharTextArea(auxParametros);
            break;
        default:
            nameComp = '';
    }
    return nameComp;
}

function desenharComponenteSomenteLeitura(dvPai,
                                          nomeComponente,
                                          textoPreenchimento) {
    var dvFormGroup = $('<div class="form-group col-lg-6 dvFormGroupDinamico"></div>');
    var labelNomeComp = $('<label>' + nomeComponente + ':</label>');

    dvFormGroup.append(labelNomeComp);
    if (isNullOrEmpty(textoPreenchimento)) {
        dvFormGroup.append('<br/>');
    } else {
        dvFormGroup.append('<br/>' + textoPreenchimento);
    }
    dvPai.append(dvFormGroup);
}