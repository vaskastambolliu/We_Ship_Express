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
using System.Threading.Tasks;
using API_We_Ship_Express;
using System.Collections.Generic;

namespace WEShipExpress_20241111
{
    public partial class StoredProcedures
    {
        [Microsoft.SqlServer.Server.SqlProcedure(Name = "WEShipExpress_20241111")]
        public static async void OrderTrackingProcedure(SqlString jsonBody)
        {
            // Initialize DataTable with specified structure
            DataTable trackingData = new DataTable("trackingData");
            AddTrackingDataColumns(trackingData);

            string jsonBodyString = jsonBody.ToString();

            string APIUrl = "https://ws-sandbox.advatix.net/weship-fep/api/v1/OrderTracking/getOrderTrackingv2";

            try
            {
                // Log initial information
                SqlContext.Pipe.Send("sono arrivati questi dati:");
                SqlContext.Pipe.Send("------------------------------------------");
                SqlContext.Pipe.Send("jsonBody");
                SqlContext.Pipe.Send(jsonBodyString);
                SqlContext.Pipe.Send("------------------------------------------");


                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;
                WebPermission permission = new WebPermission(NetworkAccess.Connect, APIUrl);
                permission.AddPermission(NetworkAccess.Connect, APIUrl);
                permission.AddPermission(NetworkAccess.Accept, APIUrl);
                permission.Demand();

                IEnumerator myConnectEnum = permission.ConnectList;

                while (myConnectEnum.MoveNext())
                { }

                IEnumerator myAcceptEnum = permission.AcceptList;

                // Configure HTTP request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIUrl);
                string requestBody = jsonBodyString;
                SqlContext.Pipe.Send("Popolo HTTP request");
                byte[] bytes;
                bytes = Encoding.ASCII.GetBytes(requestBody);


                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Method = "POST";
                request.Headers.Add("APIKey", "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJcdTAwMTFcdTAwMUYtOjlcdTAwMTZcXDZIXCImPlx1MDAwRnxcXFx1MDAxMigtXHUwMDE2XHUwMDE0XHUwMDE5XHUwMDA0XGZsXVVFRVx1MDAxNlx1MDAwNVx1MDAwRVtmXmsiLCJpc3MiOiJjb20uYWR2YXRpeC5zZXJ2aWNlcyIsImV4cCI6MTY1NjMzNjQ3MCwiaWF0IjoxNjU2MzI5MjcwLCJqdGk");
                request.Headers.Add("AccountId", "BTV");
                request.Headers.Add("DEVICE-TYPE", "Web");
                request.Headers.Add("VER", "1.0");

                //SetRequestHeaders(request);
                SqlContext.Pipe.Send("request: " + request);

                // Prepare request body
                //string jsonBodyString = "{\"orderNumber\": \"4716039536792\"}";  // replace with actual jsonBody content if needed
                //SqlContext.Pipe.Send("Preparo Stream per chiamata");
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();

                SqlContext.Pipe.Send("requestBody: " + requestBody);
                // Process HTTP response
                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    SqlContext.Pipe.Send("StatusCode: " + response.StatusCode);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        SqlContext.Pipe.Send("StatusCode: " + response.StatusCode);
                        //HandleSuccessfulResponse(response, trackingData);


                        Stream responseStream = null;
                        string responseStr = "";
                        try
                        {
                            responseStream = response.GetResponseStream();
                            responseStr = new StreamReader(responseStream).ReadToEnd();
                        }
                        catch (Exception ex)
                        {
                            SqlContext.Pipe.Send("responseStream ex: " + ex);
                            HandleGeneralException(ex);
                            throw;
                        }

                        // Parse JSON response and populate trackingData
                        //var jsonObject = new JObject();

                        try
                        {

                            //var jsonObject = JObject.Parse(responseStr);
                            //SqlContext.Pipe.Send("arr: " + arr);

                            SendLongMessage("responseStr: " + responseStr);

                            // Only parse the JSON to check the root-level properties
                            var jsonObjecta = JObject.Parse(responseStr);
                            SendLongMessage("jsonObject: " + jsonObjecta);

                            // Check if the JSON string represents an array or an object
                            JToken jsonToken = JToken.Parse(responseStr);
                            SendLongMessage("jsonToken: " + jsonToken);

                            if (jsonToken is JArray jsonArray)
                            {
                                SendLongMessage("Parsed JSON as an array");
                                // Handle JSON array
                                foreach (var item in jsonArray)
                                {
                                    SendLongMessage("Array item: " + item.ToString());
                                }
                            }
                            else if (jsonToken is JObject jsonObject)
                            {
                                SendLongMessage("Parsed JSON as an object");
                                // Handle JSON object
                                foreach (var property in jsonObject.Properties())
                                {
                                    SendLongMessage($"Property: {property.Name}, Value: {property.Value}");
                                }
                            }
                            else
                            {
                                SendLongMessage("Unknown JSON structure");
                            }

                        }
                        catch (Exception ex)
                        {

                            SqlContext.Pipe.Send("jsonObject ex: " + ex);
                            HandleGeneralException(ex);
                            throw;
                        }






                        // Ensure LabelURL is always an array
                        //try
                        //{
                        //    JToken labelUrlToken;
                        //    if (jsonObject.TryGetValue("labelUrl", out labelUrlToken))
                        //    {
                        //        if (labelUrlToken.Type == JTokenType.String)
                        //        {
                        //            var urlString = labelUrlToken.ToString();
                        //            if (!string.IsNullOrEmpty(urlString))
                        //            {
                        //                jsonObject["labelUrl"] = new JArray(urlString);
                        //            }
                        //            else
                        //            {
                        //                jsonObject["labelUrl"] = new JArray(); // Empty string case
                        //            }
                        //        }
                        //        else if (labelUrlToken.Type == JTokenType.Array)
                        //        {
                        //            // If it's an array, do nothing, it's already correct
                        //        }
                        //        else
                        //        {
                        //            jsonObject["labelUrl"] = new JArray(); // Handle empty or other cases
                        //        }
                        //    }
                        //    else
                        //    {
                        //        jsonObject["labelUrl"] = new JArray(); // Handle case where LabelURL is missing
                        //    }


                        //    SqlContext.Pipe.Send("labelUrl: " + jsonObject["labelUrl"]);
                        //}
                        //catch (Exception ex)
                        //{

                        //    SqlContext.Pipe.Send("labelUrl ex: " + jsonObject["labelUrl"]);
                        //    HandleGeneralException(ex);
                        //    throw;
                        //}




                        // Convert back to JSON string

                        //try
                        //{

                        //    responseStr = jsonObject.ToString(Formatting.Indented);
                        //    SqlContext.Pipe.Send("responseStr: " + responseStr);
                        //}
                        //catch (Exception ex)
                        //{

                        //    SqlContext.Pipe.Send("responseStr ex: " + ex.Message);
                        //    HandleGeneralException(ex);
                        //    throw;
                        //}


                        //var ship = JsonConvert.DeserializeObject<ResponseObject>(responseStr);

                        //DataRow row = trackingData.NewRow();
                        //row["LabelUrl"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["labelUrl"];
                        //SqlContext.Pipe.Send("LabelUrl: " + row["LabelUrl"]);

                    }
                    else
                    {
                        HandleErrorResponse(response);
                    }
                }
                catch (WebException ex)
                {
                    HandleWebException(ex);
                }
                finally
                {
                    response?.Close();
                }

