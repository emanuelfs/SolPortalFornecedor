var camposFormCadastro = {
    nomeTipoLista: {
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
        data: 'listaPai.NOME',
        name: 'NOME_PAI'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'TiposListas/Get';
metodoInsert = 'TiposListas/Insert';

function inicializarTiposLista(ID_FILHO, ID_PAI) {
    mostrarPopup();

    var auxOption = $('<option value="">Informe a lista vinculada</option>');

    if (undefined != $('#idPai')[0].selectize) {
        $('#idPai')[0].selectize.destroy();
    }
    $('#idPai').html('');
    $('#idPai').append(auxOption);

    $.ajax({
        url: isNullOrEmpty(ID_FILHO) ? 'TiposListas/GetParaItem' : 'TiposListas/GetParaSubLista',
        data: isNullOrEmpty(ID_FILHO) ? {} : { ID: ID_FILHO },
        type: 'POST',
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                $('#idPai').append(auxOption);
            }

            $('#idPai').selectize();

            if (undefined == ID_PAI) {
                $('#idPai')[0].selectize.setValue('');
            } else {
                $('#idPai')[0].selectize.setValue(ID_PAI);
            }

            tiposLitasObtidos = true;
        },
        error: function (request, status, error) {
            fecharPopup();
            $('#idPai').selectize();
            mostrarMsgErro('Falha ao tentar obter as listas, favor tente novamente');
        }
    });
}

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeProp('idRegistro');
    $('#nomeTipoLista').val('');
    inicializarTiposLista();
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

function carregarFormCadastro(ID, NOME, ID_PAI) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').prop('idRegistro', ID);
    $('#nomeTipoLista').val(NOME);
    inicializarTiposLista(ID, ID_PAI);
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

//Setar para habilitar botao de excluir
metodoDelete = 'TiposListas/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'ID\': ' + objetoTabela.ID + ' }';
}
//Setar para habilitar botao de excluir

//Setar para habilitar o botao de alteracao
metodoUpdate = 'TiposListas/Update';
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.ID + '\', \'' + objetoTabela.NOME + '\', \'' + objetoTabela.listaPai.ID + '\'';
}
//Setar para habilitar o botao de alteracao

function montarDadosCadastro() {
    return { 
        NOME: $('#nomeTipoLista').val(),
        ID_PAI: $('#idPai').val()
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
        exportarArquivo('TiposListas/GetParaExcel', 'TiposListas/ExportarExcel', 'Listas');
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