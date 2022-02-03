var currentIdMail = undefined;
var curentIdReplied = undefined;
var informacionCorreos = []
var currentPayloadActionItemMail = undefined;// no es un manejador de estado

//var fIsInDetalleCorreo = false;
var currentView = 'inbox';
var currentPageMail = 1;

var textEditorReplyCorreo = undefined;
var textEditorNuevoCorreo = undefined;

$(document).ready(function () {

    $('#btn-confirmar-papelera-correo').click(function () {
        const {
            id,
            btnEliminar,
        } = currentPayloadActionItemMail;

        ActualizarCorreoPapelera(btnEliminar, id)
        $('#modal-correo-integracion-enviar-papelera').modal('hide');
    })

    // fuciones modal nuevo correo 
    $('#mailComposeBtn').on('click', function () {
        $('#mailCompose').addClass('show');
        $('#mailCompose').addClass('shrink');
    })

    $('#mailComposeClose').on('click', function (e) {
        e.preventDefault()

        if ($('#mailCompose').hasClass('minimize') || $('#mailCompose').hasClass('shrink')) {
            $('#mailCompose').addClass('d-none');

            setTimeout(function () {
                $('#mailCompose').attr('class', 'mail-compose');
            }, 500);

        } else {
            $('#mailCompose').removeClass('show');
        }
    })

    $('#mailComposeShrink').on('click', function (e) {
        e.preventDefault()
        $('#mailCompose').toggleClass('shrink')
        $('#mailCompose').removeClass('minimize')
    })

    $('#mailComposeMinimize').on('click', function (e) {
        e.preventDefault()
        $('#mailCompose').toggleClass('minimize')
    })

    /* eventos de paginacion*/
    $('#paginacion-prev').click(function () {
        currentPageMail--;
        FnListarCorreos()
        
    })

    $('#paginacion-next').click(function () {
        currentPageMail++;
        FnListarCorreos()
    })

    // Routes

    $('#btn-inbox-correos').on('click', function (e) {
        if (currentIdMail) { // dentro de detalle
            FnComebackMailContainer()
        }
        currentView = 'inbox'
        FnListarCorreos()
    })

    $('#btn-starred-correos').on('click', function (e) {
        if (currentIdMail) { // dentro de detalle
            FnComebackMailContainer()
        }
        currentView = 'starred'
        FnListarCorreos()
    })

    $('#btn-sent-correos').on('click', function (e) {
        if (currentIdMail) { // dentro de detalle
            FnComebackMailContainer()
        }
        currentView = 'sent'
        FnListarCorreos()
    })

    $('#btn-archive-correos').on('click', function (e) {
        if (currentIdMail) { // dentro de detalle
            FnComebackMailContainer()
        }
        currentView = 'archived'
        FnListarCorreos()
    })

    $('#btn-bin-correos').on('click', function (e) {
        if (currentIdMail) { // dentro de detalle
            FnComebackMailContainer()
        }
        currentView = 'trash'
        FnListarCorreos()
    })

    $('#btn-drafts-correos').on('click', function (e) {
        if (currentIdMail) { // dentro de detalle
            FnComebackMailContainer()
        }
        currentView = 'draft'
        FnListarCorreos()
    })

    //Btn cerrar correo nuevo y enviar a borrador
    $('#mailComposeClose').on('click' , function(e){
        FnEnviarCorreoABorrador(false)
        $('#input-para-compose-nuevoCorreo').val('')
        $('#asunto-nuevoCorreo').val('')
        $('#input-cco-nuevoCorreo').val('')
        $('#input-cco-nuevoCorreo').val('')
        textEditorNuevoCorreo.setContents([])
    })

    //Btn atras enviar correo respuesta a borradores
    $('#btn-regresar-rta').on('click', function (e) {
        FnEnviarCorreoABorrador(true)
        document.getElementById('section-bandeja-temporal').querySelector('.content-body').scrollTo(0, 0)
        $('#input-emailto-reply-detalleCorreo').val('')
        $('#input-cc-reply-detalleCorreo').val('')
        $('#input-cc-reply-detalleCorreo').val('')
      //  textEditorReplyCorreo.setContents([])
        var element = document.getElementsByClassName("ql-editor");
        element[0].innerHTML = "";
    })
    
    // libreria quill para reply
    textEditorReplyCorreo = new Quill('#editor-texto', {
        modules: {
            toolbar: '#toolbar-container'
        },
        theme: 'snow',
    });

    // libreria quill para nuevo correo
    textEditorNuevoCorreo = new Quill('#editor-texto-nuevoCorreo', {
        modules: {
            toolbar: '#toolbar-container-nuevoCorreo'
        },
        theme: 'snow',
    });

    // evento btn enviar nuevo correo
    $('#frm-compose-nuevo-correo').submit(function (e) {
        e.preventDefault();
        enviarCorreo()
    })

    // evento btn enviar respuesta correo
    $('#btn-enviar-respuestaCoreo').on('click', function () {
        ReplyCorreoIntegracion()
        //FnComebackMailContainer()
    });

    //Refrescar
    $('#btn-refrescarlistadocorreos').click(function () {
        FnListarCorreos()
    });

    //filtro de busqueda
    $('#frm-filtroBusqueda-correos').submit(function (e) {
        e.preventDefault();
        FnFiltroBusquedaCorreos()
    })

    // eliminar nuevo correo
    $('#eliminar-nuevoCorreo').on('click', function (e) {
        $('#mailCompose').removeClass('d-none')
        $('#mailCompose').removeClass('show');
    })

    // funcion btn replay
    $('#btn_reply').on('click', function (e) {
        FnIniciarReplyCorreo(currentIdMail)
    })

    //btn delete
    $('#btn-delete').on('click', function (e) {
        $('#container-reply').addClass('d-none')
    })

    //btn regresar reply
    $('#btn-regresar-rta').on('click', function (e) {
        $('#container-reply').addClass('d-none')
    })

    //btn marcar favorito seccion (correo detalle)
    $('#btn-fav-detalleCorreo').on('click', function (e) {
        AgregarFavoritoDetalleCorreo()
    })

    //btn marcar archivado seccion (correo detalle)
    $('#btn-archivar-detalleCorreo').on('click', function (e) {
        AgregarArchivadoDetalleCorreo()
    })

    //btn enviar a papelera seccion (correo detalle)
    $('#btn-papelera-detalleCorreo').on('click', function (e) {
        ActualizarCorreoPapeleraDetalleCorreo()
    })

    //btn eliminar definitivo(correo detalle)
    $('#btn-eliminarDefinitiva-detalleCorreo').on('click', function (e) {
        FnEliminarCorreoDefDetalleCorreo(currentIdMail);
    })

    //Restaurar Correo (correo detalle)
    $('#btn-restaurar-detalleCorreo').on('click', function (e) {
        RestaurarCorreoDetalleCorreo();
    })

    //generar oportunidad crm
     $('#btn-generaroportunidadcrm-correos').click(function () {
         FnGenerarOportunidadContactoCorreo()
     })

    //Agregar comentario
    $('#btn-guardar-comentarios').click(function () {
        CrearComentario()
    })

    //Agregar evento calendarios 
    $('#frm-create-event-correo').submit(function (e) {
        e.preventDefault()
        FnGenerarEventosCorreo()
    })

    // Refrescar listado eventos
    $('#btn-refresar-calendario-correo').click(function (e) {
        FnListarEventosCorreo()
    });

    // Keydown-generar tarea
    let inputTareaCorreo = document.getElementById('input-tab-correo-creartarea');
    inputTareaCorreo.addEventListener('keydown', (e) => {
        let code = e.keyCode;
        if (code === 13) {
            if (inputTareaCorreo.value.length === 0) return;
            FnGenerarTareaCorreos(inputTareaCorreo.value);
            inputTareaCorreo.value = "";
        }
    })

    //Keydown Buscar Correos
    /*let inputBuscarCorreos = document.getElementById('input-buscarCorreos');
    inputBuscarCorreos.addEventListener('keydown', (e) => {
        let code = e.keyCode;
        if (code === 13) {
            if (inputBuscarCorreos.value.length === 0) return;
            (inputBuscarCorreos.value);
            inputBuscarCorreos.value = "";
            console.log('Keydown busqueda')
        }
    })*/

   //Refrescar listado tareas
     $('#btn-refresar-tareas-correo').click(function () {
         FnListarTareasCorreo()
    });
 
    //btns cc y cco en reply
    $('#btn-add-cc').click(function () {
        $('#compose-reply-cc-container').removeClass('d-none')
        $('#separacion-linea-cc-reply').removeClass('d-none')
    })

    $('#btn-add-bcc').click(function () {
        $('#compose-reply-cco-container').removeClass('d-none')
        $('#separacion-linea-cco-reply').removeClass('d-none')
    })

    //btns para cerrar cc y cco en reply
    $('#compose-reply-close-cc').click(function () {
        $('#compose-reply-cc-container').addClass('d-none')
        $('#separacion-linea-cc-reply').addClass('d-none')

    })

    $('#compose-reply-close-cco').click(function () {
        $('#compose-reply-cco-container').addClass('d-none')
        $('#separacion-linea-cco-reply').addClass('d-none')
    })

    //btns cc y cco en nuevo correo
    $('#btn-cc-nuevoCorreo').click(function () {
        $('#contenedorNuevoCorreo-cc').removeClass('d-none')
        $('#separacion-linea-cc-nuevo').removeClass('d-none')
        console.log('open cc container')
    })

    $('#btn-cco-nuevoCorreo').click(function () {
        $('#contenedorNuevoCorreo-cco').removeClass('d-none')
        $('#separacion-linea-cco-nuevo').removeClass('d-none')
        console.log('open cco container')
    })

    //btns para cerrar cc y cco en nuevo correo
    $('#compose-nuevo-close-cc').click(function () {
        $('#contenedorNuevoCorreo-cc').addClass('d-none')
        $('#separacion-linea-cc-nuevo').addClass('d-none')
    })

    $('#compose-nuevo-close-cco').click(function () {
        $('#contenedorNuevoCorreo-cco').addClass('d-none')
        $('#separacion-linea-cco-nuevo').addClass('d-none')
    })

    $('#btn-correo-marcarnoleido').click(function () {
        FnActualizarCorreoLeidoNoLeido(currentIdMail, 1)
    })

    //const managerCarpetas = new SideNavEmailComponent();
    const managerCarpetas = new ManagerCarpetas();
    managerCarpetas.on('create', (obj) => {
        console.log(obj)
        FnAgregarCarpetaEmail(obj);
    })

    onInitCorreo();
    //ContadorCorreosNuevos();
    FnListarCorreos();
    FnListarCarpetasSideNav();
    FnMostrarEmailFrom()
})

