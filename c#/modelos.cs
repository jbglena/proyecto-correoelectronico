public DataTable GetListadoEmailFrom(int idUsuario)
    {
        DataTable dt = new DataTable();
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = @"SELECT email , nombre FROM PlUsuario 
                                                WHERE id=@idUsuario";
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
                    // objCommand.Parameters.AddWithValue("@emailFromName", emailFromName);
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    dt.Load(objCommand.ExecuteReader());
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return dt;
    }
//
  public DataTable ListarBandejaCorreoFiltros(string paramDe, string paramPara, string paramAsunto, int paramTipo, string fechaInicio, string fechaFinal, string texto )
    {
        DataTable dt = new DataTable();
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    string queryTipoGeneral = "";
                    if (paramTipo == -1) queryTipoGeneral = "";
                    else if (paramTipo == 1 || paramTipo == 2 || paramTipo == 4)// recibidos - enviados - borrador 
                    {
                        queryTipoGeneral = $"AND idEstado = {paramTipo} ";

                    }
                    else if (paramTipo == 3)// Favoritos
                    {
                        queryTipoGeneral = $"AND isFavorite = 1 ";
                    }

                    else if (paramTipo == 5)// Papelera
                    {
                        queryTipoGeneral = $"AND fPapelera = 1 ";
                    }


                    string condicionDe = "";
                    if (!string.IsNullOrEmpty(paramDe)) condicionDe = $" AND emailTo like '%{ paramDe }%'";
                    string condicionPara = "";
                    if (!string.IsNullOrEmpty(paramPara)) condicionPara = $" AND emailFrom like '%{ paramPara }%'";
                    string condicionAsunto = "";
                    if (!string.IsNullOrEmpty(paramPara)) condicionAsunto = $" AND asunto like '%{ paramAsunto }%'";
                    string condicionFechas = "";
                    if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal)) condicionFechas = $" AND CAST(fechaCreacion AS DATE) between '{fechaInicio}' and '{fechaFinal}'";
                    string condicionTexto = "";
                    if (!string.IsNullOrEmpty(texto)) condicionTexto = $"AND body like '%{ texto }%'";

                    objCommand.CommandText = $@"SELECT * FROM PlIntegracionCorreoElectronico
                                                WHERE idCorreoPadre = -1 AND fEstadoSistema = 1 AND idCarpeta = 0 
                                                { condicionDe }  { condicionPara } { condicionAsunto } { queryTipoGeneral } { condicionFechas } {condicionTexto}
                                                ORDER BY fechaCreacion DESC 
                                                OFFSET @offset ROWS FETCH NEXT 15 ROWS ONLY;";
                    objCommand.Parameters.AddWithValue("@offset", 3);
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    dt.Load(objCommand.ExecuteReader());
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return dt;
    }





//
public DataTable ListarBandejaCorreoPaginacion(int pagina)
    {
        int init = (pagina - 1) * qtyPaginacionListado;
        DataTable dt = new DataTable();
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = @"SELECT 
                        plc.id , plc.idSource , plc.idCarpeta, plc.idEstado, plc.isImportant, plc.emailTo , plc.emailFrom , plc.body , plc.emailCC , plc.emailBCC , plc.asunto , plc.linkReunion , plc.fechaEnvioCorreo , plc.fechaGuardado ,  FORMAT(plc.fechaRecepcion, 'dd/MM/yyyy HH:mm') as 'fechaRecepcion'  , plc.isFavorite, plc.codigoOportunidad , plc.codigoUsuarioSender , plc.fEnviadoRecibido , plc.fLeidoNoLeido , plc.fEstadoSistema , plc.idCorreoPadre , plc.emailFromName , plc.bodyPreview , plc.bodyCorreoRespondido , plc.tipo , plc.idUsuario , plc.idUsuarioPrincipal ,  plc.fPapelera , plc.fArchivado , FORMAT(plc.fechaCreacion, 'dd/MM/yyyy HH:mm') as 'fechacreacion'
                        FROM PlIntegracionCorreoElectronico plc WHERE plc.idCorreoPadre = -1 AND plc.fEstadoSistema = 1 AND plc.idEstado = 1 AND plc.idCarpeta = 0 AND plc.fPapelera = 0 ORDER BY plc.fechaCreacion DESC OFFSET @offset ROWS FETCH NEXT 15 ROWS ONLY;";
                    //objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);
                    objCommand.Parameters.AddWithValue("@offset", init);
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    dt.Load(objCommand.ExecuteReader());
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return dt;
    }   

//insersiones
 public DataTable ListarCorreoIntegracion(int id)
    {
        DataTable dt = new DataTable();
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    // listar los primeros trescientos que esten en bandeja o spam o en papelera orderados por el id 
                    objCommand.CommandText = @"SELECT 
                        plc.id , plc.idSource , plc.idCarpeta, plc.idEstado, plc.isImportant, plc.emailTo , plc.emailFrom , plc.body , plc.emailCC , plc.emailBCC , plc.asunto , plc.linkReunion , FORMAT(plc.fechaEnvioCorreo , 'dd/MM/yyyy HH:mm') as 'fechaEnvioCorreo' , plc.fechaGuardado ,  FORMAT(plc.fechaRecepcion, 'dd/MM/yyyy HH:mm') as 'fechaRecepcion'  , plc.isFavorite, plc.codigoOportunidad , plc.codigoUsuarioSender , plc.fEnviadoRecibido , plc.fLeidoNoLeido , plc.fEstadoSistema , plc.idCorreoPadre , plc.emailFromName , plc.bodyPreview , plc.bodyCorreoRespondido , plc.tipo , plc.idUsuario , plc.idUsuarioPrincipal ,  plc.fPapelera , plc.fArchivado , FORMAT(plc.fechaCreacion, 'dd/MM/yyyy HH:mm') as 'fechacreacion'
                        FROM PlIntegracionCorreoElectronico plc WHERE plc.id = @id "; 
                    objCommand.Parameters.AddWithValue("@id", id);
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    dt.Load(objCommand.ExecuteReader());
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return dt;
    }

  // Inserciones 

