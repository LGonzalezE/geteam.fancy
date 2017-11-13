using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace geteam.fancy.web.api.Controllers
{
    public class UsersController : ControllerBase
    {
        
        // GET api/<controller>/5
        public async System.Threading.Tasks.Task<HttpResponseMessage> Get(int id)
        {

            await this.ValidateToken();

            if (this.Token.PerformStatus != LGzE.Models.PerformsStatus.success)
            {
                return this.JsonResponse(this.ResponseModel);
            }

            geteam.fancy.models.users.UsersModel user = new geteam.fancy.models.users.UsersModel(this.ConnectionString);
            user.Attr("_user", this.Token["user_id"]);
            user.Attr("user_id", id);
            user.Query("read");
            return this.JsonResponse(user.ResultSet);

        }

        // POST api/<controller>
        public async System.Threading.Tasks.Task<HttpResponseMessage> Post()
        {

            await this.ValidateToken();

            if (this.Token.PerformStatus != LGzE.Models.PerformsStatus.success)
            {
                return this.JsonResponse(this.ResponseModel);
            }

            string perform = string.Empty;
            perform = this.HttpForm["_perform"];
            geteam.fancy.models.users.UsersModel user = new geteam.fancy.models.users.UsersModel(this.ConnectionString);
            geteam.fancy.models.users.RolesModel role = new geteam.fancy.models.users.RolesModel(this.ConnectionString);
            user.Attr("_user", this.Token["user_id"]);
            role.Attr("_user", this.Token["user_id"]);

            switch (perform)
            {
                case "new":                    
                    user.Performs("new");
                    return this.JsonResponse(user);
                case "create":
                    this.SetAttribs(user);
                    user.Performs("create");
                    return this.JsonResponse(user);
                case "update":
                    this.SetAttribs(user);
                    user.Performs("update");
                    return this.JsonResponse(user);
                case "search":
                    user.Attr("_keywords", this.HttpForm["_keywords"]);
                    user.Attr("_page_size", this.HttpForm["_page_size"]);
                    user.Query("search");
                    return this.JsonResponse(user.ResultSet);
                case "roles":
                    role.Attr("_keywords", this.HttpForm["_keywords"]);
                    role.Attr("_page_size", this.HttpForm["_page_size"]);
                    role.Query("all");
                    return this.JsonResponse(role.ResultSet);
                case "my.account.update":
                    this.SetAttribs(user);
                    user.Performs("my.account.update");
                    return this.JsonResponse(user);
                default:
                    user.Query("");
                    return this.JsonResponse(user.ResultSet);
            }



        }

    }
}