function onInitCorreo() {
    // mostrar toolbar en seccion reply
    $('#btn-textoEditReply').on('click', function (e) {
        $('#toolbar-container').removeClass('d-none')
    })

    // mostrar toolbar en seccion nuevo correo
    $('#textoEdit-nuevoCorreo').on('click', function (e) {
        $('#toolbar-container-nuevoCorreo').removeClass('d-none')
    })

    //btn NavBar active
    var btnNavBarContainer = document.getElementById('navBar-container');
    if (btnNavBarContainer) {
        var btns = btnNavBarContainer.querySelectorAll('#navBar-container .nav-link')
        btns.forEach((btn, index) => {
            btn.addEventListener('click', function () {
                btns.forEach(btn => {
                    btn.classList.remove('active')
                })
                btn.classList.add('active')
            })
        })
    }
}

function FnMostrarEmailFrom() {
    //let datosEmailFrom = data[0]
    $.ajax({
        url: `/Bandeja/GetListarCorreosFrom`,
        type: 'GET',
        dataType: 'json',
        success: (data) => {
            let datosEmailFrom = data[0];
            $('#reply-emailFrom-name')[0].innerHTML = datosEmailFrom.nombre
            $('#reply-emailFrom')[0].innerHTML = datosEmailFrom.email
            $('#nuevo-emailFrom-name')[0].innerHTML = datosEmailFrom.nombre
            $('#nuevo-emailFrom')[0].innerHTML = datosEmailFrom.email
        },
    })
}

//Estilos marcar favorito y archivado
const DomainCorreos = {
    pintarFavorito: (isFavorite, btnFavorito) => {
        console.log({ isFavorite, btnFavorito })
        if (isFavorite == true) {
            btnFavorito.querySelector('svg').style.fill = '#ffe033';
            btnFavorito.querySelector('svg').style.stroke = '#ffe033'
        } else {
            btnFavorito.querySelector('svg').style.fill = 'none'
        }
    },

    pintarArchivados: (fArchivado, btnArchivado) => {
        console.log({ fArchivado, btnArchivado })
        if (fArchivado == true) {
            btnArchivado.querySelector('svg').style.fill = '#c5d5e6';
        } else {
            btnArchivado.querySelector('svg').style.fill = 'none'
        }
    }
},

//Filtro de busqueda
FnFiltroBusquedaCorreos = () => {
    let emailDe = $('#filtropor-de').val()
    let emailPara = $('#filtropor-para').val()
    let emailAsunto = $('#filtropor-asunto').val()
    let contienePalabra= $('#filtropor-contienepalabra').val()
    let fechaInicio = $('#dt-fechainiciobusqueda-correos').val()
    let fechaFinal = $('#dt-fechafinalbusqueda-correos').val()
    let tipoCorreo = $('#select-tipobusqueda-correos').val()

    $.ajax({
        type: "GET",
        beforeSend: function () {
            LoadButton($('#btn_buscar-filtroBusqueda'))
        },
        url: `/Bandeja/GetListadoCorreos_filtroBusqueda?paramDe=${emailDe}&paramPara=${emailPara}&paramAsunto=${emailAsunto}&paramTipo=${tipoCorreo}&fechaInicio=${fechaInicio}&fechaFinal=${fechaFinal}&texto=${contienePalabra}`,
        //data: JSON.stringify(jsonPost),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            fnPopularCorreosRec(data)
                $('#modal-filtradoBusqueda-correo').modal('hide')
        },

        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
        },
        complete: function () {
            UnLoadButton($('#btn_buscar-filtroBusqueda'))
        }
    });
}

// Listas 
function FnListarCorreos() {

    const availableRequests = {
        inbox: `/Bandeja/GetListadoCorreoBandejaPaginacion?pagina=${currentPageMail}`,
        starred: `/Bandeja/GetListadoCorreosFavoritosPaginacion?pagina=${currentPageMail}`,
        sent: `/Bandeja/GetListadoCorreosEnviadosPaginacion?pagina=${currentPageMail}`,
        draft: `/Bandeja/GetListadoCorreosBorradoresPaginacion?pagina=${currentPageMail}`,
        trash: `/Bandeja/GetListadoCorreosPapeleraPaginacion?pagina=${currentPageMail}`,
        archived: `/Bandeja/GetListadoCorreosArchivadoPaginacion?pagina=${currentPageMail}`
    }

    $('#span-numero-paginacion')[0].innerHTML = `${currentPageMail} de 30`;

    console.log(availableRequests[currentView])
    $.ajax({
        url: availableRequests[currentView],
        type: 'GET',
        dataType: 'json',
        beforeSend: () => {
            //setLoadingView('loading', $('#lista-correos'))
        },
        success: function (data) {
            fnPopularCorreosRec(data)
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
        },
        complete: function (xhr, status) {
            //setLoadingView('showing', $('#lista-correos'))
        }
    });

}

function FnListarReplies(id) {
    $.ajax({
        method: 'GET',
        url: `/Bandeja/GetListadoReplicasCorreoIntegracion?id=${id}`,
        data: JSON.stringify({}),
        beforeSend: () => { },
        success: (data) => {
            if (data) {
                data.forEach(email => {
                    FnAgregarCorreoReply(email)
                })
            }
            
        },
        complete: () => { },
        error: () => {}
    })
}

function FnAbrirCorreoDetalle(id) {

    const popularDetalleCorreo = (data) => {
        $('#listado-correos-detalle-correo .reply-identificator').remove()

        if (data.length > 0) {
            let misDatos = data[0];
            // instanciamos selectores e insertamos
            $('#name-from')[0].innerHTML = misDatos.emailFrom
            $('#asunto-correo-detalle-correo')[0].innerHTML = misDatos.asunto
            $('#contenido-correo')[0].innerHTML = misDatos.body
            $('#fecha-envio-correo')[0].innerHTML = misDatos.fechaEnvioCorreo
            //misDatos.bodyCorreoRespondido

            if (misDatos.isFavorite) {
                $('#btn-fav-detalleCorreo').css('stroke', '#ffcc00')
                $('#btn-fav-detalleCorreo').css('fill', '#ffcc00')
            } else {
                $('#btn-fav-detalleCorreo').css('stroke', 'gray')
            }

            if (misDatos.fArchivado) {
                $('#btn-archivar-detalleCorreo').css('stroke', '#214f7e')
            } else {
                $('#btn-archivar-detalleCorreo').css('fill', 'none')
            }

            $('#btn-fav-detalleCorreo').removeClass('d-none')
            $('#btn-archivar-detalleCorreo').removeClass('d-none')
            $('#btn-moverCarpeta-detalleCorreo').removeClass('d-none')
            $('#btn-restaurar-detalleCorreo').removeClass('d-none')
            $('#btn-papelera-detalleCorreo').removeClass('d-none')
            $('#btn-eliminarDefinitiva-detalleCorreo').removeClass('d-none')

            if (misDatos.fPapelera === 1) {
                $('#btn-fav-detalleCorreo').addClass('d-none')
                $('#btn-archivar-detalleCorreo').addClass('d-none')
                $('#btn-moverCarpeta-detalleCorreo').addClass('d-none')
                $('#btn-papelera-detalleCorreo').addClass('d-none')
            } else {
                $('#btn-restaurar-detalleCorreo').addClass('d-none')
                $('#btn-eliminarDefinitiva-detalleCorreo').addClass('d-none')
            }

        } else {
            console.error('correo en lista no encontrado')
        }

        FnListarReplies(id)
        FnListarComentariosCorreos()
        FnListadoOportunidadesInner('testcorreo', 'sessionid')
        FnListarEventosCorreo()
        FnListarTareasCorreo()

        $('#middle-section-detalle-correo').scrollTop(0)
    }

    const popularBorrador = (data) => {
        textEditorNuevoCorreo.setContents(JSON.parse(data[0].body));
    }

    $.ajax({
        url: `/Bandeja/GetCorreoIntegracion?id=${id}`,
        type: 'GET',
        dataType: 'json',
        //beforeSend: 
        success: function (data) {
            let idEstado = data[0].idEstado;
            if (idEstado == 4) {
                popularBorrador(data)
            } else {
                popularDetalleCorreo(data);
            }
        }
    })
}

