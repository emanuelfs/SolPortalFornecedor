var camposFormCadastro = {
    nome: {
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
        render: renderColunaOpcoes
    }
];
metodoGet = 'Grupo/Get';
metodoInsert = 'Grupo/Insert';

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeProp('idRegistro');
    $('#nome').val('');
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

function carregarFormCadastro(NOME) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').prop('idRegistro', NOME);
    $('#nome').val(NOME);
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

//Setar para habilitar botao de excluir
metodoDelete = 'Grupo/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'NOME\': \'' + objetoTabela.NOME + '\' }';
}
//Setar para habilitar botao de excluir

//Setar para habilitar o botao de alteracao
metodoUpdate = 'Grupo/Update';
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.NOME + '\'';
}
//Setar para habilitar o botao de alteracao

function montarDadosCadastro() {
    return {
        NOME: $('#nome').val()
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
        exportarArquivo('Grupo/GetParaExcel', 'Grupo/ExportarExcel', 'Grupos');
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