using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace geteam.fancy.web.api.Controllers
{
    public class AuthorizationController : ControllerBase
    {
        // Obtiene un nuevo access token
        public HttpResponseMessage Get()
        {
            //variables del encabezado requeridos para la autenticación
            List<string> requiredHeaders = new List<string>(new[] { "client_id", "username", "password" });
            //valida si la solicitud tienen los encabezados requeridos. Retorna la lista de encabezados faltantes
            requiredHeaders = this.ValidateRequiredHeaders(requiredHeaders);
            if (requiredHeaders.Count > 0)
            {
                //responder con el mensaje de error, listando las variables que faltan
                this.ResponseModel.SetPerformsStatus(LGzE.Models.PerformsStatus.danger);
                this.ResponseModel.SetMessage(string.Join(",", requiredHeaders) + " is REQUIRED'}");
                return this.JsonResponse(this.ToJson(this.ResponseModel));
            }

            //obtener el client_id del encabezado
            string client_id = this.HttpHeaders.GetValues("client_id").First();
            //instanciar el cliente
            models.security.AuthorizationClientsModel client = new models.security.AuthorizationClientsModel(this.ConnectionString);
            //asignar al modelo el client_id
            client.Attr("client_id", client_id);
            //leer la información del cliente
            client.Performs("read");
            //verificar si la operación no fue exitosa
            if (client.PerformStatus != LGzE.Models.PerformsStatus.success)
            {
                //establecer el valor del estado
                this.ResponseModel.SetPerformsStatus(LGzE.Models.PerformsStatus.danger);
                //establecer el mensaje de error
                this.ResponseModel.SetMessage("invalid client_id");

                this.ResponseModel.Attr("_status", "danger");
                this.ResponseModel.Attr("_message", "invalid client_id");
                //responder la solicitud
                return this.JsonResponse(this.ToJson(this.ResponseModel));
            }

            //instanciar la clase para los usuarios
            models.users.UsersModel user = new models.users.UsersModel(this.ConnectionString);
            //establecer usuario
            user.Attr("name", this.HttpHeaders.GetValues("username").First());
            //establecer contraseña
            user.Attr("password", this.HttpHeaders.GetValues("password").First());

            //intentar hacer el inicio de sesión
            user.Performs("login");
            //si no fue exitoso el inicio de sesión
            if (user.PerformStatus != LGzE.Models.PerformsStatus.success)
            {
                //establecer el valor del estado
                this.ResponseModel.SetPerformsStatus(LGzE.Models.PerformsStatus.danger);
                //establecer el mensaje de error
                this.ResponseModel.SetMessage("invalid credentials");

                this.ResponseModel.Attr("_status", "danger");
                this.ResponseModel.Attr("_message", "invalid credentials");

                //responder la solicitud
                return this.JsonResponse(this.ToJson(this.ResponseModel));
            }

            //instanciar la clase para los tokens
            models.security.AuthorizationTokensModel token = new models.security.AuthorizationTokensModel(this.ConnectionString);

            //asignar el cliente
            token.Client = client;
            //asignar el usuario
            token.User = user;
            //generar un nuevo token (lo crea en la base de datos)
            token.NewToken();
            //si no fue exitosa la creación del token
            if (token.PerformStatus != LGzE.Models.PerformsStatus.success)
            {
                //establecer el valor del estado
                this.ResponseModel.SetPerformsStatus(LGzE.Models.PerformsStatus.danger);
                //establecer el mensaje de error
                this.ResponseModel.SetMessage(token.Message);

                this.ResponseModel.Attr("_status", "danger");
                this.ResponseModel.Attr("_message", token.Message);

                //responder la solicitud
                return this.JsonResponse(this.ToJson(this.ResponseModel));
            }

            //para finalizar, devolver la información del nuevo token
            return this.JsonResponse(this.ToJson(token));

        }

        // Obtiene la información del access token especificado en id
        public HttpResponseMessage Get(string id)
        {
            List<string> requiredHeaders = new List<string>(new[] { "client_id", "username" });

            requiredHeaders = this.ValidateRequiredHeaders(requiredHeaders);
            if (requiredHeaders.Count > 0)
            {
                //responder con el mensaje de error, listando las variables que faltan
                this.ResponseModel.SetPerformsStatus(LGzE.Models.PerformsStatus.danger);
                this.ResponseModel.SetMessage(string.Join(",", requiredHeaders) + " is REQUIRED'}");
                return this.JsonResponse(this.ToJson(this.ResponseModel));
            }

            string client_id = this.HttpHeaders.GetValues("client_id").First();
            string username = this.HttpHeaders.GetValues("username").First();
            models.users.UsersModel user = new models.users.UsersModel(this.ConnectionString);
            models.security.AuthorizationClientsModel client = new models.security.AuthorizationClientsModel(this.ConnectionString);
            models.security.AuthorizationTokensModel token = new models.security.AuthorizationTokensModel(this.ConnectionString);
            token.Attr("access_token", id);
            token.Attr("client_id", client_id);
            token.Attr("_user", username);
            token.Performs("info");


            return this.JsonResponse(this.ToJson(token));
        }

        // Retorna la información del token y renueva su vigencia
        public HttpResponseMessage Post()
        {
            List<string> requiredHeaders = new List<string>(new[] { "client_id", "username", "access_token" });

            requiredHeaders = this.ValidateRequiredHeaders(requiredHeaders);
            if (requiredHeaders.Count > 0)
            {
                //responder con el mensaje de error, listando las variables que faltan
                this.ResponseModel.SetPerformsStatus(LGzE.Models.PerformsStatus.danger);
                this.ResponseModel.SetMessage(string.Join(",", requiredHeaders) + " is REQUIRED'}");
                return this.JsonResponse(this.ToJson(this.ResponseModel));
            }

            string client_id = this.HttpHeaders.GetValues("client_id").First();
            string username = this.HttpHeaders.GetValues("username").First();
            string access_token = this.HttpHeaders.GetValues("access_token").First();

            models.users.UsersModel user = new models.users.UsersModel(this.ConnectionString);
            models.security.AuthorizationClientsModel client = new models.security.AuthorizationClientsModel(this.ConnectionString);
            models.security.AuthorizationTokensModel token = new models.security.AuthorizationTokensModel(this.ConnectionString);
            token.Attr("access_token", access_token);
            token.Attr("client_id", client_id);
            token.Attr("_user", username);
            token.Performs("refresh");

            return this.JsonResponse(this.ToJson(token), Encoding.UTF8, token.PerformStatus == LGzE.Models.PerformsStatus.success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);

        }

    }
}