//Enviar correo 
function enviarCorreo() {
    const validarCorreos = (handlerCorreo) => {
        let errors = [];
        handlerCorreo.getAllValues().forEach(correo => {
            if (!pruebaemail(correo)) {
                errors.push({
                    type: 'correo no validaado',
                    details: `correo ${correo} no valido`
                });
            }
        })

        return errors;
    }

    let validarEmailTo = validarCorreos(inputParaSendMail);
    //let validarCC = validarCorreos(inputParaSendMailCC);

    if (validarEmailTo.length > 0) {
        // mostrar el erros
        console.log('correos no validos')
        return;
    }

    let jsonPost = {
        idSource: '',
        idCarpeta: 0, //enviado
        idEstado: 1,
        isImportant: 0,
        emailTo: inputParaSendMail.getAllValuesFormatted(),
        emailFrom: $('#nuevo-emailFrom').text(),
        body: $('#editor-texto-nuevoCorreo .ql-editor').html(),
        emailCC: '' , //getAllValuesFormatted(),
        emailBCC: '', //getAllValuesFormatted() ,
        asunto: $('#asunto-nuevoCorreo').val(),
        linkReunion: '',
        isFavorite: 0,
        codigoOportunidad: 0,
        codigoUsuarioSender: 0,
        fEnviadoRecibido: 1,
        fLeidoNoLeido: 0,
        fEstadoSistema: 1,
        idCorreoPadre: -1,
        emailFromName: $('#nuevo-emailFrom-name').text(),
        bodyPreview: textEditorNuevoCorreo.getText().slice(0, 30),
        bodyCorreoRespondido: '',
    }
    $.ajax({
        url: `/Bandeja/SendCorreoIntegracion`,
        type: 'POST',
        dataType: 'json',
        contentType: "application/json",
        data: JSON.stringify(jsonPost),
        success: function (data) {
            console.log(data);
            if (data.response == true) {
                FnListarCorreos()
                //cerrar compose
                $('#mailCompose').removeClass('d-none')
                $('#mailCompose').removeClass('show');
            } else {
                alert('Error al intentar enviar el correo')
            }
        },
        complete: () => {
            textEditorNuevoCorreo.setContents([]);
            $('#asunto-nuevoCorreo').val('')
            inputParaSendMail.setValues('')
        }
    })
}

    //mandar Correo nuevo a Borradores
function FnEnviarCorreoABorrador(isInDetalleCorreo) {

    let idCorreoPadre_ = isInDetalleCorreo == true ? currentIdMail : -1;
    let EditorDeTexto = isInDetalleCorreo == true ? textEditorReplyCorreo : textEditorNuevoCorreo;
    let asuntoCorreo = isInDetalleCorreo == true ? $('#asunto-correo-detalle-correo').text() : $('#asunto-nuevoCorreo').val();
    //let emailFrom_ = isInDetalleCorreo == true ? $('#') : $('#');
    //let cc_ = isInDetalleCorreo == true ? $('#) : $('#');
     //let cco_ = isInDetalleCorreo == true ? $('#) : $('#');
    if (EditorDeTexto.getContents().ops[0].insert.length <= 2) {
        return;
    }

    let data = {
        idSource: '',
        idCarpeta: -1,
        idEstado: 4,//borrador
        isImportant: 0,
        emailTo: '',
        emailFrom: $('#nuevo-emailFrom').text(),
        body: $('#editor-texto-nuevoCorreo').html(),//JSON.stringify(EditorDeTexto.getContents().ops),
        emailCC: '',
        emailBCC: '',
        asunto: asuntoCorreo,
        linkReunion: '',
        isFavorite: 0,
        codigoOportunidad: 0,
        codigoUsuarioSender: 0,
        fEnviadoRecibido: 0,
        fLeidoNoLeido: 0,
        fEstadoSistema: 1,
        idCorreoPadre: idCorreoPadre_,
        emailFromName: $('#nuevo-emailFrom-name').text(),
        bodyPreview: EditorDeTexto.getText().slice(0, 30), /*metodos de manipulacion de js string para obtener los primero 30 caracteres */
        bodyCorreoRespondido: '',
    }

    if (!isInDetalleCorreo) {
        if (currentIdMail) {
            currentIdMail = null;
            return
        }
    } else {
        if (EditorDeTexto.getContents().ops[0].insert.length <= 2) {
            return;
        }
    }

    $.ajax({
        url: `/Bandeja/SendCorreoIntegracion`,
        type: 'POST',
        dataType: 'json',
        contentType: "application/json",
        data: JSON.stringify(data),
        success: function (data) {
            FnListarCorreos()
            ShowAlertMessage($('#show-alerts'), 'success', 'Información', 'Mensaje enviado a borrador.')
        },
        error: function (xhr, status) {
        },
        complete: function (xhr, status) {
        }
    });
}

    // Reply 
function ReplyCorreoIntegracion() {
    let json = {
        idSource: '',
        idCarpeta: -1,
        idEstado: 1,
        isImportant: 1,
        emailTo: '',
        emailFrom: $('#nuevo-emailFrom').text(),
        body: $('#editor-texto .ql-editor').html(),
        emailCC: '',
        emailBCC: '',
        asunto: '',
        linkReunion: '',
        isFavorite: 1,
        codigoOportunidad: 0,
        codigoUsuarioSender: 12,
        fEnviadoRecibido: 1,
        fLeidoNoLeido: 1,
        fEstadoSistema: 1,
        idCorreoPadre: curentIdReplied || -1,
        emailFromName: $('#nuevo-emailFrom-name').text(),
        bodyPreview: textEditorReplyCorreo.getText().slice(0, 30),
        bodyCorreoRespondido: $(`#reply-correo-${curentIdReplied}`).find('.body-email').html() || ''
    }
    $.ajax({
        url: `/Bandeja/ReplyCorreoIntegracion`,
        type: 'POST',
        dataType: 'json',
        contentType: "application/json",
        data: JSON.stringify(json),
        success: function (data) {
            if (data) {
                if (data.response == true) {
                    FnAgregarCorreoReply({
                        fechaEnvioCorreo: new Date().toDateString(),
                        body: json.body,
                        emailFrom: json.emailFrom,
                        id: data.idCorreo
                    })
                } else {
                    FnListarReplies(currentIdMail)
                    FnListarCorreos()
                }
                $('#middle-section-detalle-correo').scrollTop($('#listado-correos-detalle-correo').height())
                $('#container-reply').addClass('d-none');
            } else {
                console.error('Error al intentar enviar el correo')
            }
        },
        error: () => {
            FnListarReplies(currentIdMail)
        },
        complete: () => {
            textEditorReplyCorreo.setContents([])
        }
    })
}

function FnIniciarReplyCorreo(idReplied) {
    curentIdReplied = idReplied;
    $('#container-reply').removeClass('d-none');
    $('#middle-section-detalle-correo').scrollTop($('#listado-correos-detalle-correo').height())
    document.querySelector('#container-reply').querySelector('.ql-editor').focus()
}

/*Cargar Archivo en correo nuevo*/
$('#archivo-correonuevo, #imagenes-correonuevo , #adjuntarLink-nuevoCoreo ').change(function () {

    if (this.files[0] === undefined) {
        $(this).closest('.custom-file').find('label').text('Buscar archivo')
    }
    else {
        var fileSize = this.files[0].size;
        if (fileSize > 10000000) {
            ShowAlertMessage($('#container-archivos-nuevoCorreo'), 'warning', 'Advertencia', ' archivo no puede pesar mas de 10 MB.')
            this.value = '';

        } else {
            let filename = this.files[0].name
            $('#txt-archivo-correo , #imagenes-correonuevo , #adjuntarLink-nuevoCoreo').html(filename)
            FnInsertarArchivoCorreo();
        }
    }
})

//contador Correos nuevos
function ContadorCorreosNuevos() {
    $.ajax({
        url: `/Bandeja/GetContadorCorreosEnBandeja`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log(data)
            $('#contador-correos-nuevos')[0].innerText = data;
            $('#contador-correos-inbox')[0].innerText = data;
        }
    })
}

