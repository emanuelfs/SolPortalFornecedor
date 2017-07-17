var TOT_FORMULARIOS_LINHA = 4;
var guardaTituloPagina = $('#tituloPagina').html();
var tipoNoStatusFluxoStart = 'start';
var tipoNoStatusFluxoAcao = 'action';
var tipoNoStatusFluxoQuestion = 'question';
var tipoNoStatusFluxoSaida = 'output';
var dvNosCriados = undefined;
var widthPadrao = 200;
var heigthPadrao = 200;
var topPadrao = 50;
var leftInicial = topPadrao;
var incrementoLeft = 350;

function noStatusExistente(idNo) {
    var noStatusFluxo = dvNosCriados.find('#' + idNo);
    return undefined != noStatusFluxo
        && undefined != noStatusFluxo.data('noStatusFluxo')
        ? noStatusFluxo.data('noStatusFluxo') : undefined;
}

function criarNoStatusFluxo(idNo, textoNo, tipoNo, valorLeft) {
    var no = {
        id: 'noStatusFluxo' + idNo,
        text: textoNo,
        type: tipoNo,
        w: widthPadrao,
        h: heigthPadrao,
        top: topPadrao,
        left: valorLeft
    };
    var noStatusFluxo = $('<div id="' + idNo + '"></div>');
    noStatusFluxo.data('noStatusFluxo', no);
    dvNosCriados.append(noStatusFluxo);
    return no;
}

