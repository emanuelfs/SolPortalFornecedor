var modulosObtidos = false;
var camposFormCadastro = {
    nome: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    nomeModulo: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    caminho: {
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
        data: 'modulo.NOME',
        name: 'NOME_MODULO'
    },
    {
        data: 'CAMINHO',
        name: 'CAMINHO'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'Funcionalidade/Get';
metodoInsert = 'Funcionalidade/Insert';

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeProp('idRegistro');
    $('#nome').val('');
    $('#caminho').val('');

    if (modulosObtidos) {
        $('#nomeModulo')[0].selectize.setValue('');
        $('#formCadastro').data('bootstrapValidator').updateStatus('nomeModulo', 'NOT_VALIDATED');
    }

    var listaSelects =
    [
        {
            registrosObtidos: modulosObtidos,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarModulos
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

function carregarFormCadastro(NOME, NOME_MODULO, CAMINHO) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').prop('idRegistro', CAMINHO);
    $('#nome').val(NOME);
    $('#caminho').val(CAMINHO);

    if (modulosObtidos) {
        $('#nomeModulo')[0].selectize.setValue(NOME_MODULO);
        $('#formCadastro').data('bootstrapValidator').updateStatus('nomeModulo', 'NOT_VALIDATED');
    }

    var listaSelects =
    [
        {
            registrosObtidos: modulosObtidos,
            idRegistroEscolhido: NOME_MODULO,
            funcaoInicializacao: inicializarModulos
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

//Setar para habilitar botao de excluir
metodoDelete = 'Funcionalidade/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'CAMINHO\': \'' + objetoTabela.CAMINHO + '\' }';
}
//Setar para habilitar botao de excluir

//Setar para habilitar o botao de alteracao
metodoUpdate = 'Funcionalidade/Update';
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.NOME + '\', \'' + objetoTabela.modulo.NOME + '\', \'' + objetoTabela.CAMINHO + '\'';
}
//Setar para habilitar o botao de alteracao

function inicializarModulos(nomeModulo) {
    mostrarPopup();

    $.ajax({
        url: 'Modulo/GetParaFuncionalidade',
        type: 'POST',
        success: function (result) {
            fecharPopup();

            var dados = result.data;
            var auxOption = $('<option value="">Informe o módulo</option>');
            $('#nomeModulo').append(auxOption);
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].NOME + '">' + dados[i].NOME + '</option>');
                $('#nomeModulo').append(auxOption);
            }
            $('#nomeModulo').selectize();

            if (undefined == nomeModulo) {
                $('#nomeModulo')[0].selectize.setValue('');
                $('#formCadastro').data('bootstrapValidator').updateStatus('nomeModulo', 'NOT_VALIDATED');
            } else {
                $('#nomeModulo')[0].selectize.setValue(nomeModulo);
                $('#formCadastro').data('bootstrapValidator').updateStatus('nomeModulo', 'VALID');
            }

            modulosObtidos = true;
        },
        error: function (request, status, error) {
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os módulos, favor tente novamente');
        }
    });
}

function montarDadosCadastro() {
    return {
        NOME: $('#nome').val(),
        NOME_MODULO: $('#nomeModulo').val(),
        CAMINHO: $('#caminho').val()
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
        exportarArquivo('Funcionalidade/GetParaExcel', 'Funcionalidade/ExportarExcel', 'Funcionalidades');
    });

    $('#btnSalvar').click(function () {
        var auxUrl;
        var auxData = montarDadosCadastro();

        var auxId = $(this).prop('idRegistro');
        if (undefined == auxId) {
            auxUrl = metodoInsert;
        } else {
            auxUrl = metodoUpdate;
            auxData.CAMINHO_ANTERIOR = auxId;
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