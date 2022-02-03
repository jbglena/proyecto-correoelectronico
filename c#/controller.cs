[HttpGet]
        public ContentResult GetListarCorreosFrom()
        {
            int idUsuario = Int32.Parse(Session["IdUsuario"].ToString());
            int idUsuarioM = Int32.Parse(Session["IdUsuarioAdm"].ToString());
            DataTable dtCorreo = objBandejaCorreo.GetListadoEmailFrom(idUsuario);

            return new ContentResult
            {
                Content = DatatableToJson(dtCorreo),
                ContentType = "application/json"
            };
        }

//Filtro de busqueda
[HttpGet]
    public ContentResult GetListadoCorreos_filtroBusqueda(string paramDe, string paramPara, string paramAsunto, int paramTipo, string fechaInicio, string fechaFinal, string texto)
    {
        DataTable dtCorreo = objBandejaCorreo.ListarBandejaCorreoFiltros(paramDe, paramPara, paramAsunto, paramTipo, fechaInicio, fechaFinal, texto);

        return new ContentResult
        {
            Content = DatatableToJson(dtCorreo),
            ContentType = "application/json"
        };
    }


//
[HttpGet]
    public ContentResult GetListadoCorreoBandejaPaginacion(int pagina)
    {
        DataTable dtCorreo = objBandejaCorreo.ListarBandejaCorreoPaginacion(pagina);

        return new ContentResult
        {
            Content = DatatableToJson(dtCorreo),
            ContentType = "application/json"
        };
    }

//
[HttpGet]
    public ContentResult GetListadoReplicasCorreoIntegracion(string id)
    {
        DataTable dtCorreo = objBandejaCorreo.ListarConversacionCorreoIntegracion(id);

        return new ContentResult
        {
            Content = DatatableToJson(dtCorreo),
            ContentType = "application/json"
        };
    }


//
[HttpGet]
    public ContentResult GetCorreoIntegracion(int id)
    {
        DataTable dtCorreo = objBandejaCorreo.ListarCorreoIntegracion(id);

        return new ContentResult
        {
            Content = DatatableToJson(dtCorreo),
            ContentType = "application/json"
        };
    }
//
[HttpPost]
    public JsonResult SendCorreoIntegracion(string idSource, int idCarpeta, int idEstado, int isImportant, string emailTo, string emailFrom, string body, string emailCC, string emailBCC, string asunto, string linkReunion, int isFavorite, int codigoOportunidad, int codigoUsuarioSender, int fEnviadoRecibido, int fLeidoNoLeido, int fEstadoSistema, int idCorreoPadre, string emailFromName, string bodyPreview, string bodyCorreoRespondido)
    {
        // int idCarpeta, int idEstado, int isImportant ,int fLeidoNoLeido, int fEstadoSistema
        int idUsuario = Int32.Parse(Session["IdUsuario"].ToString());
        int idUsuarioM = Int32.Parse(Session["IdUsuarioAdm"].ToString());
        DataTable reqEnviarCorreo = objBandejaCorreo.EnviarCorreoIntegracionRetorno(idSource, idCarpeta, idEstado, isImportant, emailTo, emailFrom, body, emailCC, emailBCC, asunto, linkReunion, isFavorite, codigoOportunidad, codigoUsuarioSender, fEnviadoRecibido, fLeidoNoLeido, fEstadoSistema, idCorreoPadre, emailFromName, bodyPreview, bodyCorreoRespondido, idUsuario, idUsuarioM);
        bool retorno = false;
        int idCorreo = 0;
        if (reqEnviarCorreo.Rows.Count > 0)
        {
            idCorreo = Convert.ToInt32(reqEnviarCorreo.Rows[0]["id"]);
            retorno = true;
        }
        return Json(new { response = retorno, idCorreo }, JsonRequestBehavior.AllowGet);
    }