function desenharFluxo(statusFluxo, linksFluxo) {
    $('#jtk-demo-flowchart').html('<div class="jtk-demo-main" id="jtk-demo-flowchart">'
    + '    <div class="jtk-demo-canvas">'
    + '        <div class="controls">'
    + '            <i style="font-style: normal;" reset>Ajustar Zoom</i>'
    + '        </div>'
    + '    </div>'
    + '</div>');

    jsPlumbToolkit.ready(function () {
        var idFunction = function (n) {
            return n.id;
        };

        var typeFunction = function (n) {
            return n.type;
        };

        var mainElement = document.querySelector("#jtk-demo-flowchart");
        var canvasElement = mainElement.querySelector(".jtk-demo-canvas");
        var controls = mainElement.querySelector(".controls");

        var toolkit = jsPlumbToolkit.newInstance({
            idFunction: idFunction,
            typeFunction: typeFunction,
            nodeFactory: function (type, data, callback) {
                jsPlumbToolkit.Dialogs.show({
                    id: "dlgText",
                    title: "Enter " + type + " name:",
                    onOK: function (d) {
                        data.text = d.text;
                        if (data.text) {
                            if (data.text.length >= 2) {
                                data.id = jsPlumbToolkitUtil.uuid();
                                callback(data);
                            } else {
                                alert(type + " names must be at least 2 characters!");
                            }
                        }
                    }
                });
            },
            beforeStartConnect: function (node, edgeType) {
                return (node.data.type === "start" && node.getEdges().length > 0) ? false : { label: "..." };
            }
        });

        jsPlumbToolkit.Dialogs.initialize({
            selector: ".dlg"
        });

        var _editLabel = function (edge, deleteOnCancel) {
            jsPlumbToolkit.Dialogs.show({
                id: "dlgText",
                data: {
                    text: edge.data.label || ""
                },
                onOK: function (data) {
                    toolkit.updateEdge(edge, { label: data.text || "" });
                },
                onCancel: function () {
                    if (deleteOnCancel) {
                        toolkit.removeEdge(edge);
                    }
                }
            });
        };

        var renderer = window.renderer = toolkit.render({
            container: canvasElement,
            view: {
                nodes: {
                    "start": {
                        template: "tmplStart"
                    },
                    "selectable": {
                        events: {
                            tap: function (params) {
                                toolkit.toggleSelection(params.node);
                            }
                        }
                    },
                    "question": {
                        parent: "selectable",
                        template: "tmplQuestion"
                    },
                    "action": {
                        parent: "selectable",
                        template: "tmplAction"
                    },
                    "output": {
                        parent: "selectable",
                        template: "tmplOutput"
                    }
                },
                edges: {
                    "default": {
                        anchor: "AutoDefault",
                        endpoint: "Blank",
                        connector: ["Flowchart", { cornerRadius: 5 }],
                        paintStyle: { strokeWidth: 2, stroke: "#f76258", outlineWidth: 3, outlineStroke: "transparent" },	//	paint style for this edge type.
                        hoverPaintStyle: { strokeWidth: 2, stroke: "rgb(67,67,67)" }, // hover paint style for this edge type.
                        events: {
                            "dblclick": function (params) {
                                jsPlumbToolkit.Dialogs.show({
                                    id: "dlgConfirm",
                                    data: {
                                        msg: "Delete Edge"
                                    },
                                    onOK: function () {
                                        toolkit.removeEdge(params.edge);
                                    }
                                });
                            }
                        },
                        overlays: [
                            ["Arrow", { location: 1, width: 10, length: 10 }],
                            ["Arrow", { location: 0.3, width: 10, length: 10 }]
                        ]
                    },
                    "connection": {
                        parent: "default",
                        overlays: [
                            [
                                "Label", {
                                    label: "${label}",
                                    events: {
                                        click: function (params) {
                                            _editLabel(params.edge);
                                        }
                                    }
                                }
                            ]
                        ]
                    }
                },
                ports: {
                    "start": {
                        edgeType: "default"
                    },
                    "source": {
                        maxConnections: -1,
                        edgeType: "connection"
                    },
                    "target": {
                        maxConnections: -1,
                        isTarget: true,
                        dropOptions: {
                            hoverClass: "connection-drop"
                        }
                    }
                }
            },
            layout: {
                type: "Absolute"
            },
            events: {
                canvasClick: function (e) {
                    toolkit.clearSelection();
                },
                edgeAdded: function (params) {
                    if (params.addedByMouse) {
                        _editLabel(params.edge, true);
                    }
                },
                nodeDropped: function (info) {
                    console.log("node ", info.source.id, "dropped on ", info.target.id);
                }
            },
            lassoInvert: true,
            elementsDroppable: true,
            consumeRightClick: false,
            dragOptions: {
                filter: ".jtk-draw-handle, .node-action, .node-action i"
            }
        });

        var datasetView = new jsPlumbSyntaxHighlighter(toolkit, ".jtk-demo-dataset");

        toolkit.load({
            data: {
                "nodes": statusFluxo,
                "edges": linksFluxo
            },
            onload: function () {
                renderer.zoomToFit();
            }
        });

        renderer.bind("modeChanged", function (mode) {
            jsPlumb.removeClass(controls.querySelectorAll("[mode]"), "selected-mode");
            jsPlumb.addClass(controls.querySelectorAll("[mode='" + mode + "']"), "selected-mode");
        });

        jsPlumb.on(controls, "tap", "[mode]", function () {
            renderer.setMode(this.getAttribute("mode"));
        });

        jsPlumb.on(controls, "tap", "[reset]", function () {
            toolkit.clearSelection();
            renderer.zoomToFit();
        });

        new jsPlumbToolkit.DrawingTools({
            renderer: renderer
        });

        jsPlumb.on(canvasElement, "tap", ".node-delete", function () {
            var info = renderer.getObjectInfo(this);
            jsPlumbToolkit.Dialogs.show({
                id: "dlgConfirm",
                data: {
                    msg: "Delete '" + info.obj.data.text + "'"
                },
                onOK: function () {
                    toolkit.removeNode(info.obj);
                }
            });
        });

        jsPlumb.on(canvasElement, "tap", ".node-edit", function () {
            var info = renderer.getObjectInfo(this);
            jsPlumbToolkit.Dialogs.show({
                id: "dlgText",
                data: info.obj.data,
                title: "Edit " + info.obj.data.type + " name",
                onOK: function (data) {
                    if (data.text && data.text.length > 2) {
                        toolkit.updateNode(info.obj, data);
                    }
                }
            });
        });
    });
}

function obterTipoNoStatus(inicial, totDestinos) {
    if (1 == inicial) {
        return tipoNoStatusFluxoStart;
    }
    switch (totDestinos) {
        case 0:
            return tipoNoStatusFluxoSaida;
        case 1:
            return tipoNoStatusFluxoAcao;
        default:
            return tipoNoStatusFluxoQuestion;
    }
}