//
public DataTable EnviarCorreoIntegracionRetorno(string idSource, int idCarpeta, int idEstado, int isImportant, string emailTo, string emailFrom, string body, string emailCC, string emailBCC, string asunto, string linkReunion, int isFavorite, int codigoOportunidad, int codigoUsuarioSender, int fEnviadoRecibido, int fLeidoNoLeido, int fEstadoSistema, int idCorreoPadre, string emailFromName, string bodyPreview, string bodyCorreoRespondido, int idUsuario, int idUsuarioM)
    {
        DataTable dt = new DataTable();
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = @"
                        INSERT INTO PlIntegracionCorreoElectronico values(
                            @idSource,
                            @idCarpeta,
                            @idEStado,
                            @isImportant,
                            @emailTo,
                            @emailFrom,
                            @body,
                            @emailCC,
                            @emailBCC,
                            @asunto,
                            @linkReunion,
                            GETDATE(),
                            GETDATE(),
                            GETDATE(),
                            GETDATE(),
                            @isFavorite, 
                            @codigoOportunidad , 
                            @codigoUsuarioSender, 
                            @fEnviadoRecibido,
                            @fLeidoNoLeido,
                            @fEstadoSistema ,
                            @idCorreoPadre ,
                            @emailFromName,
                            @bodyPreview,
                            @bodyCorreoRespondido,
                            1, /* tipo 1 correo */
                            @idUsuario, /* id Usuario*/
                            @idUsuarioPrincipal, /* id Usuario Principal */
                            0, /* fArchivado*/
                            0 /* fPapelera */
                        ); SELECT @@identity as id;";
                    objCommand.Parameters.AddWithValue("@idSource", idSource);
                    objCommand.Parameters.AddWithValue("@idCarpeta", idCarpeta);
                    objCommand.Parameters.AddWithValue("@idEStado", idEstado);
                    objCommand.Parameters.AddWithValue("@isImportant", isImportant);
                    objCommand.Parameters.AddWithValue("@emailTo", emailTo);
                    objCommand.Parameters.AddWithValue("@emailFrom", emailFrom);
                    objCommand.Parameters.AddWithValue("@body", body);
                    objCommand.Parameters.AddWithValue("@emailCC", emailCC);
                    objCommand.Parameters.AddWithValue("@emailBCC", emailBCC);
                    objCommand.Parameters.AddWithValue("@asunto", asunto);
                    objCommand.Parameters.AddWithValue("@linkReunion", linkReunion);
                    objCommand.Parameters.AddWithValue("@isFavorite", isFavorite);
                    objCommand.Parameters.AddWithValue("@codigoOportunidad", codigoOportunidad);
                    objCommand.Parameters.AddWithValue("@codigoUsuarioSender", codigoUsuarioSender);
                    objCommand.Parameters.AddWithValue("@fEnviadoRecibido", fEnviadoRecibido);
                    objCommand.Parameters.AddWithValue("@fLeidoNoLeido", fLeidoNoLeido);
                    objCommand.Parameters.AddWithValue("@fEstadoSistema", fEstadoSistema);
                    objCommand.Parameters.AddWithValue("@idCorreoPadre", idCorreoPadre);
                    objCommand.Parameters.AddWithValue("@emailFromName", emailFromName);
                    objCommand.Parameters.AddWithValue("@bodyPreview", bodyPreview);
                    objCommand.Parameters.AddWithValue("@bodyCorreoRespondido", bodyCorreoRespondido);
                    objCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
                    objCommand.Parameters.AddWithValue("@idUsuarioPrincipal", idUsuarioM);

                    //objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    dt.Load(objCommand.ExecuteReader());
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return dt;
    }
/* contadores */
public DataTable ListarContadorBandejaCorreoIntegracion()
    {
        DataTable dt = new DataTable();
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = "select count(id) from PlIntegracionCorreoElectronico where idEstado = 1 AND fEstadoSistema = 1 AND fLeidoNoLeido = false;";
                    //objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    dt.Load(objCommand.ExecuteReader());
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return dt;
    }
