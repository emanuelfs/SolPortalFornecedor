var gruposObtidos = false;
var formulariosObtidos = false;
var camposFormCadastro = {
    nomeGrupo: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    idFormulario: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    }
};
var colunasTabelaPrincipal = [
    {
        data: 'grupo.NOME',
        name: 'NOME_GRUPO'
    },
    {
        data: 'formulario.NOME',
        name: 'NOME_FORMULARIO'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'PermissaoExibicao/Get';
metodoInsert = 'PermissaoExibicao/Insert';

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');

    if (gruposObtidos) {
        $('#nomeGrupo')[0].selectize.setValue('');
        $('#formCadastro').data('bootstrapValidator').updateStatus('nomeGrupo', 'NOT_VALIDATED');
    }

    if (formulariosObtidos) {
        $('#idFormulario')[0].selectize.setValue('');
        $('#formCadastro').data('bootstrapValidator').updateStatus('idFormulario', 'NOT_VALIDATED');
    }

    var listaSelects =
    [
        {
            registrosObtidos: gruposObtidos,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarGrupos
        },
        {
            registrosObtidos: formulariosObtidos,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarFormularios
        }
    ];
    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

//Setar para habilitar botao de excluir
metodoDelete = 'PermissaoExibicao/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'NOME_GRUPO\': \'' + objetoTabela.grupo.NOME + '\', \'ID_FORMULARIO\': ' + objetoTabela.formulario.ID + ' }';
}
//Setar para habilitar botao de excluir

function inicializarGrupos(NOME_GRUPO) {
    mostrarPopup();

    var auxOption = $('<option value="">Informe o grupo</option>');
    $('#nomeGrupo').append(auxOption);

    $.ajax({
        url: 'Grupo/GetParaPermissao',
        type: 'POST',
        success: function (result) {
            var dados = result.data;
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].NOME + '">' + dados[i].NOME + '</option>');
                $('#nomeGrupo').append(auxOption);
            }

            $('#nomeGrupo').selectize();

            if (undefined == NOME_GRUPO) {
                $('#nomeGrupo')[0].selectize.setValue('');
                $('#formCadastro').data('bootstrapValidator').updateStatus('nomeGrupo', 'NOT_VALIDATED');
            } else {
                $('#nomeGrupo')[0].selectize.setValue(NOME_GRUPO);
                $('#formCadastro').data('bootstrapValidator').updateStatus('nomeGrupo', 'VALID');
            }

            gruposObtidos = true;

            fecharPopup();
        },
        error: function (request, status, error) {
            $('#nomeGrupo').selectize();
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os grupos, favor tente novamente');
        }
    });
}

function inicializarFormularios(ID_FORMULARIO) {
    mostrarPopup();

    var auxOption = $('<option value="">Informe o formulário</option>');
    $('#idFormulario').append(auxOption);

    $.ajax({
        url: 'Formularios/GetParaComponente',
        type: 'POST',
        success: function (result) {
            var dados = result.data;            
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].ID + '">' + dados[i].NOME + '</option>');
                $('#idFormulario').append(auxOption);
            }

            $('#idFormulario').selectize();

            if (undefined == ID_FORMULARIO) {
                $('#idFormulario')[0].selectize.setValue('');
                $('#formCadastro').data('bootstrapValidator').updateStatus('idFormulario', 'NOT_VALIDATED');
            } else {
                $('#idFormulario')[0].selectize.setValue(ID_FORMULARIO);
                $('#formCadastro').data('bootstrapValidator').updateStatus('idFormulario', 'VALID');
            }

            formulariosObtidos = true;

            fecharPopup();
        },
        error: function (request, status, error) {
            $('#idFormulario').selectize();
            fecharPopup();
            mostrarMsgErro('Falha ao tentar obter os formulários, favor tente novamente');
        }
    });
}

function montarDadosCadastro() {
    return {
        NOME_GRUPO: $('#nomeGrupo').val(),
        ID_FORMULARIO: $('#idFormulario').val()
    };
}

function salvarFormulario(continuar) {
    var auxUrl = metodoInsert;
    var auxData = montarDadosCadastro();
    salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData, undefined, continuar);
}

$(document).ready(function () {
    inicializarTabelaPrincipal($('#tabelaPrincipal'), colunasTabelaPrincipal);

    options.fields = camposFormCadastro;
    $('#formCadastro').bootstrapValidator(options);

    $('#btnNovo').click(function () {
        limparFormCadastro();
    });

    $('#btnExportarExcel').click(function () {
        exportarArquivo('PermissaoExibicao/GetParaExcel', 'PermissaoExibicao/ExportarExcel', 'Permissoes');
    });

    $('#btnSalvar').click(function () {
        salvarFormulario(false);
    });

    $('#btnSalvarContinuar').click(function () {
        salvarFormulario(true);
    });

    $('#btnCancelar').click(function () {
        mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), undefined);
    });
});