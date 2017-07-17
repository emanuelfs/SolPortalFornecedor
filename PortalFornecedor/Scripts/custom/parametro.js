var camposFormCadastro = {
    nome: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    valor: {
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
        data: 'VALOR',
        name: 'VALOR'
    },
    {
        data: 'DESCRICAO',
        name: 'DESCRICAO'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'Parametro/Get';
metodoInsert = 'Parametro/Insert';

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeProp('idRegistro');
    $('#nome').val('');
    $('#valor').val('');
    $('#descricao').val('');
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

function carregarFormCadastro(NOME, VALOR, DESCRICAO) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').prop('idRegistro', NOME);
    $('#nome').val(NOME);
    $('#valor').val(VALOR);
    $('#descricao').val(DESCRICAO);
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

//Setar para habilitar botao de excluir
metodoDelete = 'Parametro/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'NOME\': \'' + objetoTabela.NOME + '\' }';
}
//Setar para habilitar botao de excluir

//Setar para habilitar o botao de alteracao
metodoUpdate = 'Parametro/Update';
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.NOME + '\', \'' + objetoTabela.VALOR + '\', \'' + objetoTabela.DESCRICAO + '\'';
}
//Setar para habilitar o botao de alteracao

function montarDadosCadastro() {
    return {
        NOME: $('#nome').val(),
        VALOR: $('#valor').val(),
        DESCRICAO: $('#descricao').val()
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
        exportarArquivo('Parametro/GetParaExcel', 'Parametro/ExportarExcel', 'Parametros');
    });

    $('#btnSalvar').click(function () {
        var auxUrl;
        var auxData = montarDadosCadastro();

        var auxId = $(this).prop('idRegistro');
        if (undefined == auxId) {
            auxUrl = metodoInsert;
        } else {
            auxUrl = metodoUpdate;
            auxData.NOME_ANTERIOR = auxId;
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