function obterFluxos(idFormulario, nomeFormulario) {
    mostrarPopup();

    $.ajax({
        url: '../FluxoStatus/GetPorFormulario',
        type: 'POST',
        data: {
            ID_FORMULARIO: idFormulario
        },
        success: function (result) {
            var msgErro = result.msgErro;
            if (!isNullOrEmpty(msgErro)) {
                mostrarMsgErro(msgErro);
                fecharPopup();
            } else {
                var dados = result.data;
                if (dados.length > 0) {
                    dvNosCriados = $('<div></div>');
                    leftInicial = topPadrao;

                    var statusFluxo = [];
                    var linksFluxo = [];
                    var noStatusOrigem;
                    var noStatusDestino;
                    var tipoNoStatus;

                    for (var i = 0; i < dados.length; i++) {
                        noStatusOrigem = noStatusExistente(dados[i].statusOrigem.ID);
                        noStatusDestino = noStatusExistente(dados[i].statusDestino.ID);

                        if (undefined == noStatusOrigem) {
                            tipoNoStatus = obterTipoNoStatus(dados[i].statusOrigem.INICIAL, dados[i].TOT_DESTINOS_ORIGEM);
                            noStatusOrigem = criarNoStatusFluxo(dados[i].statusOrigem.ID, dados[i].statusOrigem.NOME, tipoNoStatus, leftInicial);
                            statusFluxo[statusFluxo.length] = noStatusOrigem;
                            leftInicial += incrementoLeft;
                        }
                        if (undefined == noStatusDestino) {
                            tipoNoStatus = obterTipoNoStatus(0, dados[i].TOT_DESTINOS_DESTINO);
                            noStatusDestino = criarNoStatusFluxo(dados[i].statusDestino.ID, dados[i].statusDestino.NOME, tipoNoStatus, leftInicial);
                            statusFluxo[statusFluxo.length] = noStatusDestino;
                            leftInicial += incrementoLeft;
                        }

                        linksFluxo[linksFluxo.length] = {
                            id: i,
                            source: noStatusOrigem.id,
                            target: noStatusDestino.id
                        };
                    }

                    mostrarDvTabelaPrincipal(undefined, $('#dvTabelaFormularios'), $('#dvTabelaPrincipal'), undefined, nomeFormulario);
                    desenharFluxo(statusFluxo, linksFluxo);
                    fecharPopup();
                } else {
                    $('#tituloPagina').html(guardaTituloPagina);
                    mostrarMsgErro('Nenhum fluxo foi encontrado para o formulário ' + nomeFormulario);
                    fecharPopup();
                }
            }
        },
        error: function (request, status, error) {
            mostrarMsgErro('Falha ao tentar obter os fluxos, favor tente novamente');
            fecharPopup();
        }
    });
}

function desenharFormulario(dvPai, id, nome) {
    var result = $('<div class="col-lg-3"></div>');
    var btnResult = $('<button class="btn btn-md btn-primary btn-formulario"><i class="fa fa-file-text-o" style="font-size:30px"></i><br /><label>' + nome + '</label></button>');
    btnResult.click(function () {
        obterFluxos(id, nome);
    });
    result.append(btnResult);
    dvPai.append(result);
}

function obterFormularios() {
    mostrarPopup();

    $.ajax({
        url: '../Formularios/GetParaHome',
        type: 'POST',
        data: {
            somenteAtivos: 'N', filtrarStatus: 'S'
        },
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
                    desenharFormulario(dvPai, dados[indiceFormulario].ID, dados[indiceFormulario].NOME);
                    indiceFormulario++;
                }

                dvPai = $('<div class="row"></div>');
                $('#dvPrincipal').append(dvPai);
                for (i = 0; i < totFormulariosResto; i++) {
                    desenharFormulario(dvPai, dados[indiceFormulario].ID, dados[indiceFormulario].NOME);
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

$(document).ready(function () {
    obterFormularios();

    $('#btnVoltar').click(function () {
        mostrarDvTabelaPrincipal(undefined, $('#dvTabelaPrincipal'), $('#dvTabelaFormularios'), undefined, guardaTituloPagina);
    });
});