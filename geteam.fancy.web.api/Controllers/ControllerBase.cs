using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace geteam.fancy.web.api.Controllers
{
    public class ControllerBase : ApiController
    {
        /// <summary>
        /// Obtiene la información del Token luego del llamado a la función
        /// TokenInfo()
        /// </summary>
        protected models.security.AuthorizationTokensModel Token { get; private set; }
        /// <summary>
        /// Obtiene o establece los datos de respuesta a la solucitud
        /// </summary>
        private LGzE.Models.BaseModel responseModel;
        protected LGzE.Models.BaseModel ResponseModel
        {
            get
            {
                if (this.responseModel == null)
                {
                    this.responseModel = new LGzE.Models.BaseModel("");
                }
                return this.responseModel;
            }
        }

        /// <summary>
        /// Obtiene la cadena de conexión a la base de datos establecida
        /// en el Web.config
        /// </summary>
        protected string ConnectionString
        {
            get
            {
                string cnnString = Properties.Settings.Default.ConnectionString;
                return cnnString;
            }
        }

        /// <summary>
        /// Obtiene el System.Web.HttpRequest de la solucitud
        /// </summary>
        protected System.Web.HttpRequest HttpRequest
        {

            get
            {
                return System.Web.HttpContext.Current.Request;
            }
        }

        /// <summary>
        /// Obtiene el System.Web.HttpContext.Current.HttpResponse de la sesión actual
        /// </summary>
        protected System.Web.HttpResponse HttpResponse
        {

            get
            {
                return System.Web.HttpContext.Current.Response;
            }
        }

        /// <summary>
        /// Obtiene el System.Web.HttpContext.Current.Request.Form para el acceso
        /// a los datos del fromulario para la solicitud
        /// </summary>
        protected System.Collections.Specialized.NameValueCollection HttpForm
        {
            get
            {

                return System.Web.HttpContext.Current.Request.Form;
            }
        }

        /// <summary>
        /// Obtiene el System.Net.Http.Headers.HttpRequestHeaders para acceso
        /// a los encabezados de la solicitud
        /// </summary>
        protected System.Net.Http.Headers.HttpRequestHeaders HttpHeaders
        {
            get
            {
                return Request.Headers;
            }
        }


        /// <summary>
        /// Convierte un System.Data.DataTable a un string en formato JSON
        /// </summary>
        /// <param name="dataTable">DataTable a convertir</param>
        /// <returns>Un string con los datos en formato JSON</returns>
        protected string ToJson(System.Data.DataTable dataTable)
        {
            System.Data.DataSet dataSet = new System.Data.DataSet();
            dataSet.Tables.Add(dataTable);
            string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            return json;
        }
        /// <summary>
        /// Convierte un System.Data.DataSet a un string en formato JSON
        /// </summary>
        /// <param name="dataSet">DataSet a convertir</param>
        /// <returns>Un string con las tablas y datos en formato JSON</returns>
        protected string ToJson(DataSet dataSet)
        {
            string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            return json;
        }

        /// <summary>
        /// Convierte un System.Data.DataTable a formato JSON incluyendo
        /// sólo los campos especificados en el parametro fields
        /// </summary>
        /// <param name="dataTable">System.Data.DataTable a convertir</param>
        /// <param name="fields">Campos a incluir en la conversión</param>
        /// <returns>string en formato JSON</returns>
        protected string ToJson(System.Data.DataTable dataTable, List<string> fields)
        {

            for (var i = dataTable.Columns.Count - 1; i >= 0; i--)
            {
                if (!fields.Contains(dataTable.Columns[i].ColumnName))
                    dataTable.Columns.Remove(dataTable.Columns[i].ColumnName);
            }

            System.Data.DataSet dataSet = new System.Data.DataSet();
            dataSet.Tables.Add(dataTable);

            string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            return json;
        }

        /// <summary>
        /// Convierte un LGzE.Models.BaseModel a un string en formato JSON con los
        /// atributos del modelo
        /// </summary>
        /// <param name="model">LGzE.Models.BaseModel a convertir</param>
        /// <param name="fields">Atributos a incluir en la conversión</param>
        /// <returns>string con los atributos en formato JSON</returns>
        protected string ToJson(LGzE.Models.BaseModel model, List<string> fields = null)
        {

            string json = JsonConvert.SerializeObject(model.AttribsToDataSet(fields), Formatting.None);
            return json;
        }

        /// <summary>
        /// Conviert un string en formato JSON a un DataTable
        /// </summary>
        /// <param name="json">string en formato JSON</param>
        /// <param name="dataTable">DataTable para asignar la conversión</param>
        /// <returns>DataTable con el resultado de la conversión</returns>
        protected DataTable JsonTo(string json, DataTable dataTable)
        {
            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);
            if (dataSet.Tables.Count > 0)
                dataTable = dataSet.Tables[0];
            else
                dataTable = null;

            return dataTable;
        }

        /// <summary>
        /// Conviert un string en formato JSON a un DataSet
        /// </summary>
        /// <param name="json">string en formato JSON</param>
        /// <param name="dataTable">DataSet para asignar la conversión</param>
        /// <returns>DataSet con el resultado de la conversión</returns>
        protected DataSet JsonTo(string json, DataSet dataSet)
        {
            dataSet = JsonConvert.DeserializeObject<DataSet>(json);

            return dataSet;
        }

        protected void SetAttribs(LGzE.Models.BaseModel model)
        {
            foreach (string name in model.AttribsNames())
            {
                if (this.HttpForm.AllKeys.Contains(name))
                {
                    model.Attr(name, this.HttpForm[name]);
                }
            }
        }

        /// <summary>
        /// Valida que en el Request contenga los encabezados (Headers)
        /// especificados en keys
        /// </summary>
        /// <param name="keys">Lista con los nombres de los encabezados</param>
        /// <returns>Retorna una lista con encabezados no encontrados o faltantes</returns>
        protected List<string> ValidateRequiredHeaders(List<string> keys)
        {
            List<string> notFoundKeys = new List<string>();

            foreach (string k in keys)
            {
                if (!this.HttpHeaders.Contains(k))
                {
                    notFoundKeys.Add(k);
                }
            }
            return notFoundKeys;
        }

        /// <summary>
        /// Genera una HttpResponseMessage a partir de un string en formato JSON
        /// </summary>
        /// <param name="json">string con los datos en formato JSON</param>
        /// <param name="encoding">Codificación para la respuesta</param>
        /// <param name="status">Estado HTTP para la respuesta</param>
        /// <returns>HttpResponseMessage con el string en formato JSON</returns>
        protected HttpResponseMessage JsonResponse(string json, Encoding encoding, System.Net.HttpStatusCode status)
        {
            HttpResponseMessage response = new HttpResponseMessage()
            {
                Content = new StringContent(json, encoding, "application/json")
            };
            response.StatusCode = status;
            return response;
        }

        /// <summary>
        /// Genera una HttpResponseMessage a partir de un string en formato JSON,
        /// con HttpStatusCode.OK por defecto
        /// </summary>
        /// <param name="json">string con los datos en formato JSON</param>
        /// <param name="encoding">Codificación para la respuesta</param>
        /// <returns>HttpResponseMessage con el string en formato JSON</returns>
        protected HttpResponseMessage JsonResponse(string json, Encoding encoding)
        {
            return this.JsonResponse(json, encoding, System.Net.HttpStatusCode.OK);

        }

        /// <summary>
        /// Genera una HttpResponseMessage a partir de un string en formato JSON
        /// con un Encoding.UTF8 por defecto
        /// </summary>
        /// <param name="json">string con los datos en formato JSON</param>
        /// <param name="status">Estado HTTP para la respuesta</param>
        /// <returns>HttpResponseMessage con el string en formato JSON</returns>
        protected HttpResponseMessage JsonResponse(string json, System.Net.HttpStatusCode status)
        {
            HttpResponseMessage response = new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            response.StatusCode = status;
            return response;
        }


        /// <summary>
        /// Genera una HttpResponseMessage a partir de un string en formato JSON
        /// con un Encoding.UTF8 y HttpResponse.OK por defecto
        /// </summary>
        /// <param name="json">string con los datos en formato JSON</param>
        /// <returns>HttpResponseMessage con el string en formato JSON</returns>  
        protected HttpResponseMessage JsonResponse(string json)
        {
            return this.JsonResponse(json, Encoding.UTF8, System.Net.HttpStatusCode.OK);
        }


        /// <summary>
        /// Genera una HttpResponseMessage a partir de un LGzE.Models.BaseModel en formato JSON
        /// con un Encoding.UTF8 y HttpResponse.OK por defecto
        /// </summary>
        /// <param name="json">string con los datos en formato JSON</param>
        /// <returns>HttpResponseMessage con el string en formato JSON</returns>  
        protected HttpResponseMessage JsonResponse(LGzE.Models.BaseModel model)
        {

            LGzE.Models.BaseModel header = new LGzE.Models.BaseModel();
            header.Attr("status", model.PerformStatus.ToString().ToLower());
            header.Attr("message", model.Message);

            DataSet dataSet = new DataSet();
            DataTable dataTable = header.AttribsToTable();
            dataTable.TableName = "header";
            dataSet.Tables.Add(dataTable);
            dataTable = model.AttribsToTable();
            dataTable.TableName = "data";
            dataSet.Tables.Add(dataTable);

            return this.JsonResponse(this.ToJson(dataSet), Encoding.UTF8, System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Genera una HttpResponseMessage a partir de un LGzE.Models.BaseModel en formato JSON
        /// con en una estructura de respuesta header, data. 
        /// </summary>
        /// <param name="status">estado de la operación a incluir header</param>
        /// <param name="message">mensaje de la operación a incluir header</param>
        /// <returns>HttpResponseMessage con el string en formato JSON</returns>  
        protected HttpResponseMessage JsonResponse(DataTable dataTable, string status, string message)
        {

            LGzE.Models.BaseModel header = new LGzE.Models.BaseModel();
            header.Attr("status", status);
            header.Attr("message", message);

            DataSet dataSet = new DataSet();
            DataTable dataTableH = header.AttribsToTable();
            dataTableH.TableName = "header";
            dataSet.Tables.Add(dataTableH);
            dataTable.TableName = "data";
            dataSet.Tables.Add(dataTable);

            return this.JsonResponse(this.ToJson(dataSet), Encoding.UTF8, System.Net.HttpStatusCode.OK);
        }


        /// <summary>
        /// Genera una HttpResponseMessage a partir de un DataTable en formato JSON
        /// con en una estructura de respuesta header (con estado y mensaje 'success'), data.  
        /// </summary>
        /// <returns>HttpResponseMessage con el string en formato JSON</returns>  
        protected HttpResponseMessage JsonResponse(DataTable dataTable)
        {
            return this.JsonResponse(dataTable, "success", "success");

        }

        /// <summary>
        /// Realiza un solicitud asíncrona al servicio de autorización para obtener la información del Token actual
        /// y lo almacena en el Token
        /// </summary>
        /// <param name="refresh">Determinar si la solicitud incluye el refrescamiento del token (tiempo de vigencia)</param>
        /// <returns>retorna el models.security.AuthorizationTokensModel con la información del token</returns>
        protected async System.Threading.Tasks.Task<models.security.AuthorizationTokensModel> TokenInfo(bool refresh = false)
        {
            //Creaa un nueva instancia con conexión a la base de datos
            this.Token = new models.security.AuthorizationTokensModel(this.ConnectionString);
            //Crea una lista con los encabezados por defecto
            List<string> requiredHeaders = new List<string>(new[] { "client_id", "username", "access_token" });

            //valida sin cumple con los encabezados por defecto
            requiredHeaders = this.ValidateRequiredHeaders(requiredHeaders);
            //si la lista contiene encabezados
            if (requiredHeaders.Count > 0)
            {
                //responde con error y la lista de encabezados que faltan
                this.Token.SetPerformsStatus(LGzE.Models.PerformsStatus.danger);
                this.Token.SetMessage(string.Join(",", requiredHeaders) + " is REQUIRED'}");
                return this.Token;
            }

            //crear un nuevo HttpClient para la solicitud
            HttpClient httpClient = new HttpClient();
            //obtener la url actual de la aplicación
            string url = Url.Content("~/");
            httpClient.BaseAddress = new Uri(url);
            //agregar los encabezados HTTP para la solicitud
            httpClient.DefaultRequestHeaders.Add("client_id", this.HttpHeaders.GetValues("client_id").First());
            httpClient.DefaultRequestHeaders.Add("username", this.HttpHeaders.GetValues("username").First());
            httpClient.DefaultRequestHeaders.Add("access_token", this.HttpHeaders.GetValues("access_token").First());

            string responseContentText = string.Empty;

            HttpResponseMessage response;
            //Asignar al Token el access token en la solicitud
            this.Token.Attr("access_token", this.HttpHeaders.GetValues("access_token").First());

            //determinar el tipo de solicitud
            if (!refresh)
                //Solicita sólo la información del token mediante GET
                response = await httpClient.GetAsync("api/authorization/" + this.Token.AttrToString("access_token"));
            else
            {
                //agregar a los campos del formulario el access token
                Dictionary<string, string> pairs = new Dictionary<string, string>();
                pairs.Add("access_token", this.Token.AttrToString("access_token"));
                FormUrlEncodedContent formContent = new FormUrlEncodedContent(pairs);
                //solicitar la infomación y renovación de la vigencia del token por POST
                response = await httpClient.PostAsync("api/authorization/", formContent);
            }

            //verificar la respuesta a la solicitud
            if (response.StatusCode != HttpStatusCode.OK)
            {
                //Establecer el mensaje de error de la solicitu
                this.Token.SetMessage("authorization service error");
                this.Token.SetPerformsStatus(LGzE.Models.PerformsStatus.danger);

            }
            //leer el contenido de la respuesta
            responseContentText = await response.Content.ReadAsStringAsync();

            //converir la respuesta a un DataTable
            System.Data.DataTable dataTable = new System.Data.DataTable();
            dataTable = this.JsonTo(responseContentText, dataTable);
            this.Token.AttribsFrom(dataTable);

            return this.Token;
        }

        /// <summary>
        /// Realiza la validación del Token actual, mediante solicitud asíncrona al
        /// servicio de autorización
        /// </summary>
        /// <returns>retorna un estado LGzE.Models.PerformsStatus</returns>
        protected async System.Threading.Tasks.Task<LGzE.Models.PerformsStatus> ValidateToken()
        {
            //solicitar la información del token y la renovación de la vigencia
            await this.TokenInfo(true);

            //verificar el estado de la información del token
            if (this.Token.PerformStatus != LGzE.Models.PerformsStatus.success)
            {
                this.ResponseModel.AttribsFrom(this.Token);

                return LGzE.Models.PerformsStatus.danger;
            }

            //verificar si la respuesta contiene la vigencia del token
            if (!this.Token.Contains("expires_in"))
            {
                this.ResponseModel.SetMessage("expires_in is REQUIRED");
                this.ResponseModel.SetPerformsStatus(LGzE.Models.PerformsStatus.danger);
                return LGzE.Models.PerformsStatus.danger;
            }

            //si el token expiró, retorna error
            if (this.Token.AttrToInt32("expires_in") < 0)
            {
                this.ResponseModel.SetMessage("access token expired");
                this.ResponseModel.SetPerformsStatus(LGzE.Models.PerformsStatus.danger);
                return LGzE.Models.PerformsStatus.danger;
            }



            return LGzE.Models.PerformsStatus.success;
        }


    }
}