//anadir a favoritos desde la lista
function anadirFavoritoCorreo(btnFavorito, currentId) {
    //currentIdMail
    let isFavorite;
    let currentData = informacionCorreos.find(i => i.id === currentId);
    if (currentData) {
        isFavorite = currentData.isFavorite;
    } else {
        console.warn('error en 536 temporal.js')
        return;
    }
    fetch('/Bandeja/PostActualizarCorreoFavorito', {
        method: 'POST',
        body: JSON.stringify({ id: currentId, isFavorite: !isFavorite }),
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => res.json())
        .then(data => {
            if (data.response == true) {
                FnListarCorreos()
                //DomainCorreos.pintarFavorito(!isFavorite, btnFavorito)
            }
        })
        .catch(err => {
            console.log(err)
        })
}

//anadir archivado desde la lista
function ActualizarArchivadosCorreo(btnArchivado, currentId) {
    let fArchivado;
    let currentData = informacionCorreos.find(i => i.id === currentId)
    if (currentData) {
        fArchivado = currentData.fArchivado;
    } else {
        console.warn('error en 568 temporal.js')
        return;
    }
    fetch('/Bandeja/PostActualizarCorreoArchivado', {
        method: 'POST',
        body: JSON.stringify({ id: currentId, fArchivado: !fArchivado }),
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => res.json())
        .then(data => {
            if (data.response == true) {
                FnListarCorreos()
                // si estoy listando filtrados llamae de nuevo el lista del filtro 
                // sin no has un listado de currentView
            }
        })
        .catch(err => {
            console.log(err)
        })
}

function FnActualizarCorreoLeidoNoLeido(id, marcarcomonoleido = 0) {

    let json = {
        id: id,
        marcarcomonoleido
    }

    $.ajax({
        method: 'post',
        dataType: 'json',
        contentType: 'application/json',
        url: `/Bandeja/PostActualizarCorreoLeido`,
        data: JSON.stringify(json),
        beforeSend: () => {

        },
        success: (data) => {
            if (data.response == true) {
                let fLeidoNoLeido = data.fLeidoNoLeido;
                if (fLeidoNoLeido == 1) {
                    $(`#lista-correos-${id}`).addClass('readed');
                } else {
                    $(`#lista-correos-${id}`).removeClass('readed');
                }
            }
        },
        error: () => {

        },
        complete: () => {

        }
    })

}

//Correo Papelera desde la lista
function ActualizarCorreoPapelera(btnEliminar, currentId) {
    let fPapelera;
    let currentData = informacionCorreos.find(i => i.id === currentId);
    if (currentData) {
        fPapelera = currentData.fPapelera;
    } else {
        console.warn('error en 625 temporal.js')
        return;
    }
    fetch('/Bandeja/PostActualizarCorreoPapelera', {
        method: 'POST',
        body: JSON.stringify({ id: currentId, fPapelera: !fPapelera }),
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => res.json())
        .then(data => {
            if (data.response === true) {
                FnListarCorreos()
            } else {
                console.log('Ocurrio un error al eliminar el correo')
            }
        })
        .catch(err => {
            console.log(err)
        })
}

//Eliminar correo definitivamente (desde lista)
function FnEliminarCorreoDefinitivamente(id) {
    $.ajax({
        url: `/Bandeja/EliminarCorreoIntegracion`,
        type: 'POST',
        dataType: 'json',
        contentType: "application/json",
        data: JSON.stringify({id}),
        success: function (data) {
            FnListarCorreos()
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema al eliminar el correo');
        },
        complete: function (xhr, status) {

        }
    });
}

//Restaurar Correo (desde lista)
function FnRestaurarCorreoEliminado(btnRestaurarCorreo, currentId) {
   // let btnRestaurarCorreo 
    let currentData = informacionCorreos.find(i => i.id === currentId);
    if (currentData) {
        fPapelera = currentData.fPapelera;
    } else {
        console.warn('error en 969 temporal.js')
        return;
    }
    fetch('/Bandeja/PostActualizarCorreoPapelera', {
        method: 'POST',
        body: JSON.stringify({ id: currentId, fPapelera: !fPapelera }),
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => res.json())
        .then(data => {
            if (data.response === true) {
                FnListarCorreos()
            } else {
                console.log('Ocurrio un error al restaurar el correo')
            }
        })
        .catch(err => {
            console.log(err)
        })
}

//Eliminar correo definitivamente desde el detalle
function FnEliminarCorreoDefDetalleCorreo(id) {
    $.ajax({
        url: `/Bandeja/EliminarCorreoIntegracion`,
        type: 'POST',
        dataType: 'json',
        contentType: "application/json",
        data: JSON.stringify({id}),
        success: function (data) {
            FnListarCorreos()
            FnComebackMailContainer()
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema al eliminar el correo');
        },
        complete: function (xhr, status) {

        }
    });
}

//Restaurar correo desde el detalle
function RestaurarCorreoDetalleCorreo() {
    let currentId = currentIdMail;
    let fPapelera;
    let currentData = informacionCorreos.find(i => i.id === currentId);
    if (currentData) {
        fPapelera = currentData.fPapelera;
    } else {
        console.warn('error en 625 temporal.js')
        return;
    }
    fetch('/Bandeja/PostActualizarCorreoPapelera', {
        method: 'POST',
        body: JSON.stringify({ id: currentId, fPapelera: !fPapelera }),
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => res.json())
        .then(data => {
            if (data.response === true) {
                FnComebackMailContainer()
                FnListarCorreos()
            } else {
                console.log('Ocurrio un error')
            }
        })
        .catch(err => {
            console.log(err)
        })
}

//Btn Favorito en detalle correo
function AgregarFavoritoDetalleCorreo() {
    let btnFavorito = document.getElementById('btn-fav-detalleCorreo');
    let currentId = currentIdMail;
    let isFavorite;
    let currentData = informacionCorreos.find(i => i.id === currentId);
    if (currentData) {
        isFavorite = currentData.isFavorite;
    } else {
        console.warn('error en 661 temporal.js')
        return;
    }
    fetch('/Bandeja/PostActualizarCorreoFavorito', {
        method: 'POST',
        body: JSON.stringify({ id: currentId, isFavorite: !isFavorite }),
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => res.json())
        .then(data => {
            currentData.isFavorite = !isFavorite;
            if (data.response == true) {
                if (!isFavorite == true) {
                    btnFavorito.querySelector('svg').style.stroke = '#ffcc00';
                    btnFavorito.querySelector('svg').style.fill = '#ffcc00';
                } else {
                    btnFavorito.querySelector('svg').style.stroke = '#214f7e';
                    btnFavorito.querySelector('svg').style.fill = 'none';
                }
            }
        })
        .catch(err => {
            console.log(err)
        })
}

//Btn Archivado en detalle correo
function AgregarArchivadoDetalleCorreo() {
    let btnArchivado = document.getElementById('btn-archivar-detalleCorreo');
    let currentId = currentIdMail;
    let fArchivado;
    let currentData = informacionCorreos.find(i => i.id === currentId);
    if (currentData) {
        fArchivado = currentData.isFavorite;
    } else {
        console.warn('error en 704 temporal.js')
        return;
    }
    fetch('/Bandeja/PostActualizarCorreoArchivado', {
        method: 'POST',
        body: JSON.stringify({ id: currentId, fArchivado: !fArchivado }),
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => res.json())
        .then(data => {
            currentData.fArchivado = !fArchivado;
            if (data.response == true) {
                if (!fArchivado == true) {
                    btnArchivado.querySelector('svg').style.stroke = '#31b474';
                } else {
                    btnArchivado.querySelector('svg').style.stroke = '#1b2e4b';
                }
            }
        })
        .catch(err => {
            console.log(err)
        })
}

//Btn papelera en detalle correo
function ActualizarCorreoPapeleraDetalleCorreo() {
    let currentId = currentIdMail;
    let fPapelera;
    let currentData = informacionCorreos.find(i => i.id === currentId);
    if (currentData) {
        fPapelera = currentData.fPapelera;
    } else {
        console.warn('error en 625 temporal.js')
        return;
    }
    fetch('/Bandeja/PostActualizarCorreoPapelera', {
        method: 'POST',
        body: JSON.stringify({ id: currentId, fPapelera: !fPapelera }),
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(res => res.json())
        .then(data => {
            if (data.response === true) {
                FnComebackMailContainer()
                FnListarCorreos()
            } else {
                console.log('Ocurrio un error')
            }
        })
        .catch(err => {
            console.log(err)
        })
}

/* ------------------SECCION COMENTARIOS/CRM/ACTIVIDADES/TAREAS------------------ */

// comentarios 
function FnListarComentariosCorreos() {
    $.ajax({
        url: `/Bandeja/GetListadoComentarios?idCorreo=${currentIdMail}`,
        type: 'GET',
        dataType: 'json',
        beforeSend: () => {
            // pantalla de carga 
            setLoadingView('loading', $('#contenedor-comentario'))
            $('#contenedor-comentario').empty()
        },
        success: function (data) {
            console.log(data)
            if (data.length > 0) {
                //$(data).each(itemx => { })
                data.forEach(item => {
                    let ul = document.createElement('li')
                    ul.id = `contenedor-comentario-${item.id}`
                    ul.classList.add('card-body','p-1');
                    ul.innerHTML = `
                            <li class="py-1 px-2" >
                                <div class="media">
                                    <div class="media-body">
                                        <div class="media align-items-center mb-1">
                                            <div class="media-body">
                                                <div class = "d-flex" style = "gap: .3rem">
                                                    <h6 class="mb-0">${item['nombreUsuario']}</h6>
                                                    <small class="text-muted">${item['fechaCreacion']}</small>
                                                </div>
                                            </div>
                                            <div class="dropdown">
                                                <button class="btn btn-white btn-xs px-2 border-0" data-toggle="dropdown" aria-expanded="false">
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none"                      stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"                              class="feather feather-more-vertical"><circle cx="12" cy="12" r="1"/><circle cx="12" cy="5"                        r="1"/><circle cx="12" cy="19" r="1"/></svg>
                                                </button>
                                                <div class="dropdown-menu dropdown-menu-right">
                                                    <a href="javascript:void(0)" class="dropdown-item btn-eliminar-cometario">Eliminar</a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="p-2 border rounded">
                                    <p class="m-0 text-muted tx-14">${item['contenido']}</p>
                                </div>
                            </li>
                                            `;
                    // append al template
                    $('#contenedor-comentario').append(ul);
                    let btnEliminar = ul.querySelector('.btn-eliminar-cometario')
                    if (btnEliminar != null) {
                        btnEliminar.addEventListener('click', () => { FnEliminarComentarios(item['id'])})/* thunk functions */
                    }
                })
            } else {
                // colocar pantalla de no hay comentarios
                $('#contenedor-comentario').append(`<div class ="d-flex justify-content-center align-items-center" style="height: 230px; text-align: center; font-size: 13px;">
                        <div style = "">No hay comentarios mostrar</div>
                    </div>`);
            }
        },

        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
        },
        complete: function (xhr, status) {
            console.log('Listado comentarios exitoso')
        }
    });
}

function CrearComentario() {
    let comentario = $('#textarea-comentario').val()
    $.ajax({
        url: '/Bandeja/PostCrearComentario',
        type: 'POST',
        dataType: 'json',
        contentType: "application/json",
        data: JSON.stringify({contenido: comentario, idCorreo: currentIdMail}),
        beforeSend: function () {
            LoadButton($('#btn-guardar-comentarios'))
        },
        success: function (data) {
                FnListarComentariosCorreos()
        },
         complete: function () {
             UnLoadButton($('#btn-guardar-comentarios'))
        }
    })

}

function FnEliminarComentarios(idCorreo) {
    $.ajax({
        url: `/Bandeja/PostEliminarComentario`,
        type: 'POST',
        dataType: 'json',
        contentType: "application/json",
        data: JSON.stringify({id: idCorreo}),
        success: function (data) {
            FnListarComentariosCorreos()
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
        },
        complete: function (xhr, status) {

        }
    });
}

// oportunidades
const FnGenerarOportunidadContactoCorreo = function () {

    let sessionId = 'sessionid';
    let nombreSession = '';
    let email = '';
    let canalEntrada = 7;
    let idUsuarioAsignado = parseInt($('#cbo-usuarioasignadocrm-correo').val())
    let comentarios = $('#txt-comentariocrm-correos').val()
    let codBot = 'testcorreo';
    let etapaVenta = parseInt($('#cbo-etapaventaoportunidad-correos').val())
    let telefono = ''
    let celular = ''

    $.ajax({
        type: "POST",
        url: "/CRM/PostGenerarOportunidadPorChats",
        beforeSend: function () {
            LoadButton($('#btn-generaroportunidadcrm-correos')) //pop up btn
            LoadButton($('#btn-crearoportunidadcontacto-correo'))  //svg btn
        },
        data: JSON.stringify({
            sessionId: sessionId, nombreCliente: nombreSession, emailCliente: email, idCanalEntrada: canalEntrada, idUsuarioAsignado: idUsuarioAsignado, comentarios: comentarios, codigoBot:codBot , idEtapaVenta: etapaVenta, telefono: telefono, celular: celular,
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function () {
            FnListadoOportunidadesInner('testcorreo', 'sessionid')
            $('#modal-creacionoportunidadcliente-correos').modal('toggle')
            ShowAlertMessage($('#seccion-crm-correo'), 'success', 'Información', 'La oportunidad se creó correctamente.')
        },
        failure: function () {
            ShowAlertMessage($('#container-generacionoportunidadescrm-correos'), 'danger', 'Error', 'Hubo un error generando la oportunidad.')
        },
        error: function () {
            ShowAlertMessage($('#container-generacionoportunidadescrm-correos'), 'danger', 'Error', 'Hubo un error generando la oportunidad.')
        },
        complete: function () {
            UnLoadButton($('#btn-generaroportunidadcrm-correos')) 
            UnLoadButton($('#btn-crearoportunidadcontacto-correo')) 
        }
    });
}

const FnListadoOportunidadesInner = function (codigoBot, sessionId) {
        $.ajax({
            type: "POST",
            beforeSend: function () {
                $('#panel-cargandoregistrooportunidades-correo').css('display', 'flex')
            },
            url: "/CRM/GetListadoOportunidadesPorChatCanales",
            data: JSON.stringify({ "codigoBot": codigoBot, "sessionId": sessionId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (typeof data === 'string') data = JSON.parse(data)

                let contenedor = $('#correo-container-oportunidades')
                contenedor.empty()

                if (data.length === 0) {
                    contenedor.append(`<div style="height: 230px; text-align: center; font-size: 13px;">
                                <div>No hay oportunidades a mostrar</div>
                            </div>`)
                    return
                }

                $(data).each(function (index, item) {
                    contenedor.append(`<div class="cht-item-oportunidad">
                           <div class="cht-item-oportunidad-header">
                                <div class="cht-item-oportunidad-header-leftside">
                                    <div class="cht-item-oportunidad__title">${item.nombreOportunidad}</div>
                                </div>
                                <div class="cht-item-oportunidad-header-rightside">
                                    <div class="chat-item-oportunidad__etapa">${item.descripcionEtapa}</div>
                                </div>
                            </div>
                            <div class="cht-item-oportunidad-body">
                                <div class="cht-item-oportunidad-chat-container">
                                    <p class="cht-item-oportunidad-chat__text tx-12 ">${item.descripcion}</p>
                                </div>
                            </div>
                            <div class="py-1 border-bottom d-flex justify-content-between">
                                <div class="cht-item-oportunidad-footer-left-side">
                                    <div class="cht-item-oportunidad-tags-group">
                                        <div class="d-flex justify-content-between">
                                            <div class="cht-item-oportunidad-tag-label tx-12">Estado:</div>
                                            <div class="ml-2 cht-item-oportunidad-tag-value">
                                                <div class="tx-12 ">${item.estadoOportunidad}</div>
                                            </div>
                                        </div>
                                        <div class="d-flex justify-content-between">
                                            <div class="cht-item-oportunidad-tag-label tx-12">Monto:</div>
                                            <div class="ml-2 cht-item-oportunidad-tag-value">
                                                <div class="tx-12">${item.simbolo} ${item.montoVenta}</div>
                                            </div>
                                        </div>
                                        <div class="d-flex justify-content-between">
                                            <div class="cht-item-oportunidad-tag-label tx-12">Asignado:</div>
                                            <div class="ml-2 cht-item-oportunidad-tag-value">
                                                <div class="chat-item-oportunidad__tag tx-12">${item.usuarioAsignado}</div>
                                            </div>
                                        </div>
                                        </div>
                                    </div>
                                </div>
                            </div>`)
                })
            },
            failure: function () {
                ShowAlertMessage($('#seccion-crm-correo'), 'danger', 'Error', 'Hubo un error listando las oportunidades de venta.');
            },
            error: function () {
                ShowAlertMessage($('#seccion-crm-correo'), 'danger', 'Error', 'Hubo un error listando las oportunidades de venta.');
            },
            complete: function () {
                $('#panel-cargandoregistrooportunidades-correo').css('display', 'none')
            }
        });
}

// calendario
function FnListarEventosCorreo() {
     $.ajax({
         url: `/Users/GetListadoEventosCalendario`,
         type: 'GET',
         dataType: 'json',
         beforeSend: () => {
             // pantalla de carga 
             setLoadingView('loading', $('#container-eventos-correos'))
             $('#container-eventos-correos').empty()
         },
         success: function (data) {
             console.log(data)
             if (data.length > 0) {
                 data.forEach(item => {
                     let div = document.createElement('div')
                     div.id = `container-eventos-correos-${item.Id}`
                     //div.classList.add('card');
                     div.innerHTML = `
                            <div class="py-2 d-flex flex-column">
                              <div class="d-flex justify-content-between">
                                <div class ="d-flex align-items-center" style = "gap: .5rem">
                                  <div class="p-1 justify-content-center align-items-center d-flex border rounded bg-light">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="17" height="17" viewBox="0 0 24 24" fill="none"
                                      stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
                                      class="feather feather-user">
                                      <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
                                      <circle cx="12" cy="7" r="4"></circle>
                                    </svg>
                                    <small>${item.UsuarioAsginado}</small>
                                  </div>
                                  <small class="cht-event-item__datetime text-muted">
                                    ${item.FechaEvento} ${item.HoraEvento}
                                  </small>
                                </div>
                                <div>
                                  <div class="dropdown">
                                    <button class="btn btn-white btn-xs px-2 border-0" data-toggle="dropdown" aria-expanded="false">
                                       <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none"                      stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"                              class="feather feather-more-vertical"><circle cx="12" cy="12" r="1"/><circle cx="12" cy="5"                        r="1"/><circle cx="12" cy="19" r="1"/></svg>
                                    </button>
                                    <div class="dropdown-menu" aria-labelledby="dropdownMenuButton">
                                      <a class="dropdown-item text-danger btn-delete-evento" href="javascript: Void(0)">Eliminar</a>
                                    </div>
                                  </div>
                                </div>
                              </div>
                              <div class = "mt-1">
                                <div class="cht-event-item-text-container">
                                  <p class="cht-event-item__text tx-12 m-0 ">
                                    ${item.Descripcion}
                                  </p>
                                </div>
                              </div>
                            </div>
                         `;
                     // append al template
                     $('#container-eventos-correos').append(div);
                     let btnEliminarEvento = div.querySelector('.btn-delete-evento')
                     if (btnEliminarEvento != null) {
                         btnEliminarEvento.addEventListener('click', () => { FnEliminarEventosCorreo(item['Id']) })
                     }
                 })
             } else {
                 $('#container-eventos-correos').append(div);
                 `<div style="height: 230px; text-align: center; font-size: 13px;">
                        <div>No hay eventos a mostrar</div>
                    </div>`
             }
         },

         error: function (xhr, status) {
             console.log('Disculpe, existió un problema');
         },
         complete: function (xhr, status) {
             console.log('Listado de eventos exitoso')
         }
     });
 }

function FnGenerarEventosCorreo() {
    let fechaEvento_ = moment($('#input-fecha-evento').val(), "DD/MM/YYYY").format('YYYY/MM/DD');
    let data = {
        fechaEvento: fechaEvento_,
        horaEvento: $('#input-time-create-event-correo-tarea').val(),
        descripcion: $('#input-create-event-correo-description').val(),
        idUsuarioAsignado: parseInt($('#cbo-create-event-correo-usuario-asignado').val()),
        notas: $('#input-create-event-correo-notas').val(),
        referencia: '',
        mostrarCalendario: '1',
        estadoCompletado: 0,
        estado: 0,
        tipo: 2,

        usuarioAsignado: $('#cbo-create-event-correo-usuario-asignado')[0].options[$('#cbo-create-event-correo-usuario-asignado')[0].selectedIndex].text,
        idSesion: 'none',
        codigoBot: 'none'
    }

    LoadButton($('#btn-create-event-correo-guardar'))

    if (fechaEvento_ == 'Invalid date') {
        // colocar la fecha en el formato indicado 
        return;
    }

    $.ajax({
        url: `/Users/PostCreareventoCalendario`,
        type: 'POST',
        dataType: 'json',
        contentType: "application/json",
        data: JSON.stringify(data),
        success: function (data) {
            FnListarComentariosCorreos()
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
        },
        complete: function (xhr, status) {
            UnLoadButton($('#btn-create-event-correo-guardar'))
        }
    });
}

function FnEliminarEventosCorreo(idEvento) {
    $.ajax({
        url: `/Users/PostEliminarEvento`,
        type: 'POST',
        dataType: 'json',
        contentType: "application/json",
        data: JSON.stringify({ id: idEvento}),

        success: function (data) {
            FnListarEventosCorreo()
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
        },
        complete: function (xhr, status) {

        }
    });
}

// tareas
const FnGenerarTareaCorreos = function () {
    let jsonPost = {
        descripcion: $('#input-tab-correo-creartarea').val(),
        referencia : '',
        idUsuarioAsignado : 0,
        idTipoTarea :1,
        notas : '',
        aviso : 0,
        momentoAvisoRecordatorio: 'horadelevento',
        fechaTarea: moment().format('YYYY/MM/DD'),//'2021/12/12',
        horaTarea: moment().format('LT'),
        bucarOportunidad : '',
        buscarCliente :'',
        mostrarCalendario : 1,
        codigoBot : '',
        sessionId : '',
        idOportunidad : 0,
        idCliente : 0,
        nombreOportunidad : '',
        nombreCliente : '',
        id : 0
    }
    $.ajax({
        type: "POST",
        url: `/Users/PostGuardarTarea`,
        beforeSend: function () {

        },
        data: JSON.stringify(jsonPost), 
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function () {
            FnListarTareasCorreo()
        },
        failure: function () {

        },
        error: function () {
            console.error('no se inserrto el mensaje')
        },
        complete: function () {
        }
    });
}

function FnListarTareasCorreo() {
    $.ajax({
         url: `/Users/GetListadoTareas`,
         type: 'GET',
         dataType: 'json',
         beforeSend: () => {
             // pantalla de carga
             //setLoadingView('loading', $('#correo-container-tareas'))
             $('#correo-container-tareas').empty()
         },
         success: function (data) {
             
             if (data.length > 0) {
                 data.forEach(item => {
                     let ischecked = '';//idEstadoCompletado === 0 ? '' : 'checked';
                     if (ischecked) {
                         div.classList.add(ischecked);
                     }
                     let div = document.createElement('div')
                     div.classList.add('mb-2')
                     div.id = `correo-container-tareas-${item.Id}`
                     div.innerHTML = `
                          <div class="px-2 border rounded py-1 bg-light d-flex justify-content-between" data-id = ${item.Id}>  
                                    <div class="me-3 ">
                                        <div class="cht-item-tarea__text">
                                            ${item.Descripcion}
                                        </div>
                                    </div>
                                    <div class="dropdown bg-light">
                                             <button class="btn btn-light btn-xs px-2 border-0" data-toggle="dropdown" aria-expanded="false">
                                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-more-vertical"><circle cx="12" cy="12" r="1"/><circle cx="12" cy="5" r="1"/><circle cx="12" cy="19" r="1"/></svg>
                                    </button>
                                        <div class="dropdown-menu dropdown-menu-right">
                                            <a class="dropdown-item text-danger btn-delete-task" href="javascript: Void(0)">Eliminar</a>
                                        </div>
                                    </div>
                                 </div>
                          </div>
                            `;
                     // append al template
                     $('#correo-container-tareas').append(div);
                     let btnEliminarTarea = div.querySelector('.btn-delete-task')
                     if (btnEliminarTarea != null) {
                         btnEliminarTarea.addEventListener('click', () => { FnEliminarTareaCorreo(item['Id']) })
                     }
                 })
             } else {
                 $('#correo-container-tareas').append(`<div style="height: 230px; text-align: center; font-size: 13px;">
                        <div>No hay tareas a mostrar</div>
                    </div>`);
             }
         },

         error: function (xhr, status) {
             console.log('Disculpe, existió un problema');
         },
         complete: function (xhr, status) {
             console.log('Listado de tareas exitoso')
         }
     });
}

function FnEliminarTareaCorreo(idTarea) {
    $.ajax({
        url: `/Users/PostEliminarTarea`,
        type: 'POST',
        dataType: 'json',
        contentType: "application/json",
        data: JSON.stringify({ idTarea }),

        success: function (data) {
            FnListarTareasCorreo()
        },
        error: function (xhr, status) {
            console.log('Disculpe, existió un problema');
        },
        complete: function (xhr, status) {

        }
    });
}

// utilidades de correo
function fnPopularCorreosRec(data) {

    informacionCorreos = data;

    $('#lista-correos').empty()
    data.forEach(item => {
        informacionCorreo = item;

        
        let templateNuevoFlag = '';
        if (item.fLeidoNoLeido === true) {
            templateNuevoFlag = `<span class="badge badge-success mr-3">Nuevo</span>`
        }

        let accionPapelera, accionFavorito, accionArchivado, accionRestaurar, accionPapeleraDefinitiva;

        accionPapelera = `
                <a class="btn-eliminar px-1" href="javascript:void(0)" data-toggle="tooltip" title="Papelera">
                      <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash-2 wd-20"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg>
                </a> `;

        if (item.fPapelera  === 1) {//papelera
            accionRestaurar = `
                   <a href="javascript:void(0)" class="px-1 btn-restaurar" data-toggle="tooltip" title="Restaurar">
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-inbox"><polyline points="22 12 16 12 14 15 10 15 8 12 2 12"/><path d="M5.45 5.11L2 12v6a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-6l-3.45-6.89A2 2 0 0 0 16.76 4H7.24a2 2 0 0 0-1.79 1.11z"/></svg>
                   </a>`;
            accionPapeleraDefinitiva = `<a class="btn-eliminar-definitivo px-1" href="javascript:void(0)" data-toggle="tooltip" title="Eliminar">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash-2 wd-20"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg>
            </a> `;
            accionPapelera = '';
            accionFavorito = ``;
            accionArchivado = ``;
        }
        else if (item.idEstado === 4) {
            accionRestaurar = ``;
            accionPapeleraDefinitiva = `<a class="btn-eliminar-definitivo px-1" href="javascript:void(0)" data-toggle="tooltip" title="Eliminar">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash-2 wd-20"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg>
            </a> `;
            accionArchivado = ``;
            accionFavorito = ``;
            accionPapelera = '';
        }
        else {
            accionPapeleraDefinitiva = '';
            accionRestaurar = ``;
            accionFavorito = `
            <a class="btn-favoritos px-1" href="javascript:void(0)" data-toggle="tooltip event-star" title="Favorito">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="svg-fav active feather feather-star wd-20"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon></svg>
            </a>`;
            accionArchivado = `
            <a class="btn-archivar px-1" href="javascript:void(0)"  data-toggle="tooltip" title="Archivar">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-archive wd-20"><polyline points="21 8 21 21 3 21 3 8"></polyline><rect x="1" y="3" width="22" height="5"></rect><line x1="10" y1="12" x2="14" y2="12"></line></svg>
            </a>`;
        } 
        let div = document.createElement('div')

        if (item.fLeidoNoLeido > 0) {
            div.classList.add('readed')
        }

        div.classList.add('correo-row-integracion');
        div.id = `lista-correos-${item.id}`
        //div.classList.add('');
        div.innerHTML = `
        <li class="plz-mail-item --unread list-group-item event-open" >
            <div class="row">
                <div class="col-md-10">
                    <div class="d-flex align-items-center flex-nowrap text-nowrap">
                        ${templateNuevoFlag}
                        <h6 class="plz-mail-item-title flex-shrink-0 mb-0 mr-2">${item['asunto']}</h6>
                    </div>
                </div>
                <div class="col-md text-md-right">
                    <span class="text-muted text-uppercase small">${item['fechaRecepcion']}</span>
                    
                </div>
            </div>
            <div class = "row">
                <div class = "col-md-10">
                    <p class="text-truncate text-muted mb-0">${item['bodyPreview']}</p>
                </div>
                <div class = "col-md">
                    <nav class="nav flex-nowrap plz-mail-item-actions">
                        ${accionFavorito}
                        ${accionArchivado}
                        <a class="btn-programar d-none px-1" href="javascript:void(0)" data-toggle="tooltip" title="Reloj">
                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-clock wd-20"><circle cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 16 14"></polyline></svg>
                        </a>
                        ${accionRestaurar}
                        ${accionPapelera}
                        ${accionPapeleraDefinitiva}                                   
                    </nav>
                </div>
            </div>
        </li>
         `;
        // append al template
        $('#lista-correos').append(div);

        let btnFavorito = div.querySelector('.btn-favoritos');

        if (btnFavorito) {
            btnFavorito.addEventListener('click', (e) => {
                anadirFavoritoCorreo(btnFavorito, item.id)
                e.stopPropagation();
            })
            if (item.isFavorite == true) {
                btnFavorito.querySelector('svg').style.fill = '#ffcc00';
                btnFavorito.querySelector('svg').style.stroke = '#ffcc00'
            } else {
                btnFavorito.querySelector('svg').style.fill = 'none'
            }
        }

        let btnArchivado = div.querySelector('.btn-archivar');

        if (btnArchivado) {
            btnArchivado.addEventListener('click', (e) => {
                console.log('Archivado')
                ActualizarArchivadosCorreo(btnArchivado, item.id)
                e.stopPropagation();
            })

            if (item.fArchivado == true) {
                btnArchivado.querySelector('svg').style.stroke = '#31b474'
            } else {
                btnArchivado.querySelector('svg').style.stroke = '#1b2e4b'
            }
        }

        let btnEliminar = div.querySelector('.btn-eliminar');

        if (btnEliminar) {
            btnEliminar.addEventListener('click', (e) => {
                $('#modal-correo-integracion-enviar-papelera').modal('show');
                currentPayloadActionItemMail = {
                    id: item.id,
                    btnEliminar: btnEliminar
                }
                e.stopPropagation();
            })
        }
        let btnRestaurarCorreo = div.querySelector('.btn-restaurar');

        if (btnRestaurarCorreo) {
            btnRestaurarCorreo.addEventListener('click', (e) => {
                console.log('Restaurado')
                FnRestaurarCorreoEliminado(btnRestaurarCorreo, item.id)
                e.stopPropagation();
            })
        }

        let btnEliminarDefinitivo = div.querySelector('.btn-eliminar-definitivo');

        if (btnEliminarDefinitivo) {
            btnEliminarDefinitivo.addEventListener('click', (e) => {
                console.log('Eliminado definitivo')
                FnEliminarCorreoDefinitivamente(item.id)
                e.stopPropagation();
            })
        }

        let eventOpen = div.querySelector('.event-open')// event

        // validamos si el nodeo existe
        if (eventOpen) {
            eventOpen.onclick = (e) => {

                FnActualizarCorreoLeidoNoLeido(item.id)

                currentIdMail = item.id
                if (item.idEstado === 4) {
                    // abrir compose  y shrink
                    $('#mailComposeBtn').click()
                    $('#mailComposeShrink').click()
                    FnAbrirCorreoDetalle(item.id)
                    
                } else {
                    //fIsInDetalleCorreo = true;
                    FnOpenMainSectionMail();
                    FnAbrirCorreoDetalle(item.id)
                } 
            }
        } else {
            console.error('no reconocí el nodo de clase eventOpen')
        }

    })
}

function FnAgregarCorreoReply(email) {
    $('#listado-correos-detalle-correo').append(`
    <li class= "plz-mail-replies-item reply-identificator p-3" data-id = "reply-correo-${email.id}" >
        <div class="media align-items-center mb-3">
            <div class="media-body">
                <h6 id="name-from" class="mb-0">${email.emailFrom}</h6>
            </div>
            <span  class="text-muted small text-uppercase">${email.fechaEnvioCorreo}</span>
        </div>
        <div class="mb-0 body-email">
            ${email.body}
        </div>
        <footer class="py-2 d-flex justify-content-between align-items-center border-0">
            <div class="d-flex flex-wrap">
                <button onclick = "FnIniciarReplyCorreo(${email.id})" class="btn btn-outline-primary btn-sm mr-1 mb-1"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-corner-up-left mr-1"><polyline points="9 14 4 9 9 4"></polyline><path d="M20 20v-7a4 4 0 0 0-4-4H4"></path></svg>Reply</button>
                <button class="btn btn-white btn-sm mr-1 mb-1"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-arrow-right mr-1"><line x1="5" y1="12" x2="19" y2="12"></line><polyline points="12 5 19 12 12 19"></polyline></svg>Forward</button>
                <span class="d-none d-md-block mx-md-3"></span>
            </div>
            <div class="dropdown">
                <button type="button" class="btn btn-white btn-sm px-2 mr-1" data-toggle="dropdown">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-more-vertical"><circle cx="12" cy="12" r="1"></circle><circle cx="12" cy="5" r="1"></circle><circle cx="12" cy="19" r="1"></circle></svg>
                </button>
                <div class="dropdown-menu dropdown-menu-right">
                    <a href="javascript:void(0)" class="dropdown-item">Test</a>
                    <a href="javascript:void(0)" class="dropdown-item">Opcion dos</a>
                    <a href="javascript:void(0)" class="dropdown-item">Opcion tres</a>
                </div>
            </div>
        </footer>
    </li>
                    
    `)
}

//cambiar vista al seleccionar un correo
function FnOpenMainSectionMail() {
    $('#contenedor-correos').addClass('d-none');
    $('#detalleCorreo').removeClass('d-none')
    $('#container-reply').addClass('d-none')
}

function FnComebackMailContainer() {
    $('#contenedor-correos').removeClass('d-none');
    $('#detalleCorreo').addClass('d-none')
    currentIdMail = null;
}

// Manager 

function FnListarCarpetasSideNav() {
    let container = $(`#container-carpetas-section-1-email`);

    let body = {
        idUsuario: 1
    }

    let body_ = new URLSearchParams({
        'idUsuario': 1,
    })

    let idUsuario = 1;

    $.ajax({
        method: 'GET',
        url: `/Bandeja/GetListadoCarpetasCorreoIntegracion?idUsuario=${idUsuario}`,
        beforeSend: () => {

        },
        success: (data) => {
            data.forEach(itemData => {
                let div = document.createElement('div')
                div.dataset.id = itemData.id;
                div.classList.add('item-carpeta-eleccion-section-3')
                div.addEventListener('click', function () {
                    console.log('hola')
                })
                div.innerHTML = itemData.nombre;
                container.append(div)
            })
        },
        error: () => {
            console.log('error')
        },
        complete: () => {

        }
    })
}

function FnAgregarCarpetaEmail(obj) {

}

function FnAsociarCarpetaEmail() {

}

function FnEliminarCarpeta() {

}

// manager carpetas
class ManagerCarpetas {
    isActive = false;
    calllbacks = [];
    currentSelectorAppendActive; // referencia del componente por crear
    constructor() {
        this.setControllers();
    }
    buildActiveCreator() {
        let a = document.createElement('a')
        a.classList.add('nav-link');
        a.classList.add('nav-link-item-carpeta-a');
        a.id = 'a-active-create-folder'
        a.href = "javascript: void(0);";
        a.innerHTML = `
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-folder"><path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z"></path></svg>
            <span class ="name-carpeta-section-1" contenteditable = "true" tabindex = "1"></span>
        `
        a.onclick = () => {

        }

        let isCreated = false;
        a.querySelector('.name-carpeta-section-1').addEventListener('keydown', (e) => {

            if (e.code === 'Enter') {
                e.preventDefault();
                if (a.innerText.trim().length === 0) {
                    a.remove()
                    this.killActive();
                    $(`#btn-email-add-carpeta`).enableSelection();
                    accionarHintSectionInbox('advice', 'no se creo la carpeta');
                    return
                }
                if (!isCreated) {
                    a.id = ''
                    this.activateTrigger('create', { name: a.innerText })
                    a.querySelector('.name-carpeta-section-1').contentEditable = 'false';
                    isCreated = true;
                    this.killActive();
                } else {
                    a.querySelector('.name-carpeta-section-1').contentEditable = 'false';
                    return;
                }
            }
            else if (e.code == "Escape") {
                a.remove()
                this.killActive();
            }
        })

        return a;
    }
    onclick(e) {
        if (this.isActive) {
            console.log('entrando a condicional mprincipal')
            if (!(e.target.closest(`#${this.currentSelectorAppendActive.id}`) || e.target.id == this.currentSelectorAppendActive.id)) {
                this.currentSelectorAppendActive.remove();
                this.killActive();
                console.log('kill it');
            }
        }
    }
    appendActionAdd() {
        const onc = (e) => {
            this.onclick(e);
        }
        document.addEventListener('mousedown', onc)
        this.isActive = true;
        let component = this.buildActiveCreator();
        this.currentSelectorAppendActive = component;
        $(`#container-carpetas-section-1-email`).append(component);
        component.querySelector('.name-carpeta-section-1').focus();
        
    }
    killActive() {
        const onc = (e) => {
            this.onclick(e);
        }
        document.removeEventListener('mousedown', onc)
        this.isActive = false;
        this.currentSelectorAppendActive = null;
    }
    setControllers() {

        $('#btn-email-add-carpeta').click(() => {
            if (this.isActive == true) {
                this.currentSelectorAppendActive.remove()
                this.killActive();
                this.appendActionAdd();
            } else {
                this.appendActionAdd();
            }
        })
        
    }
    on(event, callback) {
        this.calllbacks.push({name: event, callback})
    }
    activateTrigger(event, payload) {
        this.calllbacks.forEach(c => {
            if (c.name == event) {
                c.callback(payload);
            }
        })
    }
}

// componentes personalizados para correo
class HandlerInputCorreosSeleccionados {
    selectorPrincipal; // es el div principal 
    referenciaContenedor; // con guardamos el listado
    contenedorFormulario; // el div principal del formulario
    fisOpen = false;
    correosSelecccionados = [];// guardar los correos seleccionados

    constructor(selectorPrincipal, referenciaContenedor, contenedorFormulario) {
        this.selectorPrincipal = selectorPrincipal;
        this.referenciaContenedor = referenciaContenedor;
        this.contenedorFormulario = contenedorFormulario;

        this.setControllers();
    }
    //Append para mostrar burbuja con el correo seleccionado
    buildBurbujaCorreo(correoSeleccionado) {
        let span = document.createElement('span');
        span.classList.add( 'badge-light', 'badge-pill', 'bg-white', 'border', 'py-1')
        span.innerHTML = `
         <span style="font-size:12px">  ${correoSeleccionado} </span><svg xmlns="http://www.w3.org/2000/svg" style="margin-left:5px" width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-x"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
        `;
        span.querySelector('.feather-x').addEventListener('click', () => {
            this.correosSelecccionados = this.correosSelecccionados.filter(correoSeleccionado_ => correoSeleccionado_ != correoSeleccionado);
            span.remove();
        })
        console.log(span)
        return span;
    }
    //Adjuntar los  elemntos visuales de los correos seleccionados
    save(correoSeleccionado) { 
        let div = this.selectorPrincipal.querySelector('.input');
        div.style.width = '50%';
        let span = this.buildBurbujaCorreo(correoSeleccionado)

        if (this.correosSelecccionados.length > 0) {

            let fExiste = false;
            for (const correo_ of this.correosSelecccionados) {
                if (correo_ == correoSeleccionado) {
                    fExiste = true;
                    break;
                } 
            }

            if (!fExiste) {
                this.correosSelecccionados.push(correoSeleccionado);
                this.selectorPrincipal.querySelector('.conteiner .input').insertAdjacentElement('beforebegin', span);
            }
            
        } else {
            this.correosSelecccionados.push(correoSeleccionado);
            this.selectorPrincipal.querySelector('.conteiner .input').insertAdjacentElement('beforebegin',span);
        }


        let scroll = this.selectorPrincipal.querySelector('.conteiner')
        let detalles = scroll.getBoundingClientRect()
        scroll.scrollTop = detalles.height;
    }

    setControllers() {

        let div = this.selectorPrincipal.querySelector('.input'); // selector del conten editable

        div.addEventListener('keypress', (e) => {
            let keyCode = e.keyCode;
            if (keyCode == 13) { // Enter 
            //    if (correosSelecccionados.length>0){
                let span = this.buildBurbujaCorreo(div.innerHTML);
                this.correosSelecccionados.push(div.innerHTML);
                this.selectorPrincipal.querySelector('.conteiner .input').insertAdjacentElement('beforebegin', span);
                    div.innerHTML = '';
              //  }
            }
        })


        //manager width input
        if (this.correosSelecccionados.length == 0) {
            div.style.width = '100%';
        }
        
        div.addEventListener('click', () => {
            if (!this.fisOpen) {
                document.querySelector(`${this.referenciaContenedor}`).classList.remove('d-none')
                document.body.addEventListener('click', eventoOnClickPrincipal)
                document.body.addEventListener('keydown', onKeyEscape)
                FnListarCorreosInput(this.referenciaContenedor, this);
                this.fisOpen = true;// necesario que valla al final
            } else {
                this.fisOpen = false;
                document.querySelector(`${this.referenciaContenedor}`).classList.add('d-none')
                document.querySelector(this.referenciaContenedor).innerHTML = '';
                document.body.removeEventListener('click', eventoOnClickPrincipal);
                document.body.removeEventListener('keydown', onKeyEscape);
            }
        })

        const eventoOnClickPrincipal = (e) => {

            if (this.fisOpen == true) {
                if (!e.target.closest(`#${this.selectorPrincipal.id}`)) {
                    // cierralo
                    this.fisOpen = false;
                    document.querySelector(this.referenciaContenedor).innerHTML = '';
                    document.querySelector(this.referenciaContenedor).classList.add('d-none');
                    document.body.removeEventListener('click', eventoOnClickPrincipal);
                    document.body.removeEventListener('keydown', onKeyEscape);
                } 
            }
            
        }
        
        const onKeyEscape = (e) => {
            let code = e.keyCode;
            if (code == 27) { // ESC - 27
                this.fisOpen = false;
                document.querySelector(this.referenciaContenedor).innerHTML = '';
                document.querySelector(this.referenciaContenedor).classList.add('d-none');
                document.body.removeEventListener('click', eventoOnClickPrincipal);
                document.body.removeEventListener('keydown', onKeyEscape);
            } 
        }

    }
    getAllValuesFormatted() {
        // retornar los grupos de correo seleccionados (en formato String) 'correo1; correo2; correo3'
        return this.correosSelecccionados.reduce((init, current) => init + ";" + current).concat(';')
    }
    getAllValues() {
        return this.correosSelecccionados;
    }
    setValues(formatedText) {//'correo1; correo2; correo3' ['correo1','corre2','correo3']
        // split() (convertir el arrya)
        if (!formatedText) {
            this.correosSelecccionados = []
            return;
        }
        let mistring = formatedText.split(';')
        mistring.pop();
        if (mistring.length == 0) {
            this.mostrarPlaceholder();
        }
        this.correosSelecccionados = mistring;
    }
}

const inputParaSendMail = new HandlerInputCorreosSeleccionados(document.getElementById('input-para-compose-nuevoCorreo'), '#div-lista-correos', '#mailCompose');
const inputParaCc = new HandlerInputCorreosSeleccionados(document.getElementById('input-cc-compose-nuevoCorreo'), '#div-lista-correos-cc', '#mailCompose');
const inputParaCco = new HandlerInputCorreosSeleccionados(document.getElementById('input-cco-compose-nuevoCorreo'), '#div-lista-correos-cco', '#mailCompose');
const inputParaReply = new HandlerInputCorreosSeleccionados(document.getElementById('input-emailto-reply-detalleCorreo'), '#div-lista-correos-para-reply', '#btn-regresar-rta')
const inputCcReply = new HandlerInputCorreosSeleccionados(document.getElementById('input-cc-reply-detalleCorreo'), '#div-lista-correos-cc-detalleCorreo', '#btn-regresar-rta');
const inputCcoReply = new HandlerInputCorreosSeleccionados(document.getElementById('input-cco-reply-detalleCorreo'), '#div-lista-correos-cco-detalleCorreo', '#btn-regresar-rta');

// listado de correos al hacer click en input
function FnListarCorreosInput(referenciaContenedor, inputHandler) {
    $.ajax({
         url: `/Bandeja/GetListadoCorreosPlUsuarios`,
         type: 'GET',
         dataType: 'json',
         beforeSend: () => {
             $(referenciaContenedor).empty()
         },
         success: function (data) {
             console.log(data)
             if (data.length > 0) {
                 data.forEach(item => {
                     let div = document.createElement('div')
                     div.classList.add('list-group-item')
                     div.dataset.id = `div-lista-correo-${item.Id}`
                     div.innerHTML = `
                                     <div> ${item.nombre} &nbsp; ${item.email}  </div>
                                          `;
                     // eventos
                     div.addEventListener('click', () => {
                         console.log('clickeee - save ', item.email)
                         inputHandler.save(item.email);
                     })
                     // append al template
                     $(referenciaContenedor).append(div);
                 })
             }
         },
         error: function (xhr, status) {
             console.log('Disculpe, existió un problema');
         },
         complete: function (xhr, status) {
             console.log('Listado de correos en input exitoso')
         }
     });
}

function pruebaemail(email) {
    re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.exec(email) ? true : false;
}

$('#input-para-compose-nuevoCorreo').keypress(function (event) {
    if (event.keyCode == '13') {
        return false; // o event.preventDefault();
    }
});