//
[HttpPost]
    public JsonResult ReplyCorreoIntegracion(string idSource, int idCarpeta, int idEstado, int isImportant, string emailTo, string emailFrom, string body, string emailCC, string emailBCC, string asunto, string linkReunion, int isFavorite, int codigoOportunidad, int codigoUsuarioSender, int fEnviadoRecibido, int fLeidoNoLeido, int fEstadoSistema, int idCorreoPadre, string emailFromName, string bodyPreview, string bodyCorreoRespondido)
    {
        int idUsuario = Int32.Parse(Session["IdUsuario"].ToString());
        int idUsuarioM = Int32.Parse(Session["IdUsuarioAdm"].ToString());

        DataTable reqEnviarCorreo = objBandejaCorreo.EnviarCorreoIntegracionRetorno(idSource, idCarpeta, idEstado, isImportant, emailTo, emailFrom, body, emailCC, emailBCC, asunto, linkReunion, isFavorite, codigoOportunidad, codigoUsuarioSender, fEnviadoRecibido, fLeidoNoLeido, fEstadoSistema, idCorreoPadre, emailFromName, bodyPreview, bodyCorreoRespondido, idUsuario, idUsuarioM);
        bool retorno = false;
        int idCorreo = 0;
        if(reqEnviarCorreo.Rows.Count > 0)
        {
            idCorreo = Convert.ToInt32(reqEnviarCorreo.Rows[0]["id"]);
            retorno = true;
        }
            
        return Json(new { response = retorno, idCorreo }, JsonRequestBehavior.AllowGet);
    }

/* Contadores */
[HttpGet]
    public ContentResult GetContadorCorreosEnBandeja()
    {
        DataTable dtCorreo = objBandejaCorreo.ListarContadorBandejaCorreoIntegracion();

        return new ContentResult
        {
            Content = DatatableToJson(dtCorreo),
            ContentType = "application/json"
        };
    }
//
[HttpPost]
    public JsonResult PostActualizarCorreoFavorito(int id, int isFavorite)
    {
        bool response = objBandejaCorreo.ActualizarFavoritosCorreo(id, isFavorite);
        return Json(new { status = 200, response = response }, JsonRequestBehavior.AllowGet);
    }
//
[HttpPost]
    public JsonResult PostActualizarCorreoArchivado(int id, int fArchivado)
    {
        bool response = objBandejaCorreo.ActualizarArchivadosCorreo(id, fArchivado);
        return Json(new { status = 200, response = response }, JsonRequestBehavior.AllowGet);
    }
//
[HttpPost]
    public JsonResult PostActualizarCorreoLeido(int id, int marcarComoNoLeido = 0)
    {
        DataTable correoSeleccionado = objBandejaCorreo.ListarCorreoIntegracion(id);
        if (correoSeleccionado.Rows.Count == 0)
            return Json(new { status = 404, response = "No se encontró el correo a actualizar" });
        int fLeidoNoLeido = Convert.ToInt32(correoSeleccionado.Rows[0]["fLeidoNoLeido"]);
        int fLeidoNoLeidoActualizar = 1;
        if (marcarComoNoLeido == 0)
        {
            if (fLeidoNoLeido > 0)
            {
                return Json(new { status = 200, response = true , fLeidoNoLeido = 1 }, JsonRequestBehavior.AllowGet);
            }
        }
        else
        {
            fLeidoNoLeidoActualizar = 0;
        }

        bool response = objBandejaCorreo.ActualizarFLeidoNoLeidoCorreo(id, fLeidoNoLeidoActualizar);
        return Json(new { status = 200, response = response , fLeidoNoLeido = fLeidoNoLeidoActualizar }, JsonRequestBehavior.AllowGet);
    }
//
[HttpPost]
    public JsonResult PostActualizarCorreoPapelera(int id, int fPapelera)
    {
        bool response = objBandejaCorreo.ActualizarPapeleraCorreos(id, fPapelera);
        return Json(new { status = 200, response = response }, JsonRequestBehavior.AllowGet);
    }

//
[HttpPost]
    public JsonResult EliminarCorreoIntegracion(string id)
    {
        return Json(new { response = objBandejaCorreo.EliminarCorreoIntegracion(id) }, JsonRequestBehavior.AllowGet);
    }

