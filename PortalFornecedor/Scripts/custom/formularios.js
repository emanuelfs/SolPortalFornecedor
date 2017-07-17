var camposFormCadastro = {
    nome: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    codigo: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    sigla: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    anexoObrigatorio: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    exibirTodos: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    formAtivo: {
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
        data: 'CODIGO',
        name: 'CODIGO'
    },
    {
        data: 'SIGLA',
        name: 'SIGLA'
    },
    {
        data: 'TEXTO_OBRIGATORIEDADE_ANEXO',
        name: 'OBRIGATORIEDADE_ANEXO'
    },
    {
        data: 'TEXTO_EXIBIR_TODOS',
        name: 'EXIBIR_TODOS'
    },
    {
        data: 'TEXTO_ATIVO',
        name: 'ATIVO'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'Formularios/Get';
metodoInsert = 'Formularios/Insert';

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeProp('idRegistro');
    $('#nome').val('');
    $('#codigo').val('');
    $('#sigla').val('');

    var anexoObrigatorio = $('input[type=radio][name=anexoObrigatorio]:checked');
    anexoObrigatorio.attr("checked", false);
    anexoObrigatorio.removeProp('checked');

    var exibirTodos = $('input[type=radio][name=exibirTodos]:checked');
    exibirTodos.attr("checked", false);
    exibirTodos.removeProp('checked');

    //var formAtivo = $('input[type=radio][name=formAtivo]:checked');
    //formAtivo.attr("checked", false);
    //formAtivo.removeProp('checked');
    var formAtivo = $('input[type=radio][name=formAtivo][value=0]');
    formAtivo.attr("checked", true);
    formAtivo.prop('checked', 'checked');
    $('#dvFormAtivo').css('display', 'none');

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

function carregarFormCadastro(ID, NOME, CODIGO, SIGLA, OBRIGATORIEDADE_ANEXO, EXIBIR_TODOS, ATIVO) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').prop('idRegistro', ID);
    $('#nome').val(NOME);
    $('#codigo').val(CODIGO);
    $('#sigla').val(SIGLA);

    var anexoObrigatorio = $('input[type=radio][name=anexoObrigatorio][value=' + OBRIGATORIEDADE_ANEXO + ']');
    anexoObrigatorio.attr("checked", true);
    anexoObrigatorio.prop('checked', 'checked');

    var exibirTodos = $('input[type=radio][name=exibirTodos][value=' + EXIBIR_TODOS + ']');
    exibirTodos.attr("checked", true);
    exibirTodos.prop('checked', 'checked');

    var formAtivo = $('input[type=radio][name=formAtivo][value=' + ATIVO + ']');
    formAtivo.attr("checked", true);
    formAtivo.prop('checked', 'checked');
    $('#dvFormAtivo').css('display', '');

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

//Setar para habilitar botao de excluir
metodoDelete = 'Formularios/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'ID\': ' + objetoTabela.ID + ' }';
}
//Setar para habilitar botao de excluir

//Setar para habilitar o botao de alteracao
metodoUpdate = 'Formularios/Update';
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.ID + '\', \'' + objetoTabela.NOME + '\', \'' + objetoTabela.CODIGO + '\', \'' + objetoTabela.SIGLA + '\', \'' + objetoTabela.OBRIGATORIEDADE_ANEXO + '\', \'' + objetoTabela.EXIBIR_TODOS + '\', \'' + objetoTabela.ATIVO + '\'';
}
//Setar para habilitar o botao de alteracao

function montarDadosCadastro() {
    var anexoObrigatorio = $('input[type=radio][name=anexoObrigatorio]:checked');
    var exibirTodos = $('input[type=radio][name=exibirTodos]:checked');
    var formAtivo = $('input[type=radio][name=formAtivo]:checked');
    return {
        NOME: $('#nome').val(),
        CODIGO: $('#codigo').val(),
        SIGLA: $('#sigla').val(),
        OBRIGATORIEDADE_ANEXO: anexoObrigatorio.val(),
        EXIBIR_TODOS: exibirTodos.val(),
        ATIVO: formAtivo.val()
    };
}

$(document).ready(function () {
    $('#codigo').mask(VALOR_MASCARA_INTEIRO, { reverse: true });

    inicializarTabelaPrincipal($('#tabelaPrincipal'), colunasTabelaPrincipal);

    options.fields = camposFormCadastro;
    $('#formCadastro').bootstrapValidator(options);

    $('#btnNovo').click(function () {
        limparFormCadastro();
    });

    $('#btnExportarExcel').click(function () {
        exportarArquivo('Formularios/GetParaExcel', 'Formularios/ExportarExcel', 'Formularios');
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