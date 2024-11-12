using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace WEShipExpress_20241111
{
    public partial class StoredProcedures
    {
        [Microsoft.SqlServer.Server.SqlProcedure(Name = "WEShipExpress_20241111")]
        public static async void OrderTrackingProcedure(SqlString jsonBody)
        {
            DataTable trackingData = new DataTable("trackingData");

            // Define DataTable columns based on JSON structure
            trackingData.Columns.Add("ResponseStatus", typeof(bool));
            trackingData.Columns.Add("ResponseStatusCode", typeof(int));
            trackingData.Columns.Add("OrderNumber", typeof(string));
            trackingData.Columns.Add("CreatedOn", typeof(long));
            trackingData.Columns.Add("UpdatedOn", typeof(long));
            trackingData.Columns.Add("ParentOrder", typeof(string));
            trackingData.Columns.Add("OrderType", typeof(int));
            trackingData.Columns.Add("OrderTypeDesc", typeof(string));
            trackingData.Columns.Add("Status", typeof(int));
            trackingData.Columns.Add("OrderStatusDesc", typeof(string));
            trackingData.Columns.Add("AccountId", typeof(string));
            trackingData.Columns.Add("ClientName", typeof(string));
            trackingData.Columns.Add("CompanyName", typeof(string));
            trackingData.Columns.Add("ShipperId", typeof(int));
            trackingData.Columns.Add("WarehouseLocation", typeof(string));
            trackingData.Columns.Add("UpdatedBy", typeof(int));
            trackingData.Columns.Add("TotalQuantity", typeof(int));
            trackingData.Columns.Add("TotalWeight", typeof(double));
            trackingData.Columns.Add("PaymentMode", typeof(int));
            trackingData.Columns.Add("PaymentStatus", typeof(int));
            trackingData.Columns.Add("ServiceType", typeof(string));
            trackingData.Columns.Add("ShippingMode", typeof(string));
            trackingData.Columns.Add("Lob", typeof(int));
            trackingData.Columns.Add("LobStatus", typeof(string));
            trackingData.Columns.Add("StoreId", typeof(string));
            trackingData.Columns.Add("StoreName", typeof(string));
            trackingData.Columns.Add("Channel", typeof(string));
            trackingData.Columns.Add("ReferenceId", typeof(string));
            trackingData.Columns.Add("TrackingId", typeof(string));
            trackingData.Columns.Add("Carrier", typeof(string));
            trackingData.Columns.Add("LabelURL", typeof(string));
            trackingData.Columns.Add("ErrorMessage", typeof(string));

            //string jsonBodyString = jsonBody.ToString();
            // Example JSON body including accountId and deviceType
            string jsonBodyString = "{ \"accountId\": \"BTV\", \"deviceType\": \"Web\", \"orderNumber\": \"BTV-961304-1of1\" }";

         
            string APIUrl = "https://ws.advatix.net/weship-fep/api/v1/OrderTracking/getOrderTrackingv2";

            try
            {

                SqlContext.Pipe.Send("sono arrivati questi dati:");
                SqlContext.Pipe.Send("------------------------------------------");
                SqlContext.Pipe.Send("jsonBody");
                SqlContext.Pipe.Send(jsonBodyString);
                SqlContext.Pipe.Send("APIUrl ==>" + APIUrl);
                SqlContext.Pipe.Send("------------------------------------------");

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                WebPermission permission = new WebPermission(NetworkAccess.Connect, APIUrl);
                permission.AddPermission(NetworkAccess.Connect, APIUrl);
                permission.AddPermission(NetworkAccess.Accept, APIUrl);
                permission.Demand();

                IEnumerator myConnectEnum = permission.ConnectList;

                while (myConnectEnum.MoveNext())
                { }

                IEnumerator myAcceptEnum = permission.AcceptList;



                // Setup the HTTP request
                HttpWebRequest request = null;
                request = (HttpWebRequest)WebRequest.Create(APIUrl);

                string requestBody = jsonBodyString;
                SqlContext.Pipe.Send("Popolo HTTP request");
                byte[] bytes;
                bytes = Encoding.ASCII.GetBytes(requestBody);
                //request.Headers.Add("Content-Type", content_type);
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Method = "POST";
     

                SqlContext.Pipe.Send("Authorization request" + request);

                request.Headers.Add("Authorization: Basic eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJcdTAwMTFcdTAwMUYtOjlcdTAwMTZcXDZIXCImPlx1MDAwRnxcXFx1MDAxMiglXHJcdDdJXG5qWlBGTlx1MDAxQ1x1MDAwM1xuXFxkXiIsImlzcyI6ImNvbS5hZHZhdGl4LnNlcnZpY2VzIiwiZXhwIjoxNzAyNDUzMzQzLCJpYXQiOjE3MDIzMDkzNDMsImp0aSI6ImUyNzFiZTA3LTI3OWItNDk5ZS05NTNhLThhYmYxMDg0ZGMzMiJ9.X7O2Ta9qzaOszFBk28AMSq2uw85e8xCn2ibkAt3wUkI");

                //request.Headers.Add("Authorization: Bearer eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJcdTAwMTFcdTAwMUYtOjlcdTAwMTZcXDZIXCImPlx1MDAwRnxcXFx1MDAxMiglXHJcdDdJXG5qWlBGTlx1MDAxQ1x1MDAwM1xuXFxkXiIsImlzcyI6ImNvbS5hZHZhdGl4LnNlcnZpY2VzIiwiZXhwIjoxNzAyNDUzMzQzLCJpYXQiOjE3MDIzMDkzNDMsImp0aSI6ImUyNzFiZTA3LTI3OWItNDk5ZS05NTNhLThhYmYxMDg0ZGMzMiJ9.X7O2Ta9qzaOszFBk28AMSq2uw85e8xCn2ibkAt3wUkI");

                SqlContext.Pipe.Send("Authorization request  ==" + request);
                SqlContext.Pipe.Send("Authorization request  ==" + request.Headers["Authorization"]);

                // Write JSON body to request stream            
                Stream requestStream = request.GetRequestStream();
                SqlContext.Pipe.Send("requestStream  ==" + requestStream);

                requestStream.Write(bytes, 0, bytes.Length);

                SqlContext.Pipe.Send("requestStream  ==" + requestStream);
                SqlContext.Pipe.Send("bytes  ==" + bytes.ToString());
                requestStream.Close();

                // Process the HTTP response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    SqlContext.Pipe.Send("response " + response);
                    SqlContext.Pipe.Send("response.StatusCode " + response.StatusCode);
                }
                catch (Exception ex)
                {


                    SqlContext.Pipe.Send("Error details exeption: " + ex.Message);

                }



                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = response.GetResponseStream();
                        string responseStr = new StreamReader(responseStream).ReadToEnd();


                        var jsonObject = JObject.Parse(responseStr);

                        // Parse specific fields from JSON
                        DataRow row = trackingData.NewRow();
                        row["ResponseStatus"] = (bool)jsonObject["responseStatus"];
                        row["ResponseStatusCode"] = (int)jsonObject["responseStatusCode"];
                        row["OrderNumber"] = (string)jsonObject["responseObject"]["orderNumber"]["id"];
                        row["CreatedOn"] = (long)jsonObject["responseObject"]["orderInfo"]["createdOn"];
                        row["UpdatedOn"] = (long)jsonObject["responseObject"]["orderInfo"]["updatedOn"];
                        row["ParentOrder"] = (string)jsonObject["responseObject"]["orderInfo"]["parentOrder"];
                        row["OrderType"] = (int)jsonObject["responseObject"]["orderInfo"]["orderType"];
                        row["OrderTypeDesc"] = (string)jsonObject["responseObject"]["orderInfo"]["orderTypeDesc"];
                        row["Status"] = (int)jsonObject["responseObject"]["orderInfo"]["status"];
                        row["OrderStatusDesc"] = (string)jsonObject["responseObject"]["orderInfo"]["orderStatusDesc"];
                        row["AccountId"] = (string)jsonObject["responseObject"]["orderInfo"]["accountId"];
                        row["ClientName"] = (string)jsonObject["responseObject"]["orderInfo"]["clientName"];
                        row["CompanyName"] = (string)jsonObject["responseObject"]["orderInfo"]["companyName"];
                        row["ShipperId"] = (int)jsonObject["responseObject"]["orderInfo"]["shipperId"];
                        row["WarehouseLocation"] = (string)jsonObject["responseObject"]["orderInfo"]["warehouseLocation"];
                        row["UpdatedBy"] = (int)jsonObject["responseObject"]["orderInfo"]["updatedBy"];
                        row["TotalQuantity"] = (int)jsonObject["responseObject"]["orderInfo"]["totalQuantity"];
                        row["TotalWeight"] = (double)jsonObject["responseObject"]["orderInfo"]["totalWeight"];
                        row["PaymentMode"] = (int)jsonObject["responseObject"]["orderInfo"]["paymentMode"];
                        row["PaymentStatus"] = (int)jsonObject["responseObject"]["orderInfo"]["paymentStatus"];
                        row["ServiceType"] = (string)jsonObject["responseObject"]["orderInfo"]["serviceType"];
                        row["ShippingMode"] = (string)jsonObject["responseObject"]["orderInfo"]["shippingMode"];
                        row["Lob"] = (int)jsonObject["responseObject"]["orderInfo"]["lob"];
                        row["LobStatus"] = (string)jsonObject["responseObject"]["orderInfo"]["lobStatus"];
                        row["StoreId"] = (string)jsonObject["responseObject"]["orderInfo"]["storeId"];
                        row["StoreName"] = (string)jsonObject["responseObject"]["orderInfo"]["storeName"];
                        row["Channel"] = (string)jsonObject["responseObject"]["additionalInfo"]["channel"];
                        row["ReferenceId"] = (string)jsonObject["responseObject"]["additionalInfo"]["referenceId"];
                        row["TrackingId"] = (string)jsonObject["responseObject"]["additionalInfo"]["trackingId"];
                        row["Carrier"] = (string)jsonObject["responseObject"]["additionalInfo"]["carrier"];
                        row["LabelURL"] = (string)jsonObject["responseObject"]["additionalInfo"]["labelUrl"];

                        trackingData.Rows.Add(row);
                    }
                    else
                    {
                        // Log error details if the status code is not OK
                        SqlContext.Pipe.Send("Error: Response status code is " + response.StatusCode);
                        SqlContext.Pipe.Send("Status description: " + response.StatusDescription);

                        // Attempt to read the error response content
                        using (Stream errorStream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(errorStream))
                        {
                            string errorContent = reader.ReadToEnd();
                            SqlContext.Pipe.Send("Error details: " + errorContent);
                        }
                    }
                }
                catch (WebException webEx)
                {
                    // Handle exceptions for network issues or HTTP errors
                    if (webEx.Response is HttpWebResponse errorResponse)
                    {
                        SqlContext.Pipe.Send("Exception: " + errorResponse.StatusCode);
                        SqlContext.Pipe.Send("Status description: " + errorResponse.StatusDescription);

                        using (Stream errorStream = errorResponse.GetResponseStream())
                        using (StreamReader reader = new StreamReader(errorStream))
                        {
                            string errorContent = reader.ReadToEnd();
                            SqlContext.Pipe.Send("Error response content: " + errorContent);
                        }
                    }
                    else
                    {
                        SqlContext.Pipe.Send("An unexpected error occurred: " + webEx.Message);
                    }
                }
                catch (Exception ex)
                {
                    // Catch any other unexpected errors
                    SqlContext.Pipe.Send("General exception occurred: " + ex.Message);
                }


                response.Close();


                SqlContext.Pipe.Send("Sono arrivati questi dati:");
                SqlContext.Pipe.Send("------------------------------------------");
                SqlContext.Pipe.Send("In tutto sono arrivati:");
                SqlContext.Pipe.Send("Righe arrivate: " + trackingData.Rows.Count + "");
                DataRow r = trackingData.Rows[0];

                SqlContext.Pipe.Send("ResponseStatus: " + r["ResponseStatus"].ToString());
                SqlContext.Pipe.Send("ResponseStatusCode: " + r["ResponseStatusCode"].ToString());
                SqlContext.Pipe.Send("OrderNumber: " + r["OrderNumber"].ToString());
                SqlContext.Pipe.Send("CreatedOn: " + r["CreatedOn"].ToString());
                SqlContext.Pipe.Send("UpdatedOn: " + r["UpdatedOn"].ToString());
                SqlContext.Pipe.Send("ParentOrder: " + r["ParentOrder"].ToString());
                SqlContext.Pipe.Send("OrderType: " + r["OrderType"].ToString());
                SqlContext.Pipe.Send("OrderTypeDesc: " + r["OrderTypeDesc"].ToString());
                SqlContext.Pipe.Send("Status: " + r["Status"].ToString());
                SqlContext.Pipe.Send("OrderStatusDesc: " + r["OrderStatusDesc"].ToString());
                SqlContext.Pipe.Send("AccountId: " + r["AccountId"].ToString());
                SqlContext.Pipe.Send("ClientName: " + r["ClientName"].ToString());
                SqlContext.Pipe.Send("CompanyName: " + r["CompanyName"].ToString());
                SqlContext.Pipe.Send("ShipperId: " + r["ShipperId"].ToString());
                SqlContext.Pipe.Send("WarehouseLocation: " + r["WarehouseLocation"].ToString());
                SqlContext.Pipe.Send("UpdatedBy: " + r["UpdatedBy"].ToString());
                SqlContext.Pipe.Send("TotalQuantity: " + r["TotalQuantity"].ToString());
                SqlContext.Pipe.Send("TotalWeight: " + r["TotalWeight"].ToString());
                SqlContext.Pipe.Send("PaymentMode: " + r["PaymentMode"].ToString());
                SqlContext.Pipe.Send("PaymentStatus: " + r["PaymentStatus"].ToString());
                SqlContext.Pipe.Send("ServiceType: " + r["ServiceType"].ToString());
                SqlContext.Pipe.Send("ShippingMode: " + r["ShippingMode"].ToString());
                SqlContext.Pipe.Send("Lob: " + r["Lob"].ToString());
                SqlContext.Pipe.Send("LobStatus: " + r["LobStatus"].ToString());
                SqlContext.Pipe.Send("StoreId: " + r["StoreId"].ToString());
                SqlContext.Pipe.Send("StoreName: " + r["StoreName"].ToString());
                SqlContext.Pipe.Send("Channel: " + r["Channel"].ToString());
                SqlContext.Pipe.Send("ReferenceId: " + r["ReferenceId"].ToString());
                SqlContext.Pipe.Send("TrackingId: " + r["TrackingId"].ToString());
                SqlContext.Pipe.Send("Carrier: " + r["Carrier"].ToString());
                SqlContext.Pipe.Send("LabelURL: " + r["LabelURL"].ToString());
                SqlContext.Pipe.Send("ErrorMessage: " + r["ErrorMessage"].ToString());
                SqlContext.Pipe.Send("------------------------------------------");


                // Insert data into database using stored procedure
                string connString = "Data Source=DESKTOP-FO7B6CB\\SQLEXPRESS01;Initial Catalog=API_Ship_v1;User ID=sample;Password=sample";
                SqlConnection connection = new SqlConnection(connString);
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dbo.Ship_importData_20241111";
                cmd.Connection = connection; // an open SqlConnection
                SqlParameter param = new SqlParameter();
                param.ParameterName = "@DataToInsert";
                param.SqlDbType = SqlDbType.Structured;
                param.Value = trackingData;
                param.TypeName = "dbo.weshipexpresspro_temp_ships";
                cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                /*equivale a print della sp*/
                SqlContext.Pipe.Send("Errore");
                SqlContext.Pipe.Send("Errore");
                SqlContext.Pipe.Send(ex.Message);
                SqlContext.Pipe.Send("------------------------------------------");
                SqlContext.Pipe.Send(ex.StackTrace);
                SqlContext.Pipe.Send("------------------------------------------");
                SqlContext.Pipe.Send(ex.Source);
                trackingData = null;
            }
        }
    }
}