//
[HttpPost]
    public JsonResult EditarCorreoIntegracion(string id, string idSource, int idCarpeta, int idEstado, int isImportant, string emailTo, string emailFrom, string body, string emailCC, string emailBCC, string asunto, string linkReunion, int isFavorite, int codigoOportunidad, int codigoUsuarioSender, int fEnviadoRecibido, int fLeidoNoLeido, int fEstadoSistema, int idCorreoPadre, string emailFromName, string bodyPreview, string bodyCorreoRespondido)
    {
        bool retorno = objBandejaCorreo.EditarCorreoIntegracion(id, idSource, idCarpeta, idEstado, isImportant, emailTo, emailFrom, body, emailCC, emailBCC, asunto, linkReunion, isFavorite, codigoOportunidad, codigoUsuarioSender, fEnviadoRecibido, fLeidoNoLeido, fEstadoSistema, idCorreoPadre, emailFromName, bodyPreview, bodyCorreoRespondido);
        return Json(new { retorno }, JsonRequestBehavior.AllowGet);
    }

/* mantenimiento comentarios */
[HttpGet]
    public ContentResult GetListadoComentarios(int idCorreo)
    {
        DataTable dtCorreo = objBandejaCorreo.ListarComentariosCorreo(idCorreo);

        return new ContentResult
        {
            Content = DatatableToJson(dtCorreo),
            ContentType = "application/json"
        };
    }
//
[HttpPost]
    public JsonResult PostCrearComentario(int idCorreo, string contenido)
    {
        int idUsuarioCreador = Convert.ToInt32(Session["IdUsuarioAdm"]);

        int idUsuario = Int32.Parse(Session["IdUsuario"].ToString());
        int idUsuarioPrincipal = Int32.Parse(Session["IdUsuarioAdm"].ToString());
        int dtCorreo = objBandejaCorreo.InsertarComentario(idCorreo, contenido, idUsuarioCreador);
        return Json(new { newID = dtCorreo, idUsuarioM = Session["IdUsuarioAdm"], idusuario = Session["IdUsuario"] }, JsonRequestBehavior.AllowGet);
    }
//
[HttpPost]
    public JsonResult PostEliminarComentario(int id)
    {
        bool retorno = objBandejaCorreo.EliminarComentario(id);
        return Json(new { response = retorno }, JsonRequestBehavior.AllowGet);
    }
