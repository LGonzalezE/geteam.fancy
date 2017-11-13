using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace geteam.fancy.models.security
{
    public class AuthorizationTokensModel : LGzE.Models.BaseModel
    {
        public AuthorizationClientsModel Client { get; set; }
        public models.users.UsersModel User { get; set; }

        public AuthorizationTokensModel(string connectionstring) : base(connectionstring)
        {
            this.StoredProcedure = "sp_authorization_tokens";
            this.StoredProcedureParamsNames = " _perform, _user, _args, _keywords, _page, _page_size, token_id, client_id, user_id, access_token, scope, createddate, username";
        }

        public AuthorizationTokensModel() : base("")
        {
            this.StoredProcedure = "sp_authorization_tokens";
            this.StoredProcedureParamsNames = " _perform, _user, _args, _keywords, _page, _page_size, token_id, client_id, user_id, access_token, scope, createddate, username";
        }

        public LGzE.Models.PerformsStatus NewToken()
        {

            if (this.Client == null)
            {
                this.Message = "Client is NULL";
                this.PerformStatus = LGzE.Models.PerformsStatus.danger;
                return this.PerformStatus;
            }

            if (!this.Client.Contains("client_id"))
            {
                this.Message = "client_id is REQUIRED";
                this.PerformStatus = LGzE.Models.PerformsStatus.danger;
                return this.PerformStatus;
            }

            if (!this.Client.Contains("secret"))
            {
                this.Message = "secret is REQUIRED";
                this.PerformStatus = LGzE.Models.PerformsStatus.danger;
                return this.PerformStatus;
            }

            
            string access_token = string.Empty;
            access_token = string.Format("{0}{1}", this.Client.AttrToString("client_id"), DateTime.Now.ToString());            
            access_token = this.GetMd5Hash(MD5.Create(), access_token);
            this.Attr("access_token", access_token);
            this.Attr("client_id", this.Client.Attr("client_id"));
            this.Attr("user_id", this.User.Attr("user_id"));

            this.Performs("create");

            
            return this.PerformStatus;
        }

        public void AttribsFrom(DataSet dataSet)
        {
            if (dataSet.Tables.Count > 0)
            {
                this.AttribsFrom(dataSet.Tables[0]);
            }
        }

        private string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