                // Send data to SQL stored procedure
                SqlContext.Pipe.Send("InsertDataToDatabase: " + response.StatusCode);
                InsertDataToDatabase(trackingData);
            }
            catch (Exception ex)
            {
                HandleGeneralException(ex);
            }
        }

        private static void AddTrackingDataColumns(DataTable trackingData)
        {
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
        }

        private static void SetRequestHeaders(HttpWebRequest request)
        {
            request.Headers.Add("APIKey", "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJcdTAwMTFcdTAwMUYtOjlcdTAwMTZcXDZIXCImPlx1MDAwRnxcXFx1MDAxMigtXHUwMDE2XHUwMDE0XHUwMDE5XHUwMDA0XGZsXVVFRVx1MDAxNlx1MDAwNVx1MDAwRVtmXmsiLCJpc3MiOiJjb20uYWR2YXRpeC5zZXJ2aWNlcyIsImV4cCI6MTY1NjMzNjQ3MCwiaWF0IjoxNjU2MzI5MjcwLCJqdGk");
            request.Headers.Add("AccountId", "BTV");
            request.Headers.Add("DEVICE-TYPE", "Web");
            request.Headers.Add("VER", "1.0");
        }

        private static async void HandleSuccessfulResponse(HttpWebResponse response, DataTable trackingData)
        {
            //using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            //{
            Stream responseStream = response.GetResponseStream();
            string responseStr = new StreamReader(responseStream).ReadToEnd();


            // Parse JSON response and populate trackingData
            var jsonObject = JObject.Parse(responseStr);
            DataRow row = trackingData.NewRow();


            var responseStatusToken = jsonObject["responseStatus"];
            if (responseStatusToken != null)
            {
                try
                {
                    // Use Convert.ToBoolean to safely parse true/false values or "true"/"false" strings
                    row["ResponseStatus"] = Convert.ToBoolean(responseStatusToken);
                }
                catch
                {
                    // Default to false if conversion fails
                    row["ResponseStatus"] = false;
                }
            }
            else
            {
                row["ResponseStatus"] = false; // default value if null
            }

            var responseStatusCodeToken = jsonObject["responseStatusCode"];
            if (responseStatusCodeToken != null)
            {
                try
                {
                    // Convert responseStatusCode to an integer safely
                    row["ResponseStatusCode"] = Convert.ToInt32(responseStatusCodeToken);
                }
                catch
                {
                    // Default to 0 if conversion fails
                    row["ResponseStatusCode"] = 0;
                }
            }
            else
            {
                row["ResponseStatusCode"] = 0; // default value if null
            }

            //var responseObject = jsonObject["responseObject"];
            //var orderInfo = responseObject?["orderInfo"];
            //var additionalInfo = responseObject?["additionalInfo"];
            //row["CreatedOn"] = orderInfo?["createdOn"] != null ? Convert.ToInt64(orderInfo["createdOn"]) : 0;
            //row["UpdatedOn"] = orderInfo?["updatedOn"] != null ? Convert.ToInt64(orderInfo["updatedOn"]) : 0;
            //row["OrderType"] = orderInfo?["orderType"] != null ? Convert.ToInt32(orderInfo["orderType"]) : 0;
            //row["Status"] = orderInfo?["status"] != null ? Convert.ToInt32(orderInfo["status"]) : 0;
            //row["ShipperId"] = orderInfo?["shipperId"] != null ? Convert.ToInt32(orderInfo["shipperId"]) : 0;
            //row["UpdatedBy"] = orderInfo?["updatedBy"] != null ? Convert.ToInt32(orderInfo["updatedBy"]) : 0;
            //row["TotalQuantity"] = orderInfo?["totalQuantity"] != null ? Convert.ToInt32(orderInfo["totalQuantity"]) : 0;
            //row["TotalWeight"] = orderInfo?["totalWeight"] != null ? Convert.ToDouble(orderInfo["totalWeight"]) : 0.0;
            //row["PaymentMode"] = orderInfo?["paymentMode"] != null ? Convert.ToInt32(orderInfo["paymentMode"]) : 0;
            //row["PaymentStatus"] = orderInfo?["paymentStatus"] != null ? Convert.ToInt32(orderInfo["paymentStatus"]) : 0;
            //row["Lob"] = orderInfo?["lob"] != null ? Convert.ToInt32(orderInfo["lob"]) : 0;

            //row["DeliveryTargetDate"] = additionalInfo?["deliveryTargetDate"] != null ? Convert.ToInt64(additionalInfo["deliveryTargetDate"]) : 0;
            //row["BoxCount"] = additionalInfo?["boxCount"] != null ? Convert.ToInt32(additionalInfo["boxCount"]) : 0;
            //row["LabelPrinted"] = additionalInfo?["labelPrinted"] != null ? Convert.ToInt32(additionalInfo["labelPrinted"]) : 0;
            //row["Notification"] = additionalInfo?["notification"] != null ? Convert.ToBoolean(additionalInfo["notification"]) : false;
            //row["IsAsnGenerated"] = additionalInfo?["isAsnGenerated"] != null ? Convert.ToBoolean(additionalInfo["isAsnGenerated"]) : false;
            //row["OrderState"] = additionalInfo?["orderState"] != null ? Convert.ToInt32(additionalInfo["orderState"]) : 0;
            //row["Otm"] = additionalInfo?["otm"] != null ? Convert.ToInt32(additionalInfo["otm"]) : 0;
            //row["IsFulfilment"] = additionalInfo?["isFulfilment"] != null ? Convert.ToInt32(additionalInfo["isFulfilment"]) : 0;
            //row["Label"] = additionalInfo?["label"] != null ? Convert.ToInt32(additionalInfo["label"]) : 0;
            //row["IsCcp"] = additionalInfo?["isCCP"] != null ? Convert.ToBoolean(additionalInfo["isCCP"]) : false;
            //// Order Number
            //row["OrderNumber"] = (string)jsonObject["responseObject"]?["orderNumber"]?["id"];

            //// Order Info

            //row["ParentOrder"] = (string)jsonObject["responseObject"]?["orderInfo"]?["parentOrder"];

            //row["OrderTypeDesc"] = (string)jsonObject["responseObject"]?["orderInfo"]?["orderTypeDesc"];

            //row["OrderStatusDesc"] = (string)jsonObject["responseObject"]?["orderInfo"]?["orderStatusDesc"];
            //row["AccountId"] = (string)jsonObject["responseObject"]?["orderInfo"]?["accountId"];
            //row["ClientName"] = (string)jsonObject["responseObject"]?["orderInfo"]?["clientName"];
            //row["CompanyName"] = (string)jsonObject["responseObject"]?["orderInfo"]?["companyName"];

            //row["WarehouseLocation"] = (string)jsonObject["responseObject"]?["orderInfo"]?["warehouseLocation"];

            //row["ServiceType"] = (string)jsonObject["responseObject"]?["orderInfo"]?["serviceType"];
            //row["ShippingMode"] = (string)jsonObject["responseObject"]?["orderInfo"]?["shippingMode"];

            //row["LobStatus"] = (string)jsonObject["responseObject"]?["orderInfo"]?["lobStatus"];
            //row["StoreId"] = (string)jsonObject["responseObject"]?["orderInfo"]?["storeId"];
            //row["StoreName"] = (string)jsonObject["responseObject"]?["orderInfo"]?["storeName"];

            //// Additional Info
            //row["Channel"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["channel"];
            //row["ReferenceId"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["referenceId"];

            //row["DeliveryTargetTime"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["deliveryTargetTime"];
            //row["TrackingId"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["trackingId"];
            //row["InternalTrackingId"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["internalTrackingId"];
            //row["BoxId"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["boxId"];

            //row["OrderPod"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["orderPod"];
            //row["Carrier"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["carrier"];

            //row["CompanyName"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["companyName"];
            //row["FacilityName"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["facilityName"];
            //row["BeginDate"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["beginDate"];
            //row["Reason"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["reason"];

            //row["CarrierAccountId"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["carrierAccountId"];
            //row["ShipmentType"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["shipmentType"];
            //row["CustomerId"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["customerId"];

            //row["LabelUrl"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["labelUrl"];


            //// ShipFrom Info
            //row["ShipFromName"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromName"];
            //row["ShipFromAddress"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromAddress"];
            //row["ShipFromCity"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromCity"];
            //row["ShipFromState"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromState"];
            //row["ShipFromCountry"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromCountry"];
            //row["ShipFromPostalCode"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromPostalCode"];
            //row["ShipFromEmail"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromEmail"];
            //row["ShipFromMobile"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromMobile"];

            //// ShipTo Info
            //row["ShipToName"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToName"];
            //row["ShipToAddress"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToAddress"];
            //row["ShipToAddress2"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToAddress2"];
            //row["ShipToCity"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToCity"];
            //row["ShipToState"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToState"];
            //row["ShipToCountry"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToCountry"];
            //row["PostalCode"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["postalCode"];
            //row["ShipToEmail"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToEmail"];
            //row["ShipToMobile"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToMobile"];

            //// BillTo Info
            //row["BillToName"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToName"];
            //row["BillToAddress"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToAddress"];
            //row["BillToAddress2"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToAddress2"];
            //row["BillToCity"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToCity"];
            //row["BillToState"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToState"];
            //row["BillToCountry"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToCountry"];
            //row["BillToPostalCode"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToPostalCode"];
            //row["BillToEmail"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToEmail"];
            //row["BillToMobile"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToMobile"];

            //SqlContext.Pipe.Send("inserted BillToMobile: " + (string)jsonObject["responseObject"]?["billToInfo"]?["billToMobile"]);

            //trackingData.Rows.Add(row);


            SqlContext.Pipe.Send("inserted jsonBody: " + row);
            //}
        }

        private static async Task HandleErrorResponse(HttpWebResponse response)
        {
            SqlContext.Pipe.Send("Error: Response status code is " + response.StatusCode);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string errorContent = await reader.ReadToEndAsync();
                SqlContext.Pipe.Send("Error details: " + errorContent);
            }
        }

        private static async Task HandleWebException(WebException ex)
        {
            if (ex.Response is HttpWebResponse errorResponse)
            {
                SqlContext.Pipe.Send("Exception: " + errorResponse.StatusCode);
                using (StreamReader reader = new StreamReader(errorResponse.GetResponseStream()))
                {
                    string errorContent = await reader.ReadToEndAsync();
                    SqlContext.Pipe.Send("Error response content: " + errorContent);
                }
            }
            else
            {
                SqlContext.Pipe.Send("An unexpected error occurred: " + ex.Message);
            }
        }

        //private static void HandleGeneralException(Exception ex)
        //{
        //    SqlContext.Pipe.Send("General exception occurred: " + ex.Message);
        //    SqlContext.Pipe.Send(ex.StackTrace);
        //}

        private static async Task InsertDataToDatabase(DataTable trackingData)
        {
            string connString = "Data Source=DESKTOP-FO7B6CB\\SQLEXPRESS01;Initial Catalog=API_Ship_v1;User ID=sample;Password=sample";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                await connection.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("dbo.Ship_importData_20241111", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter
                    {
                        ParameterName = "@DataToInsert",
                        SqlDbType = SqlDbType.Structured,
                        Value = trackingData,
                        TypeName = "dbo.weshipexpresspro_temp_ships"


                    };

                    cmd.Parameters.Add(param);
                    SqlContext.Pipe.Send("InsertDataToDatabase param: " + param);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        private static async Task HandleGeneralException(Exception ex)
        {
          await  SendLongMessage("Handled exception: " + ex.Message);
        }


        // Helper method to send messages in chunks
        private static async Task  SendLongMessage(string message)
        {
            const int maxChunkSize = 4000;
            int messageLength = message.Length;
            for (int i = 0; i < messageLength; i += maxChunkSize)
            {
                string chunk = message.Substring(i, Math.Min(maxChunkSize, messageLength - i));
                SqlContext.Pipe.Send(chunk);
            }
        }

    }
}