//
[HttpPost]
public JsonResult PostGenerarOportunidadPorChats(string sessionId, string nombreCliente, string emailCliente, int idCanalEntrada, int idUsuarioAsignado, string comentarios, string codigoBot, int idEtapaVenta, string telefono, string celular, string seccion_call = "any", string razonSocial = "", string nroDocumento = "")
    {
        if (Session["IdUsuarioAdm"] == null || Session["IdUsuario"] == null) LlenarSesionesSistemaPlazbot();

        /*Sigue con el desarrollo normal*/

        DataTable dtCorrelativo = objOportunidad.ListarCorrelativoOportunidades(Convert.ToInt32(Session["IdUsuarioAdm"]));
        if (dtCorrelativo.Rows.Count > 0)
        {
            var str = new BTCadenaCaracteres();

            string fechaOportunidad = DateTime.Now.ToString("dd/MM/yyyy");

            string plataforma = "Web chat";
            if (idCanalEntrada == 3)
                plataforma = "Facebook";
            else if (idCanalEntrada == 2)
                plataforma = "Whatsapp";
            else if (idCanalEntrada == 4)
                plataforma = "Formulario";
            else if (idCanalEntrada == 5)
                plataforma = "Llamada";
            else if (idCanalEntrada == 7)
                plataforma = "Correo";
            else if (idCanalEntrada == 9)
                plataforma = "Llamadas Voip";
            else if (idCanalEntrada == 10)
                plataforma = "Facebook Leads";
            else if (idCanalEntrada == 12)
                plataforma = "Mercado Libre";
            else if (idCanalEntrada == 15)
                plataforma = "Telegram";

            int correlativo = Convert.ToInt32(dtCorrelativo.Rows[0]["correlativo"]) + 1;
            string codigoOportunidad = str.CadenaCorrelativoCompleto("OP", correlativo, 6);

            string nombreOportunidad = $"Oportunidad de {plataforma} - {fechaOportunidad}";
            if (seccion_call == "chat")
                nombreOportunidad = nombreCliente;

            int idCliente = 0;

            if (!string.IsNullOrEmpty(sessionId))
            {
                bool InsertarCliente = false;
                string documentoFindCliente = seccion_call == "chat" ? sessionId : nroDocumento;
                DataTable reqListarClientesOp = objOportunidad.ListarClientesOportunidades(documentoFindCliente, Convert.ToInt32(Session["IdUsuarioAdm"]));
                bool existeClienteop = reqListarClientesOp.Rows.Count > 0;

                /* validar el si se insertara cliente oportuniad */
                if (seccion_call != "chat" && existeClienteop == false)
                    InsertarCliente = false;
                else if (seccion_call != "chat" && existeClienteop == true)
                    InsertarCliente = true;
                else if (seccion_call == "chat" && existeClienteop == false)
                    InsertarCliente = true;

                /* validar actualizacion de nombre campos de cliente */
                if (existeClienteop)
                {
                    string nombreClienteListado = reqListarClientesOp.Rows[0]["nombre"].ToString();
                    int idClienteListado = Convert.ToInt32(reqListarClientesOp.Rows[0]["id"]);
                    // le damos el id del ciente al que pertenece
                    idCliente = idClienteListado;
                    if (nombreClienteListado != nombreCliente)
                    {
                        string documentoOportunidad = sessionId;
                        string nombreClienteoportunidad = nombreCliente;

                        if (!String.IsNullOrEmpty(razonSocial))
                        {

                            if (!String.IsNullOrEmpty(razonSocial)) nombreClienteoportunidad = razonSocial;

                            if (!String.IsNullOrEmpty(nroDocumento)) documentoOportunidad = nroDocumento;
                        }

                        string telefonoClienListado = reqListarClientesOp.Rows[0]["telefono"].ToString();
                        string celularClienListado = reqListarClientesOp.Rows[0]["celular"].ToString();
                        string emailClienListado = reqListarClientesOp.Rows[0]["email"].ToString();
                        string observacionClienListado = reqListarClientesOp.Rows[0]["observacion"].ToString();
                        int usuarioModificador = 0;

                        int idAgenteEntrernador = Convert.ToInt32(reqListarClientesOp.Rows[0]["idAgente"]);
                        string direccionClienteListado = reqListarClientesOp.Rows[0]["direccion"].ToString();

                        objOportunidad.ActualizarClienteOportunidadInner(idClienteListado, documentoOportunidad, nombreClienteoportunidad, direccionClienteListado, telefonoClienListado, celularClienListado, idAgenteEntrernador, usuarioModificador, observacionClienListado, emailClienListado);
                    }

                }

                /* insertar cleinte de oportunidad */
                if (InsertarCliente)
                {
                    string documentoOportunidad = sessionId;
                    string nombreClienteoportunidad = nombreCliente;

                    if (!String.IsNullOrEmpty(razonSocial))
                    {

                        if (!String.IsNullOrEmpty(razonSocial)) nombreClienteoportunidad = razonSocial;

                        if (!String.IsNullOrEmpty(nroDocumento)) documentoOportunidad = nroDocumento;
                    }
                    idCliente = objOportunidad.InsertarClienteOportunidad(documentoOportunidad, nombreClienteoportunidad, "", telefono, celular, idUsuarioAsignado, Convert.ToInt32(Session["IdUsuario"]), Convert.ToInt32(Session["IdUsuarioAdm"]), "", emailCliente);
                }
            }
//
 public JsonResult GetListadoOportunidadesPorChatCanales(string codigoBot, string sessionId)
         {
             if (!string.IsNullOrEmpty(codigoBot) && !string.IsNullOrEmpty(sessionId))
             {
                 DataTable dtOportunidad = objOportunidad.ListarOportunidadesPorChatCanalesInner(codigoBot, sessionId);
                 string jsonOport = DatatableToJson(dtOportunidad);

                 var jsonResult = Json(jsonOport, JsonRequestBehavior.AllowGet);
                 jsonResult.MaxJsonLength = int.MaxValue;
                 return jsonResult;
             }

             Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
             return Json(new { response = false }, JsonRequestBehavior.AllowGet);
         }
//
public JsonResult GetListadoEventosCalendario()
        {
            if (Session["IdUsuario"] == null) LlenarSesionesSistemaPlazbot();

            var jsonResult = Json(objUsuario.Listar_EventosCalendario(Convert.ToInt32(Session["IdUsuario"])).ToList(), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
//
 public JsonResult PostCreareventoCalendario(string horaEvento, string fechaEvento, string descripcion, string notas, string referencia, int mostrarCalendario, int estadoCompletado, int estado, int tipo, int idUsuarioAsignado, string usuarioAsignado, string idSesion, string codigoBot)
        {
            if (Session["IdUsuario"] == null) LlenarSesionesSistemaPlazbot();

            string idSesion_ = idSesion;
            if (idSesion == "none") idSesion_ = "NULL";

            string chatCodigoBot_ = codigoBot;
            if (codigoBot == "none") chatCodigoBot_ = "NULL";

            var jsonResult = Json(new { response = objUsuario.CrearEventoCalendario(horaEvento, fechaEvento, descripcion, notas, referencia, mostrarCalendario, estadoCompletado, estado, Convert.ToInt32(Session["IdUsuario"]), tipo, usuarioAsignado, idUsuarioAsignado, idSesion_, chatCodigoBot_) }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
//
public JsonResult PostEliminarEvento(int id)
        {
            var jsonResult = Json(new { response = objUsuario.EliminarEventoCalendario(id) }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
//
 public JsonResult PostGuardarTarea(int id, string descripcion, string referencia, string notas, int idUsuarioAsignado, string fechaTarea, int mostrarCalendario, int idTipoTarea, int aviso, string horaTarea, int idOportunidad, int idCliente, string nombreOportunidad, string nombreCliente, string momentoAvisoRecordatorio, string sessionId = "", string codigoBot = "")
        {
            if (Session["IdUsuarioAdm"] == null || Session["IdUsuario"] == null) LlenarSesionesSistemaPlazbot();

            fechaTarea = Convert.ToDateTime(fechaTarea).ToString("dd/MM/yyyy");

            var str = new BTCadenaCaracteres();
            string usuarioReceptor = "", usunombre = "", usuarioAsignador = "";

            DataTable dtusuarioAsginador = objUsuario.ListarUsuarioInner(Convert.ToInt32(Session["IdUsuario"]));
            if (dtusuarioAsginador.Rows.Count > 0)
            {
                usuarioAsignador = dtusuarioAsginador.Rows[0]["nombre"].ToString();
            }

            DataTable dtUsuario = objUsuario.ListarUsuarioInner(idUsuarioAsignado);
            if (dtUsuario.Rows.Count > 0)
            {
                usunombre = dtUsuario.Rows[0]["nombre"].ToString();
                usuarioReceptor = dtUsuario.Rows[0]["email"].ToString();
            }


            string _BodyMail = "";
            using (StreamReader reader = new StreamReader(HttpContext.Server.MapPath("~/Contents/templatesEmail/plantilla-asignaciontarea-usuario.html")))
            {
                _BodyMail = reader.ReadToEnd();
            }
            _BodyMail = _BodyMail.Replace("{nombreBot}", "Canal Plazbot");
            _BodyMail = _BodyMail.Replace("{nombreUsuario}", usunombre);
            _BodyMail = _BodyMail.Replace("{nombreTareaNueva}", descripcion);
            _BodyMail = _BodyMail.Replace("{nombreReferencia}", referencia);
            _BodyMail = _BodyMail.Replace("{fechaTarea}", fechaTarea);
            _BodyMail = _BodyMail.Replace("{horaTarea}", horaTarea);
            _BodyMail = _BodyMail.Replace("{nombreAsignante}", usuarioAsignador);
            _BodyMail = _BodyMail.Replace("{anioActual}", DateTime.Now.Year.ToString());

            var dynamicEmpresa = InformacionSistemaPlazbot();

            _BodyMail = _BodyMail.Replace("{logoEmpresa}", dynamicEmpresa.UrlLogoColor);
            _BodyMail = _BodyMail.Replace("{urlLogin}", $"{ dynamicEmpresa.DominioPrincipal} login/login");
            _BodyMail = _BodyMail.Replace("{correoEmpresa}", dynamicEmpresa.EmailEmpresa);
            _BodyMail = _BodyMail.Replace("{nombreEmpresa}", dynamicEmpresa.NombreEmpresa);
            _BodyMail = _BodyMail.Replace("{telefonoEmpresa}", dynamicEmpresa.TelefonoEmpresa);

            string correoFrom = dynamicEmpresa.EmailEmpresa;
            string aliasCorreo = $"Equipo { dynamicEmpresa.NombreEmpresa }";
            string sendgridToken = dynamicEmpresa.SendgridToken;

            if (!string.IsNullOrEmpty(fechaTarea)) fechaTarea = str.CadenaFechaSQL(fechaTarea);
            fechaTarea = str.CadenaFechaSQL(fechaTarea);

            bool retorno;
            if (id == 0)
                retorno = objTarea.InsertarTareaRetorno(descripcion, referencia, notas, idUsuarioAsignado, fechaTarea, mostrarCalendario, idTipoTarea, Convert.ToInt32(Session["IdUsuario"]), Convert.ToInt32(Session["IdUsuarioAdm"]), aviso, horaTarea, idOportunidad, idCliente, nombreOportunidad, nombreCliente, momentoAvisoRecordatorio, sessionId, codigoBot);
            else
                retorno = objTarea.ActualizarTareasInner(id, descripcion, referencia, notas, fechaTarea, mostrarCalendario, Convert.ToInt32(Session["IdUsuario"]), idUsuarioAsignado, aviso, horaTarea, idTipoTarea, idOportunidad, idCliente, nombreOportunidad, nombreCliente, momentoAvisoRecordatorio);

            if (Convert.ToInt32(Session["IdUsuario"]) != idUsuarioAsignado)
            {
                objMail.EnvioCorreo(_BodyMail, "Asignación de Tarea", usuarioReceptor, correoFrom, aliasCorreo, sendgridToken);
            }

            return Json(new { response = retorno }, JsonRequestBehavior.AllowGet);
        }

//
 public JsonResult GetListadoTareas()
        {
            if (Session["IdUsuario"] == null || Session["IdUsuarioAdm"] == null) LlenarSesionesSistemaPlazbot();

            dynamic informacionEmpresa = this.InformacionSistemaPlazbot();
            int horasRestar = informacionEmpresa.HorasRestar;

            DataTable dtAdmin = objUsuario.ListarUsuarioInner(Convert.ToInt32(Session["IdUsuarioAdm"]));
            if (dtAdmin.Rows.Count > 0) horasRestar = Convert.ToInt32(dtAdmin.Rows[0]["horasDescuento"]);

            var jsonResult = Json(new { }, JsonRequestBehavior.AllowGet);

            DataTable dtAcceso = objUsuario.ListarAccesoInnerxUsuario(Convert.ToInt32(Session["IdUsuario"]), "btnVerTareasOtrosAgentes");
            if (dtAcceso.Rows.Count > 0)
            {
                if (dtAcceso.Rows[0]["acceso"].ToString().Equals("y"))
                    jsonResult = Json(objTarea.ListarTareas_List(Convert.ToInt32(Session["IdUsuario"]), horasRestar, Convert.ToInt32(Session["IdUsuarioAdm"])).ToList(), JsonRequestBehavior.AllowGet);
                else
                    jsonResult = Json(objTarea.ListarTareas_List(Convert.ToInt32(Session["IdUsuario"]), horasRestar).ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                jsonResult = Json(objTarea.ListarTareas_List(Convert.ToInt32(Session["IdUsuario"]), horasRestar).ToList(), JsonRequestBehavior.AllowGet);
            }

            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

//
public JsonResult PostEliminarTarea(int idTarea)
    {
        if (objTarea.EliminarTareaInner(idTarea))
        {
            return Json(new { response = true }, JsonRequestBehavior.AllowGet);
        }

        Response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
        return Json(new { response = false }, JsonRequestBehavior.AllowGet);
    }
//
 [HttpGet]
        public ContentResult GetListadoCarpetasCorreoIntegracion(int idUsuario)
        {
            DataTable dtCorreo = objBandejaCorreo.ListarCarpetasBandejaCorreoIntegracion(idUsuario);

            return new ContentResult
            {
                Content = DatatableToJson(dtCorreo),
                ContentType = "application/json"
            };
        }
//
[HttpGet]
    public ContentResult GetListadoCorreosPlUsuarios()
    {
        int idAdministrador = Int32.Parse(Session["IdUsuarioAdm"].ToString());
        DataTable dtCorreo = objBandejaCorreo.GetListadoCorreosParaInput(idAdministrador);

        return new ContentResult
        {
            Content = DatatableToJson(dtCorreo),
            ContentType = "application/json"
        };
    }