// fav correo
 public bool ActualizarFavoritosCorreo(int idCorreo, int isFavorite)
        {
            bool retorno = false;
            try
            {
                using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
                {
                    using (SqlCommand objCommand = new SqlCommand())
                    {
                        objCommand.CommandText = @"
                        UPDATE PlIntegracionCorreoElectronico 
                        SET
	                        isFavorite =  @isFavorite /* attr*/
                        WHERE 
                            id = @id";
                        //objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);
                        objCommand.Parameters.AddWithValue("@id", idCorreo);
                        objCommand.Parameters.AddWithValue("@isFavorite", isFavorite);
                        objCommand.CommandType = CommandType.Text;
                        objCommand.Connection = sqlConnection1;

                        sqlConnection1.Open();
                        objCommand.ExecuteNonQuery();
                        retorno = true;
                        sqlConnection1.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
            }
            return retorno;
        }

// archivado correo
public bool ActualizarArchivadosCorreo(int idCorreo, int fArchivado)
    {
        bool retorno = false;
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = @"
                    UPDATE PlIntegracionCorreoElectronico 
                    SET
                        fArchivado =  @fArchivado /* attr*/
                    WHERE 
                        id = @id";
                    //objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);
                    objCommand.Parameters.AddWithValue("@id", idCorreo);
                    objCommand.Parameters.AddWithValue("@fArchivado", fArchivado);
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    objCommand.ExecuteNonQuery();
                    retorno = true;
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return retorno;
    }
// actualizaciones
public bool ActualizarFLeidoNoLeidoCorreo(int idCorreo, int fLeidoNoLeido)
    {
        bool retorno = false;
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = @"
                    UPDATE PlIntegracionCorreoElectronico 
                    SET
                        fLeidoNoLeido =  @fLeidoNoLeido
                    WHERE 
                        id = @id";
                    //objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);
                    objCommand.Parameters.AddWithValue("@id", idCorreo);
                    objCommand.Parameters.AddWithValue("@fLeidoNoLeido", fLeidoNoLeido);
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    objCommand.ExecuteNonQuery();
                    retorno = true;
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return retorno;
    }

//
public bool ActualizarPapeleraCorreos(int idCorreo, int fPapelera)
        {
            bool retorno = false;
            try
            {
                using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
                {
                    using (SqlCommand objCommand = new SqlCommand())
                    {
                        objCommand.CommandText = @"
                        UPDATE PlIntegracionCorreoElectronico 
                        SET
	                        fPapelera =  @fPapelera /* attr*/
                        WHERE 
                            id = @id";
                        //objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);
                        objCommand.Parameters.AddWithValue("@id", idCorreo);
                        objCommand.Parameters.AddWithValue("@fPapelera", fPapelera);
                        objCommand.CommandType = CommandType.Text;
                        objCommand.Connection = sqlConnection1;

                        sqlConnection1.Open();
                        objCommand.ExecuteNonQuery();
                        retorno = true;
                        sqlConnection1.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
            }
            return retorno;
        }
/* comentario manager */
public DataTable ListarComentariosCorreo(int idCorreo)
    {
        DataTable dt = new DataTable();
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = @"SELECT plcom.id, plcom.idCorreo, plcom.contenido, plcom.usuarioCreador, plus.nombre AS nombreUsuario, FORMAT                                         (plcom.fechaCreacion, 'dd/MM/yyyy HH:mm') as 'fechaCreacion', plcom.usuarioModificador, 
                                                FORMAT(plcom.fechaModificacion, 'dd/MM/yyyy HH:mm') as 'fechaModificacion'
                                                FROM  PlComentariosCorreo plcom 
                                                INNER JOIN PlUsuario plus ON plus.id = plcom.usuarioCreador
                                                WHERE plcom. idCorreo=@idCorreo
                                                ORDER BY plcom.fechaCreacion DESC";
                    //objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);
                    // objCommand.usuarioCreador = Convert.ToInt32(Session["IdUsuario"]);
                    objCommand.Parameters.AddWithValue("@idCorreo", idCorreo);
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    dt.Load(objCommand.ExecuteReader());
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return dt;
    }
//
public int InsertarComentario(int idCorreo, string contenido, int usuarioCreador)
    {
        //DataTable dt = new DataTable();
        Int32 newID = 0;
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = @"
                        INSERT INTO PlComentariosCorreo VALUES(
                            @idCorreo,
                            @contenido,
                            @usuarioCreador,
                            GETDATE(),
                            -1,/* no se  modifico entonces coloco -1*/
                            GETDATE()
                        );
                    ";
                    //objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);
                    objCommand.Parameters.AddWithValue("@idCorreo", idCorreo);
                    objCommand.Parameters.AddWithValue("@contenido", contenido);
                    objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);

                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    //objCommand.ExecuteReader();
                    newID = objCommand.ExecuteNonQuery();
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        //return dt;
        return newID;
    }
//
public bool EliminarComentario(int id)
    {
        bool retorno = false;
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = @"DELETE FROM PlComentariosCorreo
                                                WHERE id=@idComentario"; 
                    objCommand.Parameters.AddWithValue("@idComentario", id);
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    objCommand.ExecuteNonQuery();
                    retorno = true;
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return retorno;
    }
//
 public DataTable ListarCorrelativoOportunidades(int idAdministrador)
    {
        DataTable dt = new DataTable();
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = @"SELECT ISNULL(MAX(correlativo), 0) correlativo
                                                FROM PlOportunidad op
                                                WHERE idAdministrador = @idAdministrador";
                    objCommand.Parameters.AddWithValue("@idAdministrador", idAdministrador);
                    objCommand.CommandTimeout = Int32.MaxValue;
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    dt.Load(objCommand.ExecuteReader());
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return dt;
    }
//
public DataTable InsertarOportunidad(string codigoOportunidad, int correlativo, string nombreOportunidad, string codigoCliente, string nombreCliente, int idCanalEntrada, string codigoBot, int idTipoOportunidad, int idMoneda, decimal montoVenta, string descripcionOportunidad, int idEtapaVenta, int idEstadoOportunidad, string fechaOportunidad, string fechaVencimiento, int usuarioAsignado, int usuarioCreador, int idAdministrador, int fVieneDeChat, int idSegmentacion, string codigoClienteInterno, int idCliente, string chatSessionId)
    {
        DataTable dt = new DataTable();
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    if (string.IsNullOrEmpty(fechaOportunidad))
                        fechaOportunidad = "GETDATE()";
                    else
                        fechaOportunidad = $"'{fechaOportunidad}'";

                    if (string.IsNullOrEmpty(fechaVencimiento))
                        fechaVencimiento = "NULL";
                    else
                        fechaVencimiento = $"'{fechaVencimiento}'";

                    objCommand.CommandText = $"INSERT INTO PlOportunidad VALUES('{codigoOportunidad}', {correlativo}, @nombreOportunidad, @sessionId, @nombreCliente, @montoVenta, {idMoneda}, @descripcion, {idTipoOportunidad}, {fechaOportunidad}, {idEstadoOportunidad}, @codigoBot, 1, @usuarioCreador, GETDATE(), @usuarioCreador, GETDATE(), 0, 0, '', NULL, {idCanalEntrada}, {usuarioAsignado}, {idEtapaVenta}, {fechaVencimiento}, {idAdministrador}, 0, '', @fVieneDeChat, { idSegmentacion }, @codigoClienteInterno, @idCliente, @chatSessionId); Select @@identity as id;";
                    objCommand.Parameters.AddWithValue("@codigoClienteInterno", codigoClienteInterno);
                    objCommand.Parameters.AddWithValue("@nombreOportunidad", nombreOportunidad);
                    objCommand.Parameters.AddWithValue("@sessionId", codigoCliente);
                    objCommand.Parameters.AddWithValue("@nombreCliente", nombreCliente);
                    objCommand.Parameters.AddWithValue("@montoVenta", montoVenta);
                    objCommand.Parameters.AddWithValue("@descripcion", descripcionOportunidad);
                    objCommand.Parameters.AddWithValue("@codigoBot", codigoBot);
                    objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);
                    objCommand.Parameters.AddWithValue("@fVieneDeChat", fVieneDeChat);
                    objCommand.Parameters.AddWithValue("@idCliente", idCliente);
                    objCommand.Parameters.AddWithValue("@chatSessionId", chatSessionId);
                    objCommand.CommandTimeout = Int32.MaxValue;
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    dt.Load(objCommand.ExecuteReader());
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return dt;
    }
//
 public DataTable ListarClientesOportunidades(string documento, int idAdministrador)
    {
        DataTable dt = new DataTable();
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = @"SELECT cli.id, cli.documento, cli.nombre, cli.telefono, cli.celular, cli.direccion, cli.email,
                                                cli.idAgente, ISNULL(agen.nombre, '') agenteEntrenador, usu.nombre AS usuarioCreador, cli.fechaCreacion, ISNULL(cli.observacion, '') observacion
                                                FROM PlClienteOportunidad cli
                                                LEFT JOIN PlUsuario agen ON agen.id = cli.idAgente
                                                INNER JOIN PlUsuario usu ON usu.id = cli.usuarioCreador
                                                WHERE cli.estado = 1 AND cli.documento = @documentoCliente AND cli.idAdministrador = @idAdministrador
                                                ORDER BY cli.fechaCreacion DESC";
                    objCommand.Parameters.AddWithValue("@documentoCliente", documento);
                    objCommand.Parameters.AddWithValue("@idAdministrador", idAdministrador);
                    objCommand.CommandTimeout = Int32.MaxValue;
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    dt.Load(objCommand.ExecuteReader());
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return dt;
    }

//
public bool ActualizarClienteOportunidadInner(int idCliente, string documento, string nombre, string direccion, string telefono, string celular, int idAgenteEntrenador, int usuarioModificador, string observacion, string email)
    {
        bool retorno = false;
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = $@"UPDATE PlClienteOportunidad 
                                                SET documento=@documento, nombre=@nombreCliente, telefono=@telefono, celular=@celular, 
                                                    direccion=@direccion, idAgente={idAgenteEntrenador}, usuarioModificador={usuarioModificador}, fechaModificacion=GETDATE(), 
                                                    observacion=@observacion, email=@email
                                                WHERE id={idCliente}";
                    objCommand.Parameters.AddWithValue("@documento", documento);
                    objCommand.Parameters.AddWithValue("@nombreCliente", nombre);
                    objCommand.Parameters.AddWithValue("@direccion", direccion);
                    objCommand.Parameters.AddWithValue("@telefono", telefono);
                    objCommand.Parameters.AddWithValue("@celular", celular);
                    objCommand.Parameters.AddWithValue("@observacion", observacion);
                    objCommand.Parameters.AddWithValue("@email", email);
                    objCommand.CommandTimeout = Int32.MaxValue;
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    objCommand.ExecuteNonQuery();
                    retorno = true;
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return retorno;
    }

  /* Mantenimiento Clientes Oportunidades */
//
public int InsertarClienteOportunidad(string documento, string nombre, string direccion, string telefono, string celular, int idAgenteEntrenador, int usuarioCreador, int idAdministrador, string observacion, string email)

    {
        int retorno = 0;
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = $"INSERT INTO PlClienteOportunidad VALUES(@documento, @nombreCliente, @telefono, @celular, @direccion, {idAgenteEntrenador}, 1, {usuarioCreador}, GETDATE(), {usuarioCreador}, GETDATE(), {idAdministrador}, @observacion, @email) SELECT @@IDENTITY AS Id";
                    objCommand.Parameters.AddWithValue("@documento", documento);
                    objCommand.Parameters.AddWithValue("@nombreCliente", nombre);
                    objCommand.Parameters.AddWithValue("@direccion", direccion);
                    objCommand.Parameters.AddWithValue("@telefono", telefono);
                    objCommand.Parameters.AddWithValue("@celular", celular);
                    objCommand.Parameters.AddWithValue("@observacion", observacion);
                    objCommand.Parameters.AddWithValue("@email", email);
                    objCommand.CommandTimeout = Int32.MaxValue;
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    DataTable dt = new DataTable();
                    dt.Load(objCommand.ExecuteReader());
                    if (dt.Rows.Count > 0) retorno = Convert.ToInt32(dt.Rows[0]["id"]);
                    sqlConnection1.Close();
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return retorno;
    }
//
 public DataTable ListarOportunidadesPorChatCanalesInner(string codigoBot, string sessionId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
                {
                    using (SqlCommand objCommand = new SqlCommand())
                    {
                        objCommand.CommandText = @"SELECT op.idOportunidad, op.nombreOportunidad, op.descripcion, CAST(op.montoVenta AS DECIMAL(25, 2)) montoVenta, mon.simbolo, 
                                                   tm.descripcion AS estadoOportunidad, ISNULL(emb.descripcionEtapa, '')descripcionEtapa, ISNULL(usu.nombre, '') usuarioAsignado
                                                   FROM PlOportunidad op
                                                   INNER JOIN PlMoneda mon ON mon.id = op.idMoneda
                                                   INNER JOIN PlTablaMaestra tm ON tm.idTabla=8 AND tm.idColumna > 0 AND tm.valor=op.estadoOportunidad
                                                   LEFT JOIN PlEmbudoVenta emb ON emb.id=op.idEtapaVenta
                                                   LEFT JOIN PlUsuario usu ON usu.id = op.usuarioAsignado
                                                   WHERE op.chatSessionId=@sessionId AND op.codigoBot=@codigoBot AND op.estado=1
                                                   ORDER BY op.idOportunidad DESC";
                        objCommand.Parameters.AddWithValue("@sessionId", sessionId);
                        objCommand.Parameters.AddWithValue("@codigoBot", codigoBot);
                        objCommand.CommandTimeout = Int32.MaxValue;
                        objCommand.CommandType = CommandType.Text;
                        objCommand.Connection = sqlConnection1;

                        sqlConnection1.Open();
                        dt.Load(objCommand.ExecuteReader());
                        sqlConnection1.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
            }
            return dt;
        }
//
  public bool CrearEventoCalendario(string horaEvento, string fechaEvento, string descripcion, string notas, string referencia, int mostrarCalendario, int estadoCompletado, int estado, int usuarioCreador, int tipo, string usuarioAsignado, int idUsuarioAsignado, string chatSessionId, string chatCodigoBot)
    {
        bool retorno = false;
        try
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
            {
                using (SqlCommand objCommand = new SqlCommand())
                {
                    objCommand.CommandText = @"INSERT INTO PlEventosCalendario VALUES(GETDATE(), @horaEvento, @fechaEvento, @descripcion, @notas, @referencia, @mostrarCalendario, @estadoCompletado, @estado, @usuarioCreador, @tipo, @idUsuarioAsignado, @usuarioAsignado, @idSesion, @codigoBot)";
                    objCommand.Parameters.AddWithValue("@horaEvento", horaEvento);
                    objCommand.Parameters.AddWithValue("@fechaEvento", fechaEvento);
                    objCommand.Parameters.AddWithValue("@descripcion", descripcion);
                    objCommand.Parameters.AddWithValue("@notas", notas);
                    objCommand.Parameters.AddWithValue("@referencia", referencia);
                    objCommand.Parameters.AddWithValue("@mostrarCalendario", mostrarCalendario);
                    objCommand.Parameters.AddWithValue("@estadoCompletado", estadoCompletado);
                    objCommand.Parameters.AddWithValue("@estado", estado);
                    objCommand.Parameters.AddWithValue("@tipo", tipo);

                    objCommand.Parameters.AddWithValue("@idUsuarioAsignado", idUsuarioAsignado);
                    objCommand.Parameters.AddWithValue("@usuarioAsignado", usuarioAsignado);
                    objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);
                    objCommand.Parameters.AddWithValue("@codigoBot", chatCodigoBot);
                    objCommand.Parameters.AddWithValue("@idSesion", chatSessionId);

                    objCommand.CommandType = CommandType.Text;
                    objCommand.Connection = sqlConnection1;

                    sqlConnection1.Open();
                    objCommand.ExecuteReader();
                    sqlConnection1.Close();

                    retorno = true;
                }
            }
        }
        catch (Exception ex)
        {
            objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
        }
        return retorno;
    }

//
public bool EliminarEventoCalendario(int id)
        {
            bool isDeleted = false;
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
                {
                    using (SqlCommand objCommand = new SqlCommand())
                    {
                        objCommand.CommandText = @"DELETE FROM PLEventosCalendario  WHERE idEvento = @id;";
                        objCommand.Parameters.AddWithValue("@id", id);
                        objCommand.CommandType = CommandType.Text;
                        objCommand.Connection = sqlConnection1;

                        sqlConnection1.Open();
                        dt.Load(objCommand.ExecuteReader());
                        isDeleted = true;
                        sqlConnection1.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
            }
            return isDeleted;
        }
//
 public DataTable ListarUsuarioInner(int idUsuario)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
                {
                    using (SqlCommand objCommand = new SqlCommand())
                    {
                        objCommand.CommandText = @"SELECT usu.id, usu.nombre, usu.codigo, usu.email, usu.nombreImagen, usu.correlativo, DATEADD(HOUR, -5, usu.fechaCreacion)fechaCreacion, usu.idioma,
                                                   usu.idRol, rol.descripcion AS rol, rol.codigo AS codigoRol, usu.usuarioCreador, usu.fActivo, usu.fIngresoSistema, usu.codigoEmpresa, usu.idGrupo,
                                                   ISNULL(grp.descripcion, '') grupoTrabajo, usu.rutaUsuarioImagen AS avatarUsuario, usu.clave, usu.fVerTutorial, usu.mostrarPendientesNuevoChat, usu.habilitarNotificacion,
                                                   usu.zonaHorariaId, usu.horasDescuento, usu.extensionVoip, usu.contraseniaVoip, usu.estado, usu.notificacionAppToken,
                                                   CASE WHEN rol.codigo = 'ADM' THEN 'y' ELSE ISNULL(axr.acceso, 'n') END fAsignarAgentesCanal, usu.razonSocial,
                                                   CASE WHEN rol.codigo = 'ADM' THEN usu.razonSocial ELSE adm.razonSocial END razonSocialAnalisis,
                                                   CASE WHEN rol.codigo = 'ADM' THEN usu.codigoPais ELSE adm.codigoPais END codigoPais, usu.fNotificacionApp, 
                                                   CASE WHEN rol.codigo = 'ADM' THEN ISNULL(repoadm.rutaEmbebida, '') ELSE ISNULL(repo.rutaEmbebida, '') END rutaEmbebida,
                                                   hist.idPlan, pln.codigoPlan, ISNULL(usu.nombreEstacionTrabajo, '')nombreEstacionTrabajo
                                                   FROM PlUsuario usu
                                                   LEFT JOIN PlRol rol ON rol.id = usu.idRol
                                                   LEFT JOIN PlReporteAvanzadoPowerBI repoadm ON repoadm.mailSuper = usu.id
                                                   LEFT JOIN PlAgrupacionUsuarios grp ON grp.id = usu.idGrupo AND grp.estado = 1
                                                   LEFT JOIN PlAccesosxRol axr ON axr.idRol = rol.id AND axr.opcion = 'btnPermitirAgentesCanal'
                                                   LEFT JOIN PlUsuario adm ON adm.id = usu.idAdministrador AND adm.estado = 1
                                                   LEFT JOIN PlReporteAvanzadoPowerBI repo ON repo.mailSuper = adm.id
                                                   LEFT JOIN PlHistorialPlanes hist ON hist.idAdministrador = ISNULL(adm.id, usu.id) AND hist.fActivo = 1
                                                   LEFT JOIN PlPlanes pln ON pln.id = hist.idPlan
                                                   WHERE usu.estado = 1 AND usu.id = @IdUsuario";
                        objCommand.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        objCommand.CommandType = CommandType.Text;
                        objCommand.Connection = sqlConnection1;

                        sqlConnection1.Open();
                        dt.Load(objCommand.ExecuteReader());
                        sqlConnection1.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
            }
            return dt;
        }

//
  public bool InsertarTareaRetorno(string descripcion, string referencia, string notas, int usuarioAsignado, string fechaTarea, int mostrarcalendario, int tipoTarea, int usuarioCreador, int idAdministrador, int aviso, string horaTarea, int idOportunidad, int idCliente, string nombreOportunidad, string nombreCliente, string momentoAviso, string sessionId, string codigoBot)
        {
            bool retorno = false;
            try
            {
                using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
                {
                    using (SqlCommand objCommand = new SqlCommand())
                    {
                        if (string.IsNullOrEmpty(fechaTarea))
                            fechaTarea = "GETDATE()";
                        else
                            fechaTarea = $"'{fechaTarea}'";

                        objCommand.CommandText = $"INSERT INTO PlTareas VALUES(@descripcion, @referencia, @notas, @usuarioAsignado, {fechaTarea}, 0, @mostrarcalendario, @tipoTarea, 1, @usuarioCreador, GETDATE(), @usuarioCreador, GETDATE(), @idAdministrador, @aviso, @horaTarea, @idOportunidad, @idCliente, @nombreOportunidad, @nombreCliente, @momentoAviso, 0, @chatSessionId, @chatCodigoBot)";
                        objCommand.Parameters.AddWithValue("@descripcion", descripcion);
                        objCommand.Parameters.AddWithValue("@referencia", referencia);
                        objCommand.Parameters.AddWithValue("@usuarioAsignado", usuarioAsignado);
                        objCommand.Parameters.AddWithValue("@notas", notas);
                        objCommand.Parameters.AddWithValue("@mostrarcalendario", mostrarcalendario);
                        objCommand.Parameters.AddWithValue("@tipoTarea", tipoTarea);
                        objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);
                        objCommand.Parameters.AddWithValue("@idAdministrador", idAdministrador);
                        objCommand.Parameters.AddWithValue("@aviso", aviso);
                        objCommand.Parameters.AddWithValue("@horaTarea", horaTarea);
                        objCommand.Parameters.AddWithValue("@idOportunidad", idOportunidad);
                        objCommand.Parameters.AddWithValue("@idCliente", idCliente);
                        objCommand.Parameters.AddWithValue("@nombreOportunidad", nombreOportunidad);
                        objCommand.Parameters.AddWithValue("@nombreCliente", nombreCliente);
                        objCommand.Parameters.AddWithValue("@momentoAviso", momentoAviso);
                        objCommand.Parameters.AddWithValue("@chatSessionId", sessionId);
                        objCommand.Parameters.AddWithValue("@chatCodigoBot", codigoBot);
                        objCommand.CommandType = CommandType.Text;
                        objCommand.Connection = sqlConnection1;

                        sqlConnection1.Open();
                        objCommand.ExecuteNonQuery();
                        retorno = true;
                        sqlConnection1.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
                throw;
            }
            return retorno;
        }
//
 public bool ActualizarTareasInner(int id, string descripcion, string referencia, string notas, string fechaTarea, int mostrarCalendario, int usuarioMod, int usuarioAsignado, int aviso, string horaTarea, int tipoTarea, int idOportunidad, int idCliente, string nombreOportunidad, string nombreCliente, string momentoAvisoTarea)
        {
            bool retorno = false;
            try
            {
                using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
                {
                    using (SqlCommand objCommand = new SqlCommand())
                    {
                        objCommand.CommandText = @"UPDATE PlTareas
                                                   SET descripcion = @descripcion,
                                                       referencia = @referencia ,
                                                       notas = @notas ,
                                                       fechaTarea = @fechaTarea ,
                                                       usuarioAsignado = @usuarioAsignado,
                                                       mostrarCalendario = @mostrarCalendario ,
                                                       usuarioModificador=@usuarioModificador ,
                                                       envioCorreo = @aviso,
                                                       tipoTarea = @tipoTarea,
                                                       horaTarea = @horaTarea,
                                                       idOportunidad = @idOportunidad,
                                                       idCliente = @idCliente,
                                                       nombreOportunidad = @nombreOportunidad,
                                                       nombreCliente = @nombreCliente,
                                                       momentoAviso=@momentoAvisoTarea,
                                                       fCorreoEnviado=0
                                                   WHERE id = @id";
                        objCommand.Parameters.AddWithValue("@id", id);
                        objCommand.Parameters.AddWithValue("@descripcion", descripcion);
                        objCommand.Parameters.AddWithValue("@usuarioAsignado", usuarioAsignado);
                        objCommand.Parameters.AddWithValue("@referencia", referencia);
                        objCommand.Parameters.AddWithValue("@notas", notas);
                        objCommand.Parameters.AddWithValue("@fechaTarea", fechaTarea);
                        objCommand.Parameters.AddWithValue("@mostrarCalendario", mostrarCalendario);
                        objCommand.Parameters.AddWithValue("@usuarioModificador", usuarioMod);
                        objCommand.Parameters.AddWithValue("@aviso", aviso);
                        objCommand.Parameters.AddWithValue("@momentoAvisoTarea", momentoAvisoTarea);
                        objCommand.Parameters.AddWithValue("@horaTarea", horaTarea);
                        objCommand.Parameters.AddWithValue("@tipoTarea", tipoTarea);
                        objCommand.Parameters.AddWithValue("@idOportunidad", idOportunidad);
                        objCommand.Parameters.AddWithValue("@idCliente", idCliente);
                        objCommand.Parameters.AddWithValue("@nombreOportunidad", nombreOportunidad);
                        objCommand.Parameters.AddWithValue("@nombreCliente", nombreCliente);
                        objCommand.CommandType = CommandType.Text;
                        objCommand.Connection = sqlConnection1;

                        sqlConnection1.Open();
                        objCommand.ExecuteNonQuery();
                        retorno = true;
                        sqlConnection1.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
            }
            return retorno;
        }
//
public DataTable ListarUsuarioInner(int idUsuario)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
                {
                    using (SqlCommand objCommand = new SqlCommand())
                    {
                        objCommand.CommandText = @"SELECT usu.id, usu.nombre, usu.codigo, usu.email, usu.nombreImagen, usu.correlativo, DATEADD(HOUR, -5, usu.fechaCreacion)fechaCreacion, usu.idioma,
                                                   usu.idRol, rol.descripcion AS rol, rol.codigo AS codigoRol, usu.usuarioCreador, usu.fActivo, usu.fIngresoSistema, usu.codigoEmpresa, usu.idGrupo,
                                                   ISNULL(grp.descripcion, '') grupoTrabajo, usu.rutaUsuarioImagen AS avatarUsuario, usu.clave, usu.fVerTutorial, usu.mostrarPendientesNuevoChat, usu.habilitarNotificacion,
                                                   usu.zonaHorariaId, usu.horasDescuento, usu.extensionVoip, usu.contraseniaVoip, usu.estado, usu.notificacionAppToken,
                                                   CASE WHEN rol.codigo = 'ADM' THEN 'y' ELSE ISNULL(axr.acceso, 'n') END fAsignarAgentesCanal, usu.razonSocial,
                                                   CASE WHEN rol.codigo = 'ADM' THEN usu.razonSocial ELSE adm.razonSocial END razonSocialAnalisis,
                                                   CASE WHEN rol.codigo = 'ADM' THEN usu.codigoPais ELSE adm.codigoPais END codigoPais, usu.fNotificacionApp, 
                                                   CASE WHEN rol.codigo = 'ADM' THEN ISNULL(repoadm.rutaEmbebida, '') ELSE ISNULL(repo.rutaEmbebida, '') END rutaEmbebida,
                                                   hist.idPlan, pln.codigoPlan, ISNULL(usu.nombreEstacionTrabajo, '')nombreEstacionTrabajo
                                                   FROM PlUsuario usu
                                                   LEFT JOIN PlRol rol ON rol.id = usu.idRol
                                                   LEFT JOIN PlReporteAvanzadoPowerBI repoadm ON repoadm.mailSuper = usu.id
                                                   LEFT JOIN PlAgrupacionUsuarios grp ON grp.id = usu.idGrupo AND grp.estado = 1
                                                   LEFT JOIN PlAccesosxRol axr ON axr.idRol = rol.id AND axr.opcion = 'btnPermitirAgentesCanal'
                                                   LEFT JOIN PlUsuario adm ON adm.id = usu.idAdministrador AND adm.estado = 1
                                                   LEFT JOIN PlReporteAvanzadoPowerBI repo ON repo.mailSuper = adm.id
                                                   LEFT JOIN PlHistorialPlanes hist ON hist.idAdministrador = ISNULL(adm.id, usu.id) AND hist.fActivo = 1
                                                   LEFT JOIN PlPlanes pln ON pln.id = hist.idPlan
                                                   WHERE usu.estado = 1 AND usu.id = @IdUsuario";
                        objCommand.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        objCommand.CommandType = CommandType.Text;
                        objCommand.Connection = sqlConnection1;

                        sqlConnection1.Open();
                        dt.Load(objCommand.ExecuteReader());
                        sqlConnection1.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
            }
            return dt;
        }

//
 public DataTable ListarAccesoInnerxUsuario(int idUsuario, string opcionAcceso)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
                {
                    using (SqlCommand objCommand = new SqlCommand())
                    {
                        objCommand.CommandText = $@"SELECT usu.id, usu.nombre, usu.codigo, usu.email, usu.nombreImagen, axr.opcion, axr.acceso 
                                                    FROM PlUsuario usu
                                                    INNER JOIN PlAccesosxRol axr ON axr.idRol = usu.idRol AND axr.opcion='{ opcionAcceso }'
                                                    WHERE id=@idUsuario";
                        objCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
                        objCommand.CommandTimeout = Int32.MaxValue;
                        objCommand.CommandType = CommandType.Text;
                        objCommand.Connection = sqlConnection1;

                        sqlConnection1.Open();
                        dt.Load(objCommand.ExecuteReader());
                        sqlConnection1.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
            }
            return dt;
        }
//
  public List<dynamic> ListarTareas_List(int idUsuario, int horasRestar, int idUsuarioAdm = 0)
        {
            var ListaRetorno = new List<dynamic>();
            try
            {
                DataTable dtTarea = ListarTareas(idUsuario, idUsuarioAdm);
                if (dtTarea.Rows.Count > 0)
                {
                    for (int op = 0; op < dtTarea.Rows.Count; op++)
                    {

                        ListaRetorno.Add(new
                        {
                            Id = Convert.ToInt32(dtTarea.Rows[op]["id"]),
                            Descripcion = dtTarea.Rows[op]["descripcion"].ToString(),
                            Referencia = dtTarea.Rows[op]["referencia"].ToString(),
                            Notas = dtTarea.Rows[op]["notas"].ToString(),
                            NombreTarea = dtTarea.Rows[op]["nombretarea"].ToString(),
                            idEstadoCompletado = Convert.ToInt32(dtTarea.Rows[op]["estadoCompletado"]),
                            UsuarioAsginado = dtTarea.Rows[op]["usuarioAsignado"].ToString(),
                            estadoCompletado = dtTarea.Rows[op]["estadotarea"].ToString(),
                            IdUsuarioAsignado = Convert.ToInt32(dtTarea.Rows[op]["idUsuarioAsignado"]),
                            mostrarCalendario = dtTarea.Rows[op]["mostrarcalendario"].ToString(),
                            tipoTarea = Convert.ToInt32(dtTarea.Rows[op]["tipoTarea"]),
                            horaTarea = dtTarea.Rows[op]["horaTarea"].ToString(),
                            FechaTarea = Convert.ToDateTime(dtTarea.Rows[op]["fechaTarea"]).ToString("dd/MM/yyyy"),
                            FechaCreacion = Convert.ToDateTime(dtTarea.Rows[op]["fechaCreacion"]).AddHours(horasRestar).ToString("dd/MM/yyyy hh:mm tt"),
                            idCliente = (dtTarea.Rows[op]["idCliente"] == DBNull.Value) ? 0 : Convert.ToInt32(dtTarea.Rows[op]["idCliente"]),
                            idOportunidad = (dtTarea.Rows[op]["idOportunidad"] == DBNull.Value) ? 0 : Convert.ToInt32(dtTarea.Rows[op]["idOportunidad"]),
                            nombreOportunidad = (dtTarea.Rows[op]["nombreOportunidad"] == DBNull.Value) ? "" : dtTarea.Rows[op]["nombreOportunidad"].ToString(),
                            nombreCliente = (dtTarea.Rows[op]["nombreCliente"] == DBNull.Value) ? "" : dtTarea.Rows[op]["nombreCliente"].ToString(),
                            MomentoAviso = (dtTarea.Rows[op]["momentoAviso"] == DBNull.Value) ? "horadelevento" : dtTarea.Rows[op]["momentoAviso"].ToString(),
                            Aviso = dtTarea.Rows[op]["enviocorreo"].ToString()
                        }); ;
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
            }
            return ListaRetorno;
        }
//
public bool EliminarTareaInner(int idTarea)
        {
            bool retorno = false;
            try
            {
                using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
                {
                    using (SqlCommand objCommand = new SqlCommand())
                    {
                        objCommand.CommandText = @"Update pltareas set estado = 0 where id = @idtarea";
                        objCommand.Parameters.AddWithValue("@idtarea", idTarea);
                        objCommand.CommandType = CommandType.Text;
                        objCommand.Connection = sqlConnection1;

                        sqlConnection1.Open();
                        objCommand.ExecuteNonQuery();
                        retorno = true;
                        sqlConnection1.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
            }
            return retorno;
        }
//
public DataTable ListarCarpetasBandejaCorreoIntegracion(int idUsuario)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
                {
                    using (SqlCommand objCommand = new SqlCommand())
                    {
                        objCommand.CommandText = "SELECT * FROM PlMaestroCarpetasCorreo WHERE idUsuario = @idUsuario ORDER BY fechaCreacion ASC";
                        //objCommand.Parameters.AddWithValue("@usuarioCreador", usuarioCreador);
                        objCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
                        objCommand.CommandType = CommandType.Text;
                        objCommand.Connection = sqlConnection1;

                        sqlConnection1.Open();
                        dt.Load(objCommand.ExecuteReader());
                        sqlConnection1.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
            }
            return dt;
        }
//
public DataTable GetListadoCorreosParaInput(int idAdministrador)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection sqlConnection1 = new SqlConnection(objConn))
                {
                    using (SqlCommand objCommand = new SqlCommand())
                    {
                        objCommand.CommandText = "SELECT u.nombre, u.email, u.id FROM PlUsuario u WHERE idAdministrador = @idAministrador;";
                        objCommand.Parameters.AddWithValue("@idAministrador", idAdministrador);
                        objCommand.CommandType = CommandType.Text;
                        objCommand.Connection = sqlConnection1;

                        sqlConnection1.Open();
                        dt.Load(objCommand.ExecuteReader());
                        sqlConnection1.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.InsertarLog(System.Reflection.MethodInfo.GetCurrentMethod().ToString(), ex.Message);
            }
            return dt;